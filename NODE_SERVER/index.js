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

app.get('/get', (req, res) => {
  const u = req.query;

  console.log('coming');
});

app.get('/login', (req, res) => {
  const u = req.query;

  knx('users').where('username', u.username).
  then(a => {
    if(a[0].password == u.pass)
    {
      res.send(a[0].username);
    }else {
      res.send('nope');
    }
  }).
  catch(e => {
    res.send("Not found");
  });

});

app.post('/', (req, res) => {
  const u = req.body;
  console.log(u);
  res.json(u);
/*
  knx('users').insert({
    username: u["name"],
    password: u["password"]
  }).returning('*').then(a => res.status(300).json({
    status: a
  }), a => res.status(300));
*/
});

app.listen(3000);
