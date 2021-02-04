
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
  knx('users').where('username', u.username).
  then(a => {
    if(a[0].password != u.pass)
    {
      res.send('nope');
      return;
    }else {
      knx('users').where('username', u.username).
      update({
        online_status: "online"
      }).returning('username').then(a => {
        res.send(a[0]);
      });
    }
  });
}

module.exports = {
  login,
  createUser
};
