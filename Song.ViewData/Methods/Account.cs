﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Mvc;
using Song.Entities;
using Song.ServiceInterfaces;
using Song.ViewData.Attri;
using WeiSha.Common;


namespace Song.ViewData.Methods
{
    
    /// <summary>
    /// 管理账号
    /// </summary>
    [HttpGet]
    public class Account
    {
        [HttpGet(IsAllow = false)]
        public Song.Entities.Accounts ForID(int id)
        {
            Song.Entities.Accounts acc= Business.Do<IAccounts>().AccountsSingle(id);
            acc.Ac_Pw = string.Empty;
            return acc;
        }
        /// <summary>
        /// 根据账号获取学员
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public Song.Entities.Accounts ForAcc(string acc)
        {
            Song.Entities.Accounts account = Business.Do<IAccounts>().AccountsSingle(acc, -1);
            account.Ac_Pw = string.Empty;
            return account;
        }
        /// <summary>
        /// 根据名称获取学员
        /// </summary>
        /// <param name="name">学员名称</param>
        /// <returns></returns>
        public Song.Entities.Accounts[] ForName(string name)
        {
            Song.Entities.Accounts[] accs= Business.Do<IAccounts>().Account4Name(name);
            foreach (Song.Entities.Accounts ac in accs)
            {
                ac.Ac_Pw = string.Empty;
            }
            return accs;
        }
        /// <summary>
        /// 按账号和姓名查询学员
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="name">姓名，可模糊查询</param>
        /// <returns></returns>
        public Song.Entities.Accounts[] Seach(string acc, string name)
        {
            List<Song.Entities.Accounts> list = new List<Accounts>();
            Song.Entities.Accounts[] accs = Business.Do<IAccounts>().Account4Name(name);
            foreach (Song.Entities.Accounts ac in accs)
                list.Add(ac);
            Song.Entities.Accounts account = Business.Do<IAccounts>().AccountsSingle(acc, -1);
            if (account != null)
            {
                bool isExist = false;
                foreach (Song.Entities.Accounts ac in accs)
                {
                    if (ac.Ac_ID == account.Ac_ID)
                    {
                        isExist = true;
                        break;
                    }
                }
                if (!isExist) list.Add(account);
            }
            foreach (Song.Entities.Accounts ac in list)          
                ac.Ac_Pw = string.Empty;
            return list.ToArray<Accounts>();
        }
        /// <summary>
        /// 从学习记录中获取学员记录
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Song.Entities.LogForStudentStudy[] ForLogs(string acc, string name)
        {
            string sql = @"select Ac_ID,Ac_AccName, Ac_Name from (
                    select logs.* from Accounts right join 
                    (select * from LogForStudentStudy where 
                    {name} and {acc}) as logs
                    on Accounts.Ac_ID=Logs.Ac_ID) as tm
                     group by Ac_ID,Ac_AccName,Ac_Name";
            sql = sql.Replace("{name}", string.IsNullOrWhiteSpace(name) ? "1=1" : "Ac_Name like '%" + name + "%'");
            sql = sql.Replace("{acc}", string.IsNullOrWhiteSpace(acc) ? "1=1" : "Ac_AccName='" + acc + "'");

            Song.Entities.LogForStudentStudy[] accs = Business.Do<ISystemPara>().ForSql<LogForStudentStudy>(sql).ToArray<LogForStudentStudy>();
            return accs;
        }
        /// <summary>
        /// 学员的视频学习记录
        /// </summary>
        /// <param name="acid">学员id</param>
        /// <param name="couid">课程id</param>
        /// <returns></returns>
        public Song.Entities.LogForStudentStudy[] StudyLog(int acid, int couid)
        {
            return Business.Do<IStudent>().LogForStudyCount(-1, couid, -1, acid, null, 0);
        }
        /// <summary>
        /// 分页获取学员信息
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ListResult Pager(int index, int size)
        {
            int sum = 0;
            Song.Entities.Accounts[] accs = Business.Do<IAccounts>().AccountsPager(-1, size, index, out sum);
            Song.ViewData.ListResult result = new ListResult(accs);
            result.Index = index;
            result.Size = size;
            result.Total = sum;
            return result;
        }

    }
}
