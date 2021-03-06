﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.AspNetCore.Hosting;

namespace JdCat.Cat.IRepository
{
    public interface IClientRepository : IBaseRepository<TangOrder>
    {
        /// <summary>
        /// 上传列表数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        int UploadData<T>(IEnumerable<T> list) where T : BaseEntityClient;
        /// <summary>
        /// 上传订单
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int UploadOrder(IEnumerable<TangOrder> list);
        /// <summary>
        /// 上传订单产品
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int UploadOrderProducts(IEnumerable<TangOrderProduct> list);
        /// <summary>
        /// 上传订单产品
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int UploadOrderPayments(IEnumerable<TangOrderPayment> list);
        /// <summary>
        /// 上传订单活动
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int UploadOrderActivities(IEnumerable<TangOrderActivity> list);
        /// <summary>
        /// 获取客户端需要同步的数据
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetSynchronousDataAsync(int businessId);
    }
}
