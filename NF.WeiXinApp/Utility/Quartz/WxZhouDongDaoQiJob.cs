using NF.ViewModel.Models;
using NF.WeiXin.Lib.Utility;
using Quartz;
using System.Threading.Tasks;
using RedisHelper = NF.WeiXin.Lib.Utility.RedisHelper;

namespace NF.WeiXinApp.Utility.Quartz
{
    public class WxZhouDongDaoQiJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            GetWxMsgToZhouDong();

            return Task.CompletedTask;
        }
        /// <summary>
        /// 获取微信消息
        /// </summary>
        public void GetWxMsgToZhouDong()
        {
            if (RedisHelper.KeyExists("WxZhouDongTx"))
            {
                //从redis获取消息
                var opObj = RedisHelper.ListLeftPopToObj<WxTxTongZhi>("WxZhouDongTx");
                if (opObj != null)
                {


                    WeiXinMsgUtility.WxZhouDongDaoQi(opObj);//客户 消息标记1
                }
            }
        }
    }
}
