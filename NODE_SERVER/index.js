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
  }

  rooms_dictionary = {
    '0': [
         //{ username: 'mono', score: 0 },
         {username:'dolo', score: 20},

      { username: 'remo', score:10 },
      { username: 'noob', score: 4231000 },
         { username: 'claus', score: 20 },
         { username: 'son', score: 21 }
     ],

  };

  numberOfRooms = 0;

  createRoom = (addSelf = false, username='', room) =>
  {
    let {rooms_dictionary, roomNumber} = this;

    if(room == undefined)
    {
      this.numberOfRooms++;
      roomNumber = this.numberOfRooms;
    }else{
      roomNumber = room
    }

    rooms_dictionary[`${roomNumber}`] = [];

    if(addSelf)
    {
        rooms_dictionary[`${roomNumber}`].push(
          {username: `${username}`, score: 0}
        );
    }

    setTimeout(() => {
      console.log(roomNumber);
      delete this.rooms_dictionary[`${roomNumber}`];
    }, 900000);
  }

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


    //methods.CleanRoom(rooms_dictionary, numberOfRooms);

    /*If the user doens't have a number AND the room at
    said number doesn't either, create it and increase
    the number so total rooms is updated and other players
    with no number can find it easily:
    */

    //If the room doesn't exist.
    if(!rooms_dictionary.hasOwnProperty(`${numberOfRooms}`))
    {
      this.createRoom(true, username, numberOfRooms);

      return numberOfRooms;
    }
    else {

      /*TEST 1:

      Check if the object ({...}) is in the room.
      if it is and the room is full, run the room*/

      let obj_inside= false;
      for(let i = 0; i < rooms_dictionary[`${numberOfRooms}`].length; i++)
      {
        if(rooms_dictionary[`${numberOfRooms}`][i].username == username)
        {
          obj_inside = true;
          break;
        }
      }

      if(obj_inside &&
      rooms_dictionary[`${numberOfRooms}`].length == ROOM_SIZE)
      {

        //Make a new room.
        this.createRoom();

        return `${["running", numberOfRooms]}`;
      }

      /*TEST 1.5

      This is an extension of Test 1, if the object
      is inside the array BUT the room isn't full
      just return the room number */

      if(obj_inside &&
      rooms_dictionary[`${numberOfRooms}`].length < ROOM_SIZE)
      {
        return numberOfRooms
      }

      /* TEST 1 DIDN'T PASS, NOW FOR TEST 2:

      This IF block means that if the array doesn't
      include username and the room before addition is less
      than max size, then after add. is room_size execute
      this block*/

      if(!obj_inside &&
      rooms_dictionary[`${numberOfRooms}`].length < ROOM_SIZE )

      {
        /*If the room is less than ROOM_SIZE and doesn't
          include user, add him:
        */
        rooms_dictionary[`${numberOfRooms}`].push(
          {username: `${username}`, score: 0});

        /*If the room length NOW equals ROOM_SIZE as a result
        of adding user, room the room:
        */

        if(rooms_dictionary[`${numberOfRooms}`].length == ROOM_SIZE)
        {
          //Make a new room.
          this.createRoom();

          return `${["running", numberOfRooms]}`;

        }

        return numberOfRooms;
      }

      if(rooms_dictionary[`${numberOfRooms}`].length >= ROOM_SIZE)
      {
          this.createRoom(true);
          rooms_dictionary[`${this.numberOfRooms}`].push(username)

          return this.numberOfRooms;
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

app.post('/end_game', async (req, res) => {
    let user = req.body;
    let db_user_info;
    let done = false;

    await knx('users').where({
      'username': user.username,
      'password': user.password
    }).then(response => {
        if(response.length != 0)
        {
          db_user_info = response[0];
          }else{
          res.send("not ending game");
          done = true;
        }
    });

    if(!done)
    {
      if(user.score > db_user_info.highest_score)
      {
        await knx('users').where({
          'username': user.username,
          'password': user.password
        }).update({
          highest_score: user.score
        }).then(response => {
            res.send(`${db_user_info.win}, ${db_user_info.loss},
            ${db_user_info.highest_score}`);
        })
      } else {
        res.send(`${db_user_info.win}, ${db_user_info.loss},
        ${db_user_info.highest_score}`);
      }
    }
});

app.get('/show_dictionary', (req, res) => {
    console.log(room_manager.rooms_dictionary);
    res.send(room_manager.rooms_dictionary);
})

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
  res.send("done");
});

app.get('/check_rooms', (req, res) => {

  console.log(req.query)
  let result = room_manager.addUserToRoom(parseInt(req.query.room),
  req.query.username);

  res.send(`${result}`);

});

app.post('/check_in', (req, res) => {
    const u = req.body;
    res.send('done');
});

app.post('/send_result', async (req, res) => {
  const user = req.body;
  await urls.SendResult(req, res, knx, user);
})

app.post('/send_mets', async (req, res) => {

    let name = req.body.username;
    let room = req.body.room;
    let mets = req.body.mets;

    console.log(`called by ${name}`);

    let user_arr = methods.FilterRoomForUser(room_manager, room, name);
    let user_exists = false;
    let user = user_arr[Math.floor(Math.random() * user_arr.length)];
    let target_users_meteors = 0;

    await knx('users').where('username', user.username).
    then(found_user => {
      console.log(found_user);
      if(found_user.length == 1)
      {
        user_exists = true;
        target_users_meteors = found_user[0].meteors
      }else{
        user_exists = false;
        return res.send('error');
      }
    });

    if(user_exists)
    {
      await knx('users').where('username', user.username)
      .update({
        meteors: parseInt(mets) + parseInt(target_users_meteors)
      }).then(a => {
        res.send('meteors sent');
      }, a => {
        res.send("not send")
      })
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

  for(let i = 0; i < ROOM_SIZE; i++)
  {
    if(room_manager.rooms_dictionary[`${user.room}`][i].username == user.username)
    {
      if(user.score != undefined)
      {
        room_manager.rooms_dictionary[`${user.room}`][i].score = parseInt(user.score);
      }
      break;
    }
  }

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
