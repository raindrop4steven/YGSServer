using Appkiz.Library.Security;
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
    public class ApplyController : Controller
    {
        #region 部门我的申请一览
        [HttpPost]
        public ActionResult List(FormCollection collection)
        {
            using (var db = new YGSDbContext())
            {
                /*
                 * 变量定义
                 */
                var employee = (User.Identity as AppkizIdentity).Employee;
                OrgMgr orgMgr = new OrgMgr();

                // 页数
                int page = WHConstants.Default_Page;
                // 分页大小
                int pageSize = WHConstants.Default_Page_Size;
                // 申请数据
                var applyList = db.Apply.Where(n => n.UserId == employee.EmplID);

                /*
                 * 参数获取
                 */
                // 申请单状态
                var status = collection["status"];
                // 关键字
                var keyword = collection["keyword"];
                // 页数
                var pageString = collection["page"];
                // 分页大小
                var pageSizeString = collection["pageSize"];

                /*
                 * 参数校验
                 */
                // 验证状态
                if(string.IsNullOrEmpty(status))
                {
                    return ResponseUtil.Error(400, "状态不能为空");
                }
                else
                {
                    applyList = applyList.Where(n => n.ApplyStatus == status);
                }
                // 关键字
                if(!string.IsNullOrEmpty(keyword))
                {
                    applyList = applyList.Where(n => n.OutName.Contains(keyword));
                }
                // 验证分页大小
                if (!string.IsNullOrEmpty(pageString))
                {
                    if (int.TryParse(pageString, out page))
                    {
                        if (page <= 0)
                        {
                            return ResponseUtil.Error(400, "分页大小错误");
                        }
                    }
                    else
                    {
                        // 验证出错
                        return ResponseUtil.Error(400, "分页大小错误");
                    }
                }
                // 验证页数
                if (!string.IsNullOrEmpty(pageSizeString))
                {
                    if (int.TryParse(pageSizeString, out pageSize))
                    {
                        if (pageSize <= 0)
                        {
                            return ResponseUtil.Error(400, "页数错误");
                        }
                    }
                    else
                    {
                        // 验证出错
                        return ResponseUtil.Error(400, "页数错误");
                    }
                }

                /*
                 * 查询申请
                 */
                // 记录总数
                var totalCount = applyList.Count();
                // 记录总页数
                var totalPage = (int)Math.Ceiling((float)totalCount / pageSize);
                // 查询结果数据
                var resultRecords = applyList.OrderByDescending(n => n.CreateTime).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                //预约列表格式
                List<object> applys = new List<object>();

                // 履历表
                foreach (var apply in resultRecords)
                {
                    var applyEmployee = orgMgr.GetEmployee(apply.UserId);
                    // 获得所有外出人员id
                    var historyIdList = apply.OutUsers.Split(',').Select(int.Parse).ToList();
                    var outUserIds = db.History.Where(n => historyIdList.Contains(n.ID)).Select(n => n.UserId).ToList();

                    applys.Add(new
                    {
                        id = apply.ID,
                        outName = apply.OutName,
                        desc = apply.Desc,
                        applyDate = apply.ApplyDate.ToString("yyyy/MM/dd"),
                        applyUser = applyEmployee == null ? null : string.Format("{0} {1}", applyEmployee.DeptName, applyEmployee.EmplName),
                        outUsers = db.User.ToList().Where(m => outUserIds.Contains(m.ID)).Select(m => new
                        {
                            id = m.ID,
                            name = m.Name
                        }).ToList(),
                        checkOpinion = apply.CheckOpinion,
                        nextStep = apply.ApplyStatus == WHConstants.Apply_Status_Examing ? "等待领导审批" : (apply.ApplyStatus == WHConstants.Apply_Status_Passed ? "下载并填写表格" : "修改并重新提交审核")
                    });
                }

                return new JsonNetResult(new
                {
                    code = 200,
                    data = new
                    {
                        records = applys,
                        meta = new
                        {
                            current_page = page,
                            total_page = totalPage,
                            current_count = (page - 1) * pageSize + resultRecords.ToList().Count(),
                            total_count = totalCount,
                            per_page = pageSize
                        }
                    }
                });
            }
        }
        #endregion

        #region 部门查看申请详情
        public ActionResult Detail(int id)
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
                                applyAtt = db.Attachment.ToList().Where(m => apply.ApplyAtt.Split(',').Select(int.Parse).ToList().Contains(m.ID)).Select(m => new
                                {
                                    id = m.ID,
                                    name = m.Name,
                                    url = Url.Action("Download", "Common", new { id = m.ID })
                                }),
                                outDate = new
                                {
                                    show = apply.OutDate != null,
                                    data = apply.OutDate == null ? "" : apply.OutDate.Value.ToString("yyyy/MM/dd")
                                },
                                afterAtt = new
                                {
                                    show = apply.AfterAtt != null,
                                    data = apply.AfterAtt == null ? null : db.Attachment.ToList().Where(m => apply.AfterAtt.Split(',').Select(int.Parse).ToList().Contains(m.ID)).Select(m => new
                                    {
                                        id = m.ID,
                                        name = m.Name,
                                        url = Url.Action("Download", "Common", new { id = m.ID })
                                    })
                                }
                            }
                        }
                    });
                }
            }
        }
        #endregion

        #region 部门获取编辑申请详情
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
                                applyAtt = db.Attachment.ToList().Where(m => apply.ApplyAtt.Split(',').Select(int.Parse).ToList().Contains(m.ID)).Select(m => new
                                {
                                    id = m.ID,
                                    name = m.Name,
                                    url = Url.Action("Download", "Common", new { id = m.ID })
                                }),
                                outDate = new
                                {
                                    show = apply.OutDate != null,
                                    data = apply.OutDate == null ? "" : apply.OutDate.Value.ToString("yyyy/MM/dd")
                                },
                                afterAtt = new
                                {
                                    show = apply.ApplyStatus == WHConstants.Apply_Status_Passed || apply.AfterAtt != null,
                                    data = apply.AfterAtt == null ? null: db.Attachment.ToList().Where(m => apply.AfterAtt.Split(',').Select(int.Parse).ToList().Contains(m.ID)).Select(m => new
                                    {
                                        id = m.ID,
                                        name = m.Name,
                                        url = Url.Action("Download", "Common", new { id = m.ID })
                                    })
                                }
                            }
                        }
                    });
                }
            }
        }
        #endregion

        #region 部门更新申请
        [HttpPost]
        public ActionResult DoUpdate(FormCollection collection)
        {
            /*
             * 变量定义
             */
            // 申请ID
            int aid = 0;

            /*
             * 参数获取
             */
            // 申请ID
            var id = collection["id"];
            // 团组名
            var outName = collection["outName"];
            // 出访任务
            var descn = collection["desc"];
            // 出访类型
            var credType = collection["credType"];
            // 人员ID列表
            var outUsers = collection["outUsers"];
            // 申请附件ID列表
            var applyAtt = collection["applyAtt"];
            // 资料回传附件ID列表
            var afterAtt = collection["afterAtt"];

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
                if(!int.TryParse(id, out aid))
                {
                    return ResponseUtil.Error(400, "申请ID不正确");
                }
            }
            // 团组名
            if(string.IsNullOrEmpty(outName))
            {
                return ResponseUtil.Error(400, "团组名不能为空");
            }
            // 出访任务
            if(string.IsNullOrEmpty(descn))
            {
                return ResponseUtil.Error(400, "出访类型不能为空");
            }
            // 人员ID列表
            if(string.IsNullOrEmpty(outUsers))
            {
                return ResponseUtil.Error(400, "出访人员不能为空");
            }
            // 申请附件ID不能为空
            if(string.IsNullOrEmpty(applyAtt))
            {
                return ResponseUtil.Error(400, "申请附件不能为空");
            }

            /*
             * 查询申请
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
                    apply.OutName = outName;
                    apply.Desc = descn;
                    apply.CredType = credType;
                    apply.OutUsers = outUsers;
                    apply.ApplyAtt = applyAtt;
                    if(!string.IsNullOrEmpty(afterAtt))
                    {
                        apply.AfterAtt = afterAtt;
                    }
                    else
                    {
                        apply.AfterAtt = null;
                    }
                    // 如果当前申请是被拒绝，则重新到待审核中
                    if(apply.ApplyStatus == WHConstants.Apply_Status_Rejected)
                    {
                        apply.ApplyStatus = WHConstants.Apply_Status_Examing;

                        var notifyUserIdList = NotificationUtil.GetNotificationUsers();
                        foreach (var user in notifyUserIdList)
                        {
                            NotificationUtil.SendNotification(user, "您有新的出国申请审核", "/Apps/YGS/Home/Check");
                        }
                    }
                    db.SaveChanges();

                    return ResponseUtil.OK(200, "申请更新成功");
                }
            }
        }
        #endregion

        #region 部门删除申请
        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            /*
             * 变量定义
             */
            // 申请ID
            int aid = 0;

            /*
             * 参数获取
             */
            // 申请id
            var id = collection["id"];

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

            /*
             * 删除申请
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
                    // 查询所有对应的履历
                    // 获得所有外出人员id
                    var historyIdList = apply.OutUsers.Split(',').Select(int.Parse).ToList();
                    var historyList = db.History.Where(n => historyIdList.Contains(n.ID)).ToList();
                    db.History.RemoveRange(historyList);
                    db.Apply.Remove(apply);
                    db.SaveChanges();
                    return ResponseUtil.OK(200, "删除成功");
                }
            }
        }
        #endregion

        #region 部门新增申请
        public ActionResult Add()
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
                }
            });
        }
        #endregion

        #region 部门提交申请
        [HttpPost]
        public ActionResult DoAdd(FormCollection collection)
        {
            /*
             * 变量定义
             */
            // 当前用户
            var employee = (User.Identity as AppkizIdentity).Employee;
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
            // 申请附件
            var applyAtt = collection["applyAtt"];

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
            // 申请附件ID不能为空
            if (string.IsNullOrEmpty(applyAtt))
            {
                return ResponseUtil.Error(400, "申请附件不能为空");
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
                apply.ApplyAtt = applyAtt;
                apply.ApplyStatus = WHConstants.Apply_Status_Examing;
                apply.ApplyDate = DateTime.Now;
                apply.NextStep = "下载并填写表格";
                apply.CreateTime = DateTime.Now;
                apply.UpdateTime = DateTime.Now;
                db.Apply.Add(apply);
                db.SaveChanges();

                var notifyUserIdList = NotificationUtil.GetNotificationUsers();

                foreach(var user in notifyUserIdList)
                {
                    NotificationUtil.SendNotification(user, "您有新的出国申请审核", "/Apps/YGS/Home/Check");
                }
                
                return ResponseUtil.OK(200, "创建成功");
            }
        }
        #endregion
    }
}