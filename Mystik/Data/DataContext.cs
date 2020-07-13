using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mystik.Entities;

namespace Mystik.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<UserConversation> UserConversations { get; set; }
        public DbSet<ManagedConversation> ManagedConversations { get; set; }

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
