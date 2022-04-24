﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TelegramBotCosmetics.Domain;

#nullable disable

namespace TelegramBotCosmetics.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220215085315_v2")]
    partial class v2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CatalogItem", b =>
                {
                    b.Property<Guid>("CatalogsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ItemsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CatalogsId", "ItemsId");

                    b.HasIndex("ItemsId");

                    b.ToTable("CatalogItem");
                });

            modelBuilder.Entity("TelegramBotCosmetics.Domain.Entity.Catalog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Href")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Catalogs");
                });

            modelBuilder.Entity("TelegramBotCosmetics.Domain.Entity.Formula", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.ToTable("Formulas");
                });

            modelBuilder.Entity("TelegramBotCosmetics.Domain.Entity.Item", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Brend")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("Href")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IdItemOnPage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Price")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("CatalogItem", b =>
                {
                    b.HasOne("TelegramBotCosmetics.Domain.Entity.Catalog", null)
                        .WithMany()
                        .HasForeignKey("CatalogsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TelegramBotCosmetics.Domain.Entity.Item", null)
                        .WithMany()
                        .HasForeignKey("ItemsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TelegramBotCosmetics.Domain.Entity.Formula", b =>
                {
                    b.HasOne("TelegramBotCosmetics.Domain.Entity.Item", null)
                        .WithMany("Formulas")
                        .HasForeignKey("ItemId");
                });

            modelBuilder.Entity("TelegramBotCosmetics.Domain.Entity.Item", b =>
                {
                    b.Navigation("Formulas");
                });
#pragma warning restore 612, 618
        }
    }
}
