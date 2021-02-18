
const createUser = (req, res, u, knx) => {
  knx('users').insert({
    username: u["name"],
    password: u["password"]
  }).returning('*').then(a => res.status(300).json({
    status: a
  }), a => {
    console.log('sop');
    res.json(a)
  });
}

const login = (req, res, u, knx) => {

  let good = false;
  b = {};

 knx('users').where({
    username: u.username,
    password: u.pass,
    online_status: 'offline'
  }).update({
    online_status: "online"
  }).then( a=> {
    if(a == 0)
    {
      return res.send('nope');
    }
    res.send('logged in');
  }).catch(e => {
    res.send('nope');
  });
};

module.exports = {
  login,
  createUser
};
