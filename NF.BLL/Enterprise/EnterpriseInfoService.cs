using Microsoft.EntityFrameworkCore;
using NF.Common.Utility;
using NF.Model.Models;
using NF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
            var tempquery = _EnterpriseInfoSet
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
                            Title = a.Title


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
            var listfiles = Db.Set<EnterpriseFile>().Where(a => a.CompanyId == Id && (a.FileType ?? 0) == 0)
                .Select(a => new PicInfo { PicPath = a.FilePath, Id = a.Id, AttId = a.AttId, CompId = a.CompanyId }).ToList();
            var listvideofiles = Db.Set<EnterpriseFile>().Where(a => a.CompanyId == Id && a.FileType == 1)
               .Select(a => new VideoInfo { VideoPath = a.FilePath, ThumPath = a.ThumPath, Id = a.Id, AttId = a.AttId, CompId = a.CompanyId }).ToList();
            var query = from a in _EnterpriseInfoSet.AsNoTracking()
                        where a.Id == Id
                        select new
                        {
                            Id = a.Id,//Wx
                            Title = a.Title,
                            Remark = a.Remark

                        };
            var local = from a in query.AsEnumerable()

                        select new EnterpriseInfoView
                        {
                            Id = a.Id,//Wx
                            Title = string.IsNullOrEmpty(a.Title) ? "" : a.Title,
                            Remark = string.IsNullOrEmpty(a.Remark) ? "" : a.Remark,
                            PicData = GetPicList(a.Id, listfiles),
                            VideoData = GetVideoList(a.Id, listvideofiles)

                        };
            return local.FirstOrDefault();
        }




        /// <summary>
        /// 根据附件ID，查询实际图片集合
        /// </summary>
        /// <param name="attId">附件ID-现在是服务ID</param>
        /// <param name="picInfos">当前客户得图片集合</param>
        private IList<PicView> GetPicList(int attId, IList<PicInfo> picInfos)
        {
            return picInfos.Where(a => a.AttId == attId).Select(a => new PicView { Id = a.Id, PicPath = a.PicPath }).ToList();
        }

        /// <summary>
        /// 根据附件ID，查询实际视频集合
        /// </summary>
        /// <param name="attId">附件ID-现在是服务ID</param>
        /// <param name="videoInfos">当前客户得视频集合</param>
        private IList<VideoView> GetVideoList(int attId, IList<VideoInfo> videoInfos)
        {
            return videoInfos.Where(a => a.AttId == attId).Select(a => new VideoView { Id = a.Id, VideoPath = a.VideoPath, ThumPath = a.ThumPath }).ToList();
        }
    }
}
