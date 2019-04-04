using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using YGSServer.Common;
using YGSServer.Models;

namespace YGSServer.Controllers
{
    public class HistoryController : Controller
    {
        #region 添加履历
        [HttpPost]
        public ActionResult Add(FormCollection collection)
        {
            /*
             * 变量定义
             */
            // 用户ID
            int uid = 0;
            // 申请ID
            int aid = 0;
            // 签证时间
            DateTime signTime = new DateTime();
            // 是否出行
            bool isOut = true;

            /*
             * 参数获取
             */
            // 申请ID
            var aidString = collection["aid"];
            // 用户ID
            var uidString = collection["uid"];
            // 签证号
            var signNo = collection["signNo"];
            // 签证地
            var signNation = collection["signNation"];
            // 签证时间
            var signTimeString = collection["signTime"];
            // 是否出行
            var isOutString = collection["isOut"];

            /*
             * 参数校验
             */
            // 申请ID
            if(string.IsNullOrEmpty(aidString))
            {
                return ResponseUtil.Error(400, "申请ID不能为空");
            }
            else
            {
                if (!int.TryParse(aidString, out aid))
                {
                    return ResponseUtil.Error(400, "申请ID不正确");
                }
            }
            // 用户ID
            if(string.IsNullOrEmpty(uidString))
            {
                return ResponseUtil.Error(400, "用户ID不能为空");
            }
            else
            {
                if(!int.TryParse(uidString, out uid))
                {
                    return ResponseUtil.Error(400, "用户ID不正确");
                }
            }
            // 签证号
            if(string.IsNullOrEmpty(signNo))
            {
                return ResponseUtil.Error(400, "签证号不能为空");
            }
            // 签证时间
            if (!string.IsNullOrEmpty(signTimeString))
            {
                if (!DateTime.TryParse(signTimeString, out signTime))
                {
                    return ResponseUtil.Error(400, "签证时间格式错误");
                }
            }
            // 是否出行
            if (!string.IsNullOrEmpty(isOutString))
            {
                if(!bool.TryParse(isOutString, out isOut))
                {
                    return ResponseUtil.Error(400, "是否出行格式不正确");
                }
            }

            /*
             * 添加履历
             */
            using (var db = new YGSDbContext())
            {
                var history = db.History.Where(n => n.SignNo == signNo).FirstOrDefault();
                if(history != null)
                {
                    return ResponseUtil.Error(400, "签证号不能重复");
                }
                else
                {
                    history = new YGS_History();
                    history.ApplyId = aid;                      // 申请ID
                    history.UserId = uid;                       // 用户ID
                    history.SignNo = signNo;                    // 签证号
                    if (!string.IsNullOrEmpty(signTimeString))  // 签证时间
                    {
                        history.SignTime = signTime;
                    }
                    if (!string.IsNullOrEmpty(signNation))      // 签证地
                    {
                        history.SignNation = signNation;
                    }
                    history.IsOut = isOut;                      // 是否出行

                    db.History.Add(history);
                    db.SaveChanges();

                    return new JsonNetResult(new
                    {
                        code = 200,
                        data = new
                        {
                            id = history.ID
                        }
                    });
                }
            }
        }
        #endregion

        #region 删除履历
        public ActionResult Delete(int id)
        {
            /*
             * 查询履历
             */
            using (var db = new YGSDbContext())
            {
                var history = db.History.Where(n => n.ID == id).FirstOrDefault();

                if (history == null)
                {
                    return ResponseUtil.Error(400, "履历不存在");
                }
                else
                {
                    db.History.Remove(history);
                    db.SaveChanges();

                    return ResponseUtil.OK(200, "删除成功");
                }
            }
        }
        #endregion

        #region 更新履历
        [HttpPost]
        public ActionResult DoUpdate(FormCollection collection)
        {
            /*
             * 变量定义
             */
            // 履历ID
            int id = 0;
            // 签证时间
            DateTime signTime  = new DateTime();
            // 是否出行
            bool isOut = true;

            /*
             * 参数获取
             */
            // 履历ID
            var idString = collection["id"];
            // 签证号
            var signNo = collection["signNo"];
            // 签证地
            var signNation = collection["signNation"];
            // 签证时间
            var signTimeString = collection["signTime"];
            // 是否出行
            var isOutString = collection["isOut"];

            /*
             * 参数校验
             */
            // 履历ID
            if (string.IsNullOrEmpty(idString))
            {
                return ResponseUtil.Error(400, "履历ID不能为空");
            }
            else
            {
                if(!int.TryParse(idString, out id))
                {
                    return ResponseUtil.Error(400, "履历ID格式不正确");
                }
            }
            // 签证号
            if (string.IsNullOrEmpty(signNo))
            {
                return ResponseUtil.Error(400, "签证号不能为空");
            }
            // 签证时间
            if (!string.IsNullOrEmpty(signTimeString))
            {
                if (!DateTime.TryParse(signTimeString, out signTime))
                {
                    return ResponseUtil.Error(400, "签证时间格式错误");
                }
            }
            // 是否出行
            if (!string.IsNullOrEmpty(isOutString))
            {
                if (!bool.TryParse(isOutString, out isOut))
                {
                    return ResponseUtil.Error(400, "是否出行格式不正确");
                }
            }

            /*
             * 更新履历
             */
            using (var db = new YGSDbContext())
            {
                var history = db.History.Where(n => n.ID == id).FirstOrDefault();
                if (history == null)
                {
                    return ResponseUtil.Error(400, "履历不存在");
                }
                else
                {
                    history.SignNo = signNo;                    // 签证号
                    if (!string.IsNullOrEmpty(signTimeString))  // 签证时间
                    {
                        history.SignTime = signTime;
                    }
                    if (!string.IsNullOrEmpty(signNation))      // 签证地
                    {
                        history.SignNation = signNation;
                    }
                    history.IsOut = isOut;                      // 是否出行
                    db.SaveChanges();

                    return ResponseUtil.OK(200, "履历更新成功");
                }
            }
        }
        #endregion

        #region 履历详情
        public ActionResult Detail(int id)
        {
            using (var db = new YGSDbContext())
            {
                var history = db.History.Where(n => n.ID == id).FirstOrDefault();

                if (history == null)
                {
                    return ResponseUtil.Error(400, "履历不存在");
                }
                else
                {
                    return new JsonNetResult(new
                    {
                        code = 200,
                        data = new
                        {
                            id = history.ID,
                            signNo = history.SignNo,
                            signTime = history.SignTime,
                            signNation = history.SignNation,
                            isOut = history.IsOut
                        }
                    });
                }
            }
        }
        #endregion
    }
}