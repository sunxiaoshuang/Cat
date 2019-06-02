using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Mapping;
using JdCat.Cat.Model.Report;
using Microsoft.EntityFrameworkCore;

namespace JdCat.Cat.Model
{
    public class CatDbContext : DbContext
    {
        public CatDbContext(DbContextOptions<CatDbContext> options) : base(options)
        {

        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    //optionsBuilder.UseLazyLoadingProxies(); // 使用延迟加载
        //}

        public DbSet<Business> Businesses { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductFormat> ProductFormats { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<SettingProductAttribute> SettingProductAttributes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SessionData> SessionDatas { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<City> Citys { get; set; }
        public DbSet<DadaCallBack> DadaCallBacks { get; set; }
        public DbSet<DadaReturn> DadaReturns { get; set; }
        public DbSet<DadaCancelReason> DadaCancelReasons { get; set; }
        public DbSet<DadaLiquidatedDamages> DadaLiquidatedDamageses { get; set; }
        public DbSet<FeyinDevice> FeyinDevices { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<SaleFullReduce> SaleFullReduces { get; set; }
        public DbSet<SaleCoupon> SaleCoupons { get; set; }
        public DbSet<SaleCouponUser> SaleCouponUsers { get; set; }
        public DbSet<SaleProductDiscount> SaleProductDiscount { get; set; }
        public DbSet<DWDStore> DWDStores { get; set; }
        public DbSet<DWD_Recharge> DWD_Recharges { get; set; }
        public DbSet<WxListenUser> WxListenUsers { get; set; }
        public DbSet<OpenAuthInfo> OpenAuthInfos { get; set; }
        public DbSet<BusinessFreight> BusinessFreights { get; set; }
        public DbSet<YcfkLocation> YcfkLocations { get; set; }
        public DbSet<ClientPrinter> ClientPrinters { get; set; }
        public DbSet<DeskType> DeskTypes { get; set; }
        public DbSet<Desk> Desks { get; set; }
        public DbSet<OrderComment> OrderComments { get; set; }
        public DbSet<ImageWarehouse> ImageWarehouses { get; set; }
        public DbSet<TangOrder> TangOrders { get; set; }
        public DbSet<TangOrderProduct> TangOrderProducts { get; set; }
        public DbSet<TangOrderPayment> TangOrderPayments { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<StaffPost> StaffPosts { get; set; }
        public DbSet<SystemMark> SystemMarks { get; set; }
        public DbSet<CookProductRelative> CookProductRelatives { get; set; }
        public DbSet<StoreBooth> StoreBooths { get; set; }
        public DbSet<BoothProductRelative> BoothProductRelatives { get; set; }
        public DbSet<ProductRelative> ProductRelatives { get; set; }


        /// <summary>
        /// 添加FluentAPI配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 基础配置
            //modelBuilder.HasSequence<int>("StoreNumbers");            // 门店编号序列
            //modelBuilder.HasSequence<int>("OrderNumbers");            // 订单编号序列
            //modelBuilder.HasSequence<int>("FormatNumbers");           // 规格编码序列
            //modelBuilder.HasSequence<int>("SaleCouponNumbers");       // 优惠券编码序列

            //modelBuilder.Entity<Business>()
            //    .Property(a => a.StoreId)
            //    .HasDefaultValueSql("'JD' + fn_right_padding(NEXT_VAL('StoreNumbers'), 6)");
            //modelBuilder.Entity<ProductFormat>()
            //    .Property(a => a.Code)
            //    .HasDefaultValueSql("'F' + DATE_FORMAT(NOW(),'%Y') + fn_right_padding(NEXT_VAL('FormatNumbers'), 9)");
            //modelBuilder.Entity<Order>()
            //    .Property(a => a.OrderCode)
            //    .HasDefaultValueSql("DATE_FORMAT(NOW(),'%Y-%m-%d') + fn_right_padding(NEXT_VAL('OrderNumbers'), 6) + fn_right_padding(floor(rand()*100000), 5)");
            //modelBuilder.Entity<SaleCouponUser>()
            //    .Property(a => a.Code)
            //    .HasDefaultValueSql("fn_right_padding(floor(rand()*10000000), 6) + NEXT_VAL('SaleCouponNumbers') + fn_right_padding(floor(rand()*100000), 4)");

            // 配置视图
            //modelBuilder.Query<Report_SaleStatistics>().ToView("View_SaleStatistics");

            // 配置映射
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes().Where(q => q.GetInterface(typeof(IEntityTypeConfiguration<>).FullName) != null);
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration(configurationInstance);
            }

        }
    }
}
