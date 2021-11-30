using NF.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using NF.AutoMapper;
using NF.Common.Utility;
using NF.ViewModel.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using NF.Common.Extend;
using System.Threading.Tasks;
using NF.BLL.Common;

namespace NF.BLL
{
    public partial class AppInstService
    {

        #region 提交审批
        /// <summary>
        /// 提交审批流程
        /// </summary>
        /// <param name="appInst">审批实例对象</param>
        /// <returns>审批实例</returns>
        public AppInst SubmitWorkFlow(AppInst appInst)
        {
            var temphist = Db.Set<FlowTempHist>().AsNoTracking().Where(a => a.TempId == appInst.TempId).OrderByDescending(a => a.Id).FirstOrDefault();
            appInst.TempId = appInst.TempId;
            appInst.TempHistId = temphist != null ? temphist.Id : 0;
            var info = Add(appInst);
            //查询打回的审批实例
            var oldinstInfo = this.Db.Set<AppInst>().Where(a => a.AppObjId == info.AppObjId && a.ObjType == info.ObjType && a.AppState == 3).OrderByDescending(a => a.Id).FirstOrDefault();
            if (oldinstInfo != null)
            {
                oldinstInfo.NewInstId = info.Id;
            }
            this.SaveChanges();
            SaveWfNode(info, temphist);
            return appInst;
        }
        /// <summary>
        /// 保存节点
        /// </summary>
        /// <param name="appInst">审批实例</param>
        /// <param name="temphist">模板历史</param>
        private void SaveWfNode(AppInst appInst, FlowTempHist temphist)
        {
            switch (appInst.ObjType)
            {
                case (int)FlowObjEnums.Customer:
                case (int)FlowObjEnums.Supplier:
                case (int)FlowObjEnums.Other:
                case (int)FlowObjEnums.project://不需要金额判断每个节点的
                case (int)FlowObjEnums.Inquiry:
                case (int)FlowObjEnums.Questioning:
                case (int)FlowObjEnums.Tender:
                    {
                        AddNode(appInst, temphist);
                        AddNodeInfo(appInst);
                        AddLine(appInst);
                    }
                    break;
                case (int)FlowObjEnums.Contract:
                case (int)FlowObjEnums.InvoiceIn:
                case (int)FlowObjEnums.InvoiceOut:
                case (int)FlowObjEnums.payment:
                    {
                        AddNodeByAmount(appInst, temphist);

                    }
                    break;

            }
            AddArea(appInst);
            this.SaveChanges();
            SetCurrentNode(appInst);
            SetAppGroupInfo(appInst);

        }
        /// <summary>
        /// 添加根据金额判断的流程节点
        /// </summary>
        /// <param name="appInst">审批实例对象</param>
        /// <param name="temphist">模板历史</param>
        private void AddNodeByAmount(AppInst appInst, FlowTempHist temphist)
        {
            var subminfo = new SubmitWfResParam();
            subminfo.Amount = appInst.AppObjAmount ?? 0;
            subminfo.TempId = appInst.TempId ?? 0;
            var nodeIds = FlowServoceUtility.GetNodeStrIds(subminfo, this.Db);
            AddNode(appInst, temphist, nodeIds);
            AddNodeInfo(appInst, nodeIds);
            AddLine(appInst, nodeIds);
        }

        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="appInst">实例对象</param>
        private void SetAppGroupInfo(AppInst appInst)
        {
            StringBuilder sqlstr = new StringBuilder();
            var listnodes = Db.Set<AppInstNode>().AsNoTracking().Where(a => a.InstId == appInst.Id).ToList();
            var listnodeInfos = Db.Set<AppInstNodeInfo>().AsNoTracking().Where(a => a.InstId == appInst.Id).ToList();
            var appgroups = Db.Set<AppGroupUser>().AsNoTracking().Where(a => a.InstId == appInst.Id).ToList();
            foreach (var info in appgroups)
            {
                var node = listnodes.Where(a => a.NodeStrId == info.NodeStrId).FirstOrDefault();
                var nodeinfo= listnodeInfos.Where(a => a.NodeStrId == info.NodeStrId).FirstOrDefault();
                if(node!=null&& nodeinfo != null)
                {
                    sqlstr.Append($"update AppGroupUser set NodeId={node.Id},NinfoId={nodeinfo.Id} where Id={info.Id}");
                }
               

            }

            if (!string.IsNullOrEmpty(sqlstr.ToString()))
            {
                ExecuteSqlCommand(sqlstr.ToString());
            }



        }

