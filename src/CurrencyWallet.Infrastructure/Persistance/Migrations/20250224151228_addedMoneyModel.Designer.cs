﻿// <auto-generated />
using System;
using CurrencyWallet.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CurrencyWallet.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250224151228_addedMoneyModel")]
    partial class addedMoneyModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CurrencyWallet.Domain.Entities.CurrencyRate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Mid")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("CurrencyRates");
                });

            modelBuilder.Entity("CurrencyWallet.Domain.Entities.Wallet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Wallets");
                });

            modelBuilder.Entity("CurrencyWallet.Domain.Entities.Wallet", b =>
                {
                    b.OwnsMany("CurrencyWallet.Domain.Entities.WalletBalance", "Balances", b1 =>
                        {
                            b1.Property<Guid>("WalletId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.HasKey("WalletId", "Id");

                            b1.ToTable("WalletBalance");

                            b1.WithOwner()
                                .HasForeignKey("WalletId");

                            b1.OwnsOne("CurrencyWallet.Domain.ValueObjects.Money", "Money", b2 =>
                                {
                                    b2.Property<Guid>("WalletBalanceWalletId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<int>("WalletBalanceId")
                                        .HasColumnType("int");

                                    b2.Property<decimal>("Amount")
                                        .HasColumnType("decimal(18,2)")
                                        .HasColumnName("Amount");

                                    b2.Property<string>("Currency")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)")
                                        .HasColumnName("Currency");

                                    b2.HasKey("WalletBalanceWalletId", "WalletBalanceId");

                                    b2.ToTable("WalletBalance");

                                    b2.WithOwner()
                                        .HasForeignKey("WalletBalanceWalletId", "WalletBalanceId");
                                });

                            b1.Navigation("Money")
                                .IsRequired();
                        });

                    b.Navigation("Balances");
                });
#pragma warning restore 612, 618
        }
    }
}
