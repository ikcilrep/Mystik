﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mystik.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Mystik.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<byte[]>("PasswordHashData")
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.ToTable("Conversations");
                });

            modelBuilder.Entity("Mystik.Entities.ManagedConversation", b =>
                {
                    b.Property<Guid>("AdminId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ConversationId")
                        .HasColumnType("uuid");

                    b.HasKey("AdminId", "ConversationId");

                    b.HasIndex("ConversationId");

                    b.ToTable("ManagedConversation");
                });

            modelBuilder.Entity("Mystik.Entities.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ConversationId")
                        .HasColumnType("uuid");

                    b.Property<string>("EncryptedContentPath")
                        .HasColumnType("text");

                    b.Property<Guid?>("SenderId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("SentTime")
                        .HasColumnType("timestamp without time zone");

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

                    b.HasKey("UserId", "ConversationId");

                    b.HasIndex("ConversationId");

                    b.ToTable("UserConversation");
                });

            modelBuilder.Entity("Mystik.Entities.ManagedConversation", b =>
                {
                    b.HasOne("Mystik.Entities.User", "Admin")
                        .WithMany("ManagedConversations")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mystik.Entities.Conversation", "Conversation")
                        .WithMany("ManagedConversations")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mystik.Entities.Message", b =>
                {
                    b.HasOne("Mystik.Entities.Conversation", "Conversation")
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId");

                    b.HasOne("Mystik.Entities.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId");
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
