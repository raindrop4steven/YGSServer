using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Appkiz.Library.Notification;
using Appkiz.Library.Security;

namespace YGSServer.Common
{
    public class NotificationUtil
    {
        // 通知管理
        private static NotificationMgr notifyMgr = new NotificationMgr();
        private static OrgMgr orgMgr = new OrgMgr();

        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="MessageID">消息ID</param>
        /// <param name="TargetEmployeeID">接收人ID</param>
        public static bool SendNotification()
        {
            var users = GetNotificationUsers();

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                NewWin = true,
                Url = "/Apps/YGS/Home/Check"
            });

            foreach(var employeeId in users)
            {
                notifyMgr.SendNotification("YGS", employeeId, "您有新的因公出国申请通知", data);
            }
            return true;
        }

        public static List<string>GetNotificationUsers()
        {
            // 根据Permission Code获得securityId和securityType
            //var PermissionCode = "canCheck";

            //return orgMgr.ListPermissionForObject("app", "YGS", PermissionCode);
            // 获得领导角色组名称
            string GroupRoleName = System.Configuration.ConfigurationManager.AppSettings["CheckGroupName"];

            // 获得该组内所有成员
            var sql = @"SELECT
                            ORG_Employee.* 
                        FROM 
                            ORG_Employee 
                        JOIN 
                            ORG_EmplRole 
                        ON 
                            ORG_Employee.EmplID = ORG_EmplRole.EmplID 
                        JOIN 
                            ORG_Role 
                        ON 
                            ORG_EmplRole.RoleID = ORG_Role.RoleID 
                        AND 
                            ORG_Role.RoleName = '{0}'
                        ORDER BY
                            ORG_Employee.GlobalSortNo
                        DESC";

            List<Employee> EmployeeList = orgMgr.FindEmployeeBySQL(string.Format(sql, GroupRoleName));
            var ResultList = new List<string>();
            foreach (Employee employee in EmployeeList)
            {
                var originName = employee.EmplName;
                var finalName = originName;
                ResultList.Add(employee.EmplID);
            }

            return ResultList;
        }
    }
}