using Appkiz.Library.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YGSServer.Common;
using YGSServer.Models;

namespace YGSServer.Controllers
{
    public class ExamController : Controller
    {
        #region 党办申请列表一览		
        [HttpPost]
        public ActionResult List(FormCollection collection)
        {
            /*
             * 变量定义
             */
            var db = new YGSDbContext();
            OrgMgr orgMgr = new OrgMgr();

            // 开始日期
            DateTime startDate = new DateTime();
            // 结束日期
            DateTime endDate = new DateTime();
            // 页数
            int page = WHConstants.Default_Page;
            // 分页大小
            int pageSize = WHConstants.Default_Page_Size;
            // 申请列表
            var applyList = db.Apply.Where(n => n.IsDelete == false);

            /*
             * 参数获取
             */
            // 开始日期
            var startDateString = collection["startDate"];
            // 结束日期
            var endDateString = collection["endDate"];
            // 状态
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
            // 验证开始日期
            if (!string.IsNullOrEmpty(startDateString))
            {
                if (DateTime.TryParse(startDateString, out startDate))
                {
                    applyList = applyList.Where(t => DbFunctions.TruncateTime(t.CreateTime) >= DbFunctions.TruncateTime(startDate));
                }
                else
                {
                    // 验证出错
                    return ResponseUtil.Error(400, "开始日期错误");
                }
            }

            // 验证结束日期
            if (!string.IsNullOrEmpty(endDateString))
            {
                if (DateTime.TryParse(endDateString, out endDate))
                {
                    applyList = applyList.Where(t => DbFunctions.TruncateTime(t.CreateTime) <= DbFunctions.TruncateTime(endDate));
                }
                else
                {
                    // 验证出错
                    return ResponseUtil.Error(400, "结束日期错误");
                }
            }
            // 验证状态
            if (string.IsNullOrEmpty(status))
            {
                return ResponseUtil.Error(400, "验证状态不能为空");
            }
            else
            {
                applyList = applyList.Where(n => n.ApplyStatus == status);
            }
            // 关键字
            if (!string.IsNullOrEmpty(keyword))
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
             * 获取数据
             */
            // 记录总数
            var totalCount = applyList.Count();
            // 记录总页数
            var totalPage = (int)Math.Ceiling((float)totalCount / pageSize);
            // 查询结果数据
            var resultRecords = applyList.OrderByDescending(n => n.CreateTime).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            //预约列表格式
            List<object> applys = new List<object>();

            foreach (var apply in resultRecords)
            {
                var applyEmployee = orgMgr.GetEmployee(apply.UserId);
                // 获得所有外出人员id
                var outUserIds = apply.OutUsers.Split(',').Select(int.Parse).ToList();
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
                        name = m.Name,
                        status = m.CredNo == null ? "fatal" : (db.Cred.Where(n => n.UserID == m.ID).Count() > 0 ? "normal" : "warn")
                    }),
                    checkOpinion = apply.CheckOpinion
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
        #endregion

        #region 党办查看申请详情		
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
                                signStatus = apply.SignStatus,
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

        #region 党办申请更新
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
                                    show = true,
                                    data = apply.OutDate == null ? "" : apply.OutDate.Value.ToString("yyyy/MM/dd")
                                },
                                signStatus = apply.SignStatus,
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

