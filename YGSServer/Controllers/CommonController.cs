using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YGSServer.Common;
using YGSServer.Models;

namespace YGSServer.Controllers
{
    public class CommonController : Controller
    {
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload()
        {
            try
            {
                // 个人上传文件夹检查
                var folder = System.Configuration.ConfigurationManager.AppSettings["UploadPath"];
                var folderPath = folder;
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 提取上传的文件
                HttpPostedFileBase file = Request.Files[0];

                // 原文件名 Test.txt
                var fileName = Path.GetFileName(file.FileName);
                // 最终文件名 yyyyMMddHHmmss+4random.txt
                var finalFileName = DiskUtil.GetFinalFileName(fileName);
                // 最终存储路径
                var path = Path.Combine(folderPath, finalFileName);

                // 保存文件
                file.SaveAs(path);

                // 判断上传的是反馈文件还是客户签字单
                var att = new YGS_Att();
                att.Name = fileName;
                att.Path = Path.Combine(finalFileName).Replace("\\", "/");

                using(var db = new YGSDbContext())
                {
                    db.Attachment.Add(att);
                    db.SaveChanges();
                }

                return new JsonNetResult(new
                {
                    status = 200,
                    data = new
                    {
                        id = att.ID,
                        name = att.Name,
                        path = Url.Action("Download", "Common", new { id = att.ID })
                    }
                });
            }
            catch
            {
                return ResponseUtil.Error(400, "上传出错");
            }
        }

        /// <summary>
        /// 查看图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Download(int id)
        {
            using(var db = new YGSDbContext())
            {
                var attach = db.Attachment.Where(a => a.ID == id).FirstOrDefault();
                if (attach == null)
                {
                    return ResponseUtil.Error(404, "图片不存在");
                }
                else
                {
                    var filename = attach.Path;
                    var fullUploadPath = System.Configuration.ConfigurationManager.AppSettings["UploadPath"];
                    string filepath = Path.Combine(fullUploadPath, filename);
                    byte[] filedata = System.IO.File.ReadAllBytes(filepath);
                    string contentType = MimeMapping.GetMimeMapping(filepath);

                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = attach.Name,
                        Inline = true,
                    };

                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(filedata, contentType);
                }
            }
        }
    }
}