using Microsoft.EntityFrameworkCore;
using NF.Common.Utility;
using NF.Model.Models;
using NF.ViewModel;
using NF.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NF.BLL
{
    public partial class EnterpriseInfoService
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
        public LayPageInfo<EnterpriseInfoList> GetWxList<s>(PageInfo<EnterpriseInfo> pageInfo, Expression<Func<EnterpriseInfo, bool>> whereLambda,
        Expression<Func<EnterpriseInfo, s>> orderbyLambda, bool isAsc)
        {
            var tempquery =_EnterpriseInfoSet
                .Where<EnterpriseInfo>(whereLambda.Compile()).AsQueryable();
            pageInfo.TotalCount = tempquery.Count();
            if (isAsc)
            {
                tempquery = tempquery.OrderBy(orderbyLambda);
            }
            else
            {
                tempquery = tempquery.OrderByDescending(orderbyLambda);
            }
            if (!(pageInfo is NoPageInfo<EnterpriseInfo>))
            { //分页
                tempquery = tempquery.Skip<EnterpriseInfo>((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take<EnterpriseInfo>(pageInfo.PageSize);
            }

            var query = from a in tempquery
                        select new
                        {
                            Id = a.Id,//Wx
                            Title= a.Title
                            

                        };
            var local = from a in query.AsEnumerable()
                        select new EnterpriseInfoList
                        {
                            Id = a.Id,
                            Title = a.Title
                        };
            return new LayPageInfo<EnterpriseInfoList>()
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
        public EnterpriseInfoView ShowView(int Id)
        {
            var query = from a in _EnterpriseInfoSet.AsNoTracking()
                        where a.Id == Id
                        select new
                        {
                            Id = a.Id,//Wx
                            Title = a.Title,
                            Remark= a.Remark

                        };
            var local = from a in query.AsEnumerable()

                        select new EnterpriseInfoView
                        {
                            Id = a.Id,//Wx
                            Title = a.Title,
                            Remark = a.Remark
                          
                        };
            return local.FirstOrDefault();
        }
    }
}
