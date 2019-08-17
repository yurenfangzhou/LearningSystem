﻿using Song.ViewData.Attri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Song.ViewData
{
    /// <summary>
    /// 执行具体的方法
    /// </summary>
    public class ExecuteMethod
    {

        #region 单件模式
        private static readonly ExecuteMethod _instance = new ExecuteMethod();
        private ExecuteMethod() { }
        /// <summary>
        /// 返回实例
        /// </summary>
        /// <returns></returns>
        public static ExecuteMethod GetInstance()
        {
            return _instance;
        }
        #endregion

        #region 创建并缓存实例对象
        //存储对象的键值对，key为对象的类名称（全名），value为对象自身
        private Dictionary<string, object> _objects = new Dictionary<string, object>();
        /// <summary>
        /// 创建对象，如果存在，则直接返回；如果不存在，创建后返回
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public static object CreateInstance(Letter letter)
        {
            string assemblyName = "Song.ViewData";
            string classFullName = String.Format("{0}.Methods.{1}", assemblyName, letter.ClassName);
            object obj = null;
            //由缓存中查找，是否存在
            ExecuteMethod curr = ExecuteMethod.GetInstance();
            foreach (KeyValuePair<string, object> kv in curr._objects)
            {
                if (classFullName.Trim().Equals(kv.Key, StringComparison.CurrentCultureIgnoreCase))
                {
                    obj = kv.Value;
                    break;
                }
            }
            if (obj != null) return obj;
            //如果之前未创建，则重新创建
            Type type = null;
            Assembly assembly = Assembly.Load(assemblyName);
            foreach (Type info in assembly.GetTypes())
            {
                if (info.FullName.Equals(classFullName, StringComparison.CurrentCultureIgnoreCase))
                {
                    type = info;
                    break;
                }
            }
            if (type == null) throw new Exception(
                string.Format("调用的对象'{0}'不存在, 可能是'{1}'拼写错误",
                classFullName, letter.ClassName));
            obj = System.Activator.CreateInstance(type);    //创建对象
            ExecuteMethod.GetInstance()._objects.Add(type.FullName, obj);   //记录到缓存
            return obj;
        }
        #endregion
        
        /// <summary>
        /// 执行，按实际结果返回
        /// </summary>
        /// <param name="letter">客户端递交的参数信息</param>
        /// <returns></returns>
        public static object Exec(Letter letter)
        {
            //1.创建对象,即$api.get("account/single")中的account
            object execObj = ExecuteMethod.CreateInstance(letter);
            //2.获取要执行的方法，即$api.get("account/single")中的single
            MethodInfo method = getMethod(execObj.GetType(), letter);
            //3#.验证方法的特性,一是验证Http动词，二是验证是否登录后操作，三是验证权限   
 
            //4.构建执行该方法所需要的参数
            object[] parameters = getInvokeParam(method, letter);
            //5.执行方法
            return method.Invoke(execObj, parameters);
        }
        /// <summary>
        /// 执行，返回结构封装到DataResult对象中
        /// </summary>
        /// <param name="letter">客户端递交的参数信息</param>
        /// <returns></returns>
        public static DataResult ExecToResult(Letter letter)
        {
            try
            {
                object res = Exec(letter);
                if (res is ListResult) return (ListResult)res;  //如果是分页数据
                return new Song.ViewData.DataResult(res);       //普通数据
            }
            catch (Exception ex)
            {
                return new Song.ViewData.DataResult(ex);
            }
        }
        /// <summary>
        /// 根据方法名、参数数量，获取具体要执行的方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private static MethodInfo getMethod(Type type, Letter p)
        {
            //1、先将名称匹配的方法收集起来
            List<MethodInfo> list = new List<MethodInfo>();
            MethodInfo[] methods = type.GetMethods();   
            for (int i = 0; i < methods.Length; i++)
            {
                if (p.MethodName.Equals(methods[i].Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    list.Add(methods[i]);
                }
            }
            if (list.Count < 1) throw new Exception(string.Format("调用方法'{0}.{1}'不存在", p.ClassName, p.MethodName));
            //2、判断方法的参数名称，是否与传递来的参数名称匹配
            //并不要求两者的参数数量相同，也不要求数据类型相同
            MethodInfo method = null;
            foreach (MethodInfo mi in list)
            {
                //ParameterInfo[] paramInfos = mi.GetParameters();
                bool ismatch = true;    //是否匹配
                foreach (ParameterInfo pi in mi.GetParameters())
                {
                    //如果参数是Parameter类型，则跳过匹配
                    if (p.GetType().FullName.Equals(pi.ParameterType.FullName)) continue;
                    //只要有一个参数不匹配，即中断
                    if (!p.ExistParameter(pi.Name))
                    {
                        ismatch = false;
                        break;
                    }
                }
                if (ismatch) method = mi;
            }
            if (method == null) throw new Exception(
                string.Format("所调用方法'{0}.{1}'的参数名称与实际传参不匹配；实际传参：{2}",
                type.FullName, p.MethodName,
                p.ToString() == string.Empty ? "null" : p.ToString()));
            return method;
        }
        /// <summary>
        /// 返回方法执行所需要的参数
        /// </summary>
        /// <param name="method">要执行的方法</param>
        /// <param name="p">传递来的参数</param>
        /// <returns></returns>
        private static object[] getInvokeParam(MethodInfo method, Letter p)
        {
            ParameterInfo[] paramInfos = method.GetParameters();
            object[] objs = new object[paramInfos.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                ParameterInfo pi = paramInfos[i];
                //如果参数是Parameter类型，则直接赋值
                if (p.GetType().FullName.Equals(pi.ParameterType.FullName))
                {
                    objs[i] = p;
                }
                else
                {
                    //普通参数，不是输出参数
                    if (!pi.IsOut)
                    {                       
                        object val = p[pi.Name].String;
                        objs[i] = Convert.ChangeType(val, pi.ParameterType);
                    }
                    else
                    {
                        objs[i] = null;
                    }
                }
            }
            return objs;
        }
    }
}
