using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YGSServer.Common;
using YGSServer.Models;

namespace YGSServer.Controllers
{
    public class UserController : Controller
    {
        #region 用户一览
        [HttpPost]
        public ActionResult List(FormCollection collection)
        {
            using(var db = new YGSDbContext())
            {
                /*
                 * 变量定义
                 */
                // 页数
                int page = WHConstants.Default_Page;
                // 分页大小
                int pageSize = WHConstants.Default_Page_Size;
                // 性别
                int sex = 0;
                // 用户列表
                var userList = db.User.Where(n => n.ID > 0);

                /*
                 * 参数获取
                 */
                // 姓名
                var name = collection["name"];
                // 性别
                var sexString = collection["sex"];
                // 身份证号
                var credNo = collection["credNo"];
                // 页数
                var pageString = collection["page"];
                // 分页大小
                var pageSizeString = collection["pageSize"];

                /*
                 * 参数校验
                 */
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
                // 验证姓名
                if (!string.IsNullOrEmpty(name))
                {
                    userList = userList.Where(n => n.Name.Contains(name));
                }
                // 验证性别
                if (!string.IsNullOrEmpty(sexString))
                {
                    if (!int.TryParse(sexString, out sex))
                    {
                        return ResponseUtil.Error(400, "性别错误");
                    }
                    else
                    {
                        userList = userList.Where(n => n.Sex == sex);
                    }
                }
                // 验证身份证号
                if(!string.IsNullOrEmpty(credNo))
                {
                    userList = userList.Where(n => n.CredNo.Contains(credNo));
                }

                /*
                 * 分页及数据获取
                 */
                // 记录总数
                // 记录总数
                var totalCount = userList.Count();
                // 记录总页数
                var totalPage = (int)Math.Ceiling((float)totalCount / pageSize);
                // 查询结果数据
                var resultRecords = userList.OrderByDescending(n => n.CreateTime.Value).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                
                //预约列表格式
                List<object> users = new List<object>();

                foreach (var user in resultRecords)
                {
                    users.Add(new
                    {
                        id = user.ID,
                        credNo = user.CredNo,
                        name = user.Name,
                        sex = user.Sex,
                        unit = user.Unit,
                        depart = user.Depart,
                        level = user.Level,
                        duty = user.Duty
                    });
                }

                return new JsonNetResult(new
                {
                    code = 200,
                    data = new
                    {
                        users = users,
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

        #region 用户提交新增
        [HttpPost]
        public ActionResult DoAdd(FormCollection collection)
        {
            /*
             * 变量定义
             */
            // 性别
            int sex = 0;
            // 出生日期
            DateTime birthday = new DateTime();
            /*
             * 参数获取
             */
            // 姓名
            var name = collection["name"];
            // 性别
            var sexString = collection[@"sex"];
            // 出生地
            var location = collection["location"];
            // 出生日期
            var birthDay = collection["birthDay"];
            // 身份证号
            var credNo = collection["credNo"];
            // 工作单位
            var unit = collection["unit"];
            // 工作部门
            var depart = collection["depart"];
            // 级别
            var level = collection["level"];
            // 职务
            var duty = collection["duty"];

            /*
             * 参数校验
             */
            // 姓名
            if(string.IsNullOrEmpty(name))
            {
                return ResponseUtil.Error(400, "姓名不能为空");
            }
            // 性别
            if(string.IsNullOrEmpty(sexString))
            {
                return ResponseUtil.Error(400, "性别不能为空");
            }
            else
            {
                if (int.TryParse(sexString, out sex))
                {
                    if (sex != 0 && sex != 1)
                    {
                        return ResponseUtil.Error(400, "性别错误");
                    }
                }
                else
                {
                    return ResponseUtil.Error(400, "性别错误");
                }
            }
            // 出生地
            if (string.IsNullOrEmpty(location))
            {
                return ResponseUtil.Error(400, "出生地不能为空");
            }
            // 出生日期
            if (string.IsNullOrEmpty(birthDay))
            {
                return ResponseUtil.Error(400, "出生日期不能为空");
            }
            else
            {
                if(!DateTime.TryParse(birthDay, out birthday))
                {
                    return ResponseUtil.Error(400, "出生日期无效");
                }
            }
            // 身份证号
            if (string.IsNullOrEmpty(credNo))
            {
                return ResponseUtil.Error(400, "身份证号不能为空");
            }
            // 工作单位
            if (string.IsNullOrEmpty(unit))
            {
                return ResponseUtil.Error(400, "工作单位不能为空");
            }
            // 工作部门
            if (string.IsNullOrEmpty(depart))
            {
                return ResponseUtil.Error(400, "工作部门不能为空");
            }
            // 级别
            if (string.IsNullOrEmpty(level))
            {
                return ResponseUtil.Error(400, "级别不能为空");
            }
            // 职务
            if (string.IsNullOrEmpty(duty))
            {
                return ResponseUtil.Error(400, "职务不能为空");
            }

            /*
             * 添加用户
             */
            using (var db = new YGSDbContext())
            {
                var user = db.User.Where(n => n.CredNo == credNo).FirstOrDefault();
                if(user == null)
                {
                    user = new YGS_User();
                    user.Name = name;
                    user.Sex = sex;
                    user.Location = location;
                    user.BirthDay = birthday;
                    user.CredNo = credNo;
                    user.Unit = unit;
                    user.Depart = depart;
                    user.Level = level;
                    user.Duty = duty;
                    user.CreateTime = DateTime.Now;
                    user.UpdateTime = DateTime.Now;
                    db.User.Add(user);
                    db.SaveChanges();

                    return ResponseUtil.OK(200, "创建成功！");
                }
                else
                {
                    return ResponseUtil.Error(400, "用户已存在");
                }
            }

        }
        #endregion

        #region 添加临时用户
        [HttpPost]
        public ActionResult DoAddTempUser(FormCollection collection)
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
            // 身份证号
            if(string.IsNullOrEmpty(credNo))
            {
                return ResponseUtil.Error(400, "身份证号不能为空");
            }

            /*
             * 查询重复
             */
            using (var db = new YGSDbContext())
            {
                var user = db.User.Where(n => n.CredNo == credNo).FirstOrDefault();
                if (user == null)
                {
                    user = new YGS_User();
                    user.Name = name;
                    user.CredNo = credNo;
                    user.CreateTime = DateTime.Now;

                    db.User.Add(user);
                    db.SaveChanges();

                    return new JsonNetResult(new
                    {
                        code = 200,
                        data = new
                        {
                            id = user.ID
                        }
                    });
                }
                else
                {
                    return ResponseUtil.Error(400, "用户已存在");
                }
            }
        }
        #endregion

        #region 用户更新
        public ActionResult Update(int id)
        {
            /*
             * 查询目标用户
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

        #region 用户提交更新
        [HttpPost]
        public ActionResult DoUpdate(FormCollection collection)
        {
            /*
             * 变量定义
             */
            // 用户ID
            int id = 0;
            // 性别
            int sex = 0;
            // 出生日期
            DateTime birthday = new DateTime();
            /*
             * 参数获取
             */
            // 用户ID
            var userId = collection["id"];
            // 姓名
            var name = collection["name"];
            // 性别
            var sexString = collection[@"sex"];
            // 出生地
            var location = collection["location"];
            // 出生日期
            var birthDay = collection["birthDay"];
            // 身份证号
            var credNo = collection["credNo"];
            // 工作单位
            var unit = collection["unit"];
            // 工作部门
            var depart = collection["depart"];
            // 级别
            var level = collection["level"];
            // 职务
            var duty = collection["duty"];

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
                if(!int.TryParse(userId, out id))
                {
                    return ResponseUtil.Error(400, "用户ID不正确");
                }
            }
            // 姓名
            if (string.IsNullOrEmpty(name))
            {
                return ResponseUtil.Error(400, "姓名不能为空");
            }
            // 性别
            if (string.IsNullOrEmpty(sexString))
            {
                return ResponseUtil.Error(400, "性别不能为空");
            }
            else
            {
                if (int.TryParse(sexString, out sex))
                {
                    if (sex != 0 && sex != 1)
                    {
                        return ResponseUtil.Error(400, "性别错误");
                    }
                }
                else
                {
                    return ResponseUtil.Error(400, "性别错误");
                }
            }
            // 出生地
            if (string.IsNullOrEmpty(location))
            {
                return ResponseUtil.Error(400, "出生地不能为空");
            }
            // 出生日期
            if (string.IsNullOrEmpty(birthDay))
            {
                return ResponseUtil.Error(400, "出生日期不能为空");
            }
            else
            {
                if (!DateTime.TryParse(birthDay, out birthday))
                {
                    return ResponseUtil.Error(400, "出生日期无效");
                }
            }
            // 身份证号
            if (string.IsNullOrEmpty(credNo))
            {
                return ResponseUtil.Error(400, "身份证号不能为空");
            }
            // 工作单位
            if (string.IsNullOrEmpty(unit))
            {
                return ResponseUtil.Error(400, "工作单位不能为空");
            }
            // 工作部门
            if (string.IsNullOrEmpty(depart))
            {
                return ResponseUtil.Error(400, "工作部门不能为空");
            }
            // 级别
            if (string.IsNullOrEmpty(level))
            {
                return ResponseUtil.Error(400, "级别不能为空");
            }
            // 职务
            if (string.IsNullOrEmpty(duty))
            {
                return ResponseUtil.Error(400, "职务不能为空");
            }

            /*
             * 更新用户信息
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
                    var credUser = db.User.Where(n => n.CredNo == credNo).FirstOrDefault();
                    if (credUser != null && credUser.ID != user.ID)
                    {
                        return ResponseUtil.Error(400, "相同身份证号用户已存在");
                    }
                    else
                    {
                        user.Name = name;
                        user.Sex = sex;
                        user.Location = location;
                        user.BirthDay = birthday;
                        user.CredNo = credNo;
                        user.Unit = unit;
                        user.Depart = depart;
                        user.Level = level;
                        user.Duty = duty;
                        user.UpdateTime = DateTime.Now;
                        db.SaveChanges();
                        return ResponseUtil.OK(200, "更新成功");
                    }
                }
            }
        }
        #endregion

        #region 用户查看详情
        public ActionResult Detail(int id)
        {
            /*
             * 获取用户
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
                            id = user.ID,
                            name = user.Name,
                            sex = user.Sex,
                            location = user.Location,
                            birthDay = user.BirthDay == null ? null : user.BirthDay.Value.ToString("yyyy/MM/dd"),
                            credNo = user.CredNo,
                            unit = user.Unit,
                            depart = user.Depart,
                            level = user.Level,
                            duty = user.Duty
                        }
                    });
                }
            }
        }
        #endregion

        #region 用户删除
        public ActionResult Delete(FormCollection collection)
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
            var userId = collection["id"];

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

            /*
             * 用户查询
             */
            using (var db = new YGSDbContext())
            {
                var user = db.User.Where(n => n.ID == id).FirstOrDefault();
                if (user == null)
                {
                    return ResponseUtil.Error(400, "用户不存在");
                }
                else
                {
                    // 检查该用户是否有护照信息，也要一并删除
                    var credList = db.Cred.Where(n => n.UserID == id).ToList();
                    db.Cred.RemoveRange(credList);
                    db.User.Remove(user);
                    db.SaveChanges();

                    return ResponseUtil.OK(200, "用户删除成功");
                }
            }
        }
        #endregion
    }
}
 