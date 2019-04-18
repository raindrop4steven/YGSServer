using Appkiz.Library.Security.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YGSServer.Common;
using YGSServer.Models;

namespace YGSServer.Controllers
{
    public class RecordController : Controller
    {
        #region 出访记录列表
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

            var db = new YGSDbContext();
            var user = db.User.Where(n => n.ID == id).FirstOrDefault();

            if (user == null)
            {
                return ResponseUtil.Error(400, "用户不存在");
            }
            else
            {
                // 出国记录
                var outRecords = db.Apply.ToList().Where(n => n.OutUsers.Split(',').Select(int.Parse).ToList().Contains(id) && n.ApplyStatus == WHConstants.Apply_Status_Passed).ToList().Select(n => new
                {
                    id = n.ID,
                    outName = n.OutName,
                    credType = n.CredType,
                    outDate = n.OutDate == null ? "" : n.OutDate.Value.ToString("yyyy/MM/dd"),
                    outUsers = db.User.ToList().Where(m => n.OutUsers.Split(',').Select(int.Parse).ToList().Contains(m.ID)).Select(m => new {
                        id = m.ID,
                        name = m.Name
                    }),
                    desc = n.Desc,
                    history = db.History.Where(m => m.ApplyId == n.ID && m.UserId == id).ToList().Select(m => new {
                        id = m.ID,
                        signNo = m.SignNo,
                        signTime = m.SignTime == null ? null : m.SignTime.Value.ToString("yyyy/MM/dd"),
                        signNation = m.SignNation,
                        isOut = m.IsOut
                    }).ToList(),
                });

                return new JsonNetResult(new
                {
                    code = 200,
                    data = new
                    {
                        applyRecords = outRecords,
                        userId = userId
                    }
                });
            }
        }
        #endregion

        #region 出访记录新增
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

