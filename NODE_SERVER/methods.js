
function GetRoom(obj, R_S, user)
{
  let cnt = 0, h_rn = obj[0].room;
  user.room = h_rn;
  if(user.online_status == 'running')
  {
    return -1;
  }

  for(let i = 0; i < 6; i++)
  {
      if(obj[i].username == user.username)
      {
        obj.splice(i,1);
        break;
      }
  }
  obj.unshift(user);

  let ret_arr = [-1,h_rn];
  for(var i = 0; i < obj.length; i++)
  {
    if(obj[i].room == h_rn)
    {
      cnt++;
    }
  }

  if(cnt > R_S)
  {
    ret_arr[0] = 1;
  }

  if(cnt == R_S)
  {
    ret_arr[0] = 0;
  }

  return ret_arr;
}

function AssignRoom(knx, rn, name, res)
{
  knx('users').where('username', name).
  update({
    room: rn,
    online_status: "waiting"
  }).then(a => {
    res.send('no run');
  })
}

function AssignAndRunRoom(knx, rn, n, res)
{
  let arr = [];

  for(let i = 0; i < 5; i++)
  {
    arr.push(knx('users').where('username', n[i].username).
    update({
      room: rn,
      online_status: "running"
    }));
  }

  Promise.all(arr).then(a => {
      res.send('run');
  });
}

module.exports = {
    GetRoom,
    AssignAndRunRoom,
    AssignRoom
};
