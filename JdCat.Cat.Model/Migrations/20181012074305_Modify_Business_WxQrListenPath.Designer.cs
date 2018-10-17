﻿// <auto-generated />
using JdCat.Cat.Common.Dianwoda;
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
    [Migration("20181012074305_Modify_Business_WxQrListenPath")]
    partial class Modify_Business_WxQrListenPath
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("Relational:Sequence:shared.FormatNumbers", "'FormatNumbers', 'shared', '1', '1', '', '', 'Int32', 'False'")
                .HasAnnotation("Relational:Sequence:shared.OrderNumbers", "'OrderNumbers', 'shared', '1', '1', '', '', 'Int32', 'False'")
                .HasAnnotation("Relational:Sequence:shared.SaleCouponNumbers", "'SaleCouponNumbers', 'shared', '1', '1', '', '', 'Int32', 'False'")
                .HasAnnotation("Relational:Sequence:shared.StoreNumbers", "'StoreNumbers', 'shared', '1', '1', '', '', 'Int32', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("JdCat.Cat.Model.Data.Address", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AreaName");

                    b.Property<string>("CityName");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("DetailInfo");

                    b.Property<int>("Gender");

                    b.Property<double>("Lat");

                    b.Property<double>("Lng");

                    b.Property<string>("MapInfo");

                    b.Property<DateTime>("ModifyTime");

                    b.Property<string>("Phone");

                    b.Property<string>("PostalCode");

                    b.Property<string>("ProvinceName");

                    b.Property<string>("Receiver");

                    b.Property<int>("UserId");

                    b.HasKey("ID");

                    b.HasIndex("UserId");

                    b.ToTable("Address","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.Business", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("AppId");

                    b.Property<string>("BusinessEndTime");

                    b.Property<string>("BusinessLicense");

                    b.Property<string>("BusinessLicenseImage");

                    b.Property<string>("BusinessStartTime");

                    b.Property<string>("CityCode");

                    b.Property<string>("CityName");

                    b.Property<string>("Code");

                    b.Property<string>("Contact");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("DadaShopNo");

                    b.Property<string>("DadaSourceId");

                    b.Property<string>("DefaultPrinterDevice");

                    b.Property<string>("Description");

                    b.Property<string>("Email");

                    b.Property<string>("FeyinApiKey");

                    b.Property<string>("FeyinMemberCode");

                    b.Property<decimal?>("Freight");

                    b.Property<string>("InvitationCode");

                    b.Property<bool>("IsAutoReceipt");

                    b.Property<bool>("IsClose");

                    b.Property<bool>("IsPublish");

                    b.Property<double>("Lat");

                    b.Property<double>("Lng");

                    b.Property<string>("LogoSrc");

                    b.Property<string>("MchId");

                    b.Property<string>("MchKey");

                    b.Property<decimal>("MinAmount");

                    b.Property<string>("Mobile");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.Property<double>("Range");

                    b.Property<DateTime?>("RegisterDate");

                    b.Property<string>("Secret");

                    b.Property<int>("ServiceProvider");

                    b.Property<string>("SpecialImage");

                    b.Property<string>("StoreId")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("'JD' + dbo.fn_right_padding(NEXT VALUE FOR shared.StoreNumbers, 6)");

                    b.Property<string>("TemplateNotifyId");

                    b.Property<string>("WxQrListenPath");

                    b.HasKey("ID");

                    b.ToTable("Business","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.City", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("City","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.DadaCallBack", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateTime");

                    b.Property<int>("OrderId");

                    b.Property<int>("cancel_from");

                    b.Property<string>("cancel_reason");

                    b.Property<string>("client_id");

                    b.Property<int>("dm_id");

                    b.Property<string>("dm_mobile");

                    b.Property<string>("dm_name");

                    b.Property<string>("order_id");

                    b.Property<int>("order_status");

                    b.Property<string>("signature");

                    b.Property<int>("update_time");

                    b.HasKey("ID");

                    b.HasIndex("OrderId");

                    b.ToTable("DadaCallBack","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.DadaCancelReason", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateTime");

                    b.Property<int>("FlagId");

                    b.Property<string>("Reason");

                    b.HasKey("ID");

                    b.ToTable("DadaCancelReason","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.DadaLiquidatedDamages", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateTime");

                    b.Property<double>("Deduct_fee");

                    b.Property<int>("OrderId");

                    b.HasKey("ID");

                    b.HasIndex("OrderId");

                    b.ToTable("DadaLiquidatedDamages","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.DadaReturn", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<double?>("CouponFee");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<double>("DeliverFee");

                    b.Property<double>("Distance");

                    b.Property<double>("Fee");

                    b.Property<double?>("InsuranceFee");

                    b.Property<int>("OrderId");

                    b.Property<double?>("Tips");

                    b.HasKey("ID");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("DadaReturn","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.DWD_Recharge", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Amount");

                    b.Property<string>("Code");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<int>("DWD_BusinessId");

                    b.Property<string>("DwdCode");

                    b.Property<bool>("IsFinish");

                    b.Property<int>("Mode");

                    b.HasKey("ID");

                    b.HasIndex("DWD_BusinessId");

                    b.ToTable("DWD_Recharges");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.DWDStore", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BusinessId");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("addr");

                    b.Property<string>("city_code");

                    b.Property<string>("city_name");

                    b.Property<string>("external_shopid");

                    b.Property<double>("lat");

                    b.Property<double>("lng");

                    b.Property<string>("mobile");

                    b.Property<string>("shop_title");

                    b.HasKey("ID");

                    b.HasIndex("BusinessId")
                        .IsUnique();

                    b.ToTable("DWDStore","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.FeyinDevice", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApiKey");

                    b.Property<int>("BusinessId");

                    b.Property<string>("Code");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("MemberCode");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.HasIndex("BusinessId");

                    b.ToTable("FeyinDevice","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.Order", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("AchieveTime");

                    b.Property<int?>("BusinessId");

                    b.Property<double?>("CallbackCost");

                    b.Property<int>("Category");

                    b.Property<string>("CityCode");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<int>("DeliveryMode");

                    b.Property<int>("DistributionFlow");

                    b.Property<DateTime?>("DistributionTime");

                    b.Property<string>("ErrorReason");

                    b.Property<decimal?>("Freight");

                    b.Property<int>("Identifier");

                    b.Property<bool>("IsSendDada");

                    b.Property<double>("Lat");

                    b.Property<double>("Lng");

                    b.Property<int>("LogisticsType");

                    b.Property<decimal?>("OldPrice");

                    b.Property<string>("OpenId");

                    b.Property<string>("OrderCode")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("CONVERT(varchar(10), GETDATE(), 112) + dbo.fn_right_padding(NEXT VALUE FOR shared.OrderNumbers, 6) + dbo.fn_right_padding(CAST(floor(rand()*100000) as varchar(5)), 5)");

                    b.Property<DateTime?>("PayTime");

                    b.Property<int>("PaymentType");

                    b.Property<string>("Phone");

                    b.Property<string>("PrepayId");

                    b.Property<decimal?>("Price");

                    b.Property<string>("ReceiverAddress");

                    b.Property<string>("ReceiverName");

                    b.Property<string>("RejectReasion");

                    b.Property<string>("Remark");

                    b.Property<int?>("SaleCouponUserId");

                    b.Property<int?>("SaleFullReduceId");

                    b.Property<int>("Status");

                    b.Property<int?>("TablewareQuantity");

                    b.Property<decimal?>("Tips");

                    b.Property<int>("Type");

                    b.Property<int?>("UserId");

                    b.Property<string>("WxPayCode");

                    b.HasKey("ID");

                    b.HasIndex("BusinessId");

                    b.HasIndex("SaleCouponUserId")
                        .IsUnique()
                        .HasFilter("[SaleCouponUserId] IS NOT NULL");

                    b.HasIndex("SaleFullReduceId");

                    b.HasIndex("UserId");

                    b.ToTable("Order","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.OrderProduct", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("Description");

                    b.Property<decimal?>("Discount");

                    b.Property<int?>("FormatId");

                    b.Property<string>("Name");

                    b.Property<decimal?>("OldPrice");

                    b.Property<int>("OrderId");

                    b.Property<decimal?>("Price");

                    b.Property<int?>("ProductId");

                    b.Property<decimal?>("Quantity");

                    b.Property<string>("Src");

                    b.HasKey("ID");

                    b.HasIndex("FormatId");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderProduct","dbo");
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

                    b.Property<string>("Code")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("'F' + CAST(YEAR(GETDATE()) AS varchar) + dbo.fn_right_padding(NEXT VALUE FOR shared.FormatNumbers, 9)");

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

            modelBuilder.Entity("JdCat.Cat.Model.Data.SaleCoupon", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BusinessId");

                    b.Property<int>("Consumed");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<DateTime?>("EndDate");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDelete");

                    b.Property<double>("MinConsume");

                    b.Property<string>("Name");

                    b.Property<int>("Quantity");

                    b.Property<int>("Received");

                    b.Property<DateTime?>("StartDate");

                    b.Property<int>("Stock");

                    b.Property<int?>("ValidDay");

                    b.Property<double>("Value");

                    b.HasKey("ID");

                    b.HasIndex("BusinessId");

                    b.ToTable("SaleCoupon","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.SaleCouponUser", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("dbo.fn_right_padding(floor(rand()*10000000), 6) + cast(NEXT VALUE FOR shared.SaleCouponNumbers as varchar(max)) + dbo.fn_right_padding(floor(rand()*100000), 4)");

                    b.Property<int>("CouponId");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<DateTime?>("EndDate");

                    b.Property<double>("MinConsume");

                    b.Property<string>("Name");

                    b.Property<int?>("OrderId");

                    b.Property<DateTime?>("StartDate");

                    b.Property<int>("Status");

                    b.Property<DateTime?>("UseTime");

                    b.Property<int>("UserId");

                    b.Property<int?>("ValidDay");

                    b.Property<double>("Value");

                    b.HasKey("ID");

                    b.HasIndex("CouponId");

                    b.HasIndex("UserId");

                    b.ToTable("SaleCouponUser","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.SaleFullReduce", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BusinessId");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<DateTime?>("EndDate");

                    b.Property<bool>("IsDelete");

                    b.Property<bool>("IsForeverValid");

                    b.Property<decimal>("MinPrice");

                    b.Property<DateTime?>("ModifyTime");

                    b.Property<string>("Name");

                    b.Property<decimal>("ReduceMoney");

                    b.Property<DateTime?>("StartDate");

                    b.HasKey("ID");

                    b.HasIndex("BusinessId");

                    b.ToTable("SaleFullReduce","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.SaleProductDiscount", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BusinessId");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<int>("Cycle");

                    b.Property<decimal>("Discount");

                    b.Property<DateTime?>("EndDate");

                    b.Property<string>("EndTime1");

                    b.Property<string>("EndTime2");

                    b.Property<string>("EndTime3");

                    b.Property<string>("Name");

                    b.Property<decimal>("OldPrice");

                    b.Property<decimal>("Price");

                    b.Property<int>("ProductId");

                    b.Property<string>("SettingType");

                    b.Property<DateTime?>("StartDate");

                    b.Property<string>("StartTime1");

                    b.Property<string>("StartTime2");

                    b.Property<string>("StartTime3");

                    b.Property<int>("Status");

                    b.Property<int>("UpperLimit");

                    b.HasKey("ID");

                    b.HasIndex("BusinessId");

                    b.HasIndex("ProductId");

                    b.ToTable("SaleProductDiscount","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.SessionData", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("SessionKey");

                    b.Property<int>("UserId");

                    b.HasKey("ID");

                    b.HasIndex("UserId");

                    b.ToTable("SessionData","dbo");
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

            modelBuilder.Entity("JdCat.Cat.Model.Data.ShoppingCart", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BusinessId");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("Description");

                    b.Property<int>("FormatId");

                    b.Property<string>("Name");

                    b.Property<int>("PackingQuantity");

                    b.Property<decimal?>("Price");

                    b.Property<int>("ProductId");

                    b.Property<int>("Quantity");

                    b.Property<string>("Src");

                    b.Property<int>("UserId");

                    b.HasKey("ID");

                    b.HasIndex("BusinessId");

                    b.HasIndex("FormatId");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("ShoppingCart","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Age");

                    b.Property<string>("AvatarUrl");

                    b.Property<int>("BusinessId");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<int>("Gender");

                    b.Property<bool>("IsPhone");

                    b.Property<bool>("IsRegister");

                    b.Property<string>("Language");

                    b.Property<string>("NickName");

                    b.Property<string>("OpenId");

                    b.Property<string>("Phone");

                    b.Property<string>("Province");

                    b.HasKey("ID");

                    b.HasIndex("OpenId")
                        .IsUnique()
                        .HasFilter("[OpenId] IS NOT NULL");

                    b.ToTable("User","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.WxListenUser", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BusinessId");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<string>("city");

                    b.Property<string>("country");

                    b.Property<string>("headimgurl");

                    b.Property<string>("nickname");

                    b.Property<string>("openid");

                    b.Property<string>("province");

                    b.Property<int>("sex");

                    b.HasKey("ID");

                    b.HasIndex("BusinessId");

                    b.ToTable("WxListenUser","dbo");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.Address", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.User", "User")
                        .WithMany("Addresses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.DadaCallBack", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Order", "Order")
                        .WithMany("DadaCallBacks")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.DadaLiquidatedDamages", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Order", "Order")
                        .WithMany("DadaLiquidatedDamages")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.DadaReturn", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Order", "Order")
                        .WithOne("DadaReturn")
                        .HasForeignKey("JdCat.Cat.Model.Data.DadaReturn", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.DWD_Recharge", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.DWDStore", "DWD_Business")
                        .WithMany("DWD_Recharges")
                        .HasForeignKey("DWD_BusinessId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.DWDStore", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Business", "Business")
                        .WithOne("DWDStore")
                        .HasForeignKey("JdCat.Cat.Model.Data.DWDStore", "BusinessId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.FeyinDevice", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Business", "Business")
                        .WithMany("FeyinDevices")
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.Order", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Business", "Business")
                        .WithMany("Orders")
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("JdCat.Cat.Model.Data.SaleCouponUser", "SaleCouponUser")
                        .WithOne("Order")
                        .HasForeignKey("JdCat.Cat.Model.Data.Order", "SaleCouponUserId");

                    b.HasOne("JdCat.Cat.Model.Data.SaleFullReduce", "SaleFullReduce")
                        .WithMany("Orders")
                        .HasForeignKey("SaleFullReduceId");

                    b.HasOne("JdCat.Cat.Model.Data.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.OrderProduct", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.ProductFormat", "Format")
                        .WithMany("OrderProducts")
                        .HasForeignKey("FormatId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("JdCat.Cat.Model.Data.Order", "Order")
                        .WithMany("Products")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JdCat.Cat.Model.Data.Product", "Product")
                        .WithMany("OrderProducts")
                        .HasForeignKey("ProductId");
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

            modelBuilder.Entity("JdCat.Cat.Model.Data.SaleCoupon", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Business", "Business")
                        .WithMany()
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.SaleCouponUser", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.SaleCoupon", "Coupon")
                        .WithMany("CouponUsers")
                        .HasForeignKey("CouponId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JdCat.Cat.Model.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.SaleFullReduce", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Business", "Business")
                        .WithMany("SaleFullReduces")
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.SaleProductDiscount", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Business", "Business")
                        .WithMany("SaleProductDiscounts")
                        .HasForeignKey("BusinessId");

                    b.HasOne("JdCat.Cat.Model.Data.Product", "Product")
                        .WithMany("SaleProductDiscount")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.SessionData", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.SettingProductAttribute", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.SettingProductAttribute", "Parent")
                        .WithMany("Childs")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.ShoppingCart", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Business", "Business")
                        .WithMany("ShoppingCarts")
                        .HasForeignKey("BusinessId");

                    b.HasOne("JdCat.Cat.Model.Data.ProductFormat", "Format")
                        .WithMany("ShoppingCarts")
                        .HasForeignKey("FormatId");

                    b.HasOne("JdCat.Cat.Model.Data.Product", "Product")
                        .WithMany("ShoppingCarts")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JdCat.Cat.Model.Data.User", "User")
                        .WithMany("ShoppingCarts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JdCat.Cat.Model.Data.WxListenUser", b =>
                {
                    b.HasOne("JdCat.Cat.Model.Data.Business", "Business")
                        .WithMany()
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
