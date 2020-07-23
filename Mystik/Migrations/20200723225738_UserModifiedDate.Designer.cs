﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mystik.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Mystik.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20200723225738_UserModifiedDate")]
    partial class UserModifiedDate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "5.0.0-preview.6.20312.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Mystik.Entities.Conversation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<byte[]>("PasswordHashData")
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.ToTable("Conversations");
                });

            modelBuilder.Entity("Mystik.Entities.CoupleOfFriends", b =>
                {
                    b.Property<Guid>("Friend1Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("Friend2Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Friend1Id", "Friend2Id");

                    b.HasIndex("Friend2Id");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("Mystik.Entities.Invitation", b =>
                {
                    b.Property<Guid>("InvitedId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("InviterId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("InvitedId", "InviterId");

                    b.HasIndex("InviterId");

                    b.ToTable("Invitations");
                });

            modelBuilder.Entity("Mystik.Entities.ManagedConversation", b =>
                {
                    b.Property<Guid>("ManagerId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ConversationId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("ManagerId", "ConversationId");

                    b.HasIndex("ConversationId");

                    b.ToTable("ManagedConversations");
                });

            modelBuilder.Entity("Mystik.Entities.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ConversationId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ConversationId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Mystik.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Nickname")
                        .HasColumnType("text");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("bytea");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("bytea");

                    b.Property<string>("Role")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Mystik.Entities.UserConversation", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ConversationId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("UserId", "ConversationId");

                    b.HasIndex("ConversationId");

                    b.ToTable("UserConversations");
                });

            modelBuilder.Entity("Mystik.Entities.CoupleOfFriends", b =>
                {
                    b.HasOne("Mystik.Entities.User", "Friend1")
                        .WithMany("Friends2")
                        .HasForeignKey("Friend1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mystik.Entities.User", "Friend2")
                        .WithMany("Friends1")
                        .HasForeignKey("Friend2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mystik.Entities.Invitation", b =>
                {
                    b.HasOne("Mystik.Entities.User", "Invited")
                        .WithMany("ReceivedInvitations")
                        .HasForeignKey("InvitedId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mystik.Entities.User", "Inviter")
                        .WithMany("SentInvitations")
                        .HasForeignKey("InviterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mystik.Entities.ManagedConversation", b =>
                {
                    b.HasOne("Mystik.Entities.Conversation", "Conversation")
                        .WithMany("ManagedConversations")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mystik.Entities.User", "Manager")
                        .WithMany("ManagedConversations")
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mystik.Entities.Message", b =>
                {
                    b.HasOne("Mystik.Entities.Conversation", "Conversation")
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mystik.Entities.User", "Sender")
                        .WithMany("Messages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mystik.Entities.UserConversation", b =>
                {
                    b.HasOne("Mystik.Entities.Conversation", "Conversation")
                        .WithMany("UserConversations")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mystik.Entities.User", "User")
                        .WithMany("UserConversations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
