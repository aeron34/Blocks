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

/*
class RoomAssigner
{
  constructor()
  {
    console.log('made');
  }

  /*Users_array is

  users_array =
  here = () => {
    console.log('hurr');
  }
}

let r = new RoomAssigner();
*/
const ROOM_SIZE = 3;


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
          room: -1,
          meteors: 0
          }).then(a => {
          res.send('logged out');
        })
      }else {
        res.send('wrong pass');
      }
  });

})

app.get('/check_rooms/:rn', (req, res) => {
  res.send(req.params.rn);
});

app.post('/check_in', (req, res) => {
    const u = req.body;
    res.send('done');
});


app.post('/send_mets', async (req, res) => {

  let name = req.body.username;
  let room = req.body.room;
  let mets = req.body.mets;

  console.log(`called by ${name}`);

  let user_arr = [];

  await knx('users').where('room', room).
  then(users => {
    user_arr = users.filter(a => {
      if(a.username != name)
      {
        return a;
      }
    })
  }).catch(e => {
    res.json(e);
  })

  let user = user_arr[Math.floor(Math.random() * user_arr.length)]

  await knx('users').where('username', user.username).
  update({
    meteors: parseInt(mets) + parseInt(user.meteors)
  }).returning('*').then(a => {
    res.send('meteors sent')
  });

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

app.get('/rooms', async (req, res) => {

  let params = req.query;

  const user = await knx('users').where('username', params.username).
  then(users => {
    return users[0];
  }).catch(e => {
    res.send(e);
  })

  let mode = [];

  if(user.room <= -1)
  {
    user.room = 0;
  }

  const user_list = await knx('users').orderBy('room','desc').select('*')
  .limit(ROOM_SIZE+2).then(obj_arr => {
      console.log(obj_arr)
      obj_arr = obj_arr.slice(0, ROOM_SIZE);
      mode = methods.GetRoom(obj_arr, ROOM_SIZE, user);
      return obj_arr;
  });

  try {
    switch(mode[0])
    {
      case 1:
        methods.AssignRoom(knx, mode[1]+1, user.username, res);
        break;
      case 0:

        await Promise.all(methods.AssignAndRunRoom(knx, mode[1], user_list)).then(
        a => {return res.send(`${mode[1]}`)});
        break;
      case -1:
          await methods.AssignRoom(knx, mode[1], user.username, res);
        break;
      default:
        break;
      }
  }
  catch (e) {
    res.send(e);
  }
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
