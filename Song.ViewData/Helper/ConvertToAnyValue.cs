using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web;
using System.Security.Cryptography;

namespace Song.ViewData
{
    /// <summary>
    /// ��ĳ��ֵת��Ϊ������������
    /// </summary>
    public class ConvertToAnyValue
    {
        private object _paravlue;
        /// <summary>
        /// ������ԭʼֵ
        /// </summary>
        public string ParaValue
        {
            get
            {
                return _paravlue.ToString();
            }
            set
            {
                _paravlue = value;
            }
        }
        private string _parakey = "";
        /// <summary>
        /// �����ļ���
        /// </summary>
        public string ParaKey
        {
            get
            {
                return _parakey;
            }
            set
            {
                _parakey = value;
            }
        }
        private string _unit = "";
        /// <summary>
        /// �����ĵ�λ
        /// </summary>
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }
        public ConvertToAnyValue()
        {
        }
        public ConvertToAnyValue(object para)
        {
            this._paravlue = para;
        }
        public ConvertToAnyValue(string para)
        {
            this._paravlue = string.IsNullOrWhiteSpace(para) ? "" : para;
        }
        public ConvertToAnyValue(string para, string unit)
        {
            this._paravlue = para;
        }
        public static ConvertToAnyValue Get(string para)
        {
            return new ConvertToAnyValue(para);
        }
        /// <summary>
        /// ������Int16����ֵ��������������ڻ��쳣���򷵻�null;
        /// </summary>
        public int? Int16
        {
            get
            {
                if (_paravlue == null) return null;
                try
                {
                    return System.Convert.ToInt16(_paravlue);
                }
                catch
                {
                    Regex r = new Regex("\\d+");
                    Match ms = r.Match(this.String);
                    if (ms.Success)
                    {
                        string tm = ms.Groups[0].Value;
                        return System.Convert.ToInt16(tm);
                    }
                    return null;
                }
            }
        }
        /// <summary>
        /// ������Int32����ֵ��������������ڻ��쳣���򷵻�null;
        /// </summary>
        public int? Int32
        {
            get
            {
                if (_paravlue == null) return null;
                try
                {
                    return System.Convert.ToInt32(_paravlue);
                }
                catch
                {
                    Regex r = new Regex("\\d+");
                    Match ms = r.Match(this.String);
                    if (ms.Success)
                    {
                        string tm= ms.Groups[0].Value;
                        return System.Convert.ToInt32(tm);
                    }
                    return null;
                }
            }
        }
        /// <summary>
        /// ������Int64����ֵ��������������ڻ��쳣���򷵻�null;
        /// </summary>
        public long? Int64
        {
            get
            {
                if (_paravlue == null) return null;
                try
                {
                    return System.Convert.ToInt64(_paravlue);
                }
                catch
                {
                    Regex r = new Regex("\\d+");
                    Match ms = r.Match(this.String);
                    if (ms.Success)
                    {
                        string tm = ms.Groups[0].Value;
                        return System.Convert.ToInt64(tm);
                    }
                    return null;
                }
            }
        }
        /// <summary>
        /// ������Double����ֵ��������������ڻ��쳣���򷵻�null;
        /// </summary>
        public double? Double
        {
            get
            {
                if (_paravlue == null) return null;
                try
                {
                    return System.Convert.ToDouble(_paravlue);
                }
                catch
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// ������String����ֵ��������������ڻ��쳣���򷵻ؿ��ַ�������Null;
        /// </summary>
        public string String
        {
            get
            {
                return _paravlue == null ? "" : _paravlue.ToString().Trim();
            }
        }
        /// <summary>
        /// �����ı�����ֵ���Զ�ȥ��html��ǩ
        /// </summary>
        public string Text
        {
            get
            {
                string text = this.String;
                if (string.IsNullOrWhiteSpace(text)) return text;
                string strText = System.Text.RegularExpressions.Regex.Replace(text, "<[^>]+>", "");
                strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");
                return text;
            }
        }
        /// <summary>
        /// ������ֵ,���û�����ݣ�����Null
        /// </summary>
        public string Value
        {
            get
            {
                string text = this.String;
                if (string.IsNullOrWhiteSpace(text)) return null;
                return text;
            }
        }
        /// <summary>
        /// ������Boolean����ֵ��������������ڻ��쳣���򷵻�true;
        /// </summary>
        public bool? Boolean
        {
            get
            {
                if (_paravlue == null) return null;
                try
                {
                    return System.Convert.ToBoolean(_paravlue);
                }
                catch
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// ������DateTime����ֵ��������������ڻ��쳣���򷵻�null;
        /// </summary>
        public DateTime? DateTime
        {
            get
            {
                if (_paravlue == null) return null;
                try
                {
                    if (_paravlue is long)
                    {
                        long jsTimeStamp = 0;
                        long.TryParse(_paravlue.ToString(), out jsTimeStamp);
                        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // ����ʱ��
                        DateTime dt = startTime.AddMilliseconds(jsTimeStamp);
                        return dt;
                    }
                    else
                    {
                        return System.Convert.ToDateTime(_paravlue);
                    }
                }
                catch
                {
                    string t = _paravlue.ToString();
                    //���������ַ�ȫ������-
                    foreach (char c in t)
                        t += c >= 48 && c <= 57 ? c : '-';
                    string str = t;
                    //����ж��-���ߣ��򻻳�һ��
                    Regex re = new Regex(@"\-{1,}", RegexOptions.IgnorePatternWhitespace);
                    str = re.Replace(str, "-");
                    //���ǰ����-����ȥ��
                    re = new Regex(@"^\-{1,}(\w)", RegexOptions.IgnorePatternWhitespace);
                    str = re.Replace(str, "$1");
                    re = new Regex(@"(\w)\-{1,}$", RegexOptions.IgnorePatternWhitespace);
                    str = re.Replace(str, "$1");
                    //������Ϊ��λ����������λ��������λ��ȡǰ��λ
                    string year;
                    if (str.IndexOf('-') > -1)
                    {
                        year = str.Substring(0, str.IndexOf('-'));
                        year = year.Length == 1 ? "0" + year : year;
                        year = year.Length == 2 ? "19" + year : year;
                        year = year.Length > 4 ? year.Substring(0, 4) :
                        year; str = year + "-" + str.Substring(str.IndexOf('-') + 1);
                    }
                    else
                    {
                        str += "-1-1";
                    }
                    try
                    {
                        return System.Convert.ToDateTime(str);
                    }
                    catch
                    {
                        return null;
                    }
                }
                
            }
        }
        /// <summary>
        /// ������MD5����ֵ(Сд)��������������ڻ��쳣���򷵻�null;
        /// </summary>
        public string MD5
        {
            get
            {
                if (_paravlue == null) return "";
                if (string.IsNullOrWhiteSpace(_paravlue.ToString())) return "";
                System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] byteArray = Encoding.ASCII.GetBytes(_paravlue.ToString());
                byteArray = md5.ComputeHash(byteArray);
                string hashedValue = "";
                foreach (byte b in byteArray)
                {
                    hashedValue += b.ToString("x2");
                }
                return hashedValue;
            }
        }
        /// <summary>
        /// ������SHA1����ֵ��������������ڻ��쳣���򷵻�null;
        /// </summary>
        public string SHA1
        {
            get
            {
                if (_paravlue == null) return null;
                System.Security.Cryptography.SHA1 sha1 = new SHA1CryptoServiceProvider();//����SHA1����
                byte[] bytes_in = System.Text.Encoding.UTF8.GetBytes(_paravlue.ToString());//���������ַ���תΪbyte����
                byte[] bytes_out = sha1.ComputeHash(bytes_in);//Hash����
                sha1.Dispose();//�ͷŵ�ǰʵ��ʹ�õ�������Դ
                string result = BitConverter.ToString(bytes_out);//��������תΪstring����
                return result.Replace("-","");   
            }
        }
        /// <summary>
        /// �������ַ������� URL ���벢�����ѽ�����ַ�����������������ڻ��쳣���򷵻�null;
        /// </summary>
        public string UrlDecode
        {
            get
            {
                if (_paravlue == null) return null;
                return System.Web.HttpUtility.UrlDecode(_paravlue.ToString().Trim());
            }
        }
        public string UrlEncode
        {
            get
            {
                if (_paravlue == null) return null;
                return System.Web.HttpUtility.UrlEncode(_paravlue.ToString().Trim());
            }
        }
        /// <summary>
        /// �Ծ���HTML ����Ĳ������н��룬�������ѽ�����ַ�����������������ڻ��쳣���򷵻�null;
        /// </summary>
        public string HtmlDecode
        {
            get
            {
                if (_paravlue == null) return null;
                System.Web.HttpContext _context = System.Web.HttpContext.Current;
                return _context.Server.HtmlDecode(_paravlue.ToString());
            }
        }
        /// <summary>
        /// ת��Ϊ����·��
        /// </summary>
        public string MapPath
        {
            get
            {
                System.Web.HttpContext _context = System.Web.HttpContext.Current;
                return _context.Server.MapPath(_paravlue.ToString());
            }
        }
        /// <summary>
        /// ת������·��
        /// </summary>
        public string VirtualPath
        {
            get
            {
                string path = _paravlue.ToString();
                path = path.Replace("\\", "/");
                path = path.Replace("~/", System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                //��·��ĩβ����\\
                if (path.IndexOf("/") > -1)
                {
                    if (path.Substring(path.LastIndexOf("/")).IndexOf(".") < 0)
                        if (path.Substring(path.Length - 1) != "/") path += "/";
                }
                path = Regex.Replace(path, @"\/+", "/");
                return path;
                
            }
        }
        /// <summary>
        /// ���ַ����ֲ������
        /// </summary>
        /// <param name="split">���ڷֲ���ַ�</param>
        /// <returns></returns>
        public string[] Split(char split)
        {
            return _paravlue.ToString().Split(split);
        }
        /// <summary>
        /// ��C#ʱ��ת����Javascript��ʱ��
        /// </summary>
        [Obsolete] 
        public string JavascriptTime
        {
            get
            {
                if (_paravlue == null) return "";
                try
                {
                    System.DateTime time = this.DateTime ?? TimeZone.CurrentTimeZone.ToLocalTime(System.DateTime.Now);
                    string fmtDate = "ddd MMM d HH:mm:ss 'UTC'zz'00' yyyy";
                    CultureInfo ciDate = CultureInfo.CreateSpecificCulture("en-US");
                    //��C#ʱ��ת����JSʱ���ַ���    
                    string JSstring = time.ToString(fmtDate, ciDate);
                    return JSstring;
                }
                catch
                {
                    return "";
                }

            }
        }
        /// <summary>
        /// JavaScriptʱ�������ָ��������ʱ��1970��01��01��00ʱ00��00��(����ʱ��1970��01��01��08ʱ00��00��)�������ڵ��ܺ�������
        /// </summary>
        public long TimeStamp
        {
            get
            {
                System.DateTime time = this.DateTime ?? System.DateTime.Now;
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // ����ʱ��
                long timeStamp = (long)(time - startTime).TotalMilliseconds; // ��������
                return timeStamp;
            }
        }
        /// <summary>
        /// תΪָ�������ݿ�����
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object ChangeType(System.Type type)
        {
            object obj=null;
            switch (type.FullName)
            {
                case "System.DateTime":
                    obj = this.DateTime;
                    break;
                default:
                    obj = Convert.ChangeType(_paravlue, type);
                    break;
            }
            return obj;
        }

        /// <summary>
        /// �����ַ���,Ĭ����ԿΪ��ǰ����
        /// </summary>
        /// <returns></returns>
        public ConvertToAnyValue Decrypt()
        {
            string ret = string.Empty;
            ret = WeiSha.Common.DataConvert.DecryptForBase64(this.UrlDecode);
            return new ConvertToAnyValue(ret);
        }
    }
}
