using Microsoft.EntityFrameworkCore;
using NF.Common.Utility;
using NF.Model.Models;
using NF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NF.BLL
{
    public partial class  CheckInfoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="s"></typeparam>
        /// <param name="pageInfo"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderbyLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public LayPageInfo<CheckInfoList> GetWxList<s>(PageInfo<CheckInfo> pageInfo, Expression<Func<CheckInfo, bool>> whereLambda,
        Expression<Func<CheckInfo, s>> orderbyLambda, bool isAsc)
        {
            var tempquery = _CheckInfoSet
                .Where<CheckInfo>(whereLambda.Compile()).AsQueryable();
            pageInfo.TotalCount = tempquery.Count();
            if (isAsc)
            {
                tempquery = tempquery.OrderBy(orderbyLambda);
            }
            else
            {
                tempquery = tempquery.OrderByDescending(orderbyLambda);
            }
            if (!(pageInfo is NoPageInfo<CheckInfo>))
            { //分页
                tempquery = tempquery.Skip<CheckInfo>((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take<CheckInfo>(pageInfo.PageSize);
            }

            var query = from a in tempquery
                        select new
                        {
                            Id = a.Id,//Wx
                            Title = a.Title,
                            TxDate = a.TxDate,
                            CompanyName = a.CompanyName,


                        };
            var local = from a in query.AsEnumerable()
                        select new CheckInfoList
                        {
                            Id = a.Id,
                            Title = a.Title,
                            TxDate = a.TxDate,
                            CompanyName = a.CompanyName,
                        };
            return new LayPageInfo<CheckInfoList>()
            {
                data = local.ToList(),
                count = pageInfo.TotalCount,
                code = 0


            };
        }
        /// <summary>
        /// 查看信息
        /// </summary>
        /// <param name="Id">当前ID</param>
        /// <returns></returns>
        public CheckInfoView ShowView(int Id)
        {
            var query = from a in _CheckInfoSet.AsNoTracking()
                        where a.Id == Id
                        select new
                        {
                            Id = a.Id,//Wx
                            Title = a.Title,
                            TxDate=a.TxDate,
                            CompanyName= a.CompanyName,
                            Remark = a.Remark

                        };
            var local = from a in query.AsEnumerable()

                        select new CheckInfoView
                        {
                            Id = a.Id,//Wx
                            Title = a.Title,
                            TxDate = a.TxDate,
                            CompanyName = a.CompanyName,
                            Remark = a.Remark

                        };
            return local.FirstOrDefault();
        }
    }
}
