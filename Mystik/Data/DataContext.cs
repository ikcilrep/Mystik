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
                .HasKey(mc => new { mc.AdminId, mc.ConversationId });

            modelBuilder.Entity<ManagedConversation>()
                        .HasOne(mc => mc.Admin)
                        .WithMany(a => a.ManagedConversations)
                        .HasForeignKey(mc => mc.AdminId);

            modelBuilder.Entity<ManagedConversation>()
                        .HasOne(mc => mc.Conversation)
                        .WithMany(c => c.ManagedConversations)
                        .HasForeignKey(mc => mc.ConversationId);

            modelBuilder.Entity<UserConversation>()
                .HasKey(uc => new { uc.UserId, uc.ConversationId });

            modelBuilder.Entity<UserConversation>()
                        .HasOne(uc => uc.User)
                        .WithMany(u => u.UserConversations)
                        .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserConversation>()
                        .HasOne(uc => uc.Conversation)
                        .WithMany(c => c.UserConversations)
                        .HasForeignKey(uc => uc.ConversationId);
        }
    }
}
