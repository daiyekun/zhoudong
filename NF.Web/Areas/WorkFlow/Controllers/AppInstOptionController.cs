using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LhCode;
using Microsoft.AspNetCore.Mvc;
using NF.Common.Utility;
using NF.IBLL;
using NF.Model.Models;
using NF.ViewModel.Models;
using NF.ViewModel.Models.Common;
using NF.Web.Controllers;
using NF.Web.Utility;
using NF.Web.Utility.Filters;

namespace NF.Web.Areas.WorkFlow.Controllers
{
    [Area("WorkFlow")]
    [Route("WorkFlow/[controller]/[action]")]
    public class AppInstOptionController : NfBaseController
    {
        /// <summary>
        /// 映射
        /// </summary>
        private IMapper _IMapper;
        /// <summary>
        /// 实例服务
        /// </summary>
        private IAppInstService _IAppInstService;
        /// <summary>
        /// 实例节点
        /// </summary>
        private IAppInstNodeService _IAppInstNodeService;
        /// <summary>
        /// 意见
        /// </summary>
        private IAppInstOpinService _IAppInstOpinService;

        public AppInstOptionController(
            IMapper IMapper,
            IAppInstService IAppInstService
            ,IAppInstNodeService IAppInstNodeService
            , IAppInstOpinService IAppInstOpinService)
        {
            _IMapper = IMapper;
            _IAppInstService = IAppInstService;
            _IAppInstNodeService = IAppInstNodeService;
            _IAppInstOpinService = IAppInstOpinService;

        }
        /// <summary>
        /// 同意时提交意见
        /// </summary>
        /// <returns></returns>
        [CustomAction2CommitFilter]
        public async Task<IActionResult> SubmitAgreeOption(SubmitOptionInfo optionInfo)
        {
             optionInfo.SubmitUserId = this.SessionCurrUserId;
            //审批之前的实例信息，主要为了获取当前节点
             var currInst= _IAppInstService.Find(optionInfo.InstId);
            var nodeId = currInst.CurrentNodeId ?? 0;

            _IAppInstOpinService.SubmintOption(optionInfo);
           
            if (LhLicense.WxKaiQi == 1)
            {
                var instInfo = _IAppInstService.Find(optionInfo.InstId);
                ////删掉超时提醒
                //await Task.Factory.StartNew(() =>
                //{
                //    if(currInst!=null&& nodeId > 0)
                //    {
                //        _IAppInstService.ClearMsg(instInfo.Id, nodeId, optionInfo.SubmitUserId);
                //    }
                //});
                //提醒之前节点
                await Task.Factory.StartNew(() =>
                {

                    _IAppInstService.WxmsgPrvNode(instInfo, 0,"");
                });
                //添加微信消息
                if (instInfo.AppState == 2)
                {//审批通过
                    await Task.Factory.StartNew(() =>
                     {

                         _IAppInstService.WxDhOrTgMsg(instInfo, 0);


                     });
                }
                else
                {
                    //提醒下一个节点
                    await Task.Factory.StartNew(() =>
                    {

                        _IAppInstService.WeiXinFlowNodeMsg(instInfo);
                    });
                }
               
               

            }
            return GetResult();
        }

        /// <summary>
        /// 不同意时提交意见
        /// </summary>
        /// <returns></returns>
        [CustomAction2CommitFilter]
        public async Task<IActionResult> SubmitDisagreeOption(SubmitOptionInfo optionInfo)
        {
            var dds = optionInfo.DDs;
            optionInfo.SubmitUserId = this.SessionCurrUserId;
            //审批之前的实例信息，主要为了获取当前节点
            var currInst = _IAppInstService.Find(optionInfo.InstId);
            var nodeId = currInst.CurrentNodeId ?? 0;
            _IAppInstOpinService.SubmintDisagreeOption(optionInfo);
            if (LhLicense.WxKaiQi == 1)
            {
                var instInfo = _IAppInstService.Find(optionInfo.InstId);
                //删掉超时提醒
                //await Task.Factory.StartNew(() =>
                //{
                //    if (currInst != null && nodeId > 0)
                //    {
                //        _IAppInstService.ClearMsg(instInfo.Id, nodeId, optionInfo.SubmitUserId);
                //    }
                //});
                //提醒之前节点
                await Task.Factory.StartNew(() =>
                {

                    _IAppInstService.WxmsgPrvNode(instInfo, 1, dds);
                });
               
                //添加微信消息
                await Task.Factory.StartNew(() =>
                {
                    
                    _IAppInstService.WxDhOrTgMsg(instInfo,1);//不同意
                });
               
            }
            return GetResult();
        }
        /// <summary>
        /// 审批意见列表
        /// </summary>
        /// <param name="instId">审批实例ID</param>
        /// <param name="nodestrId">节点ID</param>
        /// <returns></returns>
        public IActionResult GetWfOptinions(PageparamInfo pageParam,int instId,string nodestrId)
        {
            var pageInfo = new PageInfo<AppInstOpin>(pageIndex: pageParam.page, pageSize: pageParam.limit);
            var predicateAnd = PredicateBuilder.True<AppInstOpin>();
            predicateAnd = predicateAnd.And(a=>a.InstId== instId);
            if (!string.IsNullOrEmpty(nodestrId))
            {
                predicateAnd = predicateAnd.And(a => a.NodeStrId == nodestrId);
            }
            var layPage = _IAppInstOpinService.GetOptinionList(pageInfo, predicateAnd, a => a.Id, true);
            return new CustomResultJson(layPage,true);
        }
    }
}