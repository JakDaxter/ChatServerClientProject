using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ThriftTechChat.Networking;

namespace Server
{
    public class ThriftImplementation : Service.IAsync
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly MyDbContext _dbContext;
        private static Mutex _mut = new Mutex();
        private IList<Guid> _loggedUsers;
        private IMyRabbitMQProducer _rabbitmq;
        /*
         * Create a The thrift implementation for a thrift server
         * Parameters:
         *  dbContext: A valid dbcontext who is the repo from the server 
         */
        public ThriftImplementation(MyDbContext dbContext)
        {
            _dbContext = dbContext;
            _loggedUsers = new List<Guid>();
            _rabbitmq = new MyRabbitMQProducer();
            _log.Info("ThriftImplementation created");
        }
        #region Add
        /*
         * Add a new Message in a Chat, and notify logged Users in Chat
         * Parameters:
         *  messageContent: The content of the Message
         *  chatId: The Id(guid) from a Chat,  in which the Message was sent
         *  nameOfShipper: Name of a User, who add the Message
         * Return:
         *  Default true
         * Exception:
         *  ChatException: if chat don't exist in db.
         */
        public Task<bool> AddNewMessage(string messageContent, string idChat, string nameOfShipper, CancellationToken cancellationToken = default)
        {
            _log.Info($"Start AddNewMessage in Idchad: {idChat} from {nameOfShipper}");
            Model.Chat chat;
            _mut.WaitOne();
            try
            {
                chat = _dbContext.Chats.First(x => x.IdChat == Guid.Parse(idChat));
            }
            catch (InvalidOperationException)
            {
                _mut.ReleaseMutex();
                throw new ChatException()
                {
                    Message = "idChat is Wrong"
                };
            }
            _dbContext.Add(new Model.Message()
            {
                IdMessage = Guid.NewGuid(),
                ExpedationDate = DateTime.Now,
                Content = messageContent,
                IdChat = Guid.Parse(idChat),
                NameOfShipper = nameOfShipper
            });
            _mut.ReleaseMutex();
            _log.Info("Message Added");
            chat.LastMessageDate = DateTime.Now;
            _mut.WaitOne();
            _dbContext.Update(chat);
            _dbContext.SaveChanges();
            _mut.ReleaseMutex();
            _log.Info("Chat Updated and start notify Users for the new Message");
            #region Notify
            _mut.WaitOne();
            _dbContext.UserInChats.Where(x => x.IdChat == Guid.Parse(idChat))
                .ToList()
                .ForEach(x =>
                {
                    if (x.IsMuted == false) // if is muted don't change in db the NewMessagesInChat variable for this User
                    {
                        x.NewMessagesInChat += 1;
                    }
                    if (_loggedUsers.Contains(x.IdUser)) //if are logged, notify the User
                    {
                        _rabbitmq.NewMessageInConversation(
                            $"{idChat}.{nameOfShipper}: {messageContent}",
                            _dbContext.Users.First(y => y.IdUser == x.IdUser));
                    }
                });
            _mut.ReleaseMutex();
            _log.Info("Notify completed and AddNewMessage completed\n");
            #endregion
            return Task.FromResult(true);
        }
        /*
         * Add new User in db
         * Parameters:
         *  name: Name of the user
         *  password: Password of the user
         * Return:
         *  The new User who was created from the data
         * Exception: 
         *  ChatException: Is User already exist (Name is Unique) 
         */
        public Task<User> AddNewUser(string name, string password, CancellationToken cancellationToken = default)
        {
            _log.Info($"Start AddNewUser Called {name}");
            _mut.WaitOne();
            if (_dbContext.Users.Any(x => name == x.Name))
            {
                _mut.ReleaseMutex();
                _log.Warn("The User already exists");
                throw new ChatException()
                {
                    Message = "User already exists"
                };
            }
            _mut.ReleaseMutex();
            var itemS = new Model.User()
            {
                IdUser = Guid.NewGuid(),
                Name = name,
                Password = password,
                NrOfFriendRequests = 0
            };
            _mut.WaitOne();
            _dbContext.Entry(itemS).State = EntityState.Added;
            _dbContext.SaveChanges();
            _mut.ReleaseMutex();
            _log.Info("AddNewUser completed and return the new User\n");
            return Task.FromResult(new User() //Model.User to Thrift.User
            {
                IdUser = itemS.IdUser.ToString(),
                Name = itemS.Name,
                Password = itemS.Password,
                NrOfFriendRequests=itemS.NrOfFriendRequests
            });
        }
        /*
         * Add new FriendRequest in db and notify the User who the request are sent 
         * Parameters: 
         *  idUser(FromUser): The Id(guid) From User who try to add a new Friend
         *  nameOfFriend(ToUser): Name of the User whon the request was sent
         * return: 
         *  Default true
         * Exception:
         *  ChatException:
         *   FromUser not exist in db
         *   ToUser not Exist in db
         *   FromUser and ToUser are the same User
         *   The Request Exist
         *   FromUser and ToUser are already Friends
         */
        public Task<bool> AddNewFriendRequest(string idUser, string nameOfFriend, CancellationToken cancellationToken = default)
        {
            _log.Info($"Start AddNrFriendRequest from Id: {idUser} to Name: {nameOfFriend}");
            _mut.WaitOne();
            Model.User user1, user2;
            try
            {
                user1 = _dbContext.Users.First(x => x.IdUser == Guid.Parse(idUser));
            }
            catch (InvalidOperationException)
            {
                _mut.ReleaseMutex();
                throw new ChatException()
                {
                    Message = "IdChat Invalid"
                };
            }
            try
            {
                user2 = _dbContext.Users.First(x => x.Name == nameOfFriend);
            }
            catch (InvalidOperationException)
            {
                _mut.ReleaseMutex();
                throw new ChatException()
                {
                    Message = "Don't exist a user whit that Name"
                };
            }
            _mut.ReleaseMutex();
            if (user1.IdUser == user2.IdUser)
            {

                _log.Warn("TO and From User are the same");
                throw new ChatException()
                {
                    Message = "Same User"
                };
            }
            _mut.WaitOne();
            if (_dbContext.FriendRequests.Any(x => x.IdFromUser == user1.IdUser && x.IdToUser == user2.IdUser))
            {
                _mut.ReleaseMutex();
                _log.Warn("The Request already exists");
                throw new ChatException()
                {
                    Message = "Request Are set to"
                };
            }
            if (_dbContext.FriendRequests.Any(x => x.IdFromUser == user2.IdUser && x.IdToUser == user1.IdUser))
            {
                _mut.ReleaseMutex();
                _log.Warn("The Request already exists by the other part");
                throw new ChatException()
                {
                    Message = "The Other User are set a request to you, you can't set a request too"
                };
            }
            if (_dbContext.Friends.Any(x => (x.IdUser1 == user1.IdUser || x.IdUser1 == user2.IdUser) && (x.IdUser2 == user1.IdUser || x.IdUser2 == user2.IdUser)))
            {
                _mut.ReleaseMutex();
                _log.Warn("The Users are friends already");
                throw new ChatException()
                {
                    Message = "You and the other User are already friends"
                };
            }
            _dbContext.Add(new Model.FriendRequest
            {
                IdFriendRequest = Guid.NewGuid(),
                IdFromUser = user1.IdUser,
                IdToUser = user2.IdUser,
                DateTimeWhenRequestWasCreated = DateTime.Now
            });
            _mut.ReleaseMutex();
            _log.Info("FriendRequest added in DB");
            user2.NrOfFriendRequests += 1;
            _mut.WaitOne();
            _dbContext.Update(user2);
            _dbContext.SaveChanges();
            _mut.ReleaseMutex();
            _log.Info("To User information changed and start notify it");
            #region notiy
            _rabbitmq.FriendRequest(user2);
            #endregion
            _log.Info("Notify done and AddNewFriendRequest completed\n");
            return Task.FromResult(true);
        }
        #endregion
        #region Get
        /*
         * Get all Chats from a User
         * Parameters:
         *  IdUser: Id(Guid) from a User for which the data is request
         * Return:
         *  List of Chats in which User participat
         */
        public Task<List<Chat>> GetChatsFromUser(string idUser, CancellationToken cancellationToken = default)
        {
            _log.Info($"Start GetChatsFromUser from Id: {idUser}");
            _mut.WaitOne();
            var list = _dbContext.UserInChats.Where(x => x.IdUser == Guid.Parse(idUser)); //List UserInChat where find User
            var list2 = _dbContext.Chats.Where(x => list.Any(y => y.IdChat == x.IdChat)) // List Chat where find User
                .OrderByDescending(x => x.LastMessageDate).ToList(); //order by LastMessageDate
            _mut.ReleaseMutex();
            _log.Info("All the Chats in which the User is found were found. Start preparing data to export");
            var listT = new List<Chat>();
            list2.ToList().ForEach(x => listT.Add(new Chat() // Model.Chat to Thrift.Chat
            {
                IdChat = x.IdChat.ToString(),
                Name = x.Name,
                IsCreatedDefaul = x.IsCreateDefault,
                LastMessageDate = x.LastMessageDate.ToString()
            }));
            _log.Info("GetChatsFromUser done and export the list of Chat\n");
            return Task.FromResult(listT);
        }
        /*
         * Get all Messages from a Chat
         * Parameters: 
         *  idChat: Id(Guid) from a Chat for which the data is request
         * Return:
         *  List of Messages from this Chat
         */
        public Task<List<Message>> GetMessagesFromChat(string idChat, CancellationToken cancellationToken = default)
        {
            _log.Info($"Start GetMessagesFromChat from Id: {idChat}");
            _mut.WaitOne();
            var list = _dbContext.Messages.Where(x => x.IdChat == Guid.Parse(idChat));
            _mut.ReleaseMutex();
            _log.Info("All the Messages from Chat are found. Start preparing data to export");
            var listT = new List<Message>();
            list.ToList().ForEach(x => listT.Add(new Message() //Model.Messages to Thrift.Message
            {
                IdMessage = x.IdMessage.ToString(),
                Content = x.Content,
                ExpedationDate = x.ExpedationDate.ToString(),
                IdChat = x.IdChat.ToString(),
                NameOfShipper = x.NameOfShipper
            }));
            _log.Info("GetMessagesFromChat done and export the list of Message\n");
            return Task.FromResult(listT);
        }
        /*
         * Get all UserInChat in which User participat
         * Parameters:
         *  IdUser: Id(Guid) from a User for which the data is request
         * Return:
         *  List of ChatInUser from this User
         */
        public Task<List<UserInChat>> GetUserInChatFromUser(string idUser, CancellationToken cancellationToken = default)
        {
            _log.Info($"Start GetUserInChatFromUser from Id: {idUser}");
            _mut.WaitOne();
            var list = _dbContext.UserInChats.Where(x => x.IdUser == Guid.Parse(idUser));
            _mut.ReleaseMutex();
            _log.Info("All the UserInChat from User are found. Start preparing data to export");
            var listT = new List<UserInChat>();
            list.ToList().ForEach(x => listT.Add(new UserInChat() //Model.UserInChat to Thrift.UserInChat
            {
                IdChat = x.IdChat.ToString(),
                IdUser = x.IdUser.ToString(),
                IdUserInChat = x.NewMessagesInChat.ToString(),
                IsMuted = x.IsMuted,
                NewMessagesInChat = x.NewMessagesInChat
            }));
            _log.Info("GetUserInChatFromUser done and export the list of UserInChat\n");
            return Task.FromResult(listT);
        }

