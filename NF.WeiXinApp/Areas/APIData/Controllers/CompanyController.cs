using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NF.Common.Models;
using NF.Common.Utility;
using NF.IBLL;
using NF.Model.Models;
using NF.ViewModel.Extend.Enums;
using NF.ViewModel.Models.Common;
using NF.ViewModel.Models.WeiXinModels;
using NF.WeiXin.Lib.Common;
using NF.WeiXin.Lib.Utility;
using NF.WeiXinApp.Extend;
using NF.WeiXinApp.Utility;
using NF.WeiXinApp.Utility.Common;
using NF.WeiXinApp.Utility.Filters;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace NF.WeiXinApp.Areas.APIData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : Controller
    {
        private ICompanyService _ICompanyService;
        /// <summary>
        /// 映射 AutoMapper
        /// </summary>
        private IMapper _IMapper { get; set; }
        private IProjectManagerService _IProjectManagerService;
        private ICityService _ICityService;
        private IProvinceService _IProvinceService;
        private ICountryService _ICountryService;
        private ISysPermissionModelService _ISysPermissionModelService;
        private ICompAttachmentService _ICompAttachmentService;
        /// <summary>
        /// 合同
        /// </summary>
        private IContractInfoService _IContractInfoService;
        /// <summary>
        /// 用户
        /// </summary>
        private IUserInforService _IUserInforService;
        /// <summary>
        /// 编号自动生成
        /// </summary>
        private INoHipler _INoHipler;
        public CompanyController(
            ICompAttachmentService ICompAttachmentService,
            ICompanyService ICompanyService, ICompContactService ICompContactService,
            IMapper IMapper, ICityService ICityService, ICountryService ICountryService,
            IProvinceService IProvinceService, ISysPermissionModelService ISysPermissionModelService
            , IContractInfoService IContractInfoService
            , IUserInforService IUserInforService
            , INoHipler INoHipler
            , IProjectManagerService IProjectManagerService
             )
        {
            _ICompAttachmentService = ICompAttachmentService;
            _ICompanyService = ICompanyService;
            _IMapper = IMapper;
            _ICityService = ICityService;
            _ICountryService = ICountryService;
            _IProvinceService = IProvinceService;
            _ISysPermissionModelService = ISysPermissionModelService;
            _IContractInfoService = IContractInfoService;
            _IUserInforService = IUserInforService;
            _INoHipler = INoHipler;
            _IProjectManagerService = IProjectManagerService;
        }
        // GET: api/<UserController>
        [HttpGet("WooKhlist")]
        public string Get(int page, int limit, string keyWord, string Wxzh, int FinanceType)
        {
            PageparamInfo pageParam = new PageparamInfo();
            var usinfo = _IContractInfoService.Yhinfo(Wxzh);
            var UsId = usinfo.Id;
            var UsDc = usinfo.DepartmentId;
            var pageInfo = new PageInfo<Model.Models.Company>(pageIndex: page, pageSize: limit);
            var predicateAnd = PredicateBuilder.True<Model.Models.Company>();
            predicateAnd = predicateAnd.And(GetQueryExpression(pageInfo, pageParam, UsId, UsDc ?? 0, FinanceType, keyWord));
            Expression<Func<Model.Models.Company, object>> orderbyLambda = null;
            bool IsAsc = false;
            switch (pageParam.orderField)
            {
                case "Name":
                    orderbyLambda = a => a.Name;

                    break;
                case "Code":
                    orderbyLambda = a => a.Code;
                    break;
                default:
                    orderbyLambda = a => a.Id;
                    break;
            }
            if (pageParam.orderType == "asc")
                IsAsc = true;
            var layPage = _ICompanyService.GetWxCompList(pageInfo, predicateAnd, orderbyLambda, IsAsc);
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
        private Expression<Func<Model.Models.Company, bool>> GetQueryExpression(PageInfo<Model.Models.Company> pageInfo, PageparamInfo pageParam, int UsId, int UsDc, int FinanceType, string keyWord)
        {
            var predicateAnd = PredicateBuilder.True<Model.Models.Company>();
            var predicateOr = PredicateBuilder.False<Model.Models.Company>();
            predicateAnd = predicateAnd.And(a => a.Ctype == FinanceType && a.IsDelete == 0);
            var dt = "";
            if (FinanceType == 0)
            {
                dt = "querycustomerlist";
            }
            else if (FinanceType == 1)
            {
                dt = "querysupplierlist";
            }
            else if (FinanceType == 2)
            {
                dt = "queryotherlist";
            }


            predicateAnd = predicateAnd.And(_ISysPermissionModelService.GetCmpListPermissionExpression(dt, UsId, UsDc));
            //  predicateAnd = predicateAnd.And(_ISysPermissionModelService.GetCmpListPermissionExpression(dt, UsId, UsDc));
            if (!string.IsNullOrEmpty(keyWord) && keyWord.ToLower() != "undefined")
            {
                predicateOr = predicateOr.Or(a => a.Name.Contains(keyWord));
                predicateOr = predicateOr.Or(a => a.Code.Contains(keyWord));
                predicateAnd = predicateAnd.And(predicateOr);
            }
            return predicateAnd;
        }

        // GET api/<UserController>/5
        [HttpGet("KhView")]
        public string KhView(int id)
        {
            var Kh = _ICompanyService.KhView(id);
            return new RequestData(data: Kh).ToWxJson();
        }
        [HttpGet("WxQtlxr")]
        public string WxQtlxr(int id)
        {
            var Re = _ICompanyService.WxQtlxr(id);
            return new RequestData(data: Re).ToWxJson();

        }

        /// <summary>
        ///客户附件列表
        /// </summary>
        /// <param name="File">文件id</param>
        /// <returns></returns>
        [HttpGet("GetcompViwe")]
        public string GetcompViwe(int Id)
        {

            var khFile = _ICompAttachmentService.GetcompViwe(Id);
            return new RequestData(data: khFile).ToWxJson();

        }
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="txtId">合同文本ID</param>
        /// <returns></returns>
        [HttpGet("DownLoadTxt")]
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="txtId">合同文本ID</param>
        /// <returns></returns>
        [HttpGet("DownLoadFile")]
        public IActionResult DownLoadFile(int txtId, int loadtype = 0)
        {
            // var txtId = 1;
            var httxinfo = _ICompAttachmentService.Find(txtId);
            DownLoadAndUploadRequestInfo downLoad = new DownLoadAndUploadRequestInfo();
            downLoad.Id = txtId;
            downLoad.folderIndex = 0;
            if (loadtype == 0)
            {
                var txturl = $"{Constant.WxDownloadurl}/{httxinfo.Path}";
                //本地保存路径
                var pathf = Path.Combine(
                                Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), downLoad.folderIndex)
                                , httxinfo.FileName);
                RequestUtility.Download(txturl, pathf);//下载到wxapp
                var downInfo = FileStreamingHelper.Download(pathf);//下载到微信客户端
                return File(downInfo.NfFileStream, downInfo.Memi, downInfo.FileName);
            }
            else
            {

                string guidFileName = httxinfo.GuidFileName;


                if (guidFileName.StartsWith('~'))
                {
                    var filearr = StringHelper.Strint2ArrayString(guidFileName, "/");

                    guidFileName = filearr.LastOrDefault();
                }
                //var pathf = Path.Combine(
                //                 Directory.GetCurrentDirectory(), "wwwroot", "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), downLoadAndUploadRequestInfo.folderIndex),
                //                 guidFileName);
                var pathf = Path.Combine(
                                 Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 0),
                                 guidFileName);

                var downInfo = FileStreamingHelper.Download(pathf);
                //var s = ToBase64String(downInfo.NfFileStream);

                return File(downInfo.NfFileStream, downInfo.Memi, downInfo.FileName);

            }




        }



        /// <summary>
        /// 添加客户
        /// </summary>
        /// <param name="info">添加客户</param>
        /// <returns></returns>
        [CustomAction2CommitFilter]
        [HttpPost("CustomerAdd")]
        public string CustomerAdd([FromBody] WxCustomerInfo info)
        {
            Log4netHelper.Info($"添加客户:{info.Name} 客户代码:{info.Code} 授权码:{info.QxCode}");
            if (string.IsNullOrEmpty(info.QxCode) || !info.QxCode.Equals("zd198911"))
            {
                return new RequestData(code: 1).ToWxJson();
            }
            try
            {
                var uinfo = _IUserInforService.GetWxUserById(info.WxCode);
                var compinfo = new Company();
                if (info.Id > 0)
                {
                    Log4netHelper.Info($"编辑客户:{info.Name} 客户代码:{info.Code} 客户ID:{info.Id}");
                    compinfo = _ICompanyService.Find(info.Id);
                    compinfo.Code = info.Code;
                    compinfo.Name = info.Name;
                    compinfo.FirstContact = info.FirstContact;
                    compinfo.FirstContactMobile = info.FirstContactMobile;
                    compinfo.Address = info.Address;
                    compinfo.ModifyDateTime = DateTime.Now;
                    compinfo.ModifyUserId = uinfo != null ? uinfo.UserId : 1;
                    _ICompanyService.Update(compinfo);
                }
                else
                {
                    Log4netHelper.Info($"添加客户:{info.Name} 客户代码:{info.Code}");
                    compinfo.Code = info.Code;
                    compinfo.Name = info.Name;
                    compinfo.FirstContact = info.FirstContact;
                    compinfo.FirstContactMobile = info.FirstContactMobile;
                    compinfo.Address = info.Address;
                    compinfo.ModifyDateTime = DateTime.Now;
                    compinfo.ModifyUserId = uinfo != null ? uinfo.UserId : 1;
                    compinfo.CreateDateTime = DateTime.Now;
                    compinfo.CreateUserId = 1;
                    compinfo.CreateUserId = uinfo != null ? uinfo.UserId : 1;
                    compinfo.CompClassId = 49;
                    compinfo.Ctype = 0;//客户
                    _ICompanyService.Add(compinfo);
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
        [HttpPost("CustomerDel")]
        public async Task<string> CustomerDel([FromBody] WxCustomerInfo info)
        {
            try
            {
                var uinfo = _IUserInforService.GetWxUserById(info.WxCode);
                var compinfo = new Company();
                if (info.Id > 0)
                {
                    compinfo = _ICompanyService.Find(info.Id);
                    compinfo.Code = info.Code;
                    compinfo.Name = info.Name;
                    compinfo.FirstContact = info.FirstContact;
                    compinfo.FirstContactMobile = info.FirstContactMobile;
                    compinfo.Address = info.Address;
                    compinfo.ModifyDateTime = DateTime.Now;
                    compinfo.ModifyUserId = uinfo != null ? uinfo.UserId : 1;
                    _ICompanyService.Update(compinfo);
                }
                else
                {
                    compinfo.Code = info.Code;
                    compinfo.Name = info.Name;
                    compinfo.FirstContact = info.FirstContact;
                    compinfo.FirstContactMobile = info.FirstContactMobile;
                    compinfo.Address = info.Address;
                    compinfo.ModifyDateTime = DateTime.Now;
                    compinfo.ModifyUserId = uinfo != null ? uinfo.UserId : 1;
                    compinfo.CreateDateTime = DateTime.Now;
                    compinfo.CreateUserId = 1;
                    compinfo.CreateUserId = uinfo != null ? uinfo.UserId : 1;
                    compinfo.CompClassId = 49;
                    compinfo.Ctype = 0;//客户
                    _ICompanyService.Add(compinfo);
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
        /// 添加客户
        /// </summary>
        /// <param name="info">添加客户</param>
        /// <returns></returns>
        [CustomAction2CommitFilter]
        [HttpPost("CustomerFwAdd")]
        public async Task<string> CustomerFwAdd([FromBody] WxCustomerFwInfo info)
        {
            try
            {
                var uinfo = _IUserInforService.GetWxUserById(info.WxCode);
                var compinfo = new CompAttachment();
                if (info.Id > 0)
                {
                    compinfo = _ICompAttachmentService.Find(info.Id);

                    compinfo.Name = info.Name;
                    compinfo.DownloadTimes = 0;
                    compinfo.FileName = info.Name;
                    compinfo.FolderName = info.FolderName;
                    compinfo.FwTitle = "";
                    compinfo.ModifyDateTime = DateTime.Now;
                    compinfo.ModifyUserId = uinfo != null ? uinfo.UserId : 1;
                    compinfo.GuidFileName = info.GuidFileName;
                    compinfo.IsDelete = 0;
                    compinfo.TxDate = info.TxDate;
                    compinfo.Remark = info.Remark;
                    compinfo.Path = $"{info.FolderName}/{info.GuidFileName}";
                    compinfo.CompanyId = info.CompanyId;
                    compinfo.CategoryId = 1067;
                    _ICompAttachmentService.Update(compinfo);
                    string sqlstr = $"update ContAttacFile set AttId={compinfo.Id} where AttId=-188";
                    _ICompAttachmentService.ExecuteSqlCommand(sqlstr);
                }
                else
                {
                    compinfo.Name = info.Name;

                    compinfo.DownloadTimes = 0;
                    compinfo.FileName = info.Name;
                    compinfo.FolderName = info.FolderName;
                    compinfo.FwTitle = "";
                    compinfo.ModifyDateTime = DateTime.Now;
                    compinfo.ModifyUserId = uinfo != null ? uinfo.UserId : 1;
                    compinfo.GuidFileName = info.GuidFileName;
                    compinfo.IsDelete = 0;
                    compinfo.TxDate = info.TxDate;
                    compinfo.Remark = info.Remark;
                    compinfo.Path = $"{info.FolderName}/{info.GuidFileName}";
                    compinfo.CompanyId = info.CompanyId;
                    compinfo.CategoryId = 1067;
                    compinfo.CreateDateTime = DateTime.Now;
                    compinfo.CreateUserId = uinfo != null ? uinfo.UserId : 1;
                    var reinfo = _ICompAttachmentService.Add(compinfo);
                    string sqlstr = $"update ContAttacFile set AttId={reinfo.Id} where AttId=-188";
                    _ICompAttachmentService.ExecuteSqlCommand(sqlstr);
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
        /// <param name="Id">客户ID</param>
        /// <returns></returns>
        [HttpGet("DeleteCustomer")]
        public string DeleteCustomer(int Id)
        {
            try
            {
                var Kh = _ICompanyService.Delete($"{Id}");
                return new RequestData().ToWxJson();
            }
            catch (Exception ex)
            {

                Log4netHelper.Error(ex.Message);
                return new RequestData(code: 1).ToWxJson();
            }
        }

        /// <summary>
        /// 删除服务记录
        /// </summary>
        /// <param name="Id">客户ID</param>
        /// <returns></returns>
        [HttpGet("DeleteFwRows")]
        public string DeleteFwRows(int Id)
        {
            try
            {
                var Kh = _ICompAttachmentService.Delete(a => a.Id == Id);  // .Delete($"{Id}");
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
