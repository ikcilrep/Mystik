using Microsoft.EntityFrameworkCore;
using Mystik.Entities;

namespace Mystik.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<CoupleOfFriends> Friends { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<UserConversation> UserConversations { get; set; }
        public DbSet<ManagedConversation> ManagedConversations { get; set; }
        public DbSet<Invitation> Invitations { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ManagedConversation>()
                .HasKey(mc => new { mc.ManagerId, mc.ConversationId });

            modelBuilder.Entity<ManagedConversation>()
                        .HasOne(mc => mc.Manager)
                        .WithMany(a => a.ManagedConversations)
                        .HasForeignKey(mc => mc.ManagerId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ManagedConversation>()
                        .HasOne(mc => mc.Conversation)
                        .WithMany(c => c.ManagedConversations)
                        .HasForeignKey(mc => mc.ConversationId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserConversation>()
                .HasKey(uc => new { uc.UserId, uc.ConversationId });

            modelBuilder.Entity<UserConversation>()
                        .HasOne(uc => uc.User)
                        .WithMany(u => u.UserConversations)
                        .HasForeignKey(uc => uc.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserConversation>()
                        .HasOne(uc => uc.Conversation)
                        .WithMany(c => c.UserConversations)
                        .HasForeignKey(uc => uc.ConversationId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invitation>()
                .HasKey(iu => new { iu.InvitedId, iu.InviterId });

            modelBuilder.Entity<Invitation>()
                        .HasOne(iu => iu.Inviter)
                        .WithMany(u => u.SentInvitations)
                        .HasForeignKey(iu => iu.InviterId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invitation>()
                        .HasOne(iu => iu.Invited)
                        .WithMany(u => u.ReceivedInvitations)
                        .HasForeignKey(iu => iu.InvitedId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CoupleOfFriends>()
                .HasKey(cof => new { cof.Friend1Id, cof.Friend2Id });

            modelBuilder.Entity<CoupleOfFriends>()
                        .HasOne(cof => cof.Friend1)
                        .WithMany(u => u.Friends2)
                        .HasForeignKey(cof => cof.Friend1Id)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CoupleOfFriends>()
                        .HasOne(cof => cof.Friend2)
                        .WithMany(u => u.Friends1)
                        .HasForeignKey(cof => cof.Friend2Id)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                        .HasMany(u => u.Messages)
                        .WithOne(m => m.Sender)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Conversation>()
                        .HasMany(u => u.Messages)
                        .WithOne(m => m.Conversation)
                        .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
