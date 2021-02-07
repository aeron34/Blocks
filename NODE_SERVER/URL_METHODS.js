
const createUser = (req, res, u, knx) => {
  knx('users').insert({
    username: u["name"],
    password: u["password"]
  }).returning('*').then(a => res.status(300).json({
    status: a
  }), a => res.status(300));
}

const login = (req, res, u, knx) => {

  let good = false;
  b = {};

  knx('users').where('username', u.username).
  then(a => {
    b = a[0];
    if(b.online_status != "offline")
    {
      res.send('nope');
      return;
    }
    if(a[0].password != u.pass)
    {
      res.send('nope');
      return;
    }else {
      knx('users').where('username', u.username).
      update({
        online_status: "online",
        room: -1
      }).then(a => {
        res.send('done');
      });
    }
  }).catch(e => {
    res.send('nope');
  });
}

module.exports = {
  login,
  createUser
};
