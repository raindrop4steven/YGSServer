using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace YGSServer.Models
{
    #region 用户
    public class YGS_User
    {
        // ID
        [Key]
        public int ID { get; set; }
        // 姓名
        public string Name { get; set; }
        // 性别
        public int Sex { get; set; }
        // 出生地
        public string Location { get; set; }
        // 出生日期
        public DateTime? BirthDay { get; set; }
        // 身份证号
        public string CredNo { get; set; }
        // 工作单位
        public string Unit { get; set; }
        // 部门
        public string Depart { get; set; }
        // 职务
        public string Duty { get; set; }
        // 级别
        public string Level { get; set; }
        // 添加日期
        public DateTime? CreateTime { get; set; }
        // 更新日期
        public DateTime? UpdateTime { get; set; }
    }
    #endregion

    #region 证件
    public class YGS_Cred
    {
        // ID
        [Key]
        public int ID { get; set; }
        // 用户ID
        public int UserID { get; set; }
        // 证件编号
        public string TradeCode { get; set; }
        // 证件类型
        public string CredType { get; set; }
        // 姓名
        public string Name { get; set; }
        // 性别
        public int Sex { get; set; }
        // 发证单位
        public string CredUnit { get; set; }
        // 发证日期
        public DateTime? CredDate { get; set; }
        // 有效期
        public DateTime? ValidDate { get; set; }
        // 有效状态
        public string ValidStatus { get; set; }
        // 添加日期
        public DateTime? CreateTime { get; set; }
        // 更新日期
        public DateTime? UpdateTime { get; set; }
    }
    #endregion

    #region 申请
    public class YGS_Apply
    {
        // ID
        [Key]
        public int ID { get; set; }
        // 申请人
        public int UserId { get; set; }
        // 组团名
        public string OutName { get; set; }
        // 出访任务
        public string Desc { get; set; }
        // 出访类型
        public string CredType { get; set; }
        // 申请日期
        public DateTime ApplyDate { get; set; }
        // 人员列表
        public string OutUsers { get; set; }
        // 申请附件
        public string ApplyAtt { get; set; }
        // 出访日期
        public DateTime? OutDate { get; set; }
        // 签证情况
        public string SignStatus { get; set; }
        // 资料回传附件
        public string AfterAtt { get; set; }
        // 申请状态
        public string ApplyStatus { get; set; }
        // 签批意见
        public string CheckOpinion { get; set; }
        // 下一步
        public string NextStep { get; set; }
        // 添加日期
        public DateTime CreateTime { get; set; }
        // 更新日期
        public DateTime UpdateTime { get; set; }
    }
    #endregion

    #region 附件模型
    public class YGS_Att
    {
        // 附件ID
        [Key]
        public int ID { get; set; }
        // 名称
        public string Name { get; set; }
        // 路径
        public string Path { get; set; }
    }
    #endregion

    #region 数据库上下文
    public class YGSDbContext : DbContext
    {
        // 用户
        public DbSet<YGS_User> User { get; set; }
        // 证件
        public DbSet<YGS_Cred> Cred { get; set; }
        // 申请
        public DbSet<YGS_Apply> Apply { get; set; }
        // 附件
        public DbSet<YGS_Att> Attachment { get; set; }
    }
    #endregion
}