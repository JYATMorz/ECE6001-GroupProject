﻿// <auto-generated />
using System;
using GameFellowship.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GameFellowship.Migrations
{
    [DbContext(typeof(GameFellowshipDb))]
    [Migration("20230412121625_AddUserLoginDateTime")]
    partial class AddUserLoginDateTime
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.4");

            modelBuilder.Entity("GameFellowship.Data.Database.Conversation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Context")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("CreatorId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PostId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("SendTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("PostId");

                    b.ToTable("Conversations");
                });

            modelBuilder.Entity("GameFellowship.Data.Database.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Followers")
                        .HasColumnType("INTEGER");

                    b.Property<string>("IconURI")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastPostDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("GameFellowship.Data.Database.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AudioChat")
                        .HasColumnType("INTEGER");

                    b.Property<string>("AudioLink")
                        .HasColumnType("TEXT");

                    b.Property<string>("AudioPlatform")
                        .HasColumnType("TEXT");

                    b.Property<int>("CreatorId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CurrentPeople")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("TEXT");

                    b.Property<string>("MatchType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("PlayNow")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Requirements")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("TotalPeople")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("GameId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("GameFellowship.Data.Database.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("IconURI")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GameUser", b =>
                {
                    b.Property<int>("FollowedGamesId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FollowingUsersId")
                        .HasColumnType("INTEGER");

                    b.HasKey("FollowedGamesId", "FollowingUsersId");

                    b.HasIndex("FollowingUsersId");

                    b.ToTable("GameUser");
                });

            modelBuilder.Entity("PostUser", b =>
                {
                    b.Property<int>("JoinedPostsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("JoinedUsersId")
                        .HasColumnType("INTEGER");

                    b.HasKey("JoinedPostsId", "JoinedUsersId");

                    b.HasIndex("JoinedUsersId");

                    b.ToTable("PostUser");
                });

            modelBuilder.Entity("UserUser", b =>
                {
                    b.Property<int>("FriendUsersId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("FriendUsersId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserUser");
                });

            modelBuilder.Entity("GameFellowship.Data.Database.Conversation", b =>
                {
                    b.HasOne("GameFellowship.Data.Database.User", "Creator")
                        .WithMany("MyConversations")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GameFellowship.Data.Database.Post", "Post")
                        .WithMany("Conversations")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("GameFellowship.Data.Database.Post", b =>
                {
                    b.HasOne("GameFellowship.Data.Database.User", "Creator")
                        .WithMany("CreatedPosts")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GameFellowship.Data.Database.Game", "Game")
                        .WithMany("Posts")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("Game");
                });

            modelBuilder.Entity("GameUser", b =>
                {
                    b.HasOne("GameFellowship.Data.Database.Game", null)
                        .WithMany()
                        .HasForeignKey("FollowedGamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GameFellowship.Data.Database.User", null)
                        .WithMany()
                        .HasForeignKey("FollowingUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PostUser", b =>
                {
                    b.HasOne("GameFellowship.Data.Database.Post", null)
                        .WithMany()
                        .HasForeignKey("JoinedPostsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GameFellowship.Data.Database.User", null)
                        .WithMany()
                        .HasForeignKey("JoinedUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UserUser", b =>
                {
                    b.HasOne("GameFellowship.Data.Database.User", null)
                        .WithMany()
                        .HasForeignKey("FriendUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GameFellowship.Data.Database.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GameFellowship.Data.Database.Game", b =>
                {
                    b.Navigation("Posts");
                });

            modelBuilder.Entity("GameFellowship.Data.Database.Post", b =>
                {
                    b.Navigation("Conversations");
                });

            modelBuilder.Entity("GameFellowship.Data.Database.User", b =>
                {
                    b.Navigation("CreatedPosts");

                    b.Navigation("MyConversations");
                });
#pragma warning restore 612, 618
        }
    }
}