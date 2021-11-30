using System;
using System.Collections.Generic;
using System.Text;

namespace NF.Common.Utility
{
   public class StaticData
    {
        /// <summary>
        /// 存储操作日志的RedisKey
        /// </summary>
        public static readonly string OptionLogRedisKey = "OptionLogRedis";
        /// <summary>
        /// Session UserId 存储key
        /// </summary>
        public static  readonly string NFUserId = "NFUserId";
        /// <summary>
        /// Session User对象Key
        /// </summary>
        public static readonly string NFUser = "NFUser";
        /// <summary>
        /// 验证码Session Key
        /// </summary>
        public static readonly string NFVerifyCode = "NFVerifyCode";
        /// <summary>
        /// 所在部门
        /// </summary>
        public static readonly string NFUserDeptId = "NFUserDeptId";
        /// <summary>
        /// 数据字典Hashkey
        /// </summary>
        public static readonly string RedisDataKey = "data";
        /// <summary>
        /// 用户hashkey
        /// </summary>
        public static readonly string RedisUserKey = "user";
        /// <summary>
        /// 删除数据字典队列key
        /// </summary>
        public static readonly string RedisDataDelKey = "datadel";
        /// <summary>
        /// 删除用户队列Key
        /// </summary>
        public static readonly string RedisUserDelKey = "userdel";
        /// <summary>
        /// 组织机构Hashkey
        /// </summary>
        public static readonly string RedisDeptKey = "dept";
        /// <summary>
        /// 删除组织机构Hashkey
        /// </summary>
        public static readonly string RedisDelDeptKey = "deptdel";
        /// <summary>
        /// 存储币种key
        /// </summary>
        public static readonly string RedisCurrencyKey = "currency";
        /// <summary>
        /// 删除币种
        /// </summary>
        public static readonly string RedisDelCurrencyKey = "currencydel";
        /// <summary>
        /// 市KEy
        /// </summary>
        public static readonly string RedisCityKey = "city";
        /// <summary>
        /// 省
        /// </summary>
        public static readonly string RedisProvinceKey = "province";
        /// <summary>
        /// 国家
        /// </summary>
        public static readonly string RedisCountryKey = "country";
        /// <summary>
        /// 当前用户权限
        /// </summary>
        public static readonly string UserPermissions = "userpiss";
        /// <summary>
        /// 当前用户角色权限
        /// </summary>
        public static readonly string UserRolePermissions = "urolepiss";
        /// <summary>
        /// TreeSelect需要的部门数据
        /// </summary>
        public static readonly string RedisTreeSelDeptKey = "TreeSelDeptKey";
        /// <summary>
        /// 创建合同历史信息（主要是标签信息）
        /// </summary>
        public static readonly string AddContHistory = "AddContHistory";
        /// <summary>
        /// 合同统计
        /// </summary>
        public static readonly string ContStat = "ContStat";
        /// <summary>
        /// 添加APP提醒
        /// </summary>
        public static readonly string InsertTExing = "InsertTExing";




    }
}
