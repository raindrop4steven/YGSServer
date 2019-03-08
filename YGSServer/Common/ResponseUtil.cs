using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YGSServer.Common
{
    /// <summary>
    /// 响应帮助工具类
    /// </summary>
    public class ResponseUtil
    {
        /// <summary>
        /// 20x 无数据的响应
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static JsonNetResult OK(int code, string message)
        {
            return new JsonNetResult(new
            {
                code = code,
                data = new
                {
                    message = message
                } 
            });
        }

        /// <summary>
        /// 40x,50x无数据错误响应
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static JsonNetResult Error(int code, string message)
        {
            return new JsonNetResult(new
            {
                code = code,
                data = new
                {
                    message = message
                }
            });
        }
    }
}