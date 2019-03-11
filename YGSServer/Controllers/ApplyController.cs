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
                // 页数
                int page = WHConstants.Default_Page;
                // 分页大小
                int pageSize = WHConstants.Default_Page_Size;
                // 申请数据
                var applyList = db.Apply.Where(n => n.ID > 0);

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
                // 

                /*
                 * 查询申请
                 */
                // 记录总数
                // 记录总数
                var totalCount = applyList.Count();
                // 记录总页数
                var totalPage = (int)Math.Ceiling((float)totalCount / pageSize);
                // 查询结果数据
                var resultRecords = applyList.OrderByDescending(n => n.CreateTime.Value).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                //预约列表格式
                List<object> applys = new List<object>();

                foreach(var apply in resultRecords)
                {
                    applys.Add(new
                    {
                        id = apply.ID,
                        outName = apply.OutName,
                        desc = apply.Desc,
                        applyDate = apply.ApplyDate.ToString("yyyy/MM/dd"),
                        applyUser = apply.UserId,
                        outUsers = db.User.Where(m => apply.OutUsers.Split(',').Select(int.Parse).ToList().Contains(m.ID)).Select(m => new
                        {
                            id = m.ID,
                            name = m.Name
                        }).ToList(),
                        checkOpinion = apply.CheckOpinion,
                        nextStep = apply.NextStep
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
        
        #endregion

        #region 部门获取编辑申请详情

        #endregion

        #region 部门更新申请

        #endregion

        #region 部门删除申请

        #endregion

        #region 部门新增申请

        #endregion

        #region 部门提交申请

        #endregion
    }
}