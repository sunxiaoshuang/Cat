using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JdCat.Cat.Model.Migrations
{
    public partial class InitProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Business",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    ObjectId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Contact = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Mobile = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Area = table.Column<string>(nullable: true),
                    RegisterDate = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    BusinessLicense = table.Column<string>(nullable: true),
                    BusinessLicenseImage = table.Column<string>(nullable: true),
                    SpecialImage = table.Column<string>(nullable: true),
                    InvitationCode = table.Column<string>(nullable: true),
                    AppId = table.Column<string>(nullable: true),
                    Secret = table.Column<string>(nullable: true),
                    MchId = table.Column<string>(nullable: true),
                    MchKey = table.Column<string>(nullable: true),
                    StoreId = table.Column<string>(nullable: true),
                    LogoSrc = table.Column<string>(nullable: true),
                    IsAutoReceipt = table.Column<bool>(nullable: false),
                    FreightMode = table.Column<int>(nullable: false),
                    Freight = table.Column<double>(nullable: true),
                    DadaSourceId = table.Column<string>(nullable: true),
                    DadaShopNo = table.Column<string>(nullable: true),
                    CityName = table.Column<string>(nullable: true),
                    CityCode = table.Column<string>(nullable: true),
                    Range = table.Column<double>(nullable: false),
                    FeyinMemberCode = table.Column<string>(nullable: true),
                    FeyinApiKey = table.Column<string>(nullable: true),
                    DefaultPrinterDevice = table.Column<string>(nullable: true),
                    Lng = table.Column<double>(nullable: false),
                    Lat = table.Column<double>(nullable: false),
                    BusinessStartTime = table.Column<string>(nullable: true),
                    BusinessEndTime = table.Column<string>(nullable: true),
                    BusinessStartTime2 = table.Column<string>(nullable: true),
                    BusinessEndTime2 = table.Column<string>(nullable: true),
                    BusinessStartTime3 = table.Column<string>(nullable: true),
                    BusinessEndTime3 = table.Column<string>(nullable: true),
                    MinAmount = table.Column<double>(nullable: false),
                    IsClose = table.Column<bool>(nullable: false),
                    TemplateNotifyId = table.Column<string>(nullable: true),
                    ServiceProvider = table.Column<int>(nullable: false),
                    IsPublish = table.Column<bool>(nullable: false),
                    WxQrListenPath = table.Column<string>(nullable: true),
                    AppQrCode = table.Column<string>(nullable: true),
                    Category = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Business_Business_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DadaCancelReason",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    FlagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadaCancelReason", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SettingProductAttribute",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Sort = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingProductAttribute", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SettingProductAttribute_SettingProductAttribute_ParentId",
                        column: x => x.ParentId,
                        principalTable: "SettingProductAttribute",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Age = table.Column<int>(nullable: false),
                    AvatarUrl = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    OpenId = table.Column<string>(nullable: true),
                    IsRegister = table.Column<bool>(nullable: false),
                    IsPhone = table.Column<bool>(nullable: false),
                    PurchaseTimes = table.Column<int>(nullable: false),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "YcfkLocation",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    OrderId = table.Column<int>(nullable: false),
                    Lat = table.Column<double>(nullable: false),
                    Lng = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YcfkLocation", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BusinessFreight",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    MaxDistance = table.Column<double>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: true),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessFreight", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BusinessFreight_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DWDStore",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    city_code = table.Column<string>(nullable: true),
                    city_name = table.Column<string>(nullable: true),
                    external_shopid = table.Column<string>(nullable: true),
                    shop_title = table.Column<string>(nullable: true),
                    mobile = table.Column<string>(nullable: true),
                    lng = table.Column<double>(nullable: false),
                    lat = table.Column<double>(nullable: false),
                    addr = table.Column<string>(nullable: true),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DWDStore", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DWDStore_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeyinDevice",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    MemberCode = table.Column<string>(nullable: true),
                    ApiKey = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeyinDevice", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FeyinDevice_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpenAuthInfo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    AppId = table.Column<string>(nullable: true),
                    RefreshToken = table.Column<string>(nullable: true),
                    AuthNote = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: true),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenAuthInfo", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OpenAuthInfo_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductType",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Sort = table.Column<int>(nullable: false),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductType", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductType_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleCoupon",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<double>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Stock = table.Column<int>(nullable: false),
                    MinConsume = table.Column<double>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    ValidDay = table.Column<int>(nullable: true),
                    Received = table.Column<int>(nullable: false),
                    Consumed = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDelete = table.Column<bool>(nullable: false),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCoupon", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SaleCoupon_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleFullReduce",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    IsForeverValid = table.Column<bool>(nullable: false),
                    MinPrice = table.Column<double>(nullable: false),
                    ReduceMoney = table.Column<double>(nullable: false),
                    IsDelete = table.Column<bool>(nullable: false),
                    BusinessId = table.Column<int>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleFullReduce", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SaleFullReduce_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WxListenUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    openid = table.Column<string>(nullable: true),
                    nickname = table.Column<string>(nullable: true),
                    sex = table.Column<int>(nullable: false),
                    city = table.Column<string>(nullable: true),
                    province = table.Column<string>(nullable: true),
                    country = table.Column<string>(nullable: true),
                    headimgurl = table.Column<string>(nullable: true),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WxListenUser", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WxListenUser_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Receiver = table.Column<string>(nullable: true),
                    ProvinceName = table.Column<string>(nullable: true),
                    CityName = table.Column<string>(nullable: true),
                    AreaName = table.Column<string>(nullable: true),
                    MapInfo = table.Column<string>(nullable: true),
                    Lng = table.Column<double>(nullable: false),
                    Lat = table.Column<double>(nullable: false),
                    DetailInfo = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    PostalCode = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Address_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionData",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    SessionKey = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionData", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SessionData_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DWD_Recharge",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    Mode = table.Column<int>(nullable: false),
                    IsFinish = table.Column<bool>(nullable: false),
                    DwdCode = table.Column<string>(nullable: true),
                    DWD_BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DWD_Recharge", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DWD_Recharge_DWDStore_DWD_BusinessId",
                        column: x => x.DWD_BusinessId,
                        principalTable: "DWDStore",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Tag = table.Column<string>(nullable: true),
                    Feature = table.Column<int>(nullable: false),
                    ProductIdSet = table.Column<string>(nullable: true),
                    UnitName = table.Column<string>(nullable: true),
                    MinBuyQuantity = table.Column<double>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    PublishTime = table.Column<DateTime>(nullable: true),
                    NotSaleTime = table.Column<DateTime>(nullable: true),
                    BusinessId = table.Column<int>(nullable: false),
                    ProductTypeId = table.Column<int>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Product_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Product_ProductType_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleCouponUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Value = table.Column<double>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    MinConsume = table.Column<double>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    ValidDay = table.Column<int>(nullable: true),
                    UseTime = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    CouponId = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCouponUser", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SaleCouponUser_SaleCoupon_CouponId",
                        column: x => x.CouponId,
                        principalTable: "SaleCoupon",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleCouponUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttribute",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Item1 = table.Column<string>(nullable: true),
                    Item2 = table.Column<string>(nullable: true),
                    Item3 = table.Column<string>(nullable: true),
                    Item4 = table.Column<string>(nullable: true),
                    Item5 = table.Column<string>(nullable: true),
                    Item6 = table.Column<string>(nullable: true),
                    Item7 = table.Column<string>(nullable: true),
                    Item8 = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttribute", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductAttribute_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductFormat",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: true),
                    Stock = table.Column<double>(nullable: true),
                    PackingPrice = table.Column<double>(nullable: true),
                    PackingQuantity = table.Column<double>(nullable: true),
                    UPC = table.Column<string>(nullable: true),
                    SKU = table.Column<string>(nullable: true),
                    Position = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFormat", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductFormat_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductImage",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Length = table.Column<long>(nullable: false),
                    ExtensionName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImage", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductImage_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleProductDiscount",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    OldPrice = table.Column<double>(nullable: false),
                    Discount = table.Column<double>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Cycle = table.Column<int>(nullable: false),
                    StartTime1 = table.Column<string>(nullable: true),
                    EndTime1 = table.Column<string>(nullable: true),
                    StartTime2 = table.Column<string>(nullable: true),
                    EndTime2 = table.Column<string>(nullable: true),
                    StartTime3 = table.Column<string>(nullable: true),
                    EndTime3 = table.Column<string>(nullable: true),
                    SettingType = table.Column<string>(nullable: true),
                    UpperLimit = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    BusinessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleProductDiscount", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SaleProductDiscount_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleProductDiscount_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    OrderCode = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: true),
                    OldPrice = table.Column<double>(nullable: true),
                    PackagePrice = table.Column<double>(nullable: true),
                    Freight = table.Column<double>(nullable: true),
                    ReceiverName = table.Column<string>(nullable: true),
                    ReceiverAddress = table.Column<string>(nullable: true),
                    Lat = table.Column<double>(nullable: false),
                    Lng = table.Column<double>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    Tips = table.Column<double>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    TablewareQuantity = table.Column<int>(nullable: true),
                    DeliveryMode = table.Column<int>(nullable: false),
                    WxPayCode = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    RefundStatus = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Category = table.Column<int>(nullable: false),
                    PaymentType = table.Column<int>(nullable: false),
                    PayTime = table.Column<DateTime>(nullable: true),
                    DistributionTime = table.Column<DateTime>(nullable: true),
                    AchieveTime = table.Column<DateTime>(nullable: true),
                    RejectReasion = table.Column<string>(nullable: true),
                    CityCode = table.Column<string>(nullable: true),
                    ErrorReason = table.Column<string>(nullable: true),
                    Identifier = table.Column<int>(nullable: false),
                    DistributionFlow = table.Column<int>(nullable: false),
                    LogisticsType = table.Column<int>(nullable: false),
                    CallbackCost = table.Column<double>(nullable: true),
                    PrepayId = table.Column<string>(nullable: true),
                    RefundNo = table.Column<string>(nullable: true),
                    CancelReason = table.Column<string>(nullable: true),
                    RefundReason = table.Column<string>(nullable: true),
                    OpenId = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    BusinessId = table.Column<int>(nullable: true),
                    IsSendDada = table.Column<bool>(nullable: false),
                    SaleFullReduceId = table.Column<int>(nullable: true),
                    SaleFullReduceMoney = table.Column<double>(nullable: true),
                    SaleCouponUserId = table.Column<int>(nullable: true),
                    SaleCouponUserMoney = table.Column<double>(nullable: true),
                    Distance = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Order_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Order_SaleCouponUser_SaleCouponUserId",
                        column: x => x.SaleCouponUserId,
                        principalTable: "SaleCouponUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_SaleFullReduce_SaleFullReduceId",
                        column: x => x.SaleFullReduceId,
                        principalTable: "SaleFullReduce",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCart",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Src = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    PackingQuantity = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    FormatId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    BusinessId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Business",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_ProductFormat_FormatId",
                        column: x => x.FormatId,
                        principalTable: "ProductFormat",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DadaCallBack",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    client_id = table.Column<string>(nullable: true),
                    order_id = table.Column<string>(nullable: true),
                    order_status = table.Column<int>(nullable: false),
                    cancel_reason = table.Column<string>(nullable: true),
                    cancel_from = table.Column<int>(nullable: false),
                    update_time = table.Column<int>(nullable: false),
                    signature = table.Column<string>(nullable: true),
                    dm_id = table.Column<int>(nullable: false),
                    dm_name = table.Column<string>(nullable: true),
                    dm_mobile = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadaCallBack", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DadaCallBack_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DadaLiquidatedDamages",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Deduct_fee = table.Column<double>(nullable: false),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadaLiquidatedDamages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DadaLiquidatedDamages_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DadaReturn",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Distance = table.Column<double>(nullable: false),
                    Fee = table.Column<double>(nullable: false),
                    DeliverFee = table.Column<double>(nullable: false),
                    CouponFee = table.Column<double>(nullable: true),
                    Tips = table.Column<double>(nullable: true),
                    InsuranceFee = table.Column<double>(nullable: true),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadaReturn", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DadaReturn_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderProduct",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.AutoIncrement),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Quantity = table.Column<double>(nullable: true),
                    Price = table.Column<double>(nullable: true),
                    OldPrice = table.Column<double>(nullable: true),
                    Discount = table.Column<double>(nullable: true),
                    Src = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ProductIdSet = table.Column<string>(nullable: true),
                    Feature = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: true),
                    OrderId = table.Column<int>(nullable: false),
                    FormatId = table.Column<int>(nullable: true),
                    SaleProductDiscountId = table.Column<int>(nullable: true),
                    DiscountProductQuantity = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProduct", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderProduct_ProductFormat_FormatId",
                        column: x => x.FormatId,
                        principalTable: "ProductFormat",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrderProduct_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderProduct_SaleProductDiscount_SaleProductDiscountId",
                        column: x => x.SaleProductDiscountId,
                        principalTable: "SaleProductDiscount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_UserId",
                table: "Address",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Business_ParentId",
                table: "Business",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessFreight_BusinessId",
                table: "BusinessFreight",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_DadaCallBack_OrderId",
                table: "DadaCallBack",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DadaLiquidatedDamages_OrderId",
                table: "DadaLiquidatedDamages",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DadaReturn_OrderId",
                table: "DadaReturn",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DWD_Recharge_DWD_BusinessId",
                table: "DWD_Recharge",
                column: "DWD_BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_DWDStore_BusinessId",
                table: "DWDStore",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeyinDevice_BusinessId",
                table: "FeyinDevice",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenAuthInfo_BusinessId",
                table: "OpenAuthInfo",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_BusinessId",
                table: "Order",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_SaleCouponUserId",
                table: "Order",
                column: "SaleCouponUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_SaleFullReduceId",
                table: "Order",
                column: "SaleFullReduceId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserId",
                table: "Order",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_FormatId",
                table: "OrderProduct",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_OrderId",
                table: "OrderProduct",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_ProductId",
                table: "OrderProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_SaleProductDiscountId",
                table: "OrderProduct",
                column: "SaleProductDiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_BusinessId",
                table: "Product",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductTypeId",
                table: "Product",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_ProductId",
                table: "ProductAttribute",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFormat_ProductId",
                table: "ProductFormat",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImage",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductType_BusinessId",
                table: "ProductType",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCoupon_BusinessId",
                table: "SaleCoupon",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponUser_CouponId",
                table: "SaleCouponUser",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponUser_UserId",
                table: "SaleCouponUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleFullReduce_BusinessId",
                table: "SaleFullReduce",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleProductDiscount_BusinessId",
                table: "SaleProductDiscount",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleProductDiscount_ProductId",
                table: "SaleProductDiscount",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionData_UserId",
                table: "SessionData",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingProductAttribute_ParentId",
                table: "SettingProductAttribute",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_BusinessId",
                table: "ShoppingCart",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_FormatId",
                table: "ShoppingCart",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_ProductId",
                table: "ShoppingCart",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_UserId",
                table: "ShoppingCart",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_OpenId",
                table: "User",
                column: "OpenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WxListenUser_BusinessId",
                table: "WxListenUser",
                column: "BusinessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "BusinessFreight");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "DadaCallBack");

            migrationBuilder.DropTable(
                name: "DadaCancelReason");

            migrationBuilder.DropTable(
                name: "DadaLiquidatedDamages");

            migrationBuilder.DropTable(
                name: "DadaReturn");

            migrationBuilder.DropTable(
                name: "DWD_Recharge");

            migrationBuilder.DropTable(
                name: "FeyinDevice");

            migrationBuilder.DropTable(
                name: "OpenAuthInfo");

            migrationBuilder.DropTable(
                name: "OrderProduct");

            migrationBuilder.DropTable(
                name: "ProductAttribute");

            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.DropTable(
                name: "SessionData");

            migrationBuilder.DropTable(
                name: "SettingProductAttribute");

            migrationBuilder.DropTable(
                name: "ShoppingCart");

            migrationBuilder.DropTable(
                name: "WxListenUser");

            migrationBuilder.DropTable(
                name: "YcfkLocation");

            migrationBuilder.DropTable(
                name: "DWDStore");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "SaleProductDiscount");

            migrationBuilder.DropTable(
                name: "ProductFormat");

            migrationBuilder.DropTable(
                name: "SaleCouponUser");

            migrationBuilder.DropTable(
                name: "SaleFullReduce");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "SaleCoupon");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "ProductType");

            migrationBuilder.DropTable(
                name: "Business");
        }
    }
}
