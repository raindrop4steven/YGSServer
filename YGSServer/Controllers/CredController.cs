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

            var db = new YGSDbContext();
            var user = db.User.Where(n => n.ID == id).FirstOrDefault();

            if (user == null)
            {
                return ResponseUtil.Error(400, "用户不存在");
            }
            else
            {
                // 证件列表
                var credList = db.Cred.Where(n => n.UserID == id).ToList().Select(n => new
                {
                    id = n.ID,
                    tradeCode = n.TradeCode,
                    name = n.Name,
                    sex = n.Sex,
                    credUnit = n.CredUnit,
                    credDate = n.CredDate.Value.ToString("yyyy/MM/dd"),
                    validDate = n.ValidDate.Value.ToString("yyyy/MM/dd"),
                    validStatus = n.ValidStatus
                });
                // 出国记录
                var outRecords = db.Apply.Where(n => n.OutUsers.Contains(userId) && n.ApplyStatus == WHConstants.Apply_Status_Passed).ToList().Select(n => new
                {
                    id = n.ID,
                    outName = n.OutName,
                    credType = n.CredType,
                    signStatus = n.SignStatus,
                    outDate = n.OutDate == null ? "" : n.OutDate.Value.ToString("yyyy/MM/dd"),
                    //outUsers = db.User.ToList().Where(m => n.OutUsers.Split(',').Select(int.Parse).ToList().Contains(m.ID)).Select(m => new {
                    //    id = m.ID,
                    //    name = m.Name
                    //}),
                    outUsers = db.History.Where(m => n.OutUsers.Split(',').Select(int.Parse).ToList().Contains(n.ID)).Select(m => new
                    {
                        id = m.ID,
                        name = db.User.Where(u => u.ID == m.UserId).Select(u => u.Name).FirstOrDefault()
                    }).ToList(),
                    signNo = db.History.Where(m => n.OutUsers.Split(',').Select(int.Parse).ToList().Contains(m.ID)).Select(m => m.SignNo).FirstOrDefault(),
                    signTime = db.History.Where(m => n.OutUsers.Split(',').Select(int.Parse).ToList().Contains(m.ID)).Select(m => m.SignTime.Value).FirstOrDefault().ToString("yyyy/MM/dd"),
                    desc = n.Desc
                });

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
                                location = user.Location,
                                birthDay = user.BirthDay == null ? null : user.BirthDay.Value.ToString("yyyy/MM/dd"),
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
                    cred.Name = user.Name;
                    cred.Sex = user.Sex;
                    cred.TradeCode = tradeCode;
                    cred.CredType = credType;
                    cred.CredUnit = credUnit;
                    cred.CredDate = credDate;
                    cred.ValidDate = validDate;
                    cred.ValidStatus = validStatus;
                    cred.CreateTime = DateTime.Now;
                    cred.UpdateTime = DateTime.Now;

                    db.Cred.Add(cred);
                    db.SaveChanges();

                    return ResponseUtil.OK(200, "添加成功");
                }
            }
        }
        #endregion

        #region 证件更新
        public ActionResult Update(int id)
        {
            /*
             * 变量定义
             */

            /*
             * 参数
             */
            // 用户获取
            using (var db = new YGSDbContext())
            {
                var cred = db.Cred.Where(n => n.ID == id).FirstOrDefault();
                if (cred == null)
                {
                    return ResponseUtil.Error(400, "证件不存在");
                }
                else
                {
                    var user = db.User.Where(n => n.ID == cred.UserID).FirstOrDefault();
                    if (user == null)
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
                                    location = user.Location,
                                    birthDay = user.BirthDay == null ? null : user.BirthDay.Value.ToString("yyyy/MM/dd"),
                                    unit = user.Unit,
                                    depart = user.Depart,
                                    level = user.Level,
                                    duty = user.Duty
                                },
                                trade = new
                                {
                                    tradeCode = cred.TradeCode,
                                    credType = cred.CredType,
                                    credUnit = cred.CredUnit,
                                    credDate = cred.CredDate.Value.ToString("yyyy/MM/dd"),
                                    validDate = cred.ValidDate.Value.ToString("yyyy/MM/dd"),
                                    validStatus = cred.ValidStatus
                                }
                            }
                        });
                    }
                }
            }
        }
        #endregion

        #region 证件提交更新
        [HttpPost]
        public ActionResult DoUpdate(FormCollection collection)
        {
            /*
             * 变量定义
             */
            // 证件ID
            int tradeId = 0;
            // 发照日期
            DateTime credDate = new DateTime();
            // 有效期
            DateTime validDate = new DateTime();

            /*
             * 参数获取
             */
            // 证件ID
            var tradeIdString = collection["tradeId"];
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
            // 证件ID
            if (string.IsNullOrEmpty(tradeIdString))
            {
                return ResponseUtil.Error(400, "证件ID不能为空");
            }
            else
            {
                if (!int.TryParse(tradeIdString, out tradeId))
                {
                    return ResponseUtil.Error(400, "证件ID不正确");
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
            if (string.IsNullOrEmpty(credDateString))
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
             * 证件查询
             */
            using (var db = new YGSDbContext())
            {
                var cred = db.Cred.Where(n => n.ID == tradeId).FirstOrDefault();
                if(cred == null)
                {
                    return ResponseUtil.Error(400, "证件不存在");
                }
                else
                {
                    cred.TradeCode = tradeCode;
                    cred.CredType = credType;
                    cred.CredUnit = credUnit;
                    cred.CredDate = credDate;
                    cred.ValidDate = validDate;
                    cred.ValidStatus = validStatus;
                    db.SaveChanges();
                    return ResponseUtil.OK(200, "更新成功");
                }
            }
        }
        #endregion

        #region 证件删除
        public ActionResult Delete(int id)
        {
            /*
             * 证件查询
             */
            using (var db = new YGSDbContext())
            {
                var cred = db.Cred.Where(n => n.ID == id).FirstOrDefault();
                if (cred == null)
                {
                    return ResponseUtil.Error(400, "证件不存在");
                }
                else
                {
                    db.Cred.Remove(cred);
                    db.SaveChanges();

                    return ResponseUtil.OK(200, "删除成功");
                }
            }
        }
        #endregion
    }
}