        /// <summary>
        /// 提交流程时设置当前审批节点（第一个审批节点）
        /// </summary>
        ///<param name="appInst">审批实例对象</param>
        private void SetCurrentNode(AppInst appInst)
        {
            var listnodes = Db.Set<AppInstNode>().AsNoTracking().Where(a => a.InstId == appInst.Id).ToList();
            var stratNode = listnodes.FirstOrDefault(a => a.Type == 0);
            if (stratNode != null)
            {
                var firstLine = Db.Set<AppInstNodeLine>().AsNoTracking().Where(a => a.InstId == appInst.Id && a.From == stratNode.NodeStrId).FirstOrDefault();
                if (firstLine != null)
                {
                    var currnode = listnodes.Where(a => a.NodeStrId == firstLine.To).FirstOrDefault();
                    if (currnode != null)
                    {
                        StringBuilder sqlstr = new StringBuilder();
                        sqlstr.Append($"update AppInstNodeLine set Marked=1 where Id={firstLine.Id};");
                        sqlstr.Append($"update AppInst set CurrentNodeId={currnode.Id},CurrentNodeStrId='{currnode.NodeStrId}',CurrentNodeName='{currnode.Name}',AppState=1 where Id={appInst.Id};");
                        //sqlstr.Append($"update AppInstHist set CurrentNodeId={currnode.Id},CurrentNodeStrId='{currnode.NodeStrId}',CurrentNodeName='{currnode.Name}',AppState=1 where Id={instHistId};");
                        //节点状态修改成审核中
                        sqlstr.Append($"update AppInstNode set NodeState=1,Marked=1,ReceDateTime='{DateTime.Now}' where InstId={appInst.Id} and NodeStrId='{currnode.NodeStrId}';");
                        //sqlstr.Append($"update AppInstNodeHist set NodeState=1,Marked=1,ReceDateTime='{DateTime.Now}' where InstHistId={instHistId} and NodeStrId='{currnode.NodeStrId}';");
                        //节点信息状态和节点状态一致，冗余是为了后期查询减少连表查询
                        sqlstr.Append($"update AppInstNodeInfo set NodeState=1 where InstId={appInst.Id} and NodeStrId='{currnode.NodeStrId}';");
                        //sqlstr.Append($"update AppInstNodeInfoHist set NodeState=1 where InstHistId={instHistId} and NodeStrId='{currnode.NodeStrId}';");
                        appInst.CurrentNodeStrId = currnode.NodeStrId;
                        appInst.CurrentNodeName = currnode.Name;
                        ExecuteSqlCommand(sqlstr.ToString());
                       
                    }
                }
            }
        }
        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="appInst">审批实例对象</param>
        /// <param name="temphist">模板历史</param>
        private void AddNode(AppInst appInst, FlowTempHist temphist)
        {
            var list = this.Db.Set<FlowTempNode>().AsNoTracking().Where(a => a.TempId == appInst.TempId).ToList();
            IList<AppInstNode> listnodes = new List<AppInstNode>();
            foreach (var item in list)
            {
                var node = item.ToModel<FlowTempNode, AppInstNode>();
                node.InstId = appInst.Id;
                node.TempHistId = temphist != null ? temphist.Id : 0;
                listnodes.Add(node);

            }
            this.Db.Set<AppInstNode>().AddRange(listnodes);

        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="appInst">审批实例对象</param>
        /// <param name="nodeStrIds">满足条件的节点集合</param>
        /// <param name="temphist">流程模板历史对象</param>
        private void AddNode(AppInst appInst, FlowTempHist temphist, IList<string> nodeStrIds)
        {
            int ntype0 = (int)NodeTypeEnum.NType0;
            int ntype1 = (int)NodeTypeEnum.NType1;
            var arrayIds = nodeStrIds.ToArray();
            var list = this.Db.Set<FlowTempNode>().AsNoTracking()
                .Where(a => (a.TempId == appInst.TempId && arrayIds.Contains(a.StrId))
                || (a.TempId == appInst.TempId && (a.Type == ntype0 || a.Type == ntype1))).ToList();
            IList<AppInstNode> listnodes = new List<AppInstNode>();
            foreach (var item in list)
            {
                var node = item.ToModel<FlowTempNode, AppInstNode>();
                node.InstId = appInst.Id;
                node.TempHistId = temphist != null ? temphist.Id : 0;
                listnodes.Add(node);

            }
            this.Db.Set<AppInstNode>().AddRange(listnodes);

        }
        /// <summary>
        /// 添加连线
        /// </summary>
        /// <param name="appInst">审批实例对象</param>
        private void AddNodeInfo(AppInst appInst)
        {
            var listnodeinfos = Db.Set<FlowTempNodeInfo>().AsNoTracking().Include(a => a.Group).Where(a => a.TempId == appInst.TempId).ToList();
            IList<AppInstNodeInfo> listnodeInfo = new List<AppInstNodeInfo>();
            foreach (var nInfo in listnodeinfos)
            {

                var nodeInfo = nInfo.ToModel<FlowTempNodeInfo, AppInstNodeInfo>();
                nodeInfo.InstId = appInst.Id;
                nodeInfo.NodeState = 0;//默认都是未审核
                nodeInfo.GroupName = nInfo.Group.Name;//组名称
                listnodeInfo.Add(nodeInfo);

            }
            this.Db.Set<AppInstNodeInfo>().AddRange(listnodeInfo);
            AddNodeGroupUser(appInst, listnodeinfos);
        }

        /// <summary>
        /// 添加连线
        /// </summary>
        /// <param name="appInst">审批实例对象</param>
        /// <param name="nodeStrIds">满足条件的节点</param>
        private void AddNodeInfo(AppInst appInst, IList<string> nodeStrIds)
        {
            var tempId = appInst.TempId??0;
            var tempIds = nodeStrIds.ToArray();
            var listnodeinfos = this.Db.Set<FlowTempNodeInfo>().AsNoTracking().Include(a => a.Group)
                .Where(a => a.TempId == tempId && tempIds.Contains(a.NodeStrId)).ToList();
            IList<AppInstNodeInfo> listnodeInfo = new List<AppInstNodeInfo>();
            foreach (var nInfo in listnodeinfos)
            {

                var nodeInfo = nInfo.ToModel<FlowTempNodeInfo, AppInstNodeInfo>();
                nodeInfo.InstId = appInst.Id;
                nodeInfo.NodeState = 0;//默认都是未审核
                nodeInfo.GroupName = nInfo.Group.Name;//组名称
                listnodeInfo.Add(nodeInfo);

            }
            this.Db.Set<AppInstNodeInfo>().AddRange(listnodeInfo);
            AddNodeGroupUser(appInst, listnodeinfos);
        }


        /// <summary>
        /// 添加节点组
        /// </summary>
        /// <param name="appInst">审批实例</param>
        private void AddNodeGroupUser(AppInst appInst, IList<FlowTempNodeInfo> tempNodeInfos)
        {
            var groupids = tempNodeInfos.Select(a => a.GroupId).ToList();
            var groups = this.Db.Set<GroupUser>().Where(a => groupids.Contains(a.GroupId)).ToList();
            IList<AppGroupUser> appGroupUsers = new List<AppGroupUser>();
            foreach (var tempNodeInfo in tempNodeInfos)
            {
                var userids = groups.Where(a => a.GroupId == tempNodeInfo.GroupId).Select(a => a.UserId ?? 0).ToList();
                //var userstrids = StringHelper.ArrayInt2String(userids);
                foreach (var uid in userids)
                {
                    AppGroupUser appGroupUser = new AppGroupUser();
                    appGroupUser.InstId = appInst.Id;
                    appGroupUser.NodeStrId = tempNodeInfo.NodeStrId;
                    appGroupUser.UserId = uid;
                    appGroupUser.GroupId = tempNodeInfo.GroupId;
                    appGroupUser.UserIsSp = 0;
                    appGroupUsers.Add(appGroupUser);
                }


            }
            this.Db.Set<AppGroupUser>().AddRange(appGroupUsers);



        }
        /// <summary>
        /// 添加连线
        /// </summary>
        /// <param name="appInst">实例对象</param>
        private void AddLine(AppInst appInst)
        {
            var listlines = Db.Set<TempNodeLine>().AsNoTracking().Where(a => a.TempId == appInst.TempId).ToList();
            IList<AppInstNodeLine> listline = new List<AppInstNodeLine>();
            foreach (var line in listlines)
            {
                var lineinfo = line.ToModel<TempNodeLine, AppInstNodeLine>();
                lineinfo.InstId = appInst.Id;
                listline.Add(lineinfo);
            }
            this.Db.Set<AppInstNodeLine>().AddRange(listline);

        }
        /// <summary>
        /// 添加连线
        /// </summary>
        /// <param name="appInst">实例对象</param>
        /// <param name="nodeStrIds">满足条件的节点集合</param>
        private void AddLine(AppInst appInst, IList<string> nodeStrIds)
        {
            var tempId = appInst.TempId??0;
            var nodeIds = nodeStrIds.ToArray();

            var listlines = this.Db.Set<TempNodeLine>().AsNoTracking()
                .Where(a => a.TempId == tempId && (nodeIds.Contains(a.To) || nodeIds.Contains(a.From))).ToList();
            IList<AppInstNodeLine> listline = new List<AppInstNodeLine>();
            foreach (var line in listlines)
            {
                var lineinfo = line.ToModel<TempNodeLine, AppInstNodeLine>();
                lineinfo.InstId = appInst.Id;
                listline.Add(lineinfo);
            }
            this.Db.Set<AppInstNodeLine>().AddRange(listline);

        }
        /// <summary>
        /// 区域ID
        /// </summary>
        /// <param name="appInst">审批实例对象</param>
        private void AddArea(AppInst appInst)
        {
            var listAreas = Db.Set<TempNodeArea>().AsNoTracking().Where(a => a.TempId == appInst.TempId).ToList();
            IList<AppInstNodeArea> listareas = new List<AppInstNodeArea>();
            foreach (var arra in listAreas)
            {
                var areainfo = arra.ToModel<TempNodeArea, AppInstNodeArea>();
                areainfo.InstId = appInst.Id;
                listareas.Add(areainfo);

            }

            this.Db.Set<AppInstNodeArea>().AddRange(listareas);

        }

        #endregion

        #region 已发起
        /// <summary>
        /// 已发起-参数介绍见接口
        /// </summary>
        /// <typeparam name="s"></typeparam>
        /// <param name="pageInfo"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderbyLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public LayPageInfo<AppsponsorListDTO> GetAppSponsorList<s>(PageInfo<AppInst> pageInfo, Expression<Func<AppInst, bool>> whereLambda, Expression<Func<AppInst, s>> orderbyLambda, bool isAsc)
        {
            var tempquery = _AppInstSet.AsTracking().Where(whereLambda.Compile());
            pageInfo.TotalCount = tempquery.Count();
            tempquery = tempquery.Skip<AppInst>((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take<AppInst>(pageInfo.PageSize);

            var query = from a in tempquery
                        select new
                        {
                            Id = a.Id,
                            ObjType = a.ObjType,
                            AppObjId = a.AppObjId,
                            AppObjName = a.AppObjName,
                            AppObjNo = a.AppObjNo,
                            AppObjAmount = a.AppObjAmount,
                            Mission = a.Mission,
                            CurrentNodeId = a.CurrentNodeId,
                            CurrentNodeName = a.CurrentNodeName,
                            StartDateTime = a.StartDateTime,
                            AppState = a.AppState,
                            FinceType = a.FinceType,
                            AppSecObjId= a.AppSecObjId


                        };
            var local = from a in query.AsEnumerable()
                        select new AppsponsorListDTO
                        {
                            Id = a.Id,
                            ObjType = a.ObjType,
                            ObjTypeDic = EmunUtility.GetDesc(typeof(FlowObjEnums), a.ObjType),
                            AppObjId = a.AppObjId,
                            AppObjName = a.AppObjName,
                            AppObjNo = a.AppObjNo,
                            Mission = a.Mission,
                            MissionDic = FlowUtility.GetMessionDic(a.Mission ?? -1, a.ObjType),//审批事项
                            CurrentNodeId = a.CurrentNodeId,
                            CurrentNodeName = a.CurrentNodeName,
                            StartDateTime = a.StartDateTime,
                            AppState = a.AppState,
                            AppStateDic = EmunUtility.GetDesc(typeof(AppInstEnum), a.AppState),
                            AppObjAmount = a.AppObjAmount ?? 0,
                            AppObjAmountThod = a.AppObjAmount.ThousandsSeparator(),
                            FinceType = a.FinceType,
                            AppSecObjId = a.AppSecObjId//次要字段ID，比如实际资金时合同ID

                        };
            return new LayPageInfo<AppsponsorListDTO>()
            {
                data = local.ToList(),
                count = pageInfo.TotalCount,
                code = 0


            };
        }

        ///// <summary>
        ///// 获取审批事项
        ///// </summary>
        ///// <returns></returns>
        //private string GetMessionDic(int mession, int objTypeEnum)
        //{
        //    var itemObjType = EmunUtility.GetEnumItemExAttribute(typeof(FlowObjEnums), objTypeEnum);
        //    var list = EmunUtility.GetAttr(itemObjType.TypeValue);
        //    //var listids = StringHelper.String2ArrayInt(Ids);
        //    var firstMs = list.Where(a => a.Value == mession).FirstOrDefault();
        //    return firstMs?.Desc;

        //}
        /// <summary>
        /// 提交流程修改对象流程信息
        /// </summary>
        /// <param name="appInst">审批实例</param>
        /// <returns></returns>
        public int SubmitWfUpdateObjWfInfo(AppInst appInst)
        {
            if (appInst != null)
            {
                StringBuilder strsql = new StringBuilder();
                switch (appInst.ObjType)
                {
                    case (int)FlowObjEnums.Customer:
                    case (int)FlowObjEnums.Supplier:
                    case (int)FlowObjEnums.Other:
                        strsql.Append($"update Company set WfState=1,WfItem={appInst.Mission},WfCurrNodeName='{appInst.CurrentNodeName}'  where Id={appInst.AppObjId}");
                        break;
                    case (int)FlowObjEnums.project:
                        strsql.Append($"update ProjectManager set WfState=1,WfItem={appInst.Mission},WfCurrNodeName='{appInst.CurrentNodeName}'  where Id={appInst.AppObjId}");
                        break;
                    case (int)FlowObjEnums.Contract:
                        strsql.Append($"update ContractInfo set WfState=1,WfItem={appInst.Mission},WfCurrNodeName='{appInst.CurrentNodeName}'  where Id={appInst.AppObjId}");
                        break;
                    case (int)FlowObjEnums.payment:
                        strsql.Append($"update ContActualFinance set WfState=1,WfItem={appInst.Mission},WfCurrNodeName='{appInst.CurrentNodeName}'  where Id={appInst.AppObjId}");
                        break;
                    case (int)FlowObjEnums.InvoiceIn:
                        strsql.Append($"update ContInvoice set WfState=1,WfItem={appInst.Mission},WfCurrNodeName='{appInst.CurrentNodeName}'  where Id={appInst.AppObjId}");
                        break;
                    case (int)FlowObjEnums.InvoiceOut:
                        strsql.Append($"update ContInvoice set WfState=1,WfItem={appInst.Mission},WfCurrNodeName='{appInst.CurrentNodeName}'  where Id={appInst.AppObjId}");
                        break;
                    case (int)FlowObjEnums.Inquiry:
                        strsql.Append($"update Inquiry set WfState=1,WfItem={appInst.Mission},WfCurrNodeName='{appInst.CurrentNodeName}'  where Id={appInst.AppObjId}");
                        break;
                    case (int)FlowObjEnums.Questioning:
                        strsql.Append($"update Questioning set WfState=1,WfItem={appInst.Mission},WfCurrNodeName='{appInst.CurrentNodeName}'  where Id={appInst.AppObjId}");
                        break;
                    case (int)FlowObjEnums.Tender:
                        strsql.Append($"update TenderInfor set WfState=1,WfItem={appInst.Mission},WfCurrNodeName='{appInst.CurrentNodeName}'  where Id={appInst.AppObjId}");
                        break;
                }

                return ExecuteSqlCommand(strsql.ToString());
            }
            return 0;
        }
        #endregion

        #region 待处理
        /// <summary>
        /// 待处理-参数介绍见接口
        /// </summary>
        /// <typeparam name="s"></typeparam>
        /// <param name="pageInfo"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderbyLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public LayPageInfo<AppPendingListDTO> GetAppPendingList<s>(PageInfo<AppInst> pageInfo, int sessionUserId, Expression<Func<AppInst, bool>> whereLambda, Expression<Func<AppInst, s>> orderbyLambda, bool isAsc)
        {
            var predicateAnd = PredicateBuilder.True<AppInst>();
            predicateAnd = predicateAnd.And(whereLambda);

            var currnodeIds = Db.Set<AppInst>()
                .Where(a => a.AppState == (int)AppInstEnum.AppState0)
                .Select(a => a.CurrentNodeId).Distinct().ToList();

            var appInstIds = Db.Set<AppGroupUser>()
                .Where(a => a.UserId == sessionUserId 
                && currnodeIds.Contains(a.NodeId ?? 0)&&a.UserIsSp==0)
                .Select(a => a.InstId).ToList();

           
            predicateAnd = predicateAnd.And(a => (appInstIds.Any(c => c == a.Id)));
            var tempquery = _AppInstSet.AsTracking().Where(predicateAnd.Compile());
            pageInfo.TotalCount = tempquery.Count();
            tempquery = tempquery.Skip<AppInst>((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take<AppInst>(pageInfo.PageSize);

            var query = from a in tempquery
                        select new
                        {
                            Id = a.Id,
                            ObjType = a.ObjType,
                            AppObjId = a.AppObjId,
                            AppObjName = a.AppObjName,
                            AppObjNo = a.AppObjNo,
                            AppObjAmount = a.AppObjAmount,
                            Mission = a.Mission,
                            CurrentNodeId = a.CurrentNodeId,
                            CurrentNodeName = a.CurrentNodeName,
                            StartDateTime = a.StartDateTime,
                            AppState = a.AppState,
                            StartUserId = a.StartUserId,
                            FinceType=a.FinceType
                        };
            var local = from a in query.AsEnumerable()
                        select new AppPendingListDTO
                        {
                            Id = a.Id,
                            ObjType = a.ObjType,
                            ObjTypeDic = EmunUtility.GetDesc(typeof(FlowObjEnums), a.ObjType),
                            AppObjId = a.AppObjId,
                            AppObjName = a.AppObjName,
                            AppObjNo = a.AppObjNo,
                            Mission = a.Mission,
                            MissionDic = FlowUtility.GetMessionDic(a.Mission ?? -1, a.ObjType),//审批事项
                            CurrentNodeId = a.CurrentNodeId,
                            CurrentNodeName = a.CurrentNodeName,
                            StartDateTime = a.StartDateTime,
                            StartUserName = RedisValueUtility.GetUserShowName(a.StartUserId ?? 0),
                            AppState = a.AppState,
                            AppStateDic = EmunUtility.GetDesc(typeof(AppInstEnum), a.AppState),
                            AppObjAmount = a.AppObjAmount ?? 0,
                            AppObjAmountThod = a.AppObjAmount.ThousandsSeparator(),
                            FinceType = a.FinceType

                        };
            return new LayPageInfo<AppPendingListDTO>()
            {
                data = local.ToList(),
                count = pageInfo.TotalCount,
                code = 0


            };
        }
        #endregion

        #region 已处理
        /// <summary>
        /// 已处理-参数介绍见接口
        /// </summary>
        /// <typeparam name="s"></typeparam>
        /// <param name="pageInfo"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderbyLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public LayPageInfo<AppProcessedListDTO> GetAppProcessedList<s>(PageInfo<AppInst> pageInfo, int sessionUserId, Expression<Func<AppInst, bool>> whereLambda, Expression<Func<AppInst, s>> orderbyLambda, bool isAsc)
        {

            try
            {
                var tempquery = _AppInstSet.AsTracking().Where<AppInst>(whereLambda.Compile()).AsQueryable();
                pageInfo.TotalCount = tempquery.Count();
                tempquery = tempquery.Skip((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take(pageInfo.PageSize);
                var query = from a in tempquery
                            select new
                            {
                                Id = a.Id,
                                ObjType = a.ObjType,
                                AppObjId = a.AppObjId,
                                AppObjName = a.AppObjName,
                                AppObjNo = a.AppObjNo,
                                AppObjAmount = a.AppObjAmount,
                                Mission = a.Mission,
                                CurrentNodeId = a.CurrentNodeId,
                                CurrentNodeName = a.CurrentNodeName,
                                StartDateTime = a.StartDateTime,
                                AppState = a.AppState,
                                StartUserId = a.StartUserId,
                                CompleteDateTime = a.CompleteDateTime,
                                //Option = ap.Opinion,
                                //OptionDate = ap.CreateDatetime,
                                FinceType = a.FinceType,
                                AppSecObjId = a.AppSecObjId,
                            };
                var local = from a in query.AsEnumerable()
                            select new AppProcessedListDTO
                            {
                                Id = a.Id,
                                ObjType = a.ObjType,
                                ObjTypeDic = EmunUtility.GetDesc(typeof(FlowObjEnums), a.ObjType),
                                AppObjId = a.AppObjId,
                                AppObjName = a.AppObjName,
                                AppObjNo = a.AppObjNo,
                                Mission = a.Mission,
                                MissionDic = FlowUtility.GetMessionDic(a.Mission ?? -1, a.ObjType),//审批事项
                                CurrentNodeId = a.CurrentNodeId,
                                CurrentNodeName = a.CurrentNodeName,
                                StartDateTime = a.StartDateTime,
                                StartUserName = RedisValueUtility.GetUserShowName(a.StartUserId ?? 0),
                                AppState = a.AppState,
                                AppStateDic = EmunUtility.GetDesc(typeof(AppInstEnum), a.AppState),
                                AppObjAmount = a.AppObjAmount ?? 0,
                                AppObjAmountThod = a.AppObjAmount.ThousandsSeparator(),
                                CompleteDateTime = a.CompleteDateTime,
                                FinceType = a.FinceType,
                                AppSecObjId = a.AppSecObjId,


                            };
                return new LayPageInfo<AppProcessedListDTO>()
                {
                    data = local.ToList(),
                    count = pageInfo.TotalCount,
                    code = 0
                };
            }
            catch (Exception ex)
            {

                Log4netHelper.Error(ex.Message);
                return new LayPageInfo<AppProcessedListDTO>()
                {
                    data = null,
                    count = 0,
                    code = 0
                };
            }

            #region 不需要

            //if (tempquery.Count()>0)
            //{


            //var query0 = from a in tempquery
            //             join
            //             b in Db.Set<AppInstOpin>()
            //             on a.Id equals b.InstId into ab
            //             from ap in ab.DefaultIfEmpty()
            //             where ap.CreateUserId == sessionUserId
            //             select new
            //            {
            //                Id = a.Id,
            //                ObjType = a.ObjType,
            //                AppObjId = a.AppObjId,
            //                AppObjName = a.AppObjName,
            //                AppObjNo = a.AppObjNo,
            //                AppObjAmount = a.AppObjAmount,
            //                Mission = a.Mission,
            //                CurrentNodeId = a.CurrentNodeId,
            //                CurrentNodeName = a.CurrentNodeName,
            //                StartDateTime = a.StartDateTime,
            //                AppState = a.AppState,
            //                StartUserId = a.StartUserId,
            //                 Option = ap.Opinion,
            //                 OptionDate = ap.CreateDatetime,
            //                 FinceType = a.FinceType,
            //                AppSecObjId = a.AppSecObjId,
            //            };

            //pageInfo.TotalCount = query0.Count();
            //query0 = query0.Skip((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take(pageInfo.PageSize);
            //var query = from a in query0

            //            select new
            //            {
            //                Id = a.Id,
            //                ObjType = a.ObjType,
            //                AppObjId = a.AppObjId,
            //                AppObjName = a.AppObjName,
            //                AppObjNo = a.AppObjNo,
            //                AppObjAmount = a.AppObjAmount,
            //                Mission = a.Mission,
            //                CurrentNodeId = a.CurrentNodeId,
            //                CurrentNodeName = a.CurrentNodeName,
            //                StartDateTime = a.StartDateTime,
            //                AppState = a.AppState,
            //                StartUserId = a.StartUserId,
            //                //Option = a.Option,
            //                //OptionDate = a.OptionDate,
            //                FinceType = a.FinceType,
            //                AppSecObjId = a.AppSecObjId,




            //            };
            //var local = from a in query.AsEnumerable()
            //            select new AppProcessedListDTO
            //            {
            //                Id = a.Id,
            //                ObjType = a.ObjType,
            //                ObjTypeDic = EmunUtility.GetDesc(typeof(FlowObjEnums), a.ObjType),
            //                AppObjId = a.AppObjId,
            //                AppObjName = a.AppObjName,
            //                AppObjNo = a.AppObjNo,
            //                Mission = a.Mission,
            //                MissionDic = FlowUtility.GetMessionDic(a.Mission ?? -1, a.ObjType),//审批事项
            //                CurrentNodeId = a.CurrentNodeId,
            //                CurrentNodeName = a.CurrentNodeName,
            //                StartDateTime = a.StartDateTime,
            //                StartUserName = RedisValueUtility.GetUserShowName(a.StartUserId ?? 0),
            //                AppState = a.AppState,
            //                AppStateDic = EmunUtility.GetDesc(typeof(AppInstEnum), a.AppState),
            //                AppObjAmount = a.AppObjAmount ?? 0,
            //                AppObjAmountThod = a.AppObjAmount.ThousandsSeparator(),
            //                Option = Yj(a.Id),//a.Option,
            //                OptionDate = Sj(a.Id),// a.OptionDate,
            //                FinceType = a.FinceType,
            //                AppSecObjId = a.AppSecObjId,


            //            };
            //return new LayPageInfo<AppProcessedListDTO>()
            //{
            //    data = local.ToList(),
            //    count = pageInfo.TotalCount,
            //    code = 0
            //};
            //}
            //else
            //{
            //    return new LayPageInfo<AppProcessedListDTO>()
            //    {
            //       // data = "",
            //        count = pageInfo.TotalCount,
            //        code =0
            //    };
            //}
            #endregion
        }
        public string Yj(int id)
        {
            var sd = Db.Set<AppInstOpin>().Where(a => a.InstId == id).ToList();

            var name = sd.Count();
            var eer = sd.FirstOrDefault().Opinion;
           
            return eer;
        }
        public DateTime? Sj(int id)
        {
            var sd = Db.Set<AppInstOpin>().Where(a => a.InstId == id).ToList();

            var name = sd.Count();
            DateTime? eer = sd.FirstOrDefault().CreateDatetime;

            return eer;
        }
        #endregion

        #region 被打回 

        /// <summary>
        /// 被打回-参数介绍见接口
        /// </summary>
        /// <typeparam name="s"></typeparam>
        /// <param name="pageInfo"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderbyLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public LayPageInfo<AppsponsorListDTO> GetAppBeBackList<s>(PageInfo<AppInst> pageInfo, Expression<Func<AppInst, bool>> whereLambda, Expression<Func<AppInst, s>> orderbyLambda, bool isAsc)
        {
            var tempquery = _AppInstSet.AsTracking().Where(whereLambda.Compile())
                 .Skip<AppInst>((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take<AppInst>(pageInfo.PageSize);
            pageInfo.TotalCount = tempquery.Count();
            var query = from a in tempquery
                        select new
                        {
                            Id = a.Id,
                            ObjType = a.ObjType,

                            AppObjId = a.AppObjId,
                            AppObjName = a.AppObjName,
                            AppObjNo = a.AppObjNo,
                            AppObjAmount = a.AppObjAmount,
                            Mission = a.Mission,
                            CurrentNodeId = a.CurrentNodeId,
                            CurrentNodeName = a.CurrentNodeName,
                            StartDateTime = a.StartDateTime,
                            AppState = a.AppState,
                            CompleteDateTime = a.CompleteDateTime,
                            FinceType=a.FinceType,
                            AppSecObjId=a.AppSecObjId,

                        };
            var local = from a in query.AsEnumerable()
                        select new AppsponsorListDTO
                        {
                            Id = a.Id,
                            ObjType = a.ObjType,
                            ObjTypeDic = EmunUtility.GetDesc(typeof(FlowObjEnums), a.ObjType),
                            AppObjId = a.AppObjId,
                            AppObjName = a.AppObjName,
                            AppObjNo = a.AppObjNo,
                            Mission = a.Mission,
                            MissionDic = FlowUtility.GetMessionDic(a.Mission ?? -1, a.ObjType),//审批事项
                            CurrentNodeId = a.CurrentNodeId,
                            CurrentNodeName = a.CurrentNodeName,
                            StartDateTime = a.StartDateTime,
                            AppState = a.AppState,
                            AppStateDic = EmunUtility.GetDesc(typeof(AppInstEnum), a.AppState),
                            AppObjAmount = a.AppObjAmount ?? 0,
                            AppObjAmountThod = a.AppObjAmount.ThousandsSeparator(),
                            CompleteDateTime = a.CompleteDateTime,
                            FinceType = a.FinceType,
                            AppSecObjId = a.AppSecObjId,

                        };
            return new LayPageInfo<AppsponsorListDTO>()
            {
                data = local.ToList(),
                count = pageInfo.TotalCount,
                code = 0


            };
        }
        #endregion 

        #region 审批历史
        /// <summary>
        /// 审批历史
        /// </summary>
        /// <typeparam name="s"></typeparam>
        /// <param name="pageInfo"></param>
        /// <param name="sessionUserId"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderbyLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>

        public LayPageInfo<ApproveHistListDTO> GetAppHistList<s>(PageInfo<AppInst> pageInfo, int sessionUserId, Expression<Func<AppInst, bool>> whereLambda, Expression<Func<AppInst, s>> orderbyLambda, bool isAsc)
        {
            var tempquery = _AppInstSet.AsTracking().Where(whereLambda.Compile()).AsQueryable();
            try
            {

                pageInfo.TotalCount = tempquery.Count();
            }
            catch (Exception ex)
            {


            }
            if (isAsc)
            {
                tempquery = tempquery.OrderBy(orderbyLambda);
            }
            else
            {
                tempquery = tempquery.OrderByDescending(orderbyLambda);
            }
            if (!(pageInfo is NoPageInfo<AppInst>))
            { //分页
                tempquery = tempquery.Skip<AppInst>((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take<AppInst>(pageInfo.PageSize);
            }

            var query = from a in tempquery
                        select new
                        {
                            Id = a.Id,
                            Mission = a.Mission,
                            CurrentNodeName = a.CurrentNodeName,
                            StartDateTime = a.StartDateTime,
                            AppState = a.AppState,
                            StartUserId = a.StartUserId,
                            ObjType = a.ObjType,
                            CompleteDateTime = a.CompleteDateTime


                        };
            var local = from a in query.AsEnumerable()
                        select new ApproveHistListDTO
                        {
                            Id = a.Id,
                            Mission = a.Mission,
                            MissionDic = FlowUtility.GetMessionDic(a.Mission ?? -1, a.ObjType),//审批事项
                            CurrentNodeName = a.CurrentNodeName,
                            StartDateTime = a.StartDateTime,
                            AppState = a.AppState,
                            AppStateDic = EmunUtility.GetDesc(typeof(AppInstEnum), a.AppState),
                            StartUserName = RedisValueUtility.GetUserShowName(a.StartUserId ?? 0),
                            CompleteDateTime = a.CompleteDateTime
                        };
            return new LayPageInfo<ApproveHistListDTO>()
            {
                data = local.ToList(),
                count = pageInfo.TotalCount,
                code = 0


            };
        }
        #endregion

        #region 待处理
        /// <summary>
        /// 待审批数
        /// </summary>
        /// <param name="currUserId"></param>
        /// <returns></returns>
        private int GetPaddingMsgNumber(int currUserId)
        {

            //var groupIds = Db.Set<GroupUser>().Where(a => a.UserId == currUserId).AsNoTracking().Select(a => a.GroupId).ToList();
            //var querynode = Db.Set<AppInstNodeInfo>().Where(a => groupIds.Contains(a.GroupId) && a.NodeState == 1);
            //var queryoption = Db.Set<AppInstOpin>().AsNoTracking().Where(a => a.CreateUserId == currUserId);
            //var query0 = from n in querynode
            //             join g in queryoption
            //             on n.InstId equals g.InstId

            //             select new
            //             {
            //                 g.InstId
            //                ,
            //                 g.NodeStrId
            //             };
            //var apparr = query0.ToList();
            //var arraynodes = querynode.ToList();
            //IList<int> tempIds = new List<int>();
            //foreach (var item in arraynodes)
            //{
            //    if (!apparr.Any(a => a.InstId == item.InstId && a.NodeStrId == item.NodeStrId))
            //    {
            //        tempIds.Add(item.InstId ?? 0);
            //    }

            //}

            //return tempIds.Distinct().Count();

            var currnodeIds = Db.Set<AppInst>()
               .Where(a => a.AppState == (int)AppInstEnum.AppState0)
               .Select(a => a.CurrentNodeId).Distinct().ToList();

            var spcount = Db.Set<AppGroupUser>()
                .Where(a => a.UserId == currUserId
                && currnodeIds.Contains(a.NodeId ?? 0) && a.UserIsSp == 0)
                .Select(a => a.InstId).ToList().Distinct().Count();


            return spcount;



        }
        /// <summary>
        /// 被打回
        /// </summary>
        /// <param name="currUserId">当前登录用户</param>
        /// <returns></returns>
        private int BackNumber(int currUserId)
        {
            return this.Db.Set<AppInst>().Where(a => a.StartUserId == currUserId && a.AppState == 3 && (a.NewInstId ?? 0) <= 0).Count();





        }
        /// <summary>
        /// 提醒
        /// </summary>
        /// <returns></returns>
        public ConMesgInfo GetConsoleMsgNumber(int currUserId)
        {
            var msgInfo = new ConMesgInfo();
            msgInfo.PedingNum = GetPaddingMsgNumber(currUserId).ToString();
            msgInfo.BeiDahuiNum = BackNumber(currUserId).ToString();
            return msgInfo;
        }
        #endregion

        #region 我审批列表

        /// <summary>
        /// 我审批数据（通过-打回）
        /// </summary>
        /// <typeparam name="s"></typeparam>
        /// <param name="pageInfo"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderbyLambda"></param>
        /// <param name="isAsc"></param>
        /// <param name="wosp">1：已通过，0：被打回</param>
        /// <returns></returns>
        public LayPageInfo<AppProcessedListDTO> GetWoShenPiList<s>(PageInfo<AppInst> pageInfo, int sessionUserId, Expression<Func<AppInst, bool>> whereLambda, Expression<Func<AppInst, s>> orderbyLambda, bool isAsc, int wosp)
        {
            //var predicateAnd = PredicateBuilder.True<AppInst>();
            //predicateAnd = predicateAnd.And(whereLambda);

            var tempquery = _AppInstSet.AsTracking().Where(whereLambda.Compile());
            var queryoption = Db.Set<AppInstOpin>().AsNoTracking().Where(a=>a.CreateUserId== sessionUserId);
            if (wosp == 0)
            {//被打回查询
                queryoption = queryoption.Where(a=>a.Result==5);
            }else if (wosp==1)
            {
                queryoption = queryoption.Where(a => a.Result == 2||a.Result==4);
            }
            
            var query = from a in tempquery
                        join
                        b in queryoption
                        on a.Id equals b.InstId
                        select new
                        {
                            Id = a.Id,
                            ObjType = a.ObjType,
                            AppObjId = a.AppObjId,
                            AppObjName = a.AppObjName,
                            AppObjNo = a.AppObjNo,
                            AppObjAmount = a.AppObjAmount,
                            Mission = a.Mission,
                            CurrentNodeId = a.CurrentNodeId,
                            CurrentNodeName = a.CurrentNodeName,
                            StartDateTime = a.StartDateTime,
                            AppState = a.AppState,
                            StartUserId = a.StartUserId,
                            Option = b.Opinion,
                            OptionDate = b.CreateDatetime,
                            FinceType = a.FinceType,
                            AppSecObjId = a.AppSecObjId,




                        };
            //pageInfo.TotalCount = query.Count();
            pageInfo.TotalCount = query.Count();
            var query1 = query.OrderByDescending(a => a.Id).Skip((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take(pageInfo.PageSize);
            var local = from a in query1.AsEnumerable()
                        select new AppProcessedListDTO
                        {
                            Id = a.Id,
                            ObjType = a.ObjType,
                            ObjTypeDic = EmunUtility.GetDesc(typeof(FlowObjEnums), a.ObjType),
                            AppObjId = a.AppObjId,
                            AppObjName = a.AppObjName,
                            AppObjNo = a.AppObjNo,
                            Mission = a.Mission,
                            MissionDic = FlowUtility.GetMessionDic(a.Mission ?? -1, a.ObjType),//审批事项
                            CurrentNodeId = a.CurrentNodeId,
                            CurrentNodeName = a.CurrentNodeName,
                            StartDateTime = a.StartDateTime,
                            StartUserName = RedisValueUtility.GetUserShowName(a.StartUserId ?? 0),
                            AppState = a.AppState,
                            AppStateDic = EmunUtility.GetDesc(typeof(AppInstEnum), a.AppState),
                            AppObjAmount = a.AppObjAmount ?? 0,
                            AppObjAmountThod = a.AppObjAmount.ThousandsSeparator(),
                            Option = a.Option,
                            OptionDate = a.OptionDate,
                            FinceType = a.FinceType,
                            AppSecObjId = a.AppSecObjId,


                        };
            return new LayPageInfo<AppProcessedListDTO>()
            {
                data = local.ToList(),
                count = pageInfo.TotalCount,
                code = 0


            };
        }
        #endregion

        /// <summary>
        /// 用于提交流程后删除自选节点
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Fgld"></param>
        /// <param name="Ksyj"></param>
        /// <returns></returns>
        public int DEMb(int id, string Fgld, string Ksyj) 
        {
            var idse1 = "";
            var idse2 = "";
            try
            {
                 idse1 = Db.Set<FlowTempNode>().Where(a => a.TempId == id && (a.Name == Ksyj)).FirstOrDefault().StrId;
                 idse2 = Db.Set<FlowTempNode>().Where(a => a.TempId == id && (a.Name == Fgld)).FirstOrDefault().StrId;
                StringBuilder strsql = new StringBuilder();
                // strsql.Append($"upd FlowTempNodeInfo  where NodeStrId in('{idse1}','{idse2}') ");
                strsql.Append($"update FlowTempNodeInfo set GroupId='' where NodeStrId in({idse1},{idse2}) ");
                //strsql.Append($"delete FlowTempNodeInfo  where NodeStrId in('{idse1}','{idse2}') ");
                ExecuteSqlCommand(strsql.ToString());
                Log4netHelper.Info($"删除节点信息：" + strsql);
            }
            catch (Exception ex)
            {
                  Log4netHelper.Error("删除节点信息:"+id +":"+Fgld+":"+Ksyj);

                return 0;
            }

            return 0;
          
          


        }

    }
}
