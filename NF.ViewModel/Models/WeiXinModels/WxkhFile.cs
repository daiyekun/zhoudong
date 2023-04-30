using System;
using System.Collections.Generic;

namespace NF.ViewModel
{
    public class WxkhFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string CategoryName { get; set; }
        /// <summary>
        /// 提醒时间
        /// </summary>
        public DateTime? TxDate { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>

        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 图片集合
        /// </summary>

        public IList<PicView> PicData { get; set; }
        /// <summary>
        /// 视频集合
        /// </summary>
        public IList<VideoView> VideoData { get; set; }




    }

    /// <summary>
    /// 视频显示
    /// </summary>
    public class PicView
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string PicPath { get; set; }
    }
    /// <summary>
    /// 视频显示
    /// </summary>
    public class VideoView
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 视频路径
        /// </summary>
        public string VideoPath { get; set; }
        /// <summary>
        /// 缩略图
        /// </summary>
        public string ThumPath { get; set; }
    }

    /// <summary>
    /// 图片对象
    /// </summary>
    public class PicInfo : PicView
    {

        /// <summary>
        /// 客户ID
        /// </summary>
        public int? CompId { get; set; } = 0;
        /// <summary>
        /// 服务ID
        /// </summary>
        public int? AttId { get; set; } = 0;
    }

    /// <summary>
    /// 视频对象
    /// </summary>
    public class VideoInfo : VideoView
    {

        /// <summary>
        /// 客户ID
        /// </summary>
        public int? CompId { get; set; } = 0;
        /// <summary>
        /// 服务ID
        /// </summary>
        public int? AttId { get; set; } = 0;
    }
}
