﻿// <auto-generated />
using JdCat.Cat.Model;
using JdCat.Cat.Model.Enum;
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

                    b.Property<int>("Feature");

                    b.Property<decimal?>("MinBuyQuantity");

                    b.Property<DateTime?>("ModifyTime");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime?>("NotSaleTime");

                    b.Property<int?>("ProductTypeId");

                    b.Property<DateTime?>("PublishTime");

                    b.Property<int>("Status");

                    b.Property<string>("Tag");

                    b.Property<string>("UnitName");

                    b.HasKey("ID");

                    b.HasIndex("BusinessId");

                    b.HasIndex("ProductTypeId");

                    b.ToTable("Product","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.ProductAttribute", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("Item1");

                    b.Property<string>("Item2");

                    b.Property<string>("Item3");

                    b.Property<string>("Item4");

                    b.Property<string>("Item5");

                    b.Property<string>("Item6");

                    b.Property<string>("Item7");

                    b.Property<string>("Item8");

                    b.Property<string>("Name");

                    b.Property<int>("ProductId");

                    b.HasKey("ID");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductAttribute","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.ProductFormat", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("Name");

                    b.Property<decimal?>("PackingPrice");

                    b.Property<decimal?>("PackingQuantity");

                    b.Property<string>("Position");

                    b.Property<decimal?>("Price");

                    b.Property<int>("ProductId");

                    b.Property<string>("SKU");

                    b.Property<decimal?>("Stock");

                    b.Property<string>("UPC");

                    b.HasKey("ID");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductFormat","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.ProductImage", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("ExtensionName");

                    b.Property<long>("Length");

                    b.Property<string>("Name");

                    b.Property<int>("ProductId");

                    b.Property<int>("Type");

                    b.HasKey("ID");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImage","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.ProductType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BusinessId");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Sort");

                    b.HasKey("ID");

                    b.HasIndex("BusinessId");

                    b.ToTable("ProductType","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.SettingProductAttribute", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateTime");

                    b.Property<int>("Level");

                    b.Property<string>("Name");

                    b.Property<int?>("ParentId");

                    b.Property<int>("Sort");

                    b.HasKey("ID");

                    b.HasIndex("ParentId");

                    b.ToTable("SettingProductAttribute","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.Product", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Business", "Business")
                        .WithMany("Products")
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JdCat.Cat.Model.Data.ProductType", "ProductType")
                        .WithMany("Products")
                        .HasForeignKey("ProductTypeId");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.ProductAttribute", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Product", "Product")
                        .WithMany("Attributes")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.ProductFormat", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Product", "Product")
                        .WithMany("Formats")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.ProductImage", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Product", "Product")
                        .WithMany("Images")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.ProductType", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Business", "Business")
                        .WithMany("ProductsTypes")
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.SettingProductAttribute", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.SettingProductAttribute", "Parent")
                        .WithMany("Childs")
                        .HasForeignKey("ParentId");
                });
#pragma warning restore 612, 618
        }
    }
}
