using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YGSServer.Common;
using YGSServer.Models;

namespace YGSServer.Controllers
{
    public class CredController : Controller
    {
        #region 证件一览
        [HttpPost]
        public ActionResult List(FormCollection collection)
        {
            /*
             * 变量定义
             */
            // 用户ID
            int id = 0;

            /*
             * 参数获取
             */
            // 用户ID
            var userId = collection["userId"];

            /*
             * 参数校验
             */
            // 用户ID
            if(string.IsNullOrEmpty(userId))
            {
                return ResponseUtil.Error(400, "用户ID不能为空");
            }
            else
            {
                if(!int.TryParse(userId, out id))
                {
                    return ResponseUtil.Error(400, "用户ID不正确");
                }
            }

            using (var db = new YGSDbContext())
            {
                var user = db.User.Where(n => n.ID == id).FirstOrDefault();
                if (user == null)
                {
                    return ResponseUtil.Error(400, "用户不存在");
                }
                else
                {
                    // 证件列表
                    var credList = db.Cred.Where(n => n.UserID == id).Select(n => new
                    {
                        id = n.ID,
                        tradeCode = n.TradeCode,
                        name = n.Name,
                        sex = n.Sex,
                        credUnit = n.CredUnit,
                        credDate = n.CredDate.Value.ToString("yyyy/MM/dd"),
                        validDate = n.ValidDate.Value.ToString("yyyy/MM/dd"),
                        validStatus = n.ValidStatus
                    }).ToList();
                    // 出国记录
                    var outRecords = db.Apply.Where(n => n.UserId == id && n.ApplyStatus == WHConstants.Apply_Status_Passed).Select(n => new
                    {
                        id = n.ID,
                        outName = n.OutName,
                        credType = n.CredType,
                        signStatus = n.SignStatus,
                        outDate = n.OutDate == null ? "" : n.OutDate.Value.ToString("yyyy/MM/dd"),
                        outUsers = db.User.Where(m => n.OutUsers.Split(',').Select(int.Parse).ToList().Contains(m.ID)).Select(m => new {
                            id = m.ID,
                            name = m.Name
                        }).ToList(),
                        desc = n.Desc
                    }).ToList();

                    return new JsonNetResult(new
                    {
                        code = 200,
                        data = new
                        {
                            creds = credList,
                            applyRecords = outRecords
                        }
                    });
                }
            }
        }
        #endregion

        #region 证件新增
        [HttpPost]
        public ActionResult Add(FormCollection collection)
        {
            /*
             * 变量定义
             */
            // 用户ID
            int id = 0;

            /*
             * 参数获取
             */
            // 用户ID
            var userId = collection["userId"];

            /*
             * 参数校验
             */
            // 用户ID
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseUtil.Error(400, "用户ID不能为空");
            }
            else
            {
                if (!int.TryParse(userId, out id))
                {
                    return ResponseUtil.Error(400, "用户ID不正确");
                }
            }

            /*
             * 用户查询
             */
            using (var db = new YGSDbContext())
            {
                var user = db.User.Where(n => n.ID == id).FirstOrDefault();
                if(user == null)
                {
                    return ResponseUtil.Error(400, "用户不存在");
                }
                else
                {
                    return new JsonNetResult(new
                    {
                        code = 200,
                        data = new
                        {
                            credTypes = new object[]
                            {
                                new {
                                    id = 1,
                                    type = WHConstants.Cred_Type_Passport,
                                    name = "护照"
                                },
                                new
                                {
                                    id = 2,
                                    type = WHConstants.Cred_Type_Permit,
                                    name = "通行证"
                                }
                            },
                            user = new
                            {
                                id = user.ID,
                                credNo = user.CredNo,
                                name = user.Name,
                                sex = user.Sex,
                                unit = user.Unit,
                                depart = user.Depart,
                                level = user.Level,
                                duty = user.Duty
                            }
                        }
                    });
                }
            }
        }
        #endregion

        #region 证件提交新增
        [HttpPost]
        public ActionResult DoAdd(FormCollection collection)
        {
            /*
             * 变量定义
             */
            // 用户ID
            int uid = 0;
            // 发照日期
            DateTime credDate = new DateTime();
            // 有效期
            DateTime validDate = new DateTime();

            /*
             * 参数获取
             */
            // 用户ID
            var userId = collection["userId"];
            // 证件号
            var tradeCode = collection["tradeCode"];
            // 证件类型
            var credType = collection["credType"];
            // 发照机关
            var credUnit = collection["credUnit"];
            // 发照日期
            var credDateString = collection["credDate"];
            // 有效期
            var validDateString = collection["validDate"];
            // 有效状态
            var validStatus = collection["validStatus"];

            /*
             * 参数校验
             */
            // 用户ID
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseUtil.Error(400, "用户ID不能为空");
            }
            else
            {
                if (!int.TryParse(userId, out uid))
                {
                    return ResponseUtil.Error(400, "用户ID不正确");
                }
            }
            // 证件号
            if (string.IsNullOrEmpty(tradeCode))
            {
                return ResponseUtil.Error(400, "证件号不能为空");
            }
            // 证件类型
            if (string.IsNullOrEmpty(credType))
            {
                return ResponseUtil.Error(400, "证件类型不能为空");
            }
            // 发照机关
            if (string.IsNullOrEmpty(credUnit))
            {
                return ResponseUtil.Error(400, "发照机关不能为空");
            }
            // 发照日期
            if(string.IsNullOrEmpty(credDateString))
            {
                return ResponseUtil.Error(400, "发照日期不能为空");
            }
            else
            {
                if (!DateTime.TryParse(credDateString, out credDate))
                {
                    return ResponseUtil.Error(400, "发照日期格式不正确");
                }
            }
            // 有效期
            if (string.IsNullOrEmpty(validDateString))
            {
                return ResponseUtil.Error(400, "有效期不能为空");
            }
            else
            {
                if (!DateTime.TryParse(validDateString, out validDate))
                {
                    return ResponseUtil.Error(400, "有效期格式不正确");
                }
            }
            // 有效状态
            if (string.IsNullOrEmpty(validStatus))
            {
                return ResponseUtil.Error(400, "有效状态不能为空");
            }

            /*
             * 用户查询
             */
            using (var db = new YGSDbContext())
            {
                var user = db.User.Where(n => n.ID == uid).FirstOrDefault();
                if (user == null)
                {
                    return ResponseUtil.Error(400, "用户不存在");
                }
                else
                {
                    var cred = new YGS_Cred();
                    cred.UserID = uid;
                    cred.TradeCode = tradeCode;
                    cred.CredType = credType;
                    cred.CredUnit = credUnit;
                    cred.CredDate = credDate;
                    cred.ValidDate = validDate;
                    cred.ValidStatus = validStatus;

                    db.Cred.Add(cred);
                    db.SaveChanges();

                    return ResponseUtil.OK(200, "添加成功");
                }
            }
        }
        #endregion
    }
}