using Microsoft.EntityFrameworkCore;
using NF.Common.Extend;
using NF.Common.Utility;
using NF.Model.Models;
using NF.ViewModel;
using NF.ViewModel.Extend.Enums;
using NF.ViewModel.Models;
using NF.ViewModel.Models.Common;
using NF.ViewModel.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NF.AutoMapper;
using NF.ViewModel.Models.Finance.Enums;

namespace NF.BLL
{
    /// <summary>
    /// 合同操作
    /// </summary>
    public partial class ContractInfoService
    {

        /// <summary>
        /// 校验某一字段值是否已经存在
        /// </summary>
        /// <param name="fieldInfo">字段相关信息</param>
        /// <returns>True:存在/False不存在</returns>
        public bool CheckInputValExist(UniqueFieldInfo fieldInfo)
        {
            var predicateAnd = PredicateBuilder.True<ContractInfo>();
            //不等于fieldInfo.CurrId是排除修改的时候
            predicateAnd = predicateAnd.And(a => a.IsDelete == 0 && a.Id != fieldInfo.Id);
            switch (fieldInfo.FieldName)
            {
                case "Name":
                    predicateAnd = predicateAnd.And(a => a.Name == fieldInfo.FieldValue);
                    break;
                case "Code":
                    predicateAnd = predicateAnd.And(a => a.Code == fieldInfo.FieldValue);
                    break;

            }
            return GetQueryable(predicateAnd).AsNoTracking().Any();

        }
        /// <summary>
        /// 保存合同信息
        /// </summary>
        /// <param name="contractInfo">合同信息</param>
        /// <param name="contractInfoHistory">合同历史信息</param>
        /// <returns>Id:合同ID、Hid:历史ID</returns>
        public Dictionary<string, int> AddSave(ContractInfo contractInfo, ContractInfoHistory contractInfoHistory)
        {
            contractInfo.ModificationTimes = 0;//变更次数
            contractInfoHistory.ModificationTimes = 0;//默认值
            var inof = Add(contractInfo);
            CreateHistroy(contractInfoHistory, inof);
            return ResultContIds(contractInfo, contractInfoHistory);
        }
        /// <summary>
        /// 创建合同历史数据及修改
        /// </summary>
        /// <param name="contractInfoHistory">历史合同对象</param>
        /// <param name="inof">当前合同对方</param>
        private void CreateHistroy(ContractInfoHistory contractInfoHistory, ContractInfo inof)
        {
            EventUtility eventUtility = new EventUtility();
            eventUtility.ContHistoryEvent += CreateContHistroy;
            eventUtility.ContHistoryEvent += UpdateItems;
            eventUtility.ContHistoryEvent += CreateContHistoryData;
            eventUtility.ExceContHistoryEvent(inof, contractInfoHistory);
        }

        /// <summary>
        /// 创建合同历史
        /// </summary>
        /// <param name="contractInfoHistory">合同历史</param>
        /// <param name="inof">合同对象</param>
        private void CreateContHistroy(ContractInfo inof, ContractInfoHistory contractInfoHistory)
        {
            contractInfoHistory.ContId = inof.Id;
            contractInfoHistory.ModifyDateTime = DateTime.Now;
            contractInfoHistory.ModifyUserId = inof.ModifyUserId;
            contractInfoHistory.ContId = inof.Id;
            Db.Set<ContractInfoHistory>().Add(contractInfoHistory);
            Db.SaveChanges();
        }
        #region 创建和修改标签历史数据
        /// <summary>
        /// 变更创建合同历史
        /// </summary>
        /// <param name="contId">合同ID</param>
        /// <param name="contHisId">合同历史ID</param>
        private void CreateContHistoryData(ContractInfo contractInfo, ContractInfoHistory contractInfoHistory)
        {
            //计划资金
            var listplanFinances = Db.Set<ContPlanFinance>().AsNoTracking().Where(a => a.ContId == contractInfo.Id).ToList();
            foreach (var plance in listplanFinances)
            {
                var infoHi = plance.ToModel<ContPlanFinance, ContPlanFinanceHistory>();
                infoHi.PlanFinanceId = plance.Id;
                infoHi.ContHisId = contractInfoHistory.Id;
                Db.Set<ContPlanFinanceHistory>().Add(infoHi);
            }
            //标的历史
            var listSubmatter = Db.Set<ContSubjectMatter>().AsNoTracking().Where(a => a.ContId == contractInfo.Id).ToList();
            foreach (var subjectMatter in listSubmatter)
            {
                var infoHi = subjectMatter.ToModel<ContSubjectMatter, ContSubjectMatterHistory>();
                infoHi.SubjId = subjectMatter.Id;
                infoHi.ContHisId = contractInfoHistory.Id;
                Db.Set<ContSubjectMatterHistory>().Add(infoHi);
            }
            //合同文本
            var listconttext = Db.Set<ContText>().AsNoTracking().Where(a => a.ContId == contractInfo.Id).ToList();
            foreach (var contText in listconttext)
            {
                var infoHi = contText.ToModel<ContText, ContTextHistory>();
                infoHi.ContTxtId = contText.Id;
                infoHi.ContHisId = contractInfoHistory.Id;
                Db.Set<ContTextHistory>().Add(infoHi);
            }
            Db.SaveChanges();

        }
        /// <summary>
        /// 创建和修改历史数据
        /// </summary>
        /// <param name="contId">合同ID</param>
        /// <param name="contHisId">合同历史ID</param>
        private void UpdateContHistoryData(ContractInfo contractInfo, ContractInfoHistory contractInfoHistory)
        {
            var listplanFinances = Db.Set<ContPlanFinance>().AsNoTracking().Where(a => a.ContId == contractInfo.Id).ToList();
            var listhisplanfinances = Db.Set<ContPlanFinanceHistory>().AsNoTracking().Where(a => a.ContId == contractInfo.Id).ToList();
            foreach (var plance in listplanFinances)
            {
                var hiinfo = plance.ToModel<ContPlanFinance, ContPlanFinanceHistory>();
                var hisInfo = listhisplanfinances.Where(a => a.PlanFinanceId == plance.Id).OrderByDescending(a => a.Id).FirstOrDefault();
                if (hisInfo != null)
                {
                    hiinfo.Id = hisInfo.Id;
                    hiinfo.PlanFinanceId = plance.Id;
                    hiinfo.ContHisId = contractInfoHistory.Id;
                    Db.Entry<ContPlanFinanceHistory>(hiinfo).State = EntityState.Modified;
                }
                else
                {//如果没有就新建
                    hiinfo.PlanFinanceId = plance.Id;
                    hiinfo.ContHisId = contractInfoHistory.Id;
                    Db.Set<ContPlanFinanceHistory>().Add(hiinfo);

                }
            }

            Db.SaveChanges();
        }
        #endregion

