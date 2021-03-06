﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace QzoneAlbumDownloader
{
    public static class RequestHelper
    {

        public static string GetResponse(string url, string cookie = "", string referer = "", string encoding = "utf-8")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 60000;
                request.ReadWriteTimeout = 60000;
                request.Method = "GET";
                request.Referer = referer;
                request.Headers["Upgrade-Insecure-Requests"] = "1";
                request.Accept = "Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.Headers["Accept-Language"] = "zh-CN,zh;q=0.8";
                request.Headers["Accept-Charset"] = "GBK,utf-8;q=0.7,*;q=0.3";
                request.Headers["Cookie"] = cookie;
                request.UserAgent = "User-Agent:Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1";
                request.KeepAlive = true;
                request.ContentType = "application/x-www-form-urlencoded";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding));
                return myStreamReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
        }

        public static Stream GetResponse(string url, string cookie = "", string referer = "")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Referer = referer;
                request.Headers["Upgrade-Insecure-Requests"] = "1";
                request.Accept = "Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.Headers["Accept-Language"] = "zh-CN,zh;q=0.8";
                request.Headers["Accept-Charset"] = "GBK,utf-8;q=0.7,*;q=0.3";
                request.Headers["Cookie"] = cookie;
                request.UserAgent = "User-Agent:Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1";
                request.KeepAlive = true;
                request.ContentType = "application/x-www-form-urlencoded";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response.GetResponseStream();
            }
            catch
            {
                throw;
            }
        }


    }
}