            var db = new YGSDbContext();
            var user = db.User.Where(n => n.ID == id).FirstOrDefault();

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
                            name = user.Name,
                            credNo = user.CredNo
                        }
                    }
                });
            }
        }
        #endregion

        #region 提交出访记录
        [HttpPost]
        public ActionResult DoAdd(FormCollection collection)
        {
            /*
             * 变量定义
             */
            // 当前用户
            var employee = (User.Identity as AppkizIdentity).Employee;
            // 出访日期
            DateTime outDate = new DateTime();
            // 履历IDs
            List<int> historyIdList = new List<int>();

            /*
             * 参数获取
             */
            // 组团名
            var outName = collection["outName"];
            // 任务描述
            var descn = collection["desc"];
            // 出访类型
            var credType = collection["credType"];
            // 出访人员履历
            var outUsers = collection["outUsers"];
            // 出访日期
            var outDateString = collection["outDate"];
            // 履历ID列表
            var historyIds = collection["historyIds"];

            /*
             * 参数校验
             */
            // 团组名
            if (string.IsNullOrEmpty(outName))
            {
                return ResponseUtil.Error(400, "团组名不能为空");
            }
            // 出访任务
            if (string.IsNullOrEmpty(descn))
            {
                return ResponseUtil.Error(400, "任务描述不能为空");
            }
            // 出访类型
            if (string.IsNullOrEmpty(credType))
            {
                return ResponseUtil.Error(400, "出访类型不能为空");
            }
            // 人员ID列表
            if (string.IsNullOrEmpty(outUsers))
            {
                return ResponseUtil.Error(400, "出访人员不能为空");
            }
            // 出访日期
            if (!string.IsNullOrEmpty(outDateString))
            {
                if (!DateTime.TryParse(outDateString, out outDate))
                {
                    return ResponseUtil.Error(400, "出访日期格式不正确");
                }
            }
            // 履历ID列表
            if (!string.IsNullOrEmpty(historyIds))
            {
                historyIdList = historyIds.Split(',').Select(int.Parse).ToList();
            }
            /*
             * 存储申请
             */
            using (var db = new YGSDbContext())
            {
                var apply = new YGS_Apply();
                apply.OutName = outName;
                apply.Desc = descn;
                apply.UserId = employee.EmplID;
                apply.CredType = credType;
                apply.OutUsers = outUsers;
                //apply.ApplyAtt = applyAtt;
                apply.ApplyStatus = WHConstants.Apply_Status_Passed;
                apply.ApplyDate = DateTime.Now;
                apply.NextStep = "";
                if (!string.IsNullOrEmpty(outDateString))
                {
                    apply.OutDate = outDate;
                }
                apply.CreateTime = DateTime.Now;
                apply.UpdateTime = DateTime.Now;
                apply.IsDelete = true;
                db.Apply.Add(apply);
                db.SaveChanges();

                // 更新对应的履历
                if(historyIdList.Count > 0)
                {
                    var histories = db.History.Where(n => historyIdList.Contains(n.ID)).ToList();
                    foreach (var history in histories)
                    {
                        history.ApplyId = apply.ID;
                    }
                    db.SaveChanges();
                }

                return ResponseUtil.OK(200, "创建成功");
            }
        }
        #endregion

        #region 编辑出访记录
        public ActionResult Update(int id)
        {
            /*
             * 查询申请
             */
            using (var db = new YGSDbContext())
            {
                var apply = db.Apply.Where(n => n.ID == id).FirstOrDefault();
                if (apply == null)
                {
                    return ResponseUtil.Error(400, "申请不存在");
                }
                else
                {
                    // 获得所有外出人员id
                    var userIdList = apply.OutUsers.Split(',').Select(int.Parse).ToList();
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
                            record = new
                            {
                                id = apply.ID,
                                outName = apply.OutName,
                                desc = apply.Desc,
                                credType = apply.CredType,
                                applyDate = apply.ApplyDate.ToString("yyyy/MM/dd"),
                                outUsers = db.User.Where(n => userIdList.Contains(n.ID)).Select(n => new
                                {
                                    id = n.ID,
                                    name = n.Name,
                                    credNo = n.CredNo,
                                    history = db.History.Where(m => m.ApplyId == apply.ID && m.UserId == n.ID).Select(m => new {
                                        id = m.ID,
                                        signNo = m.SignNo,
                                        signNation = m.SignNation,
                                        signTime = m.SignTime,
                                        isOut = m.IsOut
                                    }).ToList()
                                }).ToList(),
                                outDate = apply.OutDate == null ? "" : apply.OutDate.Value.ToString("yyyy/MM/dd")
                            }
                        }
                    });
                }
            }
        }
        #endregion

        #region 出访记录编辑提交
        [HttpPost]
        public ActionResult DoUpdate(FormCollection collection)
        {
            /*
            * 变量定义
            */
            // 申请ID
            int aid = 0;
            // 出访日期
            DateTime outDate = new DateTime();

            /*
             * 参数获取
             */
            // 申请ID
            var id = collection["id"];
            // 组团名
            var outName = collection["outName"];
            // 任务描述
            var descn = collection["desc"];
            // 出访类型
            var credType = collection["credType"];
            // 出访日期
            var outDateString = collection["outDate"];
            // 履历ID列表
            var outUsers = collection["outUsers"];

            /*
             * 参数校验
             */
            // 申请ID
            if (string.IsNullOrEmpty(id))
            {
                return ResponseUtil.Error(400, "申请ID不能为空");
            }
            else
            {
                if (!int.TryParse(id, out aid))
                {
                    return ResponseUtil.Error(400, "申请ID不正确");
                }
            }
            // 组团名
            if (string.IsNullOrEmpty(outName))
            {
                return ResponseUtil.Error(400, "组团名不能为空");
            }
            // 任务描述
            if (string.IsNullOrEmpty(descn))
            {
                return ResponseUtil.Error(400, "任务描述不能为空");
            }
            // 出访类型
            if (string.IsNullOrEmpty(credType))
            {
                return ResponseUtil.Error(400, "出访类型不能为空");
            }
            // 出访日期
            if (!string.IsNullOrEmpty(outDateString))
            {
                if (!DateTime.TryParse(outDateString, out outDate))
                {
                    return ResponseUtil.Error(400, "出访日期格式不正确");
                }
            }

            // 履历ID列表
            if (string.IsNullOrEmpty(outUsers))
            {
                return ResponseUtil.Error(400, "出访人员不能为空");
            }

            /*
             * 更新申请
             */
            using (var db = new YGSDbContext())
            {
                var apply = db.Apply.Where(n => n.ID == aid).FirstOrDefault();
                if (apply == null)
                {
                    return ResponseUtil.Error(400, "申请不能为空");
                }
                else
                {
                    apply.OutName = outName;
                    apply.Desc = descn;
                    apply.CredType = credType;
                    // 原有的outUsers
                    //var oldOutUsers = apply.OutUsers.Split(',').ToList().Select(int.Parse).ToList();
                    // 最新的outUsers
                    //var currentOutUsers = outUsers.Split(',').ToList().Select(int.Parse).ToList();
                    // 获得筛选掉的用户，并移除
                    //foreach (int outUid in oldOutUsers)
                    //{
                    //    if (!currentOutUsers.Contains(outUid))
                    //    {
                    //        var deleteUser = db.User.Where(n => n.ID == outUid).FirstOrDefault();
                    //        if (deleteUser != null)
                    //        {
                    //            db.User.Remove(deleteUser);
                    //        }
                    //    }
                    //}
                    if (!string.IsNullOrEmpty(outDateString))
                    {
                        apply.OutDate = outDate;
                    }
                    else
                    {
                        apply.OutDate = null;
                    }
                    apply.OutUsers = outUsers;
                    db.SaveChanges();

                    return ResponseUtil.OK(200, "更新成功");
                }
            }
        }
        #endregion

        #region 删除出访记录
        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            /*
             * 变量定义
             */
            // 申请ID
            int aid = 0;
            // 用户ID
            int uid = 0;

            /*
             * 参数获取
             */
            // 申请ID
            var applyId = collection["applyId"];
            // 用户ID
            var userId = collection["userId"];

            /*
             * 参数校验
             */
            // 申请ID
            if (string.IsNullOrEmpty(applyId))
            {
                return ResponseUtil.Error(400, "申请ID不能为空");
            }
            else
            {
                if(!int.TryParse(applyId, out aid))
                {
                    return ResponseUtil.Error(400, "申请ID不正确");
                }
            }
            // 用户ID
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseUtil.Error(400, "用户ID不能为空");
            }
            else
            {
                if(!int.TryParse(userId, out uid))
                {
                    return ResponseUtil.Error(400, "用户ID不正确");
                }
            }

            /*
             * 将userId用户从申请的出国人员中去掉
             */
            using (var db = new YGSDbContext())
            {
                var apply = db.Apply.Where(n => n.ID == aid).FirstOrDefault();
                if (apply == null)
                {
                    return ResponseUtil.Error(400, "申请不存在");
                }
                else
                {
                    // 原有的outUsers
                    var oldOutUsers = apply.OutUsers.Split(',').ToList().Select(int.Parse).ToList();
                    // 最新的outUsers
                    var currentOutUsers = new List<int>();

                    // 获得筛选掉的用户，并移除
                    foreach (int outUid in oldOutUsers)
                    {
                        if (outUid != uid)
                        {
                            currentOutUsers.Add(outUid);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    // 判断是否还有出国人员
                    if(currentOutUsers.Count == 0)
                    {
                        db.Apply.Remove(apply);
                        db.SaveChanges();
                    }
                    else
                    {
                        var finalOutUsers = string.Join(",", currentOutUsers);
                        apply.OutUsers = finalOutUsers;
                        db.SaveChanges();
                    }

                    return ResponseUtil.Error(200, "删除成功");
                }
            }
        }
        #endregion
    }
}