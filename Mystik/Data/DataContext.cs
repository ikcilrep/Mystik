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
        public DbSet<ConversationMember> UserConversations { get; set; }
        public DbSet<ConversationManager> ManagedConversations { get; set; }
        public DbSet<Invitation> Invitations { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConversationManager>()
                .HasKey(mc => new { mc.ManagerId, mc.ConversationId });

            modelBuilder.Entity<ConversationManager>()
                        .HasOne(mc => mc.Manager)
                        .WithMany(u => u.ManagedConversations)
                        .HasForeignKey(mc => mc.ManagerId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConversationManager>()
                        .HasOne(mc => mc.Conversation)
                        .WithMany(c => c.ManagedConversations)
                        .HasForeignKey(mc => mc.ConversationId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConversationMember>()
                .HasKey(uc => new { uc.UserId, uc.ConversationId });

            modelBuilder.Entity<ConversationMember>()
                        .HasOne(uc => uc.User)
                        .WithMany(u => u.ParticipatedConversations)
                        .HasForeignKey(uc => uc.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConversationMember>()
                        .HasOne(uc => uc.Conversation)
                        .WithMany(c => c.UserConversations)
                        .HasForeignKey(uc => uc.ConversationId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invitation>()
                .HasKey(i => new { i.InvitedId, i.InviterId });

            modelBuilder.Entity<Invitation>()
                        .HasOne(i => i.Inviter)
                        .WithMany(u => u.SentInvitations)
                        .HasForeignKey(i => i.InviterId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invitation>()
                        .HasOne(i => i.Invited)
                        .WithMany(u => u.ReceivedInvitations)
                        .HasForeignKey(i => i.InvitedId)
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
