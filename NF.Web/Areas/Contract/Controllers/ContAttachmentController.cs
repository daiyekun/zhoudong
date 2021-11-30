using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NF.Common.Models;
using NF.Common.Utility;
using NF.IBLL;
using NF.Model.Models;
using NF.ViewModel.Extend.Enums;
using NF.ViewModel.Models;
using NF.ViewModel.Models.Common;
using NF.Web.Controllers;
using NF.Web.Utility;
using NF.Web.Utility.Filters;

namespace NF.Web.Areas.Contract.Controllers
{
    /// <summary>
    /// 合同附件
    /// </summary>
    [Area("Contract")]
    [Route("Contract/[controller]/[action]")]
    public class ContAttachmentController : NfBaseController
    {
        private IContAttachmentService _IContAttachmentService;
        private IMapper _IMapper;

        public ContAttachmentController(IContAttachmentService IContAttachmentService, IMapper IMapper)
        {
            _IContAttachmentService = IContAttachmentService;
            _IMapper = IMapper;
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="projectId">对方ID</param>
        /// <returns></returns>
        public IActionResult GetList(PageparamInfo pageParam, int contId)
        {
            var pageInfo = new NoPageInfo<ContAttachment>();
            var predicateAnd = PredicateBuilder.True<ContAttachment>();
            var predicateOr = PredicateBuilder.False<ContAttachment>();
            predicateOr = predicateOr.Or(a => a.ContId == -this.SessionCurrUserId && a.IsDelete == 0);
            if (contId != 0)
            {
                predicateOr = predicateOr.Or(a => a.ContId == contId && a.IsDelete == 0);
            }
            predicateAnd = predicateAnd.And(predicateOr);
            var layPage = _IContAttachmentService.GetList(pageInfo, predicateAnd, a => a.Id, false);
            return new CustomResultJson(layPage);
        }
        /// <summary>
        /// 新建
        /// </summary>
        /// <returns></returns>
        public IActionResult Build()
        {
            return View();

        }
        /// <summary>
        /// 新建附件
        /// </summary>
        /// <returns></returns>
        [NfCustomActionFilter("新建合同附件", OptionLogTypeEnum.Add, "新建合同附件", true)]
        public IActionResult Save(ContAttachmentDTO ContAttachmentDTO)
        {

            var saveInfo = _IMapper.Map<ContAttachment>(ContAttachmentDTO);
            saveInfo.CreateDateTime = DateTime.Now;
            saveInfo.ModifyDateTime = DateTime.Now;
            saveInfo.CreateUserId = this.SessionCurrUserId;
            saveInfo.ModifyUserId = this.SessionCurrUserId;
            saveInfo.IsDelete = 0;
            saveInfo.Path = "Uploads/" + ContAttachmentDTO.FolderName + "/" + ContAttachmentDTO.GuidFileName;
            saveInfo.ContId = (ContAttachmentDTO.ContId ?? 0) <= 0 ? -this.SessionCurrUserId : ContAttachmentDTO.ContId;
            saveInfo.FolderName = ContAttachmentDTO.FolderName;
            _IContAttachmentService.Add(saveInfo);

            return GetResult();

        }
        /// <summary>
        /// 修改附件
        /// </summary>
        /// <returns></returns>
        [NfCustomActionFilter("修改合同附件", OptionLogTypeEnum.Update, "修改合同附件", true)]
        public IActionResult UpdateSave(ContAttachmentDTO ContAttachmentDTO)
        {
            if (ContAttachmentDTO.Id > 0)
            {
                var updateinfo = _IContAttachmentService.Find(ContAttachmentDTO.Id);
                var updatedata = _IMapper.Map(ContAttachmentDTO, updateinfo);
                updateinfo.Path = "Uploads/" + ContAttachmentDTO.FolderName + "/" + ContAttachmentDTO.GuidFileName;
                updateinfo.FolderName = ContAttachmentDTO.FolderName;
                updatedata.ModifyUserId = this.SessionCurrUserId;
                updatedata.ModifyDateTime = DateTime.Now;
                _IContAttachmentService.Update(updatedata);
            }

            return GetResult();

        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <returns></returns>
        [NfCustomActionFilter("删除合同附件", OptionLogTypeEnum.Del, "删除合同附件", false)]
        public IActionResult Delete(string Ids)
        {
            _IContAttachmentService.Delete(Ids);
            return GetResult();
        }
        /// <summary>
        /// 查看
        /// </summary>
        /// <returns></returns>
        public IActionResult ShowView(int Id)
        {
            return new CustomResultJson(new RequstResult()
            {
                Msg = "",
                Code = 0,
                Data = _IContAttachmentService.ShowView(Id)


            });
        }

       
    }
}