
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

module.exports = {
  FilterRoomForUser
};