        /// <summary>
        /// 合同历史相关信息
        /// </summary>
        private void CreateContHistoryRedis(int contId, int contHisId)
        {
            MappContToHistory contToHistory = new MappContToHistory
            {
                ContId = contId,
                ContHisId = contHisId

            };
            RedisHelper.ListRightPush(StaticData.AddContHistory, contToHistory);
        }
        /// <summary>
        /// 修改合同信息
        /// </summary>
        /// <param name="contractInfo">合同信息</param>
        /// <param name="contractInfoHistory">合同历史信息</param>
        /// <returns>Id:合同ID、Hid:历史ID</returns>
        public Dictionary<string, int> UpdateSave(ContractInfo contractInfo, ContractInfoHistory contractInfoHistory)
        {
            var inof = Update(contractInfo);
            //保存历史表
            EventUtility eventUtility = new EventUtility();
            eventUtility.ContHistoryEvent += UpdateContHisttory;
            eventUtility.ContHistoryEvent += UpdateItems;
            eventUtility.ContHistoryEvent += UpdateContHistoryData;
            eventUtility.ExceContHistoryEvent(contractInfo, contractInfoHistory);
            return ResultContIds(contractInfo, contractInfoHistory);
        }
        /// <summary>
        /// 返回合同ID和历史合同ID
        /// </summary>
        /// <param name="contractInfo">合同</param>
        /// <param name="contractInfoHistory">历史合同</param>
        /// <returns></returns>
        private Dictionary<string, int> ResultContIds(ContractInfo contractInfo, ContractInfoHistory contractInfoHistory)
        {
            var dic = new Dictionary<string, int>();
            dic.Add("Id", contractInfo.Id);
            dic.Add("Hid", contractInfoHistory.Id);
            return dic;
        }

