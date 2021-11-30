using Microsoft.EntityFrameworkCore;
using NF.Common.Extend;
using NF.Common.Utility;
using NF.Model.Models;
using NF.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NF.BLL
{
  public partial  class WinningItemsService
    {
        public LayPageInfo<WinningItemsDTO> GetList<s>(PageInfo<WinningItems> pageInfo, Expression<Func<WinningItems, bool>> whereLambda, Expression<Func<WinningItems, s>> orderbyLambda, bool isAsc)
        {
            //var tempquery = GetQueryToPage(pageInfo, whereLambda, orderbyLambda, isAsc);
            //var tempquery = _WinningItemsSet.AsTracking().Where<WinningItems>(whereLambda.Compile()).AsQueryable();
            var tempquery = Db.Set<WinningItems>().AsTracking().Where<WinningItems>(whereLambda.Compile()).AsQueryable();
            pageInfo.TotalCount = tempquery.Count();
            if (isAsc)
            {
                tempquery = tempquery.OrderBy(orderbyLambda);
            }
            else
            {
                tempquery = tempquery.OrderByDescending(orderbyLambda);
            }
            if (!(pageInfo is NoPageInfo<WinningItems>))
                tempquery = tempquery.Skip<WinningItems>((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take<WinningItems>(pageInfo.PageSize);
            var query = from a in tempquery
                        select new
                        {
                            Id = a.Id,
                            WinningName = a.WinningName,
                            WinningUntiId = a.WinningUntiId,
                            WinningModel = a.WinningModel,
                            WinningTotalPrice = a.WinningTotalPrice,
                            WinningUitprice = a.WinningUitprice,
                            WinningQuantity = a.WinningQuantity,
                            IsDelete = a.IsDelete
                        };
            var local = from a in query.AsEnumerable()
                        select new WinningItemsDTO
                        {
                            Id = a.Id,
                            WinningName = a.WinningName,
                            WinningUntiId = a.WinningUntiId,
                            WinningModel = a.WinningModel,
                            WinningTotalPrice = a.WinningTotalPrice,
                            WinningUitprice = a.WinningUitprice,
                            WinningQuantity = a.WinningQuantity,
                            IsDelete = a.IsDelete
                        };
            return new LayPageInfo<WinningItemsDTO>()
            {
                data = local.ToList(),
                count = pageInfo.TotalCount,
                code = 0


            };
        }

        public LayPageInfo<WinningItemsDTO> GetListView<s>(PageInfo<WinningItems> pageInfo, Expression<Func<WinningItems, bool>> whereLambda, Expression<Func<WinningItems, s>> orderbyLambda, bool isAsc)
        {
            //var tempquery = GetQueryToPage(pageInfo, whereLambda, orderbyLambda, isAsc);
            var tempquery = Db.Set<WinningItems>().AsTracking().Where<WinningItems>(whereLambda.Compile()).AsQueryable();
            pageInfo.TotalCount = tempquery.Count();
            if (isAsc)
            {
                tempquery = tempquery.OrderBy(orderbyLambda);
            }
            else
            {
                tempquery = tempquery.OrderByDescending(orderbyLambda);
            }
            if (!(pageInfo is NoPageInfo<WinningItems>))
                tempquery = tempquery.Skip<WinningItems>((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take<WinningItems>(pageInfo.PageSize);
            var query = from a in tempquery
                        select new
                        {
                            Id = a.Id,
                            WinningName = a.WinningName,
                            WinningUntiId = a.WinningUntiId,
                            WinningModel = a.WinningModel,
                            WinningTotalPrice = a.WinningTotalPrice,
                            WinningUitprice = a.WinningUitprice,
                            WinningQuantity = a.WinningQuantity,
                            IsDelete = a.IsDelete
                        };
            var local = from a in query.AsEnumerable()
                        select new WinningItemsDTO
                        {
                            Id = a.Id,
                            WinningName = a.WinningName,
                            WinningUntiId = a.WinningUntiId,
                            WinningModel = a.WinningModel,
                            WinningTotalPrice = a.WinningTotalPrice,
                            WinningUitprice = a.WinningUitprice,
                            WinningQuantity = a.WinningQuantity,
                            IsDelete = a.IsDelete
                        };
            return new LayPageInfo<WinningItemsDTO>()
            {
                data = local.ToList(),
                count = pageInfo.TotalCount,
                code = 0
            };
        }

        public int Delete(string Ids)
        {
            string sqlstr = "update WinningItems set IsDelete=1 where Id in(" + Ids + ")";
            return ExecuteSqlCommand(sqlstr);
        }

    }
}
