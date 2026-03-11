using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NF.BLL;
using NF.Common.Utility;
using NF.IBLL;
using NF.Model.Models;
using NF.ViewModel;
using NF.ViewModel.Models.Common;
using NF.WeiXinApp.Extend;
using NF.WeiXinApp.Utility;
using NF.WeiXinApp.Utility.Filters;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NF.WeiXinApp.Areas.APIData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckController : Controller
    {
        private ICheckInfoService  _checkInfoService;
        private IUserInforService _userInforService;
        private IMapper _IMapper;
        private IContractInfoService _IContractInfoService;
        public CheckController(ICheckInfoService checkInfoService,
              IMapper IMapper
               , IUserInforService  userInforService,
               IContractInfoService iContractInfoService)
        {
            _checkInfoService = checkInfoService;
            _userInforService = userInforService;
            _IMapper = IMapper;
            _IContractInfoService = iContractInfoService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("checklist")]
        public string Get(int page, int limit, string keyWord, string Wxzh, int FinanceType)
        {
            PageparamInfo pageParam = new PageparamInfo();
            var usinfo = _IContractInfoService.Yhinfo(Wxzh);
            var UsId = usinfo.Id;
            var UsDc = usinfo.DepartmentId;
            var pageInfo = new PageInfo<Model.Models.CheckInfo>(pageIndex: page, pageSize: limit);
            var predicateAnd = PredicateBuilder.True<Model.Models.CheckInfo>();
            predicateAnd = predicateAnd.And(GetQueryExpression(pageInfo, pageParam, UsId, UsDc ?? 0, FinanceType, keyWord));
            Expression<Func<Model.Models.CheckInfo, object>> orderbyLambda = null;
            bool IsAsc = false;
            switch (pageParam.orderField)
            {
                case "Title":
                    orderbyLambda = a => a.Title;

                    break;
                default:
                    orderbyLambda = a => a.Id;
                    break;
            }
            if (pageParam.orderType == "asc")
                IsAsc = true;
            var layPage = _checkInfoService.GetWxList(pageInfo, predicateAnd, orderbyLambda, IsAsc);
            var json = layPage.ToWxJson();
            //Log4netHelper.Info($"hisdata返回数据:{json}");
            return json;
        }
        /// <summary>
        /// 获取查询条件表达式
        /// </summary>
        /// <param name="pageInfo">查询分页器，传NoPageInfo对象不分页</param>
        /// <param name="keyWord">查询关键字</param>
        /// <returns></returns>
        private Expression<Func<Model.Models.CheckInfo, bool>> GetQueryExpression(PageInfo<Model.Models.CheckInfo> pageInfo, PageparamInfo pageParam, int UsId, int UsDc, int FinanceType, string keyWord)
        {
            var predicateAnd = PredicateBuilder.True<Model.Models.CheckInfo>();
            var predicateOr = PredicateBuilder.False<Model.Models.CheckInfo>();
            //predicateAnd = predicateAnd.And(a =>&& a.IsDelete == 0);

            if (!string.IsNullOrEmpty(keyWord) && keyWord.ToLower() != "undefined")
            {
                predicateOr = predicateOr.Or(a => a.Title.Contains(keyWord));
                predicateAnd = predicateAnd.And(predicateOr);
            }
            return predicateAnd;
        }

        // GET api/<UserController>/5
        [HttpGet("checkView")]
        public string CheckView(int id)
        {
            var Kh = _checkInfoService.ShowView(id);
            return new RequestData(data: Kh).ToWxJson();
        }


        ///// <summary>
        /////客户附件列表
        ///// </summary>
        ///// <param name="File">文件id</param>
        ///// <returns></returns>
        //[HttpGet("GetcompViwe")]
        //public string GetcompViwe(int Id)
        //{

        //    var khFile = _ICompAttachmentService.GetcompViwe(Id);
        //    return new RequestData(data: khFile).ToWxJson();

        //}

        /// <summary>
        /// 添加客户
        /// </summary>
        /// <param name="info">添加客户</param>
        /// <returns></returns>
        [CustomAction2CommitFilter]
        [HttpPost("CheckAddSave")]
        public string CheckAddSave([FromBody] WxCheckInfo info)
        {
            Log4netHelper.Info($"添加客户:{info.Title}  授权码:{info.QxCode}");
            if (string.IsNullOrEmpty(info.QxCode) || !info.QxCode.Equals("zd198911"))
            {
                return new RequestData(code: 1).ToWxJson();
            }
            try
            {
                var uinfo = _userInforService.GetWxUserById(info.WxCode);
                var compinfo = new CheckInfo();
                if (info.Id > 0)
                {
                    compinfo = _checkInfoService.Find(info.Id);
                    compinfo.Title = info.Title;
                    compinfo.ModifyDateTime = DateTime.Now;
                    compinfo.ModifyUserId = uinfo != null ? uinfo.UserId : 1;
                    _checkInfoService.Update(compinfo);
                    string sqlstr = $"update CheckFile set AttId={compinfo.Id} where AttId=-188";
                    _checkInfoService.ExecuteSqlCommand(sqlstr);
                }
                else
                {
                    compinfo.Title = info.Title;
                    compinfo.ModifyDateTime = DateTime.Now;
                    compinfo.ModifyUserId = uinfo != null ? uinfo.UserId : 1;
                    compinfo.CreateDateTime = DateTime.Now;
                    compinfo.CreateUserId = 1;
                    compinfo.CreateUserId = uinfo != null ? uinfo.UserId : 1;
                    _checkInfoService.Add(compinfo);
                    string sqlstr = $"update CheckFile set AttId={compinfo.Id} where AttId=-188";
                    _checkInfoService.ExecuteSqlCommand(sqlstr);
                }



                return new RequestData().ToWxJson();
            }
            catch (Exception ex)
            {
                Log4netHelper.Error(ex.Message);
                return new RequestData(code: 1).ToWxJson();
            }
        }

        /// <summary>
        /// 删除客户
        /// </summary>
        /// <param name="info">删除客户</param>
        /// <returns></returns>
        [CustomAction2CommitFilter]
        [HttpPost("checkDel")]
        public async Task<string> CustomerDel([FromBody] WxCheckInfo info)
        {
            try
            {
                var uinfo = _userInforService.GetWxUserById(info.WxCode);
                _checkInfoService.Delete(a => a.Id == info.Id);
                return new RequestData().ToWxJson();
            }
            catch (Exception ex)
            {
                Log4netHelper.Error(ex.Message);
                return new RequestData(code: 1).ToWxJson();
            }
        }
    }
}
