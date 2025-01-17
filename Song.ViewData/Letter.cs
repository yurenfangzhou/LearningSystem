﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Song.ViewData
{
    /// <summary>
    /// 记录客户端递交来的信息
    /// </summary>
    public class Letter : IEnumerable
    {
        #region 属性
        /// <summary>
        /// HTTP请求谓词，即Get、Post、Put等
        /// </summary>
        public string REQUEST_METHOD { get; set; }
        /// <summary>
        /// 所请求接口的类名称
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 所请求接口的方法
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// id参数，id是作为普通参数，因为常用，所以单独列出
        /// </summary>
        public int ID { get; set; }

        private Dictionary<string, string> _params = new Dictionary<string, string>();
        /// <summary>
        /// 参数集，包括Get和Post，即地址栏参数和form参数；
        /// 如果参数名称有重名，后者会替换前者的值
        /// </summary>
        public Dictionary<string, string> Params
        {
            get
            {
                return _params;
            }
            set
            {
                _params = value;
            }
        }
        private Dictionary<string, string> _cookies = new Dictionary<string, string>();
        /// <summary>
        /// Cookies参数集
        /// </summary>
        public Dictionary<string, string> Cookies
        {
            get
            {
                return _cookies;
            }
            set
            {
                _cookies = value;
            }
        }
        /// <summary>
        /// 服务器端的信息
        /// </summary>
        public Song.ViewData.Server Sever
        {
            get
            {
                return Song.ViewData.Server.GetServer();
            }
        }
        /// <summary>
        /// 请求的来源，即客户端
        /// </summary>
        public Song.ViewData.Browser From
        {
            get
            {
                return Song.ViewData.Browser.GetBrowser();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造函数，来自MVC控制器的调用，客户端采用$api时会经过MVC控制器
        /// 通过此种方式，会获取get、post参数，并从当前线程获取cookies集合
        /// </summary>
        /// <param name="httprequest">api控制器的访问对象</param>
        public Letter(HttpContext context)
        {
            HttpRequest request = context.Request;//定义传统request对象
            REQUEST_METHOD = request.HttpMethod;
            //从请求地址中，分析类名与方法名
            //string[] arr = httprequest.RequestUri.Segments;
            string[] arr = request.Url.Segments;
            //获取类名与方法名
            string clasname = arr[3];
            string action = arr[4];
            if (clasname.EndsWith("/")) clasname = clasname.Substring(0, clasname.LastIndexOf("/"));
            if (action.EndsWith("/")) action = action.Substring(0, action.LastIndexOf("/"));
            this.ClassName = clasname;
            this.MethodName = action;
            //获取参数         
            
            //获取get参数
            for (int i = 0; i < context.Request.QueryString.Count; i++)
            {
                string key = context.Request.QueryString.Keys[i].ToString().Trim();
                string val = Microsoft.JScript.GlobalObject.unescape(context.Request.QueryString[i].ToString().Trim());
                if (_params.ContainsKey(key))
                    _params[key] = val;
                else
                    _params.Add(key, val);
            }
            //获取post参数，put,delete,patch,options都从这里获取
            for (int i = 0; i < context.Request.Form.Count; i++)
            {
                string key = context.Request.Form.Keys[i].ToString().Trim();
                string val = Microsoft.JScript.GlobalObject.unescape(context.Request.Form[i].ToString().Trim());
                if (_params.ContainsKey(key))
                    _params[key] = val;
                else
                    _params.Add(key, val);
            }
            this.ID = this["id"].Int32 ?? 0;
            //获取cookies
            for (int i = 0; i < context.Request.Cookies.Count; i++)
            {
                string key = context.Request.Cookies.Keys[i].ToString();
                string val = context.Request.Cookies[i].Value.ToString();
                if (_cookies.ContainsKey(key))
                    _cookies[key] = val;
                else
                    _cookies.Add(key, val);
            }
        }
        /// <summary>
        /// 构造方法，直接用字符串传递参数，服务器端@Api调用时用此方法
        /// 通过此种方式，利用参数2（string paramseters）解析所需参数，并从当前线程获取cookies集合
        /// </summary>
        /// <param name="method">格式：类名/方法名</param>
        /// <param name="paramseters">格式："p1:str,p2:str"</param>
        public Letter(string method, string paramseters)
        {
            //获取类名和方法
            string classname = string.Empty, methodname = string.Empty;
            if (method.IndexOf("/") > -1)
            {
                classname = method.Substring(0, method.LastIndexOf("/"));
                methodname = method.Substring(method.LastIndexOf("/") + 1);
            }
            this.ClassName = classname;
            this.MethodName = methodname;
            //解析参数
            string rep = DateTime.Now.Ticks.ToString();
            paramseters = paramseters.Replace(@"\,", rep);      //将\,逗号处理一下
            foreach (string item in paramseters.Split(','))
            {
                string t = item.Replace(rep, @",");     //将逗号再弄回来
                t = t.Replace(@"\:", rep);
                string[] arr = t.Split(':');
                if (arr.Length < 2) continue;
                arr[1] = arr[1].Replace(rep, @":").Trim();
                //去除参数的前后单引号
                if (arr[1].StartsWith("'")) arr[1] = arr[1].Substring(1);
                if (arr[1].EndsWith("'")) arr[1] = arr[1].Length > 1 ? arr[1].Substring(0, arr[1].Length - 1) : "";
                _params.Add(arr[0].Trim(), arr[1].Trim());
            }
            this.ID = this["id"].Int32 ?? 0;
            //获取cookies       
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            for (int i = 0; i < context.Request.Cookies.Count; i++)
            {
                string key = context.Request.Cookies.Keys[i].ToString();
                string val = context.Request.Cookies[i].Value.ToString();
                if (_cookies.ContainsKey(key))
                    _cookies[key] = val;
                else
                    _cookies.Add(key, val);
            }
        }
        #endregion

        #region 获取参数的方法
        /// <summary>
        /// 获取参数的值，等同GetParameter(string key)方法
        /// </summary>
        /// <param name="key">参数名</param>
        /// <returns></returns>
        public ConvertToAnyValue this[string key]
        {
            get
            {
                return GetParameter(key);
            }
        }
        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="key">参数名</param>
        /// <returns>参数Value值</returns>
        public ConvertToAnyValue GetParameter(string key)
        {
            string val = string.Empty;
            foreach (KeyValuePair<string, string> kv in _params)
            {
                if (key.Trim().Equals(kv.Key, StringComparison.CurrentCultureIgnoreCase))
                {
                    val = kv.Value;
                    break;
                }
            }
            return new ConvertToAnyValue(val);
        }
        /// <summary>
        /// 是否存在某参数
        /// </summary>
        /// <param name="key">参数的Key值</param>
        /// <returns>参数Value值</returns>
        public bool ExistParameter(string key)
        {
            if (_params.ContainsKey(key))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取cookie值
        /// </summary>
        /// <param name="key">cookie名称</param>
        /// <returns>cookie的值</returns>
        public ConvertToAnyValue GetCookie(string key)
        {
            string val = string.Empty;
            foreach (KeyValuePair<string, string> kv in _cookies)
            {
                if (key.Equals(kv.Key, StringComparison.CurrentCultureIgnoreCase))
                {
                    val = kv.Value;
                    break;
                }
            }
            return new ConvertToAnyValue(val);
        }
        #endregion
        /// <summary>
        /// 重写ToString方法，将参数串连成字符串
        /// </summary>
        /// <returns>格式：key=value;</returns>
        public override string ToString()
        {
            string str = string.Empty;
            foreach (KeyValuePair<string, string> kv in _params)
            {
                str += kv.Key + "=" + kv.Value + ";";
            }
            return str;
        }
        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            foreach (KeyValuePair<string, string> kv in _params)
            {
                yield return kv;
            }
        }
    }
}
