using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YGSServer.Common
{
    public class WHConstants
    {
        /*
         * 分页默认大小
         */
        // 页数
        public static int Default_Page = 1;
        // 页面显示个数
        public static int Default_Page_Size = 10;

        /*
         * 申请单状态
         */
        // 待审核
        public static string Apply_Status_Examing = "examing";
        // 已通过
        public static string Apply_Status_Passed = "passed";
        // 已拒绝
        public static string Apply_Status_Rejected = "rejected";

        /*
         * 证件类型
         */
        // 护照
        public static string Cred_Type_Passport = "passport";
        // 通行证
        public static string Cred_Type_Permit = "permit";

        /*
         * 证件状态
         */
        // 有效
        public static string Cred_Status_Valid = "valid";
        // 无效
        public static string Cred_Status_InValid = "invalid";

        /*
         * 审核操作
         */
        // 通过
        public static string Check_Status_Pass = "pass";
        // 拒绝
        public static string Check_Status_Reject = "reject";
    }
}