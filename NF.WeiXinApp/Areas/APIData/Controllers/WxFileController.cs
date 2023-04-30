using Microsoft.AspNetCore.Mvc;
using NF.Common.Models;
using NF.Common.Utility;
using NF.IBLL;
using NF.Model.Models;
using NF.ViewModel.Extend.Enums;
using NF.WeiXinApp.Utility;
using System;
using System.IO;

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
            uploadFileInfo.GuidFileName = fname;
            uploadFileInfo.Extension = extend;
            uploadFileInfo.FolderName = EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 0);
            uploadFileInfo.SourceFileName = $"/Uploads/{uploadFileInfo.FolderName}/{uploadFileInfo.GuidFileName}";
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 0)
                        , fname);

            var filebyte = FileBinaryConvertHelper.FromBase64String(pics[1]);

            FileBinaryConvertHelper.Bytes2File(filebyte, path);


            return new WxResultJson(new RequstResult()
            {
                Msg = "上传成功",
                Code = 0,
                Data = uploadFileInfo


            });
        }


        /// <summary>
        ///多图片上传
        /// </summary>
        /// <param name="imgbase64">图片信息base64压缩</param>
        /// /// <param name="commpId">客户ID</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult WxUpload1s(string imgbase64, int commpId)
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
            attacFile.FileType = 0;//文件类型，图片
            attacFile.CreateDateTime = DateTime.Now;
            attacFile.CreateUserId = 1;
            attacFile.ModifyUserId = 1;
            attacFile.ModifyDateTime = DateTime.Now;
            attacFile.CompanyId = commpId;
            attacFile.AttId = -188;//附件信息
            attacFile.Extend = extend;
            attacFile.GuidFileName = fname;
            attacFile.FolderName = EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 0);
            attacFile.FilePath = $"/Uploads/{attacFile.FolderName}/{attacFile.GuidFileName}";
            var path = Path.Combine(
                       Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 0)
                       , fname);

            var filebyte = FileBinaryConvertHelper.FromBase64String(pics[1]);

            FileBinaryConvertHelper.Bytes2File(filebyte, path);


            var info = _IContAttacFileService.Add(attacFile);



            return new WxResultJson(new RequstResult()
            {
                Msg = "上传成功",
                Code = 0,
                Data = info


            });
        }

        /// <summary>
        ///多图片上传
        /// </summary>
        /// <param name="imgbase64">图片信息base64压缩</param>
        /// /// <param name="commpId">客户ID</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult WxUploadvideo
            (string imgbase64, int commpId, string video, string compressedVideo)
        {

            var fname = $"{Guid.NewGuid().ToString()}";
            string extend = ".mp4";

            // 获取上传的视频文件
            var videoFile = Request.Form.Files["video"];
            extend = Path.GetExtension(videoFile.FileName);

            // 获取压缩后的视频文件
            var compressedVideoFile = Request.Form.Files["compressedVideo"];

            // 获取视频文件名
            var fileName = videoFile.FileName;

            // 保存视频文件
            var tempdev4path = Path.Combine(
                      Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 1)
                      , $"{fname}{extend}");
            using (var fileStream = new FileStream(tempdev4path, FileMode.Create))
            {
                videoFile.CopyTo(fileStream);
            }
            //生成缩略图
            var repic = FFmpegUtiltiy.CreateVideoPic($"{fname}{extend}");

            //压缩图片保存压缩后的视频文件
            if (compressedVideoFile != null && compressedVideoFile.Length > 0)
            {
                var dev4path = Path.Combine(
                       Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 1)
                       , ($"{fname}.png"));
                using (var fileStream = new FileStream(dev4path, FileMode.Create))
                {
                    compressedVideoFile.CopyTo(fileStream);
                }
            }



            ContAttacFile attacFile = new ContAttacFile();
            attacFile.IsDelete = 0;
            attacFile.FileType = 1;//文件类型，视频
            attacFile.CreateDateTime = DateTime.Now;
            attacFile.CreateUserId = 1;
            attacFile.ModifyUserId = 1;
            attacFile.ModifyDateTime = DateTime.Now;
            attacFile.CompanyId = commpId;
            attacFile.AttId = -188;//附件信息
            attacFile.Extend = extend;
            attacFile.GuidFileName = $"{fname}{extend}";
            attacFile.FileName = fileName;
            attacFile.FolderName = EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 1);
            attacFile.FilePath = $"/Uploads/{attacFile.FolderName}/{attacFile.GuidFileName}";
            attacFile.ThumPath = repic.Result;
            //var path = Path.Combine(
            //           Directory.GetCurrentDirectory(), "Uploads", EmunUtility.GetDesc(typeof(UploadAndDownloadFoldersEnum), 1)
            //           , ($"{fname}{extend}");
            var info = _IContAttacFileService.Add(attacFile);
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
