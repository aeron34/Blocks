
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

  knx('users').where({
      username: u.username,
      password: u.pass
    }).update({
      online_status: "online",
      meteors: 0
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

async function SendResult(req, res, knx, user)
{
  let result_num = 0;

  await knx('users').where({
    'username': user.username,
    'password': user.pass
  }).then(response => {
    if(response.length > 0)
    {
      result_num = response[0][`${user.result}`] + 1;
    }else{
      return res.send('user doesnt exist');
    }
  });

  if(result_num != 0)
  {
    let update_obj = {}
    update_obj[user.result] = result_num;

    await knx('users').where('username', user.username)
    .update(update_obj).then(response => {
      res.send('done');
    }, reject => {
      res.send('not done')
    })
  }
}

module.exports = {
  login,
  createUser,
  SendResult
};
