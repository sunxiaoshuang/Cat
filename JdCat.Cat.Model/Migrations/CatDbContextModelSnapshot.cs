﻿// <auto-generated />
using JdCat.Cat.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace JdCat.Cat.Model.Migrations
{
    [DbContext(typeof(CatDbContext))]
    partial class CatDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("JdCat.Cat.Model.Data.Business", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("AppId");

                    b.Property<string>("BusinessLicense");

                    b.Property<string>("Code");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("Description");

                    b.Property<string>("Email");

                    b.Property<string>("InvitationCode");

                    b.Property<string>("Mobile");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.Property<DateTime?>("RegisterDate");

                    b.Property<string>("StoreId");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("ID");

                    b.ToTable("Business","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.Product", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BusinessId");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime>("PublishTime");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("ID");

                    b.HasIndex("BusinessId");

                    b.ToTable("Product","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.ProductImage", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("ExtensionName");

                    b.Property<long>("Length");

                    b.Property<string>("Path");

                    b.Property<int>("ProductId");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("ID");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImage","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.Product", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Business", "Business")
                        .WithMany("Products")
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.ProductImage", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Product", "Product")
                        .WithMany("Images")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