        #region 党办编辑申请
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
            // 出访日期
            var outDateString = collection["outDate"];
            // 签证情况
            var signStatus = collection["signStatus"];
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
            // 出访日期
            //if(string.IsNullOrEmpty(outDateString))
            //{
            //    return ResponseUtil.Error(400, "出访日期不能为空");
            //}
            //else
            //{
            //    if(!DateTime.TryParse(outDateString, out outDate))
            //    {
            //        return ResponseUtil.Error(400, "出访日期格式不正确");
            //    }
            //}
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
                    // 原有的outUsers
                    //var oldOutUsers = apply.OutUsers.Split(',').ToList().Select(int.Parse).ToList();
                    // 最新的outUsers
                    //var currentOutUsers = outUsers.Split(',').ToList().Select(int.Parse).ToList();
                    // 获得筛选掉的用户，并移除
                    //foreach(int outUid in oldOutUsers)
                    //{
                    //    if (!currentOutUsers.Contains(outUid))
                    //    {
                    //        var deleteUser = db.User.Where(n => n.ID == outUid).FirstOrDefault();
                    //        if(deleteUser != null)
                    //        {
                    //            db.User.Remove(deleteUser);
                    //        }
                    //    }
                    //}
                    if (!string.IsNullOrEmpty(outDateString))
                    {
                        apply.OutDate = outDate;
                    } else
                    {
                        apply.OutDate = null;
                    }
                    apply.SignStatus = signStatus;
                    apply.OutUsers = outUsers;
                    db.SaveChanges();

                    return ResponseUtil.OK(200, "更新成功");
                }
            }
        }
        #endregion

        #region 党办删除申请
        public ActionResult Delete(int id)
        {
            using (var db = new YGSDbContext())
            {
                var apply = db.Apply.Where(n => n.ID == id).FirstOrDefault();
                if (apply == null)
                {
                    return ResponseUtil.Error(400, "申请不存在");
                }
                else
                {
                    // 查询所有对应的履历
                    // 获得所有外出人员id
                    //var historyIdList = apply.OutUsers.Split(',').Select(int.Parse).ToList();
                    //var historyList = db.History.Where(n => historyIdList.Contains(n.ID)).ToList();
                    //db.History.RemoveRange(historyList);
                    //db.Apply.Remove(apply);
                    apply.IsDelete = true;
                    db.SaveChanges();

                    return ResponseUtil.OK(200, "删除成功");
                }
            }
        }
        #endregion

        #region 党办审核申请
        [HttpPost]
        public ActionResult Check(FormCollection collection)
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
            // 审批意见
            var checkOpinion = collection["checkOpinion"];
            // 审批状态
            var checkStatus = collection["checkStatus"];

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
            // 审批意见
            if (string.IsNullOrEmpty(checkOpinion))
            {
                return ResponseUtil.Error(400, "审批意见不能为空");
            }
            // 审批状态
            if (string.IsNullOrEmpty(checkStatus))
            {
                return ResponseUtil.Error(400, "审批状态不能为空");
            }

            /*
             * 审核逻辑
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
                    apply.CheckOpinion = checkOpinion;
                    if (checkStatus == WHConstants.Check_Status_Pass)
                    {
                        // 获得所有外出人员id
                        //var historyIdList = apply.OutUsers.Split(',').Select(int.Parse).ToList();
                        //var outUserIds = db.History.Where(n => historyIdList.Contains(n.ID)).Select(n => n.UserId).ToList();
                        //var illegalUsers = db.User.Where(m => outUserIds.Contains(m.ID) && string.IsNullOrEmpty(m.CredNo)).ToList();

                        //if (illegalUsers.Count > 0)
                        //{
                        //    return ResponseUtil.Error(400, "出国人员缺少身份证号，请先到申请详情中补全");
                        //}
                        //else
                        //{
                        //    apply.ApplyStatus = WHConstants.Apply_Status_Passed;
                        //    NotificationUtil.SendNotification(apply.UserId, "您的出国申请已通过", "/Apps/YGS/Home/");
                        //}
                        apply.ApplyStatus = WHConstants.Apply_Status_Passed;
                        NotificationUtil.SendNotification(apply.UserId, "您的出国申请已通过", "/Apps/YGS/Home/");
                    }
                    else
                    {
                        apply.ApplyStatus = WHConstants.Apply_Status_Rejected;
                        NotificationUtil.SendNotification(apply.UserId, "您的出国申请被拒绝", "/Apps/YGS/Home/");
                    }
                    db.SaveChanges();

                    return ResponseUtil.OK(200, "审批成功");
                }
            }
        }
        #endregion
    }
}