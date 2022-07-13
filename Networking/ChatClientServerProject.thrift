namespace netstd ThriftTechChat.Networking


struct User{
	1: string IdUser,
	2: string Name,
	3: string Password,
	4: i32 NrOfFriendRequests
}

struct Message{
	1: string IdMessage,
	2: string ExpedationDate,
	3: string Content,
	4: string IdChat
	5: string NameOfShipper
}

struct Chat{
	1: string IdChat,
	2: string Name,
	3: bool IsCreatedDefaul
	4: string LastMessageDate
}

struct Friend{
	1: string idFriend,
	2: string idUser1,
	3: string idUser2,
	4: string DateTimeWhenRelacionWasCreated
}
struct FriendRequest{
	1: string idFriendRequest,
	2: string IdToUser,
	3: string IdFromUser,
	4: string DateTimeWhenRequestWasCreated
}
struct UserInChat{
	1: string IdUserInChat,
	2: string IdUser,
	3: string IdChat,
	4: i32 NewMessagesInChat,
	5: bool IsMuted
}

exception ChatException{
	1: string message
}

service Service{
	User Login(1: string name,2: string password) throws (1:ChatException e),
	bool Logout(1:string idUser) throws (1:ChatException e),
	User AddNewUser(1: string name,2: string password) throws (1:ChatException e),
	list<Chat> GetChatsFromUser(1:string idUser),
	list<Message> GetMessagesFromChat(1:string idChat),
	list<UserInChat> GetUserInChatFromUser(1: string idUser),
	bool AddNewMessage(1: string messageContent, 2: string idChat, 3:string nameOfShipper) throws (1:ChatException e),
	bool AddNewFriendRequest(1: string idUser, 2: string nameOfFriend) throws (1:ChatException e),
	bool SetNewMessagesInChatTo0(1: string idUser, 2:string idChat) throws (1:ChatException e),
	bool AcceptFriendRequest(1: string idToUser,2: string idFromUser) throws (1:ChatException e),
	bool DenyFriendRequest(1: string idToUser,2: string idFromUser) throws (1:ChatException e),
	list<User> GetFromUsersInFriendRequestFromUser(1: string idUser),
	bool IsMutedChangeFromAnUserInAChat(1: string idUser, 2: string idChat) throws (1:ChatException e)
}