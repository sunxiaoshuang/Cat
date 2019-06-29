using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JdCat.Cat.Repository
{
    public class TangRepository : BaseRepository<Staff>, ITangRepository
    {
        public TangRepository(CatDbContext context) : base(context)
        {
        }
        public async Task<List<StaffPost>> GetStaffPostsAsync(int businessId)
        {
            return await Context.StaffPosts
                .Where(a => a.Status == EntityStatus.Normal && a.BusinessId == businessId)
                .ToListAsync();
        }
        public async Task<bool> IsExistPostAsync(int businessId, StaffPost post)
        {
            var name = post.Name.ToLower();
            return await Context.StaffPosts.CountAsync(a => a.Status == EntityStatus.Normal && a.BusinessId == businessId && a.ID != post.ID && a.Name.ToLower() == name) > 0;
        }
        public async Task<int> UpdateStaffPostAsync(StaffPost post)
        {
            var entity = new StaffPost { ID = post.ID };
            Context.Attach(entity);
            entity.Name = post.Name;
            entity.Authority = post.Authority;
            entity.ModifyTime = DateTime.Now;
            return await Context.SaveChangesAsync();
        }
        public async Task<JsonData> DeleteStaffPostAsync(int id)
        {
            var count = await Context.Staffs.CountAsync(a => a.Status == EntityStatus.Normal && a.StaffPostId == id);
            if (count > 0)
            {
                return new JsonData { Msg = $"有[{count}]位员工属于该岗位，不允许删除" };
            }
            var entity = new StaffPost { ID = id };
            Context.Attach(entity);
            entity.Status = EntityStatus.Deleted;
            var result = new JsonData { Success = await Context.SaveChangesAsync() > 0 };
            if (result.Success)
            {
                result.Msg = "删除成功";
                return result;
            }
            result.Msg = "未找到岗位数据，请刷新后重试";
            return result;
        }

        public async Task<List<Staff>> GetStaffsAsync(int businessId)
        {
            return await Context.Staffs
                .Where(a => a.Status == EntityStatus.Normal && a.BusinessId == businessId)
                .Include(a => a.StaffPost)
                .ToListAsync();
        }
        public async Task<bool> IsExistStaffAsync(int businessId, Staff staff)
        {
            var alise = staff.Alise.ToLower();
            return await Context.Staffs.CountAsync(a => a.Status == EntityStatus.Normal && a.BusinessId == businessId && a.ID != staff.ID && a.Alise.ToLower() == staff.Alise) > 0;
        }
        public async Task<Staff> AddStaffAsync(Staff staff)
        {
            var code = await GetMaxStaffCodeAsync(staff.BusinessId) + 1;
            staff.Code = "P" + code.ToString().PadLeft(6, '0');
            staff.Password = UtilHelper.MD5Encrypt(staff.Password);
            staff.Status = EntityStatus.Normal;
            return await AddAsync(staff);
        }
        public async Task<Staff> UpdateStaffAsync(Staff staff)
        {
            var entity = await Context.Staffs.FirstAsync(a => a.ID == staff.ID);
            entity.Name = staff.Name;
            entity.Gender = staff.Gender;
            entity.Birthday = staff.Birthday;
            entity.EnterTime = staff.EnterTime;
            entity.CardId = staff.CardId;
            entity.StaffPostId = staff.StaffPostId;
            entity.ModifyTime = DateTime.Now;
            await Context.SaveChangesAsync();
            return entity;
        }
        public async Task<JsonData> DeleteStaffAsync(int id)
        {
            var entity = new Staff { ID = id };
            Context.Attach(entity);
            entity.Status = EntityStatus.Deleted;
            var result = new JsonData { Success = await Context.SaveChangesAsync() > 0 };
            if (result.Success)
            {
                result.Msg = "删除成功";
                return result;
            }
            result.Msg = "删除失败";
            return result;
        }
        public async Task<List<int>> GetProductIdsByCookAsync(int cookId)
        {
            return await Context.CookProductRelatives.Where(a => a.StaffId == cookId).Select(a => a.ProductId).ToListAsync();
        }
        public async Task<object> GetProductIdsWithCookAsync(int id)
        {
            return await Context.Staffs
                .AsNoTracking()
                .Include(a => a.CookProductRelatives)
                .Where(a => a.BusinessId == id && a.Status != EntityStatus.Deleted)
                .Select(a => new { a.ID, Ids = a.CookProductRelatives.Select(b => b.ProductId).ToList() })
                .ToListAsync();
        }
        public async Task<bool> ResetPasswordAsync(Staff staff)
        {
            var entity = new Staff { ID = staff.ID };
            Context.Attach(entity);
            entity.Password = UtilHelper.MD5Encrypt(staff.Password);
            entity.ModifyTime = DateTime.Now;
            await Context.SaveChangesAsync();
            return true;
        }
        public async Task<List<CookProductRelative>> BindProductsForCookAsync(int cookId, IEnumerable<CookProductRelative> relatives)
        {
            var entitys = Context.CookProductRelatives.Where(a => a.StaffId == cookId).ToList();
            Context.RemoveRange(entitys);
            if (relatives != null && relatives.Count() > 0)
            {
                Context.AddRange(relatives);
            }
            await Context.SaveChangesAsync();
            return entitys.ToList();
        }


        public async Task<List<SystemMark>> GetMarksAsync(int businessId)
        {
            return await Context.SystemMarks.Where(a => a.BusinessId == businessId).ToListAsync();
        }
        public async Task<bool> IsExistMarkAsync(SystemMark mark)
        {
            var name = mark.Name.ToLower();
            var count = await Context.SystemMarks.CountAsync(a => a.BusinessId == mark.BusinessId && a.Category == mark.Category && a.Name.ToLower() == name);
            return count > 0;
        }

        public async Task<List<JdCat.Cat.Model.Data.PaymentType>> GetPaymentsAsync(int businessId)
        {
            return await Context.PaymentTypes
                .Where(a => a.BusinessId == businessId && a.Status == EntityStatus.Normal)
                .OrderBy(a => a.Sort)
                .ToListAsync();
        }
        public async Task<bool> IsExistPaymentAsync(JdCat.Cat.Model.Data.PaymentType payment)
        {
            var name = payment.Name.ToLower();
            return await Context.PaymentTypes.CountAsync(a => a.Status == EntityStatus.Normal && a.BusinessId == payment.BusinessId && a.Name.ToLower() == name) > 0;
        }
        public async Task<JdCat.Cat.Model.Data.PaymentType> AddPaymentAsync(JdCat.Cat.Model.Data.PaymentType payment)
        {
            payment.Category = PaymentCategory.Other;
            var lastEntity = await Context.PaymentTypes
                .OrderBy(a => a.Sort)
                .LastOrDefaultAsync(a => a.BusinessId == payment.BusinessId);
            payment.Sort = (lastEntity?.Sort ?? 0) + 1;
            payment.Status = EntityStatus.Normal;
            Context.Add(payment);
            await Context.SaveChangesAsync();
            return payment;
        }
        public async Task<bool> SetPaymentSortAsync(JdCat.Cat.Model.Data.PaymentType payment)
        {
            var entity = new JdCat.Cat.Model.Data.PaymentType { ID = payment.ID };
            Context.Attach(entity);
            entity.Sort = payment.Sort;
            return await Context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeletePaymentAsync(int id)
        {
            var entity = new JdCat.Cat.Model.Data.PaymentType { ID = id };
            Context.Attach(entity);
            entity.Status = EntityStatus.Deleted;
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<List<DeskType>> GetDeskTypesAsync(int businessId)
        {
            var types = await Context.DeskTypes
                .Include(a => a.Desks)
                .Where(a => a.BusinessId == businessId && a.Status == EntityStatus.Normal)
                .OrderBy(a => a.Sort)
                .ToListAsync();
            types.ForEach(type => {
                if (type.Desks == null) return;
                type.Desks = type.Desks.Where(a => a.Status == EntityStatus.Normal).ToList();
            });
            return types;
        }
        public async Task<bool> IsExistDeskTypeAsync(DeskType type)
        {
            var name = type.Name.ToLower();
            return await Context.DeskTypes.CountAsync(a => a.BusinessId == type.BusinessId && type.Name.ToLower() == name) > 0;
        }
        public async Task<bool> IsExistDeskAsync(Desk desk)
        {
            var name = desk.Name.ToLower();
            return await Context.Desks.CountAsync(a => a.Status == EntityStatus.Normal && a.DeskTypeId == desk.DeskTypeId && a.Name.ToLower() == name) > 0;
        }
        public async Task<JsonData> DeleteDeskTypeAsync(int id)
        {
            var result = new JsonData();
            var deskCount = await Context.Desks.CountAsync(a => a.DeskTypeId == id && a.Status == EntityStatus.Normal);
            if(deskCount > 0)
            {
                result.Msg = "类别下存在餐桌，不允许删除";
                return result;
            }
            var entity = new DeskType { ID = id };
            Context.Attach(entity);
            entity.Status = EntityStatus.Deleted;
            await Context.SaveChangesAsync();
            result.Msg = "删除成功";
            result.Success = true;
            return result;
        }
        public async Task<bool> DeleteDeskAsync(int id)
        {
            var entity = new Desk { ID = id };
            Context.Attach(entity);
            entity.Status = EntityStatus.Deleted;
            return await Context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateDeskTypesAsync(IEnumerable<DeskType> types)
        {
            var ids = types.Select(a => a.ID);
            var entitys = await Context.DeskTypes.Where(a => ids.Contains(a.ID)).ToListAsync();
            foreach (var item in types)
            {
                var entity = entitys.First(a => a.ID == item.ID);
                entity.Name = item.Name;
                entity.Sort = item.Sort;
            }
            return await Context.SaveChangesAsync() > 0;
        }
        public async Task<Desk> UpdateDeskAsync(Desk desk)
        {
            var entity = await Context.Desks.FirstAsync(a => a.ID == desk.ID);
            entity.Name = desk.Name;
            entity.Quantity = desk.Quantity;
            entity.DeskTypeId = desk.DeskTypeId;
            await Context.SaveChangesAsync();
            return entity;
        }

        public async Task<List<ClientPrinter>> GetPrintersAsync(int businessId)
        {
            return await Context.ClientPrinters.Where(a => a.BusinessId == businessId).ToListAsync();
        }
        //public async Task<bool> UpdatePrinterAsync(ClientPrinter printer)
        //{
        //    var entity = new ClientPrinter { ID = printer.ID };
        //    Context.Attach(entity);
        //    entity.Name = printer.Name;
        //    entity.IP = printer.IP;
        //    entity.Port = printer.Port;
        //    entity.Type = printer.Type;
        //    entity.State = printer.State;
        //    entity.Quantity = printer.Quantity;
        //    entity.Mode = printer.Mode;
        //    entity.Format = printer.Format;
        //    return await Context.SaveChangesAsync() > 0;
        //}
        //public async Task<bool> UpdatePrinterProductsAsync(ClientPrinter printer)
        //{
        //    var entity = new ClientPrinter { ID = printer.ID };
        //    Context.Attach(entity);
        //    entity.Name = printer.Name;
        //    entity.IP = printer.IP;
        //    entity.Port = printer.Port;
        //    entity.Type = printer.Type;
        //    entity.State = printer.State;
        //    entity.Quantity = printer.Quantity;
        //    entity.Mode = printer.Mode;
        //    entity.Format = printer.Format;
        //    return await Context.SaveChangesAsync() > 0;
        //}


        public async Task<List<StoreBooth>> GetBoothsAsync(int businessId)
        {
            return await Context.StoreBooths
                .Where(a => a.BusinessId == businessId)
                .ToListAsync();
        }
        public async Task<List<BoothProductRelative>> BindProductsForBoothAsync(int id, IEnumerable<BoothProductRelative> relatives)
        {
            var entitys = Context.BoothProductRelatives.Where(a => a.StoreBoothId == id).ToList();
            Context.RemoveRange(entitys);
            if (relatives != null && relatives.Count() > 0)
            {
                foreach (var item in relatives)
                {
                    item.StoreBoothId = id;
                }
                Context.AddRange(relatives);
            }
            await Context.SaveChangesAsync();
            return relatives.ToList();
        }
        public async Task<List<int>> GetProductIdsByBoothAsync(int boothId)
        {
            return await Context.BoothProductRelatives.Where(a => a.StoreBoothId == boothId).Select(a => a.ProductId).ToListAsync();
        }

        public async Task<object> GetProductIdsWithBusinessBoothAsync(int businessId)
        {
            return await Context.StoreBooths
                .AsNoTracking()
                .Include(a => a.BoothProductRelatives)
                .Where(a => a.BusinessId == businessId)
                .Select(a => new { a.ID, Ids = a.BoothProductRelatives.Select(b => b.ProductId).ToList() })
                .ToListAsync();
        }


        /// <summary>
        /// 获取最大员工编号
        /// </summary>
        /// <returns></returns>
        private async Task<int> GetMaxStaffCodeAsync(int businessId)
        {
            var staff = await Context.Staffs.LastOrDefaultAsync(a => a.BusinessId == businessId);
            if (staff == null) return 0;
            var match = Regex.Match(staff.Code, @"\d+");
            return int.Parse(match.Value);
        }
    }
}
