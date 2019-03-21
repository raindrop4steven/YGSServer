using System;
using System.Collections.Generic;
using System.Linq;
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
             * 参数获取
             */
            // 姓名
            var name = collection["name"];
            // 身份证号
            var credNo = collection["credNo"];

            /*
             * 参数校验
             */
            // 姓名
            if (string.IsNullOrEmpty(name))
            {
                return ResponseUtil.Error(400, "姓名不能为空");
            }

            /*
             * 查询重复
             */
            using (var db = new YGSDbContext())
            {
                // 外出履历
                var history = new YGS_History();

                // 身份证号
                if (string.IsNullOrEmpty(credNo))
                {
                    // 身份证号为空，说明只填写了姓名，直接建立新用户，返回新用户ID给前端
                    var tempUser = new YGS_User();
                    tempUser.Name = name;
                    tempUser.CreateTime = DateTime.Now;

                    db.User.Add(tempUser);
                    db.SaveChanges();

                    // 添加履历
                    history.UserId = tempUser.ID;
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
                else
                {
                    // 输入了身份证号，则进行校验，身份信息是否正确。
                    // 如果身份证与姓名一致，则返回用户ID，如果不正确，则该用户已存在，输入身份证重复
                    var user = db.User.Where(n => n.CredNo == credNo).FirstOrDefault();
                    if (user == null)
                    {
                        user = new YGS_User();
                        user.Name = name;
                        user.CredNo = credNo;
                        user.CreateTime = DateTime.Now;

                        db.User.Add(user);
                        db.SaveChanges();

                        // 添加履历
                        history.UserId = user.ID;
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
                    else
                    {
                        if (user.Name == name)
                        {
                            // 同一个人，直接添加履历
                            history.UserId = user.ID;
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
                        else
                        {
                            // 不同人，同身份证
                            return ResponseUtil.Error(400, "相同身份证的用户已存在");
                        }
                    }
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

            /*
             * 参数获取
             */
            // 履历ID
            var hid = collection["id"];
            // 姓名
            var name = collection["name"];
            // 身份证号
            var credNo = collection["credNo"];
            // 签证号
            var signNo = collection["signNo"];
            // 签证时间
            var signTimeString = collection["signTime"];

            /*
             * 参数校验
             */
            // 履历ID
            if(string.IsNullOrEmpty(hid))
            {
                return ResponseUtil.Error(400, "履历ID不能为空");
            }
            else
            {
                if(!int.TryParse(hid, out id))
                {
                    return ResponseUtil.Error(400, "履历ID不正确");
                }
            }
            // 姓名
            if (string.IsNullOrEmpty(name))
            {
                return ResponseUtil.Error(400, "请填写姓名");
            }
            // 身份证必填
            if(string.IsNullOrEmpty(credNo))
            {
                return ResponseUtil.Error(400, "请填写身份证号");
            }
            // 签证时间
            if(!string.IsNullOrEmpty(signTimeString))
            {
                if(!DateTime.TryParse(signTimeString, out signTime))
                {
                    return ResponseUtil.Error(400, "签证时间格式错误");
                }
            }

            /*
             * 查询目标履历
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
                    // 查询履历对应的用户
                    var user = db.User.Where(n => n.ID == history.UserId).FirstOrDefault();
                    if (user == null)
                    {
                        return ResponseUtil.Error(400, "履历对应用户不存在");
                    }
                    else
                    {
                        // 检测是否更新了身份证
                        if (user.CredNo == credNo)
                        {
                            // 直接更新用户信息即可
                            user.Name = name;
                            // 签证号
                            if (!string.IsNullOrEmpty(signNo))
                            {
                                history.SignNo = signNo;
                            }
                            // 签证时间
                            if (!string.IsNullOrEmpty(signTimeString))
                            {
                                history.SignTime = signTime;
                            }
                            db.SaveChanges();
                            return ResponseUtil.OK(200, "更新成功");
                        }
                        else
                        {
                            // 身份证不同，2种可能：
                            // 1.如果系统中不存在该用户，则更新用户信息。
                            // 2.系统中存在CredNo，则判断是否名称也相同则替换该用户为已有用户。
                            var searchUser = db.User.Where(n => n.CredNo == credNo).FirstOrDefault();
                            if (searchUser == null)
                            {
                                user.Name = name;
                                user.CredNo = credNo;
                                // 签证号
                                if (!string.IsNullOrEmpty(signNo))
                                {
                                    history.SignNo = signNo;
                                }
                                // 签证时间
                                if (!string.IsNullOrEmpty(signTimeString))
                                {
                                    history.SignTime = signTime;
                                }
                                db.SaveChanges();
                                return ResponseUtil.OK(200, "更新成功");
                            }
                            else
                            {
                                // 判断输入的人姓名和原有用户姓名是否一致
                                if(searchUser.Name != user.Name)
                                {
                                    return ResponseUtil.Error(400, "输入用户与现有用户冲突");
                                }
                                else
                                {
                                    history.UserId = searchUser.ID;
                                    db.SaveChanges();
                                    return ResponseUtil.OK(200, "更新成功");
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}