const dum = require('./dummy');
const urls = require('./URL_METHODS');
const methods = require('./methods');
const express = require('express');
const path = require('path');


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

app.get('/rooms', (req, res) => {
  knx('users').orderBy('room','desc').select('*')
  .then(a => {
    let n = a;

    n = n.slice(0, ROOM_SIZE+4);
    methods.GetRoom(n, ROOM_SIZE);

    console.log(n[0]);
    res.send('here');

  });
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

/*n = n.slice(0, ROOM_SIZE+4);
methods.GetRoom(n, ROOM_SIZE);
*/
