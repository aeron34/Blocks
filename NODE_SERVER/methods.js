
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


/*
  This function sanitizes the room. this gets
  rid of any array indexes that aren't
  dicts, for example:

    [
      {...},
      'dolo', <--- this will get deleted/"cleaned"
    ]
*/

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
};
