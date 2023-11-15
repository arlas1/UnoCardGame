﻿// <auto-generated />

using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Domain.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.13");

            modelBuilder.Entity("Domain.Database.GameState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CurrentPlayerIndex")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameDirection")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IsColorChosen")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SelectedCardIndex")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("GameStates");
                });

            modelBuilder.Entity("Domain.Database.Hand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardColor")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardValue")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameStateId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Hands");
                });

            modelBuilder.Entity("Domain.Database.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameStateId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Domain.Database.StockPile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardColor")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardValue")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameStateId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("StockPiles");
                });

            modelBuilder.Entity("Domain.Database.UnoDeck", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardColor")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardValue")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameStateId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("UnoDecks");
                });
#pragma warning restore 612, 618
        }
    }
}