        /// <summary>
        /// 变更
        /// </summary>
        /// <param name="contractInfo">合同信息</param>
        /// <param name="contractInfoHistory">合同历史信息</param>
        /// <returns>Id:合同ID、Hid:历史ID</returns>
        public Dictionary<string, int> ChangeSave(ContractInfo contractInfo, ContractInfoHistory contractInfoHistory)
        {
            contractInfo.ContState = 0; //未执行
            var inof = Update(contractInfo);
            //保存历史表
            contractInfoHistory.ModificationTimes = contractInfo.ModificationTimes;//变更次数
            CreateHistroy(contractInfoHistory, contractInfo);
            return ResultContIds(contractInfo, contractInfoHistory);
        }
        /// <summary>
        /// 修改历史
        /// </summary>
        /// <param name="infoHistory">修改历史合同</param>
        /// <returns></returns>
        private void UpdateContHisttory(ContractInfo contractInfo, ContractInfoHistory infoHistory)
        {
            infoHistory.ContId = contractInfo.Id;
            infoHistory.Id = contractInfo.ContHid ?? 0;
            Db.Entry<ContractInfoHistory>(infoHistory).State = EntityState.Modified;
            Db.SaveChanges();
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="s">排序字段</typeparam>
        /// <param name="pageInfo">分页对象</param>
        /// <param name="whereLambda">Where条件表达式</param>
        /// <param name="orderbyLambda">排序表达式</param>
        /// <param name="isAsc">是否正序</param>
        /// <returns></returns>
        public LayPageInfo<ContractInfoListViewDTO> GetList<s>(PageInfo<ContractInfo> pageInfo, Expression<Func<ContractInfo, bool>> whereLambda,
             Expression<Func<ContractInfo, s>> orderbyLambda, bool isAsc)
        {
            var tempquery = _ContractInfoSet
                .Include(a => a.Project)
                .Include(a => a.Comp)
                .Include(a => a.ContStatic)
                .Include(a => a.CreateUser)
                 .AsTracking()
                .Where<ContractInfo>(whereLambda.Compile()).AsQueryable();
            pageInfo.TotalCount = tempquery.Count();
            if (isAsc)
            {
                tempquery = tempquery.OrderBy(orderbyLambda);
            }
            else
            {
                tempquery = tempquery.OrderByDescending(orderbyLambda);
            }
            if (!(pageInfo is NoPageInfo<ContractInfo>))
            { //分页
                tempquery = tempquery.Skip<ContractInfo>((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take<ContractInfo>(pageInfo.PageSize);
            }

            var query = from a in tempquery
                        select new
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Code = a.Code,
                            OtherCode = a.OtherCode,
                            ContSourceId = a.ContSourceId,
                            ContTypeId = a.ContTypeId,
                            IsFramework = a.IsFramework,
                            ContDivision = a.ContDivision,
                            CompId = a.CompId,
                            CompName = a.Comp == null ? "" : a.Comp.Name,
                            BankName = a.Comp == null ? "" : a.Comp.BankName,
                            BankAccount = a.Comp == null ? "" : a.Comp.BankAccount,
                            ProjName = a.Project == null ? "" : a.Project.Name,
                            AmountMoney = a.AmountMoney,
                            CurrencyId = a.CurrencyId,
                            CurrencyRate = a.CurrencyRate,
                            EstimateAmount = a.EstimateAmount,
                            AdvanceAmount = a.AdvanceAmount,
                            StampTax = a.StampTax,
                            CreateUserId = a.CreateUserId,
                            CreateDateTime = a.CreateDateTime,
                            SngnDateTime = a.SngnDateTime,
                            EffectiveDateTime = a.EffectiveDateTime,
                            PlanCompleteDateTime = a.PlanCompleteDateTime,
                            ActualCompleteDateTime = a.ActualCompleteDateTime,
                            DeptId = a.DeptId,
                            ProjectId = a.ProjectId,
                            ContState = a.ContState,
                            MainDeptId = a.MainDeptId,
                            Reserve1 = a.Reserve1,
                            Reserve2 = a.Reserve2,
                            PrincipalUserId = a.PrincipalUserId,
                            SumContId = a.SumContId,//总包合同ID
                            ModificationTimes = a.ModificationTimes,//变更次数
                            WfState = a.WfState,
                            WfCurrNodeName = a.WfCurrNodeName,
                            WfItem = a.WfItem,
                            CompInAm = a.ContStatic == null ? 0 : a.ContStatic.CompInAm,//已确认发票
                            CompActAm = a.ContStatic == null ? 0 : a.ContStatic.CompActAm,//已确实际资金
                            CompRatio = a.ContStatic == null ? 0 : a.ContStatic.CompRatio,//完成比例
                            BalaTick = a.ContStatic == null ? 0 : a.ContStatic.BalaTick,//票款差
                            // Zbid = a.Zbid,
                            //zb = a.Zb == null ? "" : a.Zb.Project.Name,
                            ////zb = a.Zb.Project.Name,
                            //// Xjid = a.Xjid,
                            //xj = a.Xj == null ? "" : a.Xj.ProjectNameNavigation.Name,
                            ////  Ytid = a.Ytid,
                            //yt = a.Yt == null ? "" : a.Yt.ProjectNameNavigation.Name,
                            // yt =a.Yt.ProjectNameNavigation.Name,
                            ContSingNo = a.ContSingNo,//签约人身份证号
                            FinanceType = a.FinanceType,
                            HtXmnr=a.HtXmnr

                            // SumContName=a.SumCont.Name,

                        };
            var local = from a in query.AsEnumerable()
                        select new ContractInfoListViewDTO
                        {
                            Id = a.Id,
                            Name = a.Name,//名称
                            Code = a.Code,//编号
                            OtherCode = a.OtherCode,//合同对方编号
                            ContTypeId = a.ContTypeId,//合同类别ID
                            ContTypeName = DataDicUtility.GetDicValueToRedis(a.ContTypeId ?? 0, DataDictionaryEnum.contractType),//合同类别
                            //合同来源
                            ContSName = DataDicUtility.GetDicValueToRedis(a.ContSourceId ?? 0, DataDictionaryEnum.contSource),
                            CompId = a.CompId,
                            CompName = a.CompName,//合同对方
                            BankName = a.BankName,
                            BankAccount = a.BankAccount,
                            ProjName = a.ProjName,//项目名称
                            ContPro = EmunUtility.GetDesc(typeof(ContractProperty), (a.IsFramework ?? 0)),//合同属性
                            ContSum = (a.ContDivision ?? 0) > 0 ? "是" : "否",
                            AmountMoney = a.AmountMoney ?? 0,//合同金额
                            ContAmThod = a.AmountMoney.ThousandsSeparator(),//合同金额千分位
                            ContAmRmbThod = ((a.AmountMoney ?? 0) * (a.CurrencyRate ?? 1)).ThousandsSeparator(),//折合本币
                            ContAmRmb = (a.AmountMoney ?? 0) * (a.CurrencyRate ?? 1),//折合本币
                            CurrName = RedisValueUtility.GetCurrencyName(a.CurrencyId, fileName: "Name"),//币种
                            Rate = a.CurrencyRate ?? 1,//汇率
                            CurrencyId = a.CurrencyId,
                            EsAmountThod = (a.EstimateAmount ?? 0).ThousandsSeparator(),//预估金额
                            AdvAmountThod = (a.AdvanceAmount ?? 0).ThousandsSeparator(),//预收预付
                            StampTax = (a.StampTax ?? 0).ThousandsSeparator(),//千分位
                            CreateUserName = RedisValueUtility.GetUserShowName(a.CreateUserId), //创建人
                            CreateDateTime = a.CreateDateTime,//创建时间
                            SngDate = a.SngnDateTime,//签订时间
                            EfDate = a.EffectiveDateTime,//生效日期
                            PlanDate = a.PlanCompleteDateTime,//计划完成时间
                            ActDate = a.ActualCompleteDateTime,//实际完成时间
                            DeptName = RedisValueUtility.GetDeptName(a.DeptId ?? -2),//经办机构
                            DeptId = a.DeptId,//经办机构
                            MdeptName = RedisValueUtility.GetDeptName(a.MainDeptId ?? -2), //签约主体
                            MainDeptId = a.MainDeptId,
                            ContState = a.ContState,
                            ContStateDic = EmunUtility.GetDesc(typeof(ContractState), a.ContState),
                            Reserve1 = a.Reserve1,//备注1
                            Reserve2 = a.Reserve2,//备注2
                            ModificationTimes = a.ModificationTimes ?? 0,//变更次数
                            PrincUserName = RedisValueUtility.GetUserShowName(a.PrincipalUserId ?? -1), //负责人
                            WfState = a.WfState,
                            WfCurrNodeName = a.WfCurrNodeName,
                            WfItemDic = FlowUtility.GetMessionDic(a.WfItem ?? -1, 0),
                            WfStateDic = EmunUtility.GetDesc(typeof(WfStateEnum), a.WfState ?? -1),
                            //ZbName =a.zb,// ZbName(a.Zbid),
                            // XjName =a.xj,// XjName(a.Xjid),
                            // YtName =a.yt,// YtName(a.Ytid),
                            ContSingNo = a.ContSingNo,//签约人身份证号
                            FinanceType = a.FinanceType,
                            CompInAmThod = a.CompInAm.ThousandsSeparator(),//已确认发票
                            CompActAmThod = a.CompActAm.ThousandsSeparator(),//已确实际资金
                            CompRatioStr = a.CompRatio.ConvertToPercent(),//完成比例
                            BalaTickThod = a.BalaTick.ThousandsSeparator(),//票款差
                            CompInAm = a.CompInAm ?? 0,//发票已确认
                            CompActAm = a.CompActAm ?? 0,//实际资金已确认
                            HtXmnr = a.HtXmnr
                        };
            return new LayPageInfo<ContractInfoListViewDTO>()
            {
                data = local.ToList(),
                count = pageInfo.TotalCount,
                code = 0
            };
        }

        public string ZbName(int? id)
        {
            var zbname = "";
            try
            {
                zbname = this.Db.Set<TenderInfor>().Include(a => a.Project).AsEnumerable().Where(a => a.Id == id).FirstOrDefault().Project.Name;
            }
            catch (Exception)
            {

                return zbname;
            }


            return zbname;
        }
        public string XjName(int? id)
        {
            var zbname = "";
            try
            {
                zbname = this.Db.Set<Inquiry>().Include(a => a.ProjectNameNavigation).AsEnumerable().Where(a => a.Id == id).FirstOrDefault().ProjectNameNavigation.Name;
                //zbname = this.Db.Set<Inquiry>().AsEnumerable().Where(a => a.Id == id).FirstOrDefault().ProjectNameNavigation.Name;
            }
            catch (Exception)
            {

                return zbname;
            }

            return zbname;
        }
        public string YtName(int? id)
        {
            var zbname = "";
            try
            {
                zbname = this.Db.Set<Questioning>().Include(a => a.ProjectNameNavigation).AsEnumerable().Where(a => a.Id == id).FirstOrDefault().ProjectNameNavigation.Name;
                // zbname = this.Db.Set<Questioning>().AsEnumerable().Where(a => a.Id == id).FirstOrDefault().ProjectNameNavigation.Name;
            }
            catch (Exception)
            {

                return zbname;
            }

            return zbname;
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="Ids">需要删除的Ids集合</param>
        /// <returns>受影响行数</returns>
        //public int Delete(string Ids)
        //{
        //    string sqlstr = "update ContractInfo set IsDelete=1 where Id in(" + Ids + ")";
        //    return ExecuteSqlCommand(sqlstr);
        //}

        public int Delete(string Ids, int i)
        {
            StringBuilder strsql = new StringBuilder();


            if (i == 0)
            {
                strsql.Append($"update ContractInfo set IsDelete = 1 where Id in (" + Ids + ")");
            }
            else if (i == 1)
            {
              var s=  Bdms(Ids);
                strsql.Append($"update ContractInfo set IsDelete = 1 where Id in (" + Ids + ");");
                //strsql.Append($"update ContractInfoHistory set IsDelete = 1 where ContId in (" + Ids + ");");
                //strsql.Append($"update ContInvoice set IsDelete = 1 where ContId in (" + Ids + ");");//发票
                //strsql.Append($"update ContActualFinance  set IsDelete = 1 where ContId in (" + Ids + ");");//是实际资金
                //strsql.Append($"update ContractInfoHistory set IsDelete = 1 where ContId in (" + Ids + ");");
                strsql.Append($"update ContDescription set IsDelete = 1  where ContId in (" + Ids + ");");//合同备忘
                strsql.Append($"update ContPlanFinance set IsDelete = 1  where ContId in (" + Ids + ");");//计划资金
                strsql.Append($"update ContPlanFinanceHistory set IsDelete = 1  where ContId in (" + Ids + ");");//计划资金历史
                strsql.Append($"update ContAttachment set IsDelete = 1  where ContId in (" + Ids + ");");//附件
                strsql.Append($"update ContText set IsDelete = 1 where  ContId in (" + Ids + ");");//合同文本
                strsql.Append($"update ContTextHistory set IsDelete = 1  where ContId in (" + Ids + ");");//合同文本历史
                strsql.Append($"update ContSubjectMatter set IsDelete = 1  where  ContId in (" + Ids + ");");
                strsql.Append($"update ContSubjectMatterHistory set IsDelete = 1   where ContId in (" + Ids + ") or ContHisId in (" + Ids + ");");
                if (s!="")
                {
                    strsql.Append($"update ContSubDe set IsDelete = 1  where  SubId in (" + s + ");"); //标的明细
                }
               
                strsql.Append($"delete ContConsult  where ContId=(" + Ids + ");");//合同查阅人

            }

            return ExecuteSqlCommand(strsql.ToString());
        }

        public string  Bdms(string ids) {
            var s = "";
            var listIds = StringHelper.String2ArrayInt(ids);
            var d = Db.Set<ContSubjectMatter>().Where(a => listIds.Contains(a.ContId ?? 0));
            foreach (var item in d)
            {

                if (s=="")
                {
                    s= "" + item.Id;
                }
                else
                {
                    s =s+ "," + item.Id;
                }
               
            }

            return s;
        }
        public DELETElist GetIsFpt(string Ids)
        {
            DELETElist u = new DELETElist();

            var listIds = StringHelper.String2ArrayInt(Ids);
            var nums = Db.Set<ContInvoice>().Where(a => listIds.Contains(a.ContId ?? 0) && a.IsDelete == 0).Count();
            u.Num = nums;
            u.DateName = "发票";
            return u;
        }


        public DELETElist GetIsSjzj(string Ids)
        {
            DELETElist u = new DELETElist();

            var listIds = StringHelper.String2ArrayInt(Ids);
            var nums = Db.Set<ContActualFinance>().Where(a => listIds.Contains(a.ContId ?? 0) && a.IsDelete == 0).Count();
            u.Num = nums;
            u.DateName = "实际资金";
            return u;
        }

        /// <summary>
        /// 查看信息
        /// </summary>
        /// <param name="Id">当前ID</param>
        /// <returns></returns>
        public ContractInfoViewDTO ShowView(int Id)
        {
            var query = from a in _ContractInfoSet.Include(a => a.Comp)
                        .Include(a => a.Project).Include(a => a.CompId3Navigation)
                        .Include(a => a.Zb).Include(a => a.Xj).Include(a => a.Yt)
                        .Include(a => a.CompId4Navigation).Include(a => a.SumCont)
                        .AsNoTracking()
                        where a.Id == Id
                        select new
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Code = a.Code,
                            OtherCode = a.OtherCode,
                            ContSourceId = a.ContSourceId,
                            ContTypeId = a.ContTypeId,
                            IsFramework = a.IsFramework,
                            ContDivision = a.ContDivision,
                            CompId = a.CompId,
                            Ctype = a.Comp == null ? 0 : a.Comp.Ctype,
                            CompName = a.Comp == null ? "" : a.Comp.Name,
                            ProjName = a.Project == null ? "" : a.Project.Name,
                            AmountMoney = a.AmountMoney,
                            CurrencyId = a.CurrencyId,
                            CurrencyRate = a.CurrencyRate,
                            EstimateAmount = a.EstimateAmount,
                            AdvanceAmount = a.AdvanceAmount,
                            StampTax = a.StampTax,
                            CreateUserId = a.CreateUserId,
                            CreateDateTime = a.CreateDateTime,
                            SngnDateTime = a.SngnDateTime,
                            EffectiveDateTime = a.EffectiveDateTime,
                            PlanCompleteDateTime = a.PlanCompleteDateTime,
                            ActualCompleteDateTime = a.ActualCompleteDateTime,
                            DeptId = a.DeptId,
                            ProjectId = a.ProjectId,
                            ContState = a.ContState,
                            MainDeptId = a.MainDeptId,
                            Reserve1 = a.Reserve1,
                            Reserve2 = a.Reserve2,
                            Comp3Name = a.CompId3Navigation == null ? "" : a.CompId3Navigation.Name,
                            Comp4Name = a.CompId4Navigation == null ? "" : a.CompId4Navigation.Name,
                            CompId3 = a.CompId3,
                            CompId4 = a.CompId4,
                            PrincipalUserId = a.PrincipalUserId,
                            FinanceTerms = a.FinanceTerms,
                            PerformanceDateTime = a.PerformanceDateTime,
                            SumContName = a.SumCont == null ? "" : a.SumCont.Name,//总包合同
                            SumContId = a.SumContId,
                            //htwcje = GetHtWcJe(a.Id),//实际资金已确认
                            //fpje=GetFpJe(a.Id),//发票已确认金额
                            // Zbid = a.Zbid,
                            zb = a.Zb.Project.Name,
                            // Xjid = a.Xjid,
                            xj = a.Xj.ProjectNameNavigation.Name,
                            //  Ytid = a.Ytid,
                            yt = a.Yt.ProjectNameNavigation.Name,
                            ContSingNo = a.ContSingNo,//签约人身份证号
                            FinanceType = a.FinanceType,
                            HtXmnr = a.HtXmnr
                        };
            var local = from a in query.AsEnumerable()
                        select new ContractInfoViewDTO
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Code = a.Code,
                            OtherCode = a.OtherCode,
                            ContSourceId = a.ContSourceId,
                            ContTypeId = a.ContTypeId,
                            IsFramework = a.IsFramework,
                            //如果不是String.修改就得手动绑定Radio
                            ContDivision = (a.ContDivision ?? 0).ToString(),
                            CompId = a.CompId,
                            AmountMoney = a.AmountMoney,
                            CurrencyId = a.CurrencyId,
                            CurrencyRate = a.CurrencyRate,
                            EstimateAmount = a.EstimateAmount,
                            AdvanceAmount = a.AdvanceAmount,
                            StampTax = a.StampTax,
                            CreateUserId = a.CreateUserId,
                            CreateDateTime = a.CreateDateTime,
                            SngnDateTime = a.SngnDateTime,
                            EffectiveDateTime = a.EffectiveDateTime,
                            PlanCompleteDateTime = a.PlanCompleteDateTime,
                            ActualCompleteDateTime = a.ActualCompleteDateTime,
                            DeptId = a.DeptId,
                            ProjectId = a.ProjectId,
                            ContState = a.ContState,
                            MainDeptId = a.MainDeptId,
                            Reserve1 = a.Reserve1,
                            Reserve2 = a.Reserve2,
                            //合同类别
                            ContTypeName = DataDicUtility.GetDicValueToRedis(a.ContTypeId, DataDictionaryEnum.contractType),//合同类别
                            //合同来源
                            ContSName = DataDicUtility.GetDicValueToRedis(a.ContSourceId, DataDictionaryEnum.contSource),
                            CompName = a.CompName,//合同对方
                            ProjName = a.ProjName,//项目名称
                            ContPro = EmunUtility.GetDesc(typeof(ContractProperty), a.IsFramework ?? 0),//合同属性
                            ContSum = a.ContDivision > 0 ? "是" : "否",
                            ContAmThod = a.AmountMoney.ThousandsSeparator(),//合同金额千分位
                            CurrencyName = RedisValueUtility.GetCurrencyName(a.CurrencyId, fileName: "Name"), ///币种
                            Rate = a.CurrencyRate ?? 1,//汇率
                            EsAmountThod = a.EstimateAmount.ThousandsSeparator(),//预估金额
                            AdvAmountThod = a.AdvanceAmount.ThousandsSeparator(),//预收预付
                            StampTaxThod = a.StampTax.ThousandsSeparator(),//千分位
                            CreateUserName = RedisValueUtility.GetUserShowName(a.CreateUserId),//创建人
                            PrincipalUserName = RedisValueUtility.GetUserShowName(a.PrincipalUserId ?? 0),//负责人
                            DeptName = RedisValueUtility.GetDeptName(a.DeptId ?? -2),
                            MdeptName = RedisValueUtility.GetDeptName(a.MainDeptId ?? -2),
                            StateDic = EmunUtility.GetDesc(typeof(ContractState), a.ContState),
                            Comp3Name = a.Comp3Name,
                            Comp4Name = a.Comp4Name,
                            CompId3 = a.CompId3,
                            CompId4 = a.CompId4,
                            FinanceTerms = a.FinanceTerms,//资金条款
                            PerformanceDateTime = a.PerformanceDateTime,
                            SumContName = a.SumContName,
                            SumContId = a.SumContId,//总包ID。修改时绑定
                                                    //HtWcJeThod = GetHtWcJe(a.Id).ThousandsSeparator(),//完成金额
                                                    //HtWcBl = GetWcBl(a.AmountMoney ?? 0, GetHtWcJe(a.Id)),//完成比例
                                                    //PiaoKaunCha = (GetFpJe(a.Id) - GetHtWcJe(a.Id)).ThousandsSeparator(),//票款差额
                                                    //FaPiaoThod = GetFpJe(a.Id).ThousandsSeparator()//发票金额
                            ZbName = a.zb,// ZbName(a.Zbid),
                            XjName = a.xj,// XjName(a.Xjid),
                            YtName = a.yt,// YtName(a.Ytid),
                            ContSingNo = a.ContSingNo,//签约人身份证号
                            FinanceType = a.FinanceType,
                            Ctype = a.Ctype,// 合同对方类型id
                            HtXmnr = a.HtXmnr
                        };

            var teminfo = local.FirstOrDefault();
            if (teminfo != null)
            {
                teminfo.HtWcJeThod = GetHtWcJe(teminfo.Id).ThousandsSeparator();//完成金额
                teminfo.HtWcBl = GetWcBl(teminfo.AmountMoney ?? 0, GetHtWcJe(teminfo.Id));//完成比例
                teminfo.PiaoKaunCha = (GetFpJe(teminfo.Id) - GetHtWcJe(teminfo.Id)).ThousandsSeparator();//票款差额
                teminfo.FaPiaoThod = GetFpJe(teminfo.Id).ThousandsSeparator();//发票金额
            }
            return teminfo;







        }
        #region 计算字段方法
        /// <summary>
        /// 合同完成金额
        /// </summary>
        /// <param name="Id">当前合同ID</param>
        /// <returns></returns>
        private decimal GetHtWcJe(int Id)
        {
            var info = Db.Set<ContStatistic>().Where(a => a.ContId == Id).FirstOrDefault();
            if (info != null)
                return info.CompActAm ?? 0;
            return 0;

        }
        /// <summary>
        /// 合同完成金额
        /// </summary>
        /// <param name="Id">当前合同ID</param>
        /// <returns></returns>
        private decimal GetFpJe(int Id)
        {
            var info = Db.Set<ContStatistic>().Where(a => a.ContId == Id).FirstOrDefault();
            if (info != null)
                return info.CompInAm ?? 0;
            return 0;

        }
        /// <summary>
        /// 完成比例
        /// </summary>
        /// <returns></returns>
        private string GetWcBl(decimal htje, decimal wcje)
        {
            return ((htje == 0 || wcje == 0) ? 0 : (wcje / htje)).ConvertToPercent();


        }



        #endregion



        /// <summary>
        /// 修改当前对应标签下的-UserId数据
        /// </summary>
        /// <param name="Id">当前合同ID</param>
        /// <param name="HisId">合同历史ID</param>
        public void UpdateItems(ContractInfo contInfo, ContractInfoHistory infoHistory)
        {
            StringBuilder strsql = new StringBuilder();
            var currUserId = contInfo.ModifyUserId;
            strsql.Append($"update ContractInfo set ContHid={infoHistory.Id} where Id={contInfo.Id};");
            strsql.Append($"update ContPlanFinance set Ftype={contInfo.FinanceType},ContId={contInfo.Id},CurrencyId={contInfo.CurrencyId},CurrencyRate={contInfo.CurrencyRate} where ContId={contInfo.Id} or ContId={-currUserId};");
            strsql.Append($"update ContPlanFinanceHistory set ContId={contInfo.Id} where ContId={-currUserId};");
            strsql.Append($"update ContDescription set ContId={contInfo.Id} where ContId={-currUserId};");
            strsql.Append($"update ContAttachment set ContId={contInfo.Id} where ContId={-currUserId};");
            strsql.Append($"update ContText set ContId={contInfo.Id} where ContId={-currUserId};");
            strsql.Append($"update ContTextHistory set ContId={contInfo.Id},ContHisId={infoHistory.Id} where ContId={-currUserId};");
            strsql.Append($"update ContSubjectMatter set ContId={contInfo.Id} where ContId={-currUserId};");
            strsql.Append($"update ContSubjectMatterHistory set ContId={contInfo.Id},ContHisId={infoHistory.Id} where ContId={-currUserId} or ContHisId={-currUserId};");
            ExecuteSqlCommand(strsql.ToString());

        }
        /// <summary>
        /// 清除标签垃圾数据
        /// </summary>
        /// <param name="currUserId">当前用户ID</param>
        /// <returns></returns>
        public int ClearJunkItemData(int currUserId)
        {
            StringBuilder strsql = new StringBuilder();
            strsql.Append($"delete ContDescription  where ContId={-currUserId};");//合同备忘
            strsql.Append($"delete ContPlanFinance  where ContId={-currUserId};");//计划资金
            strsql.Append($"delete ContPlanFinanceHistory  where ContId={-currUserId};");//计划资金历史
            strsql.Append($"delete ContAttachment  where ContId={-currUserId};");//附件
            //strsql.Append($"delete ContConsult  where ContId={-currUserId};");//合同查阅人
            strsql.Append($"delete ContText  where ContId={-currUserId};");//合同文本
            strsql.Append($"delete ContTextHistory  where ContId={-currUserId};");//合同文本历史
            strsql.Append($"delete ContSubjectMatter  where ContId={-currUserId};");
            strsql.Append($"delete ContSubjectMatterHistory  where ContId={-currUserId} or ContHisId={-currUserId};");
            //添加其他标签表
            return ExecuteSqlCommand(strsql.ToString());
        }
        /// <summary>
        /// 修改字段
        /// </summary>
        /// <param name="info">修改的字段对象</param>
        /// <returns>返回受影响行数</returns>
        public int UpdateField(UpdateFieldInfo info)
        {
            string sqlstr = "";
            switch (info.FieldName)
            {

                case "OtherCode"://对方合同编号
                case "Reserve1":
                case "Reserve2":
                    sqlstr = $"update  ContractInfo set {info.FieldName}='{info.FieldValue}' where Id={info.Id}";
                    break;
                case "PrincipalUserName"://负责人
                    sqlstr = $"update  ContractInfo set PrincipalUserId={info.FieldValue} where Id={info.Id}";
                    break;
                case "PerformanceDateTime"://实际履行日期
                    sqlstr = $"update  ContractInfo set PerformanceDateTime='{info.FieldValue}' where Id={info.Id}";
                    break;
                case "SngnDateTime"://签订日期
                    sqlstr = $"update  ContractInfo set SngnDateTime='{info.FieldValue}' where Id={info.Id}";
                    break;
                case "EffectiveDateTime"://生效日期
                    sqlstr = $"update  ContractInfo set EffectiveDateTime='{info.FieldValue}' where Id={info.Id}";
                    break;
                case "ActualCompleteDateTime"://实际完成日期
                    sqlstr = $"update  ContractInfo set ActualCompleteDateTime='{info.FieldValue}' where Id={info.Id}";
                    break;
                case "PlanCompleteDateTime"://计划完成日期
                    sqlstr = $"update  ContractInfo set PlanCompleteDateTime='{info.FieldValue}' where Id={info.Id}";
                    break;

                case "ContSName"://合同来源
                    sqlstr = $"update  ContractInfo set ContSourceId={info.FieldValue} where Id={info.Id}";
                    break;
                case "ContPro"://合同属性
                    sqlstr = $"update  ContractInfo set IsFramework={info.FieldValue} where Id={info.Id}";
                    break;
                case "ContTypeName"://合同类别
                    sqlstr = $"update  ContractInfo set ContTypeId={info.FieldValue} where Id={info.Id}";
                    break;
                case "DeptName"://经办机构
                    sqlstr = $"update  ContractInfo set DeptId={info.FieldValue} where Id={info.Id}";
                    break;
                case "ProjName"://项目
                    sqlstr = $"update  ContractInfo set ProjectId={info.FieldValue} where Id={info.Id}";
                    break;

                default:
                    break;
            }
            if (!string.IsNullOrEmpty(sqlstr))
                return ExecuteSqlCommand(sqlstr);
            return 0;

        }

        /// <summary>
        /// 修改多个字段
        /// </summary>
        /// <param name="fields">当前字段集合</param>
        /// <returns>返回受影响行数</returns>
        public int UpdateField(IList<UpdateFieldInfo> fields)
        {
            StringBuilder sqlstr = new StringBuilder($"update  ContractInfo set ModifyUserId={fields[0].CurrUserId},ModifyDateTime='{DateTime.Now}'");
            foreach (var fd in fields)
            {
                switch (fd.FieldType)
                {
                    case "int":
                        sqlstr.Append($" ,{fd.FieldName}={Convert.ToInt32(fd.FieldValue)} ");
                        break;
                    case "float":
                        sqlstr.Append($" ,{fd.FieldName}={Convert.ToDouble(fd.FieldValue)} ");
                        break;
                    default:
                        var time = "";
                        if (fd.FieldValue == null)
                        {
                            time = DateTime.Now.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            time = fd.FieldValue;
                        }
                        sqlstr.Append($" ,{fd.FieldName}='{time}' ");
                        break;

                }
            }
            sqlstr.Append($"where Id={Convert.ToInt32(fields[0].Id)}");
            if (!string.IsNullOrEmpty(sqlstr.ToString()))
                return ExecuteSqlCommand(sqlstr.ToString());
            return 0;
        }
        /// <summary>
        /// 查询选择合同信息列表
        /// </summary>
        /// <param name="pageInfo">分页对象</param>
        /// <param name="whereLambda">查询条件表达式</param>
        /// <returns>返回layui所需对象</returns>
        public LayPageInfo<SelectContractInfoDTO> GetSelectList<s>(PageInfo<ContractInfo> pageInfo, Expression<Func<ContractInfo, bool>> whereLambda, Expression<Func<ContractInfo, s>> orderbyLambda, bool isAsc)
        {
            var tempquery = _ContractInfoSet.AsTracking().Where<ContractInfo>(whereLambda.Compile()).AsQueryable();
            pageInfo.TotalCount = tempquery.Count();
            if (isAsc)
            {
                tempquery = tempquery.OrderBy(orderbyLambda);
            }
            else
            {
                tempquery = tempquery.OrderByDescending(orderbyLambda);
            }
            if (!(pageInfo is NoPageInfo<ContractInfo>))
            { //分页
                tempquery = tempquery.Skip<ContractInfo>((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take<ContractInfo>(pageInfo.PageSize);
            }

            var query = from a in tempquery
                        select new
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Code = a.Code,
                            ContTypeId = a.ContTypeId,
                            CompId = a.CompId,
                            CompName = a.Comp == null ? "" : a.Comp.Name,
                            AmountMoney = a.AmountMoney,
                            CreateUserId = a.CreateUserId,
                            CreateDateTime = a.CreateDateTime,
                            ContState = a.ContState,
                            FinanceType = a.FinanceType,
                            ContSingNo = a.ContSingNo,//签约人身份证号

                        };
            var local = from a in query.AsEnumerable()
                        select new SelectContractInfoDTO
                        {
                            Id = a.Id,
                            Name = a.Name,//名称
                            Code = a.Code,//编号
                            ContTypeName = DataDicUtility.GetDicValueToRedis(a.ContTypeId, DataDictionaryEnum.contractType),//合同类别
                            ContAmThod = a.AmountMoney.ThousandsSeparator(),//合同金额千分位
                            CreateUserName = RedisValueUtility.GetUserShowName(a.CreateUserId), //创建人
                            CreateDateTime = a.CreateDateTime,//创建时间
                            ContState = a.ContState,
                            ContStateDic = EmunUtility.GetDesc(typeof(ContractState), a.ContState),
                            FinanceTypeDesc = EmunUtility.GetDesc(typeof(FinceType), a.FinanceType),
                            ContSingNo = a.ContSingNo,//签约人身份证号


                        };
            return new LayPageInfo<SelectContractInfoDTO>()
            {
                data = local.ToList(),
                count = pageInfo.TotalCount,
                code = 0


            };

        }

        public LayPageInfo<CompanyContract> GetContsByCompId<s>(PageInfo<ContractInfo> pageInfo, Expression<Func<ContractInfo, bool>> whereLambda, Expression<Func<ContractInfo, s>> orderbyLambda, bool isAsc)
        {
            var tempquery = _ContractInfoSet.AsTracking().Where<ContractInfo>(whereLambda.Compile()).AsQueryable();
            pageInfo.TotalCount = tempquery.Count();
            if (isAsc)
            {
                tempquery = tempquery.OrderBy(orderbyLambda);
            }
            else
            {
                tempquery = tempquery.OrderByDescending(orderbyLambda);
            }
            if (!(pageInfo is NoPageInfo<ContractInfo>))
            { //分页
                tempquery = tempquery.Skip<ContractInfo>((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take<ContractInfo>(pageInfo.PageSize);
            }

            var query = from a in tempquery
                        select new
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Code = a.Code,
                            ContTypeId = a.ContTypeId,
                            AmountMoney = a.AmountMoney,
                            ContState = a.ContState,
                            ContSingNo = a.ContSingNo,//签约人身份证号


                        };
            var local = from a in query.AsEnumerable()
                        select new CompanyContract
                        {
                            Id = a.Id,
                            Name = a.Name,//名称
                            Code = a.Code,//编号
                            ContTypeName = DataDicUtility.GetDicValueToRedis(a.ContTypeId, DataDictionaryEnum.contractType),//合同类别
                            ContAmThod = a.AmountMoney.ThousandsSeparator(),//合同金额千分位
                            ContStateDic = EmunUtility.GetDesc(typeof(ContractState), a.ContState),




                        };
            return new LayPageInfo<CompanyContract>()
            {
                data = local.ToList(),
                count = pageInfo.TotalCount,
                code = 0


            };
        }
        /// <summary>
        ///资金统计
        /// </summary>
        public ContractStatic GetContractStatic(int ContId)
        {
            var info = Db.Set<ContStatistic>().AsNoTracking().FirstOrDefault(a => a.ContId == ContId);
            ContractStatic contractStatic = new ContractStatic();
            if (info != null)
            {
                var continfo = Db.Set<ContractInfo>().AsNoTracking().FirstOrDefault(a => a.Id == ContId);
                contractStatic.ActMoneryThod = info.CompActAm > 0 ? info.CompActAm.ThousandsSeparator() : "0";
                contractStatic.InvoiceMoneryThod = info.CompInAm > 0 ? info.CompInAm.ThousandsSeparator() : "0";
                //应收=实际开票-实际资金 
                var Ys = ((info.CompInAm ?? 0) - (info.CompActAm ?? 0));
                contractStatic.ReceivableThod = Ys > 0 ? Ys.ThousandsSeparator() : "0";


                //预收=实际资金-实际开票
                var Ysk = ((info.CompActAm ?? 0) - (info.CompInAm ?? 0));
                contractStatic.ReceivableThod = Ysk > 0 ? Ysk.ThousandsSeparator() : "0";
                if (continfo != null)
                {
                    var cs1 = ((continfo.AmountMoney ?? 0) - (info.CompActAm ?? 0));
                    contractStatic.ActNoMoneryThod = cs1 > 0 ? cs1.ThousandsSeparator() : "0";
                    var cs2 = ((continfo.AmountMoney ?? 0) - (info.CompInAm ?? 0));
                    contractStatic.InvoiceNoMoneryThod = cs2 > 0 ? cs2.ThousandsSeparator() : "0";
                }
                //  contractStatic.AdvanceThod = continfo.AdvanceAmount.ThousandsSeparator();//预付预收
                decimal yf = 0;
                yf = ((info.CompInAm ?? 0) - (info.CompActAm ?? 0));
                if (yf > 0)
                {
                    contractStatic.AdvanceThod = yf.ThousandsSeparator();
                }
                else
                {
                    contractStatic.AdvanceThod = "0.00";
                }

            }
            else
            {
                contractStatic.ActMoneryThod = "0.00";
                contractStatic.InvoiceMoneryThod = "0.00";
                contractStatic.ReceivableThod = "0.00";
                contractStatic.ReceivableThod = "0.00";
                contractStatic.ActNoMoneryThod = "0.00";
                contractStatic.InvoiceNoMoneryThod = "0.00";
                contractStatic.AdvanceThod = "0.00";
            }

            return contractStatic;

        }
        /// <summary>
        /// 首页合同列表
        /// </summary>
        /// <typeparam name="s">排序字段</typeparam>
        /// <param name="pageInfo">分页对象</param>
        /// <param name="whereLambda">Where条件表达式</param>
        /// <param name="orderbyLambda">排序表达式</param>
        /// <param name="isAsc">是否正序</param>
        /// <returns></returns>
        public LayPageInfo<ConsoleContractInfoDTO> GetListConsoleContracts<s>(PageInfo<ContractInfo> pageInfo, Expression<Func<ContractInfo, bool>> whereLambda, Expression<Func<ContractInfo, s>> orderbyLambda, bool isAsc)
        {


            var tempquery = _ContractInfoSet.Include(a => a.ContStatic).AsTracking().Where<ContractInfo>(whereLambda.Compile()).AsQueryable();
            pageInfo.TotalCount = tempquery.Count();
            if (isAsc)
            {
                tempquery = tempquery.OrderBy(orderbyLambda.Compile()).AsQueryable();
            }
            else
            {
                tempquery = tempquery.OrderByDescending(orderbyLambda.Compile()).AsQueryable();
            }
            if (!(pageInfo is NoPageInfo<ContractInfo>))
            { //分页
                tempquery = tempquery.Skip<ContractInfo>((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take<ContractInfo>(pageInfo.PageSize);
            }

            var query = from a in tempquery
                            //join b in Db.Set<ContStatistic>()
                            //on a.Id equals b.ContId into ht
                            //from dci in ht.DefaultIfEmpty()
                        select new
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Code = a.Code,
                            AmountMoney = a.AmountMoney,
                            FpJe = a.ContStatic != null ? a.ContStatic.CompInAm : 0,
                            ActAmt = a.ContStatic != null ? a.ContStatic.CompActAm : 0,
                            WcBl = a.ContStatic != null ? a.ContStatic.CompRatio : 0,
                            ContSingNo = a.ContSingNo,//签约人身份证号


                        };
            var local = from a in query.AsEnumerable()
                        select new ConsoleContractInfoDTO
                        {
                            Id = a.Id,
                            Name = a.Name,//名称
                            Code = a.Code,//编号
                            HtJeThond = a.AmountMoney.ThousandsSeparator(),//合同金额千分位
                            FpJeThod = a.FpJe.ThousandsSeparator(),//发票金额
                            HtWcBl = a.WcBl.ConvertToPercent()
                           ,
                            ContSingNo = a.ContSingNo,//签约人身份证号



                        };
            return new LayPageInfo<ConsoleContractInfoDTO>()
            {
                data = local.ToList(),
                count = pageInfo.TotalCount,
                code = 0


            };
        }
        /// <summary>
        /// 合同进度-用于首页
        /// </summary>
        /// <returns></returns>
        public ProgressInfoDTO GetProgress()
        {
            //实际资金
            var listacts = Db.Set<ContActualFinance>().Where(a => a.Astate == (int)ActFinanceStateEnum.Confirmed && (a.Cont != null && a.Cont.IsFramework == 0))
                        .Select(a => new { ftype = a.FinceType, je = a.AmountMoney ?? 0 }).ToList();
            //合同
            var lishts = _ContractInfoSet.Where(a => a.ContState != (int)ContractState.Dozee && a.IsFramework == 0).Select(a => new
            {
                ftype = a.FinanceType,
                je = a.AmountMoney ?? 0
            }).ToList();
            //发票
            var lisfps = Db.Set<ContInvoice>().Where(a => a.InState == (int)InvoiceStateEnum.ReceiptInvoice
              || a.InState == (int)InvoiceStateEnum.Invoicing && (a.Cont != null && a.Cont.IsFramework == 0)).Select(a => new { a.InState, je = a.AmountMoney }).ToList();
            //收款实际资金
            var sksjzj = listacts.Where(a => a.ftype == 0).Sum(a => a.je);
            //收款合同
            var skhtje = lishts.Where(a => a.ftype == 0).Sum(a => a.je);
            //付款实际资金
            var fksjzj = listacts.Where(a => a.ftype == 1).Sum(a => a.je);
            //付款合同金额
            var fkhtje = lishts.Where(a => a.ftype == 1).Sum(a => a.je);

            //已收票金额
            var yspje = lisfps.Where(a => a.InState == (int)InvoiceStateEnum.ReceiptInvoice).Sum(a => a.je);
            //已开票金额
            var ykpje = lisfps.Where(a => a.InState == (int)InvoiceStateEnum.Invoicing).Sum(a => a.je);

            ProgressInfoDTO infoDTO = new ProgressInfoDTO();
            infoDTO.SkHtWcBl = (skhtje == 0 ? 0 : sksjzj / skhtje).ConvertToPercent();
            infoDTO.FkHtWcBl = (fkhtje == 0 ? 0 : fksjzj / fkhtje).ConvertToPercent();
            infoDTO.SpWcBl = (fkhtje == 0 ? 0 : yspje / fkhtje).ConvertToPercent();
            infoDTO.KpWcBl = (skhtje == 0 ? 0 : ykpje / skhtje).ConvertToPercent();

            return infoDTO;


        }

       

    }
}
