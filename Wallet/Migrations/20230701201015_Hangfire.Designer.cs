﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Wallet.Tools.database;

#nullable disable

namespace Wallet.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20230701201015_Hangfire")]
    partial class Hangfire
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Wallet.Modules.asset_module.Asset", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("Id");

                    b.Property<int?>("Class")
                        .HasColumnType("integer")
                        .HasColumnName("Class");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("Description");

                    b.Property<double>("Price")
                        .HasColumnType("double precision")
                        .HasColumnName("Price");

                    b.Property<string>("Ticker")
                        .HasColumnType("text")
                        .HasColumnName("Ticker");

                    b.HasKey("Id");

                    b.ToTable("ASSET");
                });

            modelBuilder.Entity("Wallet.Modules.position_module.Position", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("Id");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision")
                        .HasColumnName("Amount");

                    b.Property<string>("AssetId")
                        .HasColumnType("text")
                        .HasColumnName("AssetId");

                    b.Property<double>("AveragePrice")
                        .HasColumnType("double precision")
                        .HasColumnName("AveragePrice");

                    b.Property<double>("TotalBought")
                        .HasColumnType("double precision")
                        .HasColumnName("TotalBought");

                    b.Property<double>("TotalGainLoss")
                        .HasColumnType("double precision")
                        .HasColumnName("TotalGainLoss");

                    b.Property<double>("TotalSold")
                        .HasColumnType("double precision")
                        .HasColumnName("TotalSold");

                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("UserId");

                    b.HasKey("Id");

                    b.HasIndex("AssetId");

                    b.HasIndex("UserId");

                    b.ToTable("POSITION");
                });

            modelBuilder.Entity("Wallet.Modules.trade_module.Trade", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("Id");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision")
                        .HasColumnName("Amount");

                    b.Property<string>("AssetId")
                        .HasColumnType("text")
                        .HasColumnName("AssetId");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("Date");

                    b.Property<double>("Price")
                        .HasColumnType("double precision")
                        .HasColumnName("Price");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("Type");

                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("UserId");

                    b.HasKey("Id");

                    b.HasIndex("AssetId");

                    b.HasIndex("UserId");

                    b.ToTable("TRADE");
                });

            modelBuilder.Entity("Wallet.Modules.user_module.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("Id");

                    b.Property<string>("CPF")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("CPF");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Email");

                    b.Property<bool>("IsEmailConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("IsEmailConfirmed");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Name");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("PasswordHash");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("PasswordSalt");

                    b.Property<int>("Role")
                        .HasColumnType("integer")
                        .HasColumnName("Role");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("UserName");

                    b.HasKey("Id");

                    b.ToTable("USER");
                });

            modelBuilder.Entity("Wallet.Tools.session_control.SessionControl", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("Id");

                    b.Property<DateTime?>("DateLogin")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DateLogout")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SessionId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("SESSION_CONTROL");
                });

            modelBuilder.Entity("Wallet.Modules.position_module.Position", b =>
                {
                    b.HasOne("Wallet.Modules.asset_module.Asset", "Asset")
                        .WithMany()
                        .HasForeignKey("AssetId");

                    b.HasOne("Wallet.Modules.user_module.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Asset");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Wallet.Modules.trade_module.Trade", b =>
                {
                    b.HasOne("Wallet.Modules.asset_module.Asset", "Asset")
                        .WithMany()
                        .HasForeignKey("AssetId");

                    b.HasOne("Wallet.Modules.user_module.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Asset");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Wallet.Tools.session_control.SessionControl", b =>
                {
                    b.HasOne("Wallet.Modules.user_module.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
