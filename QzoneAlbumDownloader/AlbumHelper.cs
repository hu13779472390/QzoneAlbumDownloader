﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace QzoneAlbumDownloader
{
    public static class AlbumHelper
    {

        /// <summary>
        /// 检测目标空间是否公开
        /// </summary>
        /// <param name="QQNumber"></param>
        /// <returns></returns>
        public static bool CheckIsPublic(string QQNumber)
        {
            try
            {
                string res = RequestHelper.GetResponse(
                    string.Format("http://photo.qq.com/fcgi-bin/fcg_list_album?uin={0}", QQNumber),
                    "", "", "GBK");
                string check_str = @"<err>\n<errCode>(.\d+?)</errCode>\n<errMsg>(.+?)</errMsg>\n<msg>(.+?)</msg>\n<ret>(.\d+?)</ret>\n</err>";
                Regex reg = new Regex(check_str);
                MatchCollection mc = reg.Matches(res);
                if (mc.Count == 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 检测是否拥有目标空间访问权限
        /// </summary>
        /// <param name="QQNumber"></param>
        /// <param name="Cookie"></param>
        /// <returns></returns>
        public static AccessState CheckHasAccess(string QQNumber, string Cookie, out string res)
        {
            try
            {
                res = RequestHelper.GetResponse(
                                string.Format("http://photo.qq.com/fcgi-bin/fcg_list_album?uin={0}", QQNumber),
                                Cookie, "", "GBK");
                if (res.IndexOf("<errCode>-900</errCode>") > 0)
                    return AccessState.NeedLogin;
                if (res.IndexOf("<errCode>-961</errCode>") > 0)
                    return AccessState.NoAccess;
                if (res.IndexOf("<errCode>-902</errCode>") > 0)
                    return AccessState.NumberError;
                return AccessState.OK;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 权限枚举
        /// </summary>
        public enum AccessState
        {
            OK,
            NeedLogin,
            NoAccess,
            NumberError,
            Error
        }

        /// <summary>
        /// 解析Data
        /// </summary>
        /// <returns></returns>
        public static List<AlbumInfo> ResolveAlbum(string data)
        {
            List<AlbumInfo> res = new List<AlbumInfo>();
            try
            {
                XElement xe = XElement.Parse(data);
                IEnumerable<XElement> elements = from ele in xe.Elements("album")
                                                 select ele;
                foreach (var ele in elements)
                {
                    AlbumInfo alb = new AlbumInfo();

                    res.Add(alb);
                }
            }
            catch
            {

            }
            return res;
        }

    }
}
