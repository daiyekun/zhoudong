using Microsoft.EntityFrameworkCore;
using NF.Model.Models;
using NF.ViewModel;
using NF.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NF.BLL
{
   public partial class CompAttachmentService
    {

        /// <summary>
        /// 查看或者修改
        /// </summary>
        /// <param name="Id">当前ID</param>
        /// <returns></returns>
        public IList<WxkhFile> GetcompViwe(int Id)
        {
            var listfiles = Db.Set<ContAttacFile>().Where(a => a.CompanyId == Id)
                .Select(a => new PicInfo { PicPath = a.FilePath, Id = a.Id, AttId = a.AttId, CompId = a.CompanyId }).ToList();
            var query = from a in _CompAttachmentSet.Include(a => a.Category).AsNoTracking()
                        where a.CompanyId == Id && a.IsDelete == 0
                        select new
                        {
                            Id = a.Id,//Wx
                           
                            CategoryName = a.Category.Name, //Wx
                            
                            FileName = a.FileName,//Wx
                            TxDate=a.TxDate,
                            Remark= a.Remark,
                            CreateDateTime=a.CreateDateTime,
                            


                        };
            var local = from a in query.AsEnumerable()
                        select new WxkhFile
                        {
                            Id = a.Id,
                            CategoryName = a.CategoryName,
                            FileName = a.FileName,
                            Remark = a.Remark,
                            CreateDate = a.CreateDateTime,
                            TxDate = a.TxDate,
                            PicData= GetPicList(a.Id, listfiles)

                        };
            var  filelist= local.ToList();
            return filelist;
        }

        /// <summary>
        /// 根据附件ID，查询实际图片集合
        /// </summary>
        /// <param name="attId">附件ID-现在是服务ID</param>
        /// <param name="picInfos">当前客户得图片集合</param>
        private IList<PicView> GetPicList(int attId,IList<PicInfo> picInfos)
        {
           return picInfos.Where(a => a.AttId == attId).Select(a => new PicView { Id = a.Id, PicPath = a.PicPath }).ToList();
        }

    }
}
