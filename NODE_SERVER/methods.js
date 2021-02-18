
function GetRoom(obj, R_S, user)
{
  let cnt = 0, h_rn = obj[0].room;


  user.room = h_rn;

  for(let i = 0; i < R_S+1; i++)
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
    if(obj[i].room == h_rn &&
    obj[i].online_status != "running"&&
    obj[i].online_status != "offline")
    {
        cnt++;
    }

    if(obj[i].room == h_rn &&
    obj[i].online_status == "running")
    {
        cnt = R_S;
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
  return knx('users').where('username', name).
  update({
    room: rn,
    online_status: "waiting"
  }).then(a => {
    res.send('wait');
  });
}


function AssignAndRunRoom(knx, rn, n)
{
  let arr = [];
  //Adding the promises to 'arr'
  for(let i = 0; i < 3; i++)
  {
    arr.push(knx('users').where('username', n[i].username).
    update({
      room: rn,
      online_status: "running"
    }));
  }

  //Return all the promises.
  return arr;
}

module.exports = {
    GetRoom,
    AssignAndRunRoom,
    AssignRoom
};
