using NF.Common.Utility;
using NF.Model.Models;
using NF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NF.IBLL
{
    public partial interface IEnterpriseInfoService
    {

        /// <summary>
        /// 查看列表
        /// </summary>
        /// <param name="pageInfo">分页对象</param>
        /// <param name="whereLambda">查询条件表达式</param>
        /// <returns>返回layui所需对象</returns>
        LayPageInfo<EnterpriseInfoList> GetWxList<s>(PageInfo<EnterpriseInfo> pageInfo, Expression<Func<EnterpriseInfo, bool>> whereLambda, Expression<Func<EnterpriseInfo, s>> orderbyLambda, bool isAsc);
        /// <summary>
        /// 显示查看基本信息
        /// </summary>
        /// <param name="Id">当前ID</param>
        /// <returns></returns>
        EnterpriseInfoView ShowView(int Id);
    }
}
