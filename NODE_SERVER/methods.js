
function GetRoom(obj, R_S)
{
  let cnt = 0, h_rn = obj[0].room;

  for(var i = 0; i < obj.length; i++)
  {
    if(obj[i].room == h_rn)
    {
      cnt++;
    }
  }

  if(cnt > R_S)
  {
    obj[0].room = h_rn + 1;
  }
}

module.exports = {
    GetRoom
};
