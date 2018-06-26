using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Model.Data;

namespace JdCat.Cat.IRepository
{
    public interface IBusinessRepository : IBusinessRepository<Business>
    {
        /// <summary>
        /// 获取商户对象，不要跟踪状态
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Business GetBusiness(Expression<Func<Business, bool>> expression);
        /// <summary>
        /// 保存商户基本信息
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool SaveBase(Business business);
        /// <summary>
        /// 保存小程序信息
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool SaveSmall(Business business);
        /// <summary>
        /// 保存达达配置信息
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool SaveDada(Business business);
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="businessId"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        Task<string> UploadAsync(string url, int businessId, string source);
        /// <summary>
        /// 保存飞印商户编码与apikey
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool SaveFeyin(Business business);
        /// <summary>
        /// 获取商户打印机列表
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        IEnumerable<FeyinDevice> GetPrinters(int businessId);
        /// <summary>
        /// 绑定打印设备
        /// </summary>
        /// <param name="business"></param>
        /// <param name="device"></param>
        bool BindPrintDevice(Business business, FeyinDevice device);
    }
}
