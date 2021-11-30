using NF.Common.Utility;
using NF.Model.Models;
using NF.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using NF.ViewModel.Models.Utility;
using NF.ViewModel.Extend.Enums;
using Microsoft.EntityFrameworkCore;

namespace NF.BLL
{
    /// <summary>
    /// 合同附件
    /// </summary>
    public partial  class ContAttachmentService
    {
        /// <summary>
        /// 查询分页
        /// </summary>
        /// <typeparam name="s"></typeparam>
        /// <param name="pageInfo"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderbyLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public LayPageInfo<ContAttachmentViewDTO> GetList<s>(PageInfo<ContAttachment> pageInfo, Expression<Func<ContAttachment, bool>> whereLambda, Expression<Func<ContAttachment, s>> orderbyLambda, bool isAsc)
        {
            var tempquery = GetQueryToPage(pageInfo, whereLambda, orderbyLambda, isAsc);
            var query = from a in tempquery
                        select new
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Remark = a.Remark,
                            CreateDateTime = a.CreateDateTime,
                            CreateUserId = a.CreateUserId,
                            CategoryId = a.CategoryId,
                            Path = a.Path,
                            FileName = a.FileName,
                            GuidFileName = a.GuidFileName,

                        };
            var local = from a in query.AsEnumerable()
                        select new ContAttachmentViewDTO
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Remark = a.Remark,
                            CreateDateTime = a.CreateDateTime,
                            CreateUserName = RedisValueUtility.GetUserShowName(a.CreateUserId),
                            CategoryName = DataDicUtility.GetDicValueToRedis(a.CategoryId, DataDictionaryEnum.ContAttachmentType),
                            Path = a.Path,
                            FileName = a.FileName,
                            GuidFileName = a.GuidFileName
                        };
            return new LayPageInfo<ContAttachmentViewDTO>()
            {
                data = local.ToList(),
                count = pageInfo.TotalCount,
                code = 0


            };


        }
        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="Ids">需要删除的Ids集合</param>
        /// <returns>受影响行数</returns>
        public int Delete(string Ids)
        {
            string sqlstr = "update ContAttachment set IsDelete=1 where Id in(" + Ids + ")";
            return ExecuteSqlCommand(sqlstr);
        }
        /// <summary>
        /// 查看或者修改
        /// </summary>
        /// <param name="Id">当前ID</param>
        /// <returns></returns>
        public ContAttachmentViewDTO ShowView(int Id)
        {
            var query = from a in _ContAttachmentSet.AsNoTracking()
                        where a.Id == Id
                        select new
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Remark = a.Remark,
                            CreateDateTime = a.CreateDateTime,
                            Path = a.Path,
                            FileName = a.FileName,
                            GuidFileName = a.GuidFileName,
                            FolderName = a.FolderName,
                            CategoryId = a.CategoryId,
                            CreateUserId = a.CreateUserId

                        };
            var local = from a in query.AsEnumerable()
                        select new ContAttachmentViewDTO
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Remark = a.Remark,
                            CreateDateTime = a.CreateDateTime,
                            Path = a.Path,
                            FileName = a.FileName,
                            GuidFileName = a.GuidFileName,
                            FolderName = a.FolderName,
                            CategoryId = a.CategoryId,
                            CreateUserName = RedisValueUtility.GetUserShowName(a.CreateUserId),
                            CategoryName = DataDicUtility.GetDicValueToRedis(a.CategoryId, DataDictionaryEnum.ContAttachmentType)
                        };
            return local.FirstOrDefault();
        }

    }
}
