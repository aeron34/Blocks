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
  }
});


const ROOM_SIZE = 5;


const app = express();
app.use(express.urlencoded({extended: false})); //Parses the request body
app.use(express.json());
app.use('/files', express.static('static'));


app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname+'/static/index.html'));
});

/*
knx.select('*').from('users')
.then(a => res.status(300).json(a));

knx('users').where('username', 'sonny').
then(a => res.send(a[0].username));

*/
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
          room: -1
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

app.get('/update', (req, res) => {
  let n = [
  'Son',
  'lemons',
  'Boton',
  'wutang',
  'gucci'];

  let arr = [];

  for(let i = 0; i < 5; i++)
  {
    arr.push(knx('users').where('username', n[i]).
    update({
      room: 3,
      online_status: "running"
    }));
  }

  Promise.all(arr).then(a => {
      res.send('oh');
  });
})

app.get('/rooms', (req, res) => {
  let user = {};
  let u = req.query;

  knx.transaction(trx => {
    trx('users').where('username', u.username).
    then(a => {
        user = a[0];
        trx('users').orderBy('room','desc').select('*')
        .then(a => {

        let n = a;
        n = n.slice(0, ROOM_SIZE+1);
        let b = methods.GetRoom(n, ROOM_SIZE, user);

        switch(b[0])
        {
          case 1:
            methods.AssignRoom(trx, b[1]+1, user.username, res);
            break;
          case 0:
            methods.AssignAndRunRoom(trx, b[1], n, res);
            break;
          case -1:
              methods.AssignRoom(trx, b[1], user.username, res);
            break;
          default:
            break;
        }
      });
    })
  }).catch(e => {
    res.send(e);
  })
})


app.get('/login', (req, res) => {
  const u = req.query;
  urls.login(req, res, u, knx);
});

app.post('/', (req, res) => {
  const u = req.body;
  urls.createUser(req, res, u, knx);
});

app.listen(3000);

/*n = n.slice(0, ROOM_SIZE+4);
methods.GetRoom(n, ROOM_SIZE);
*/
