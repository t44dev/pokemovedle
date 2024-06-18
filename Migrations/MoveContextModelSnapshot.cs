﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeMovedle.Models.Moves;

#nullable disable

namespace PokeMovedle.Migrations
{
    [DbContext(typeof(MoveContext))]
    partial class MoveContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("PokeMovedle.Models.Moves.Move", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("accuracy")
                        .HasColumnType("INTEGER");

                    b.Property<int>("damageClass")
                        .HasColumnType("INTEGER");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("power")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("pp")
                        .HasColumnType("INTEGER");

                    b.Property<int>("type")
                        .HasColumnType("INTEGER");

                    b.HasKey("id");

                    b.ToTable("moves");
                });

            modelBuilder.Entity("PokeMovedle.Models.Moves.TypeMatchup", b =>
                {
                    b.Property<int>("attacker")
                        .HasColumnType("INTEGER");

                    b.Property<int>("defender")
                        .HasColumnType("INTEGER");

                    b.Property<float>("multiplier")
                        .HasColumnType("REAL");

                    b.HasKey("attacker", "defender");

                    b.ToTable("matchups");
                });
#pragma warning restore 612, 618
        }
    }
}
