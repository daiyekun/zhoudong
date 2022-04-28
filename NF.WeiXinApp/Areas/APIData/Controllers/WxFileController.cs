using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NF.Common.Models;
using NF.Common.Utility;
using NF.IBLL;
using NF.Model.Models;
using NF.ViewModel.Extend.Enums;
using NF.WeiXinApp.Utility;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NF.WeiXinApp.Areas.APIData.Controllers
{

    /// <summary>
    /// 微信上传附件
    /// </summary>
    [Area("NfCommon")]
    [Route("NfCommon/[controller]/[action]")]
    public class WxFileController : Controller
    {
        private IContAttacFileService _IContAttacFileService;
        public WxFileController(IContAttacFileService IContAttacFileService)
        {
            _IContAttacFileService = IContAttacFileService;

        }
        public IActionResult Index()
        {
            return View();
        }

        


      

        /// <summary>
        ///小文件
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult WxUpload1(string imgbase64)
        {
            UploadFileInfo uploadFileInfo = new UploadFileInfo();
            var pics= imgbase64.Split(',');

            
           // var pics= StringHelper.Strint2ArrayString1(imgbase64);
            var fname = $"{Guid.NewGuid().ToString()}";
            var extend = ".png";
            switch (pics[0])
            {

                case "data:image/png;base64":
                    fname = fname + ".png";
                    extend = ".png";
                    break;
                case "data:image/gif;base64":
                    fname = fname + ".gif";
                    extend = ".gif";
                    break;
                case "data:image/jpg;base64":
                    fname = fname + ".jpg";
                    extend = ".jpg";
                    break;
                case "data:image/jpeg;base64":
                    fname = fname + ".jpeg";
                    extend = ".jpeg";
                    break;

            }
            uploadFileInfo.GuidFileName = fname;
            uploadFileInfo.Extension = extend;
            uploadFileInfo.FolderName= EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 0);
            uploadFileInfo.SourceFileName = $"/Uploads/{uploadFileInfo.FolderName}/{uploadFileInfo.GuidFileName}";
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 0)
                        , fname);

            var filebyte=FileBinaryConvertHelper.FromBase64String(pics[1]);

             FileBinaryConvertHelper.Bytes2File(filebyte, path);
           

            return new WxResultJson(new RequstResult()
            {
                Msg = "上传成功",
                Code = 0,
                Data= uploadFileInfo


            });
        }


        /// <summary>
        ///多图片上传
        /// </summary>
        /// <param name="imgbase64">图片信息base64压缩</param>
        /// /// <param name="commpId">客户ID</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult WxUpload1s(string imgbase64,int commpId)
        {
           
            var pics = imgbase64.Split(',');


            // var pics= StringHelper.Strint2ArrayString1(imgbase64);
            var fname = $"{Guid.NewGuid().ToString()}";
            var extend = ".png";
            switch (pics[0])
            {

                case "data:image/png;base64":
                    fname = fname + ".png";
                    extend = ".png";
                    break;
                case "data:image/gif;base64":
                    fname = fname + ".gif";
                    extend = ".gif";
                    break;
                case "data:image/jpg;base64":
                    fname = fname + ".jpg";
                    extend = ".jpg";
                    break;
                case "data:image/jpeg;base64":
                    fname = fname + ".jpeg";
                    extend = ".jpeg";
                    break;

            }
            ContAttacFile attacFile = new ContAttacFile();
            attacFile.IsDelete = 0;
            attacFile.CreateDateTime = DateTime.Now;
            attacFile.CreateUserId = 1;
            attacFile.ModifyUserId = 1;
            attacFile.ModifyDateTime= DateTime.Now;
            attacFile.CompanyId = commpId;
            attacFile.AttId = -188;//附件信息
            attacFile.Extend = extend;
            attacFile.GuidFileName = fname;
            attacFile.FolderName = EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 0);
            attacFile.FilePath = $"/Uploads/{ attacFile.FolderName }/{attacFile.GuidFileName}";
            var path = Path.Combine(
                       Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 0)
                       , fname);

            var filebyte = FileBinaryConvertHelper.FromBase64String(pics[1]);

            FileBinaryConvertHelper.Bytes2File(filebyte, path);


           var info= _IContAttacFileService.Add(attacFile);



            return new WxResultJson(new RequstResult()
            {
                Msg = "上传成功",
                Code = 0,
                Data = info


            });
        }


        /// <summary>
        /// 清除数据
        /// </summary>
        /// <returns></returns>
        public IActionResult ClearData()
        {

            string sqlstr = $"delete ContAttacFile where AttId={-188}";

            _IContAttacFileService.ExecuteSqlCommand(sqlstr);

          

            return new WxResultJson(new RequstResult()
            {
                Msg = "上传成功",
                Code = 0,
               

            });
        }

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <returns></returns>
        public IActionResult DeleteData(int Id)
        {

            string sqlstr = $"delete ContAttacFile where Id={Id}";

            _IContAttacFileService.ExecuteSqlCommand(sqlstr);



            return new WxResultJson(new RequstResult()
            {
                Msg = "上传成功",
                Code = 0,


            });
        }



    }
}
