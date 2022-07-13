using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model;

namespace Persistance
{
    public class MyDbContext :DbContext
    {
        private ILoggerFactory _loggerFactory;
        public MyDbContext() { }
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
        public MyDbContext(ILoggerFactory loggerFactory)
        {
            this._loggerFactory = loggerFactory;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data source = {AppDomain.CurrentDomain.BaseDirectory}\\ChatServerClientProject");
            }

            optionsBuilder.UseLoggerFactory(_loggerFactory);

            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friend>()
                .HasIndex(p => new { p.IdUser1, p.IdUser2 }).IsUnique();
            modelBuilder.Entity<FriendRequest>()
                .HasIndex(p => new { p.IdFromUser, p.IdToUser }).IsUnique();
        }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<UserInChat> UserInChats { get; set; }
        public DbSet<FriendRequest> FriendRequests{ get; set; }
    }
}