        /*
         * Get all Users which set a FriendRequest to User
         * Parameters:
         *  IdUser(FromUser): Id(Guid) from a User for which the data is request
         * Return:
         *  List of Users from this User
         */
        public Task<List<User>> GetFromUsersInFriendRequestFromUser(string idUser, CancellationToken cancellationToken = default)
        {
            _log.Info($"Start GetFromUsersInFriendRequestFromUser from Id: {idUser}");
            _mut.WaitOne();
            var list = _dbContext.FriendRequests.Where(x => x.IdToUser == Guid.Parse(idUser)); //All FriendRequest where ToUser is the User 
            var list2 = _dbContext.Users.Where(x => list.Any(y => y.IdFromUser == x.IdUser)); //All User where The User is FromUser in FriendRequest
            _mut.ReleaseMutex();
            _log.Info("All the User which sed a FriendRequest to User Are Found. Start preparing data to export");
            var listT = new List<User>();
            list2.ToList().ForEach(x => listT.Add(new User()  //Model.User to Thrift.Model
            {
                IdUser = x.IdUser.ToString(),
                Name = x.Name,
                Password = null, //set Password and NrOfFriendRequest to default, because the the User don't need to know and don't have acces to this information
                NrOfFriendRequests = 0
            }));
            _log.Info("GetFromUsersInFriendRequestFromUser completed and export the list of User\n");
            return Task.FromResult(listT);
        }
        #endregion
        #region LoginAndLogout
        /* Login from a User
         * Parameters:
         *  name: Name of a User who try login
         *  password: Password of a User who try login
         * Return:
         *  The complet User which all parameters 
         * Exception:
         *  ChatException:
         *   If User don't exist in db from this name and password
         *   If User was alredy logged in program
         */
        public Task<User> Login(string name, string password, CancellationToken cancellationToken = default)
        {
            _log.Info($"Start Login from Name: {name}");
            Model.User user;
            try
            {
                _mut.WaitOne();
                user = _dbContext.Users.First(x => x.Name == name && x.Password == password);
            }
            catch (Exception)
            {
                _mut.ReleaseMutex();
                _log.Warn("Wrong name of Password");
                throw new ChatException()
                {
                    Message = "Wrong Name or Password"
                };
            }
            _mut.ReleaseMutex();
            if (_loggedUsers.Any(x => x.Equals(user.IdUser)))
            {
                _log.Warn("User was logged");
                throw new ChatException()
                {
                    Message = "User was logged"
                };
            }
            User userT = new User() //Model.User to Thrift.User
            {
                IdUser = user.IdUser.ToString(),
                Name = user.Name,
                Password = user.Password,
                NrOfFriendRequests = user.NrOfFriendRequests
            };
            _loggedUsers.Add(user.IdUser);
            _log.Info("Login Completed, the User was added in loggedUsers list and will be returned\n");
            return Task.FromResult(userT);

        }
        /*Logout from a User
         * Parameters:
         *  idUser: Id(Guid) from a User for which is try a logout
         * Return:
         *  default true 
         * Exception:
         *  ChatException:
         *   If User don't exist in db from this name and password
         *   If User was not logged in program
         */
        public Task<bool> Logout(string idUser, CancellationToken cancellationToken = default)
        {
            _log.Info($"Start Logout from Id: {idUser}");
            Model.User user;
            _mut.WaitOne();
            try
            {
                user = _dbContext.Users.First(x => x.IdUser == Guid.Parse(idUser));
            }
            catch (InvalidOperationException)
            {
                _mut.ReleaseMutex();
                throw new ChatException()
                {
                    Message = "User don't exist in db"
                };
            }
            _mut.ReleaseMutex();
            var ok = _loggedUsers.Remove(Guid.Parse(idUser));
            if (ok == false)
            {
                _log.Warn("User was not logged");
                throw new ChatException()
                {
                    Message = "User was not logged"
                };
            }
            _log.Info("Logout Completed, the User are remove from loggedUsers list\n");
            return Task.FromResult(true);

        }
        #endregion
        #region Set
        /*
         * Set a NewMessageInChat propert from a UserInChat to 0
         * Parameters:
         *  idUser: Id(Guid) from a User
         *  idChat: Id(Guid) from a Chat
         * return:
         *  default true
         * Exception:
         *  ChatException:
         *   UserInChat don't exist in db
         */
        public Task<bool> SetNewMessagesInChatTo0(string idUser, string idChat, CancellationToken cancellationToken = default)
        {
            _log.Info($"Start SetNewMessageInChatTo0 from IdUser: {idUser} in IdChat: {idUser}");
            Model.UserInChat userInChat;
            _mut.WaitOne();
            try
            {
                userInChat = _dbContext.UserInChats.First(x => x.IdChat == Guid.Parse(idChat) && x.IdUser == Guid.Parse(idUser));
            }
            catch (InvalidOperationException)
            {
                _mut.ReleaseMutex();
                throw new ChatException()
                {
                    Message = "User is not in this Chat"
                };
            }
            _mut.ReleaseMutex();
            _log.Info("UserInChat found and start update");
            userInChat.NewMessagesInChat = 0;
            _mut.WaitOne();
            _dbContext.Update(userInChat);
            _dbContext.SaveChanges();
            _mut.ReleaseMutex();
            _log.Info("UserInChat updated and SetNewMessagesInChatTo0 is done\n");
            return Task.FromResult(true);
        }
        /*
         * Change parameter IsMuted from an UserInChat to the other value
         * Parameters:
         *  idUser: Id(Guid) from a User
         *  idChat: Id(Guid) from a Chat
         * return:
         *  To the value that has been modified IsMuted Parameter
         * Exception:
         *  ChatException:
         *   UserInChat don't exist in db
         */
        public Task<bool> IsMutedChangeFromAnUserInAChat(string idUser, string idChat, CancellationToken cancellationToken = default)
        {
            _log.Info($"Start IsMutedChangeFromAnUserInAChat from IdUser: {idUser} in IdChat: {idChat}");
            Model.UserInChat userInChat;
            _mut.WaitOne();
            try
            {
                userInChat = _dbContext.UserInChats.First(x => x.IdChat == Guid.Parse(idChat) && x.IdUser == Guid.Parse(idUser));
            }
            catch (InvalidOperationException)
            {
                _mut.ReleaseMutex();
                throw new ChatException()
                {
                    Message = "User is not in this Chat"
                };
            }
            _mut.ReleaseMutex();
            _log.Info("UserInChat found and start update");
            bool r;
            if (userInChat.IsMuted)
            {
                r = userInChat.IsMuted = false;
            }
            else
            {
                r = userInChat.IsMuted = true;
            }
            _mut.WaitOne();
            _dbContext.Update(userInChat);
            _dbContext.SaveChanges();
            _mut.ReleaseMutex();
            _log.Info("UserInChar updated and IsMutedChangeFromAnUserInAChat completed\n");
            return Task.FromResult(r);
        }
        #endregion
        #region FriendRequestOptions
        /*
         * Accept a Friend Request who was set by FromUser to ToUser and creating the relacion beetween they
         * Parameters:
         *  idToUser (ToUser): Id(Guid) from the User who recive the request
         *  idFromUser (From User): Id(guid) from the User who send the request
         * return:
         *  default true:
         * Exception:
         *  ChatException: 
         *   Is FriendRequest don't exist from FromUser and ToUser
         *   FromUser don't exist in db
         *   ToUser don't exist in db
         */
        public Task<bool> AcceptFriendRequest(string idToUser, string idFromUser, CancellationToken cancellationToken = default)
        {
            _log.Info($"Start AcceptFriendRequest From IdUser: {idToUser} and the IdRequestUser: {idFromUser}");
            Model.FriendRequest request;
            _mut.WaitOne();
            try
            {
                request = _dbContext.FriendRequests.First(x => x.IdFromUser == Guid.Parse(idFromUser) && x.IdToUser == Guid.Parse(idToUser));
            }
            catch (InvalidOperationException)
            {
                _mut.ReleaseMutex();
                throw new ChatException()
                {
                    Message = "FriendRequest don't exist from this 2 users"
                };
            }
            _mut.ReleaseMutex();
            _log.Info("FriendRequest Found and start find Users, start create the Chat and Friend between they, and Delete Request");
            Model.User user1, user2;
            _mut.WaitOne();
            try
            {
                user1 = _dbContext.Users.First(x => x.IdUser == Guid.Parse(idToUser));
            }
            catch (InvalidOperationException)
            {
                _mut.ReleaseMutex();
                throw new ChatException()
                {
                    Message = "ToUser not exist in db"
                };
            }
            try
            {
                user2 = _dbContext.Users.First(x => x.IdUser == Guid.Parse(idFromUser));
            }
            catch (InvalidOperationException)
            {
                _mut.ReleaseMutex();
                throw new ChatException()
                {
                    Message = "FromUser not exist in db"
                };
            }
            _mut.ReleaseMutex();
            user1.NrOfFriendRequests -= 1;
            var chat = new Model.Chat() // created default Chat from this new Friends
            {
                IdChat = Guid.NewGuid(),
                Name = $"{user1.Name} {user2.Name}",
                IsCreateDefault = true,
                LastMessageDate = DateTime.Now //default is now
            };
            _log.Info("Configuracion completed, start db part");
            _mut.WaitOne();
            _dbContext.Add(new Model.Friend() 
            {
                IdFriends = Guid.NewGuid(),
                IdUser1 = user1.IdUser,
                IdUser2 = user2.IdUser
            });
            _dbContext.Add(chat);
            _dbContext.Add(new Model.UserInChat()
            {
                IdUserInChat = Guid.NewGuid(),
                IdUser = user1.IdUser,
                IdChat = chat.IdChat
            });
            _dbContext.Add(new Model.UserInChat()
            {
                IdUserInChat = Guid.NewGuid(),
                IdUser = user2.IdUser,
                IdChat = chat.IdChat
            });
            _dbContext.Remove(request);
            _dbContext.Update(user1);
            _dbContext.SaveChanges();
            _mut.ReleaseMutex();
            _log.Info("All db part donem start notify part");
            #region notify
            _rabbitmq.FriendRequest(user1);
            _rabbitmq.NewChat(user1);
            if (_loggedUsers.Contains(user2.IdUser))
            {
                _rabbitmq.NewChat(user2);
            }
            _log.Info("Notify done and AcceptFriendRequest completed\n");
            #endregion
            return Task.FromResult(true);
        }
        /*
         * Deny a friendReques who set FromUser to ToUser
         * Parameters:
         *  idToUser (ToUser): Id(Guid) from the User who recive the request
         *  idFromUser (From User): Id(guid) from the User who send the request
         * return:
         *  default true:
         * Exception:
         *  ChatException: 
         *   Is FriendRequest dont exist from FromUser and ToUser
         */
        public Task<bool> DenyFriendRequest(string idToUser, string idFromUser, CancellationToken cancellationToken = default)
        {
            _log.Info($"Start DenyFriendRequest From IdUser: {idToUser} and the IdRequestUser: {idFromUser}");
            Model.FriendRequest request;
            _mut.WaitOne();
            try
            {
                request = _dbContext.FriendRequests.First(x => x.IdFromUser == Guid.Parse(idFromUser) && x.IdToUser == Guid.Parse(idToUser));
            }
            catch (InvalidOperationException)
            {
                _mut.ReleaseMutex();
                throw new ChatException()
                {
                    Message = "FriendRequest don't exist from this 2 users"
                };
            }
            _mut.ReleaseMutex();
            if (request == null)
            {
                _log.Warn("FriendRequest don't exist between Users");
                throw new ChatException()
                {
                    Message = "FriendRequest don't exist"
                };
            }

            _log.Info("FriendRequest found and start the remove and update FromUser");
            _mut.WaitOne();
            var user = _dbContext.Users.First(x => x.IdUser == Guid.Parse(idToUser));
            user.NrOfFriendRequests -= 1;
            _dbContext.Remove(request);
            _dbContext.Update(user);
            _dbContext.SaveChanges();
            _mut.ReleaseMutex();
            _log.Info("FriendRequest removed and FromUser Updated, start notify FromUser Changed data ");
            #region notify
            _rabbitmq.FriendRequest(user);
            #endregion
            _log.Info("Notify Done and DenyFriendRequest Completed\n");
            return Task.FromResult(true);
        }
        #endregion
    }
}
