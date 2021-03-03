const dum = require('./dummy');
const urls = require('./URL_METHODS');
const methods = require('./methods');
const express = require('express');
const path = require('path');
const moment_tz = require('moment-timezone');
const moment = require('moment');

const knx = require('knex')({
  client: 'pg',
  connection: {
    host : '127.0.0.1',
    user : 'postgres',
    password : 'madflow336',
    database : 'game'
  },
});


class RoomManager
{
  constructor()
  {
    console.log('made');
  }

  rooms_dictionary = {
    '0': [
      'mono',
      //'dolo',
      { username: 'claus', score: 0 },
      { username: 'son', score: 0 },
      { username: 'remo', score: 0 },
      { username: 'noob', score: 0 }
    ]
  };

  numberOfRooms = 0;

  addUserToRoom = (room_no = -1, username="") => {

    let {rooms_dictionary, numberOfRooms} = this;

    if(room_no != -1)
    {
      /*I'm using a slick trick, if room_no is
      greater than -1, menaing the user does have
      a room number, then set numberOfRooms to
      that number, remember this doesn't effect
      this.numberOfRooms (the actual number representing
      the number of rooms), so numberOfRooms =/= this.numberOfRooms*/
      numberOfRooms = room_no
    }

    /*
        If the function makes it to this line, that means
        that the user who called this function doesn't have
        a room number, so it'll be assigned numberOfRooms+1 as
        a room number. This is good because if multiple users
        make the room that's numberOfRooms+1 then the dictionary
        won't create duplicates and they'll be assigned the
        same room, userobjects CANNOT be reassigned a new room
        number unless they sign out or the match is over.
    */


    /*If the user doens't have a number AND the room at
    said number doesn't either, create it and increase
    the number so total rooms is updated and other players
    with no number can find it easily:
    */
    if(!rooms_dictionary.hasOwnProperty(`${numberOfRooms}`))
    {
      rooms_dictionary[`${numberOfRooms}`] = [];
      rooms_dictionary[`${numberOfRooms}`].push(username)
      return numberOfRooms;
    }else {


      /*If the username doesn't exist inside the array
      and the room is less than the max size, push him in*/
      if(rooms_dictionary[`${numberOfRooms}`].includes(`${username}`))
      {
        if(rooms_dictionary[`${numberOfRooms}`].length == ROOM_SIZE)
        {
          let index = rooms_dictionary[`${numberOfRooms}`].indexOf(username);
          rooms_dictionary[`${numberOfRooms}`][index] = {username:username,
          score: 0};
          return `${["running", numberOfRooms]}`;

        }
        return numberOfRooms;
      }

      if(!rooms_dictionary[`${numberOfRooms}`].includes(`${username}`)
      && rooms_dictionary[`${numberOfRooms}`].length < ROOM_SIZE )
      {
        rooms_dictionary[`${numberOfRooms}`].push(username)

        if(rooms_dictionary[`${numberOfRooms}`].length == ROOM_SIZE)
        {
          this.numberOfRooms++;
          rooms_dictionary[`${this.numberOfRooms}`] = [];
          let index = rooms_dictionary[`${numberOfRooms}`].indexOf(username);
          rooms_dictionary[`${numberOfRooms}`][index] = {username:username,
          score: 0};


          return `${["running", numberOfRooms]}`;
        }

        return numberOfRooms;
      }

      if(rooms_dictionary[`${numberOfRooms}`].length >= ROOM_SIZE)
      {
          this.numberOfRooms++;
          rooms_dictionary[`${this.numberOfRooms}`] = [];
          rooms_dictionary[`${this.numberOfRooms}`].push(username)
          return `${"running", `${this.numberOfRooms}`}`;
      }
    }
  }

  deleteUser = (username='', room) => {

    let {rooms_dictionary} = this;

    let index = rooms_dictionary[`${room}`].indexOf(username);
    rooms_dictionary[room].splice(index, 1);
  }
}

let room_manager = new RoomManager();

const ROOM_SIZE = 6;


const app = express();

app.use(express.urlencoded({extended: false})); //Parses the request body
app.use(express.json());
app.use('/files', express.static('static'));


app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname+'/static/index.html'));
});


var now = moment("2021-02-06T10:05:29");
var a = moment_tz.utc(now).tz("Asia/Taipei");
b = now.utc().format();

//console.log(a.diff(b, 'minutes'));


app.post('/logout', (req, res) => {
    let u = req.body;

    knx('users').where('username', u.username).
    then(a => {
      if(a[0].password == u.password)
      {
        knx('users').where('username', u.username).
        update({
          online_status: "offline",
          meteors: 0
          }).then(a => {
          res.send('logged out');
        })
      }else {
        res.send('wrong pass');
      }
  });

})

app.post('/delete_from_room', (req, res) => {
  let user = req.body;
  if(user.room != null)
  {
    room_manager.deleteUser(user.username, user.room);
  }
  //console.log(room_manager.rooms_dictionary);
  res.send("done");
});

app.get('/check_rooms', (req, res) => {

  let result = room_manager.addUserToRoom(parseInt(req.query.room),
  req.query.username);

  res.send(`${result}`);

});

app.post('/check_in', (req, res) => {
    const u = req.body;
    res.send('done');
});


app.post('/send_mets', async (req, res) => {

  try
  {
    let name = req.body.username;
    let room = req.body.room;
    let mets = req.body.mets;

    console.log(`called by ${name}`);

    let user_arr = methods.FilterRoomForUser(room_manager, room, name);

    let user = user_arr[Math.floor(Math.random() * user_arr.length)];
    let target_users_meteors = 0;

    await knx('users').where('username', user.username).
    then(found_user => {
      target_users_meteors = found_user[0].meteors
    }).catch(e => {
      res.send(e);
    })

    await knx('users').where('username', user.username)
    .update({
      meteors: parseInt(mets) + parseInt(target_users_meteors)
    }).then(a => {
      res.send('meteors sent');
    }).catch(e => {
      res.send(e);
    })
  }
  catch(err)
  {
    res.send('error')
  }
})

app.get('/get_mets', async (req, res) => {

  let params = req.query;
  let mets = 0;

  const user = await knx('users').where('username', params.username).
  then(user => {
    mets = user[0].meteors;
    return user[0];
  }).catch(e => {
    res.send(e);
  });

  await knx('users').where('username', params.username).
  update({meteors: 0}).then(a => {
      res.send(`${mets}`);
  });

});

app.get('/getopponentsscore', (req, res) => {
  const user = req.query;
  console.log(user);
  let user_list = methods.FilterRoomForUser(room_manager, user.room, user.username)

  user_list_string = "";

  user_list.map(user => {
    user_list_string += `${user["username"]}, ${user["score"]}|`;
  })

  res.send(`${user_list_string}`);
});

app.post('/find_winner', (req, res) => {

});

app.get('/login', (req, res) => {
  const u = req.query;
  urls.login(req, res, u, knx);
});

app.post('/', (req, res) => {
  const u = req.body;
  urls.createUser(req, res, u, knx);
});

app.listen(3000);
