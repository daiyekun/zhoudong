using Microsoft.AspNetCore.Mvc;
using NF.Common.Models;
using NF.IBLL;
using NF.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NF.Web.Areas.WeiXin.Controllers
{

    /// <summary>
    /// 到期提醒
    /// </summary>

    [Area("WeiXin")]
    [Route("WeiXin/[controller]/[action]")]
    public class DaoQiController : Controller
    {
        private IContractInfoService _IContractInfoService;
        private IAppInstService _IAppInstService;
        private ICompanyService _ICompanyService;
        public DaoQiController(IContractInfoService iContractInfoService, IAppInstService iAppInstService, ICompanyService iCompanyService)
        {
            _IContractInfoService = iContractInfoService;
            _IAppInstService = iAppInstService;
            _ICompanyService = iCompanyService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 到期合同
        /// 当状态为“执行中”的合同，
        /// 在当前时间到达“合同计划截止日期”的前三个月时，
        /// 进行提醒，提醒到合同建立人以及审批流程第一个节点的审批人的“企业微信”。
        /// </summary>
        /// <returns></returns>
        public IActionResult DaoQiHt()
        {
           
            return new CustomResultJson(new RequstResult()
            {

                Code = 0,
                Data = _IContractInfoService.DaoQqhtToRedisList()


        });
        }
        /// <summary>
        /// 到期计划资金
        /// 当状态为“执行中”的合同下的“计划资金”，
        /// 在当前时间到达“计划资金”的“计划完成时间”的前两个月时，
        /// 进行提醒，提醒到合同建立人以及审批流程第一个节点的审批人的“企业微信”。
        /// 
        /// </summary>
        /// <returns></returns>
        //public IActionResult DaoQiJh()
        //{
        //    return new CustomResultJson(new RequstResult()
        //    {

        //        Code = 0,
        //        Data = _IContractInfoService.DaoQqJhToRedisList()


        //    });

        //}
        ///// <summary>
        ///// 超过两天再次发送
        ///// </summary>
        ///// <returns></returns>
        //public IActionResult AppDaoQi()
        //{
        //    _IAppInstService.SearchAppMsg();
        //    return new CustomResultJson(new RequstResult()
        //    {

        //        Code = 0,
        //        Data ="ok"


        //    });

        //}

        ///// <summary>
        ///// 到期提醒条数
        ///// </summary>
        ///// <returns></returns>
        //public IActionResult AppRows()
        //{
        //    _IAppInstService.PubMsgRowsToList();
        //    return new CustomResultJson(new RequstResult()
        //    {

        //        Code = 0,
        //        Data = "ok"


        //    });

        //}

        /// <summary>
        /// 到期提醒条数
        /// </summary>
        /// <returns></returns>
        public IActionResult WxZhouDongTx()
        {
            _ICompanyService.DaoQiTx();
            return new CustomResultJson(new RequstResult()
            {

                Code = 0,
                Data = "ok"


            });

        }
    }
}
