
function FilterRoomForUser(room_manager, room, name)
{
  return room_manager.rooms_dictionary[`${room}`].filter(user => {
    if(user['username'] != name && user['username']
    != null)
    {
      return user;
    }
  })
}

function GetUserInRoom(room_manager, room, name)
{
  return room_manager.rooms_dictionary[`${room}`].filter(user => {
    if(user['username'] == name)
    {
      return user;
    }
  });
}


function GetOtherTeam(room_manager, room, team)
{
  return room_manager.rooms_dictionary[`${room}`].filter(user => {
    if(user['team'] != team)
    {
      return user;
    }
  });
}
/*
  This function sanitizes the room. this gets
  rid of any array indexes that aren't
  dicts, for example:

    [
      {...},
      'dolo', <--- this will get deleted/"cleaned"
    ]
*/

function GetWinningTeam(rooms_dictionary, room, name)
{
  let teamA = 0, teamB = 0;
  let myTeam ="";
  rooms_dictionary[`${room}`].map(player =>
  {
      if(player.username == name)
      {
        myTeam = player.team
      }
      if(player.team == "A")
      {
        teamA += player.score;
      }else{
        teamB += player.score;
      }
  })

  let result = "";

  if(teamA > teamB)
  {
    result = "A"
  }
  if(teamA < teamB)
  {
    result = "B"
  }
  if(teamA == teamB)
  {
    result = "DRAW"
  }

  if(myTeam == result)
  {
    result = "win"
  }else{
    result = "loss"
  }


  return result;
}

function CleanRoom(rooms_dictionary, room)
{
  if(!rooms_dictionary.hasOwnProperty(`${room}`))
  {
    return;
  }

  rooms_dictionary[`${room}`] = rooms_dictionary[`${room}`].filter(user => {
    if(user['username'] != null)
    {
      return user;
    }
  });
}

module.exports = {
  FilterRoomForUser,
  CleanRoom,
  GetUserInRoom,
  GetOtherTeam,
  GetWinningTeam
};
