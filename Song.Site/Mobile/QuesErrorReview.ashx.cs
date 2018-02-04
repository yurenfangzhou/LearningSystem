﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeiSha.Common;
using Song.ServiceInterfaces;
using VTemplate.Engine;

namespace Song.Site.Mobile
{
    /// <summary>
    /// 学员错题的练习界面
    /// </summary>
    public class QuesErrorReview : BasePage
    {
        //当前试题id
        protected int qid = WeiSha.Common.Request.QueryString["qid"].Int32 ?? 0;
        //当前学员错题
        Song.Entities.Questions[] collectQues = null;
        protected override void InitPageTemplate(HttpContext context)
        {
         
            //题型
            this.Document.SetValue("quesType", WeiSha.Common.App.Get["QuesType"].Split(','));
            //
            this.Document.SetValue("qid", qid);
            Song.Entities.Accounts st = Extend.LoginState.Accounts.CurrentUser;
            Song.Entities.Course currCourse = Extend.LoginState.Accounts.Course();
            //错题列表
            Song.Entities.Questions[] ques = Business.Do<IStudent>().QuesAll(st.Ac_ID, 0, currCourse.Cou_ID, -1);
            for (int i = 0; i < ques.Length; i++)
            {
                ques[i] = Extend.Questions.TranText(ques[i]);
                ques[i].Qus_Title = ques[i].Qus_Title.Replace("&lt;", "<");
                ques[i].Qus_Title = ques[i].Qus_Title.Replace("&gt;", ">");
                ques[i].Qus_Title = Extend.Html.ClearHTML(ques[i].Qus_Title, "p", "div", "font");
            }
            this.Document.SetValue("ques", ques);
            this.Document.RegisterGlobalFunction(this.AnswerItems);
            this.Document.RegisterGlobalFunction(this.GetAnswer);
            this.Document.RegisterGlobalFunction(this.GetOrder);
            this.Document.RegisterGlobalFunction(this.IsCollect);
        }
        /// <summary>
        /// 当前试题的选项，仅用于单选与多选
        /// </summary>
        /// <returns></returns>
        protected object AnswerItems(object[] p)
        {
            Song.Entities.Questions qus = null;
            if (p.Length > 0)
                qus = (Song.Entities.Questions)p[0];
            //当前试题的答案
            Song.Entities.QuesAnswer[] ans = Business.Do<IQuestions>().QuestionsAnswer(qus, null);
            for (int i = 0; i < ans.Length; i++)
            {
                ans[i] = Extend.Questions.TranText(ans[i]);
                //ans[i].Ans_Context = ans[i].Ans_Context.Replace("<", "&lt;");
                //ans[i].Ans_Context = ans[i].Ans_Context.Replace(">", "&gt;");
            }
            return ans;
        }
        /// <summary>
        /// 试题的答案
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        protected object GetAnswer(object[] objs)
        {
            //当前试题
            Song.Entities.Questions qus = (Song.Entities.Questions)objs[0];
            string ansStr = "";
            if (qus.Qus_Type == 1)
            {
                //当前试题的答案
                Song.Entities.QuesAnswer[] ans = Business.Do<IQuestions>().QuestionsAnswer(qus, null);
                for (int i = 0; i < ans.Length; i++)
                {
                    if (ans[i].Ans_IsCorrect)
                        ansStr += (char)(65 + i);
                }
            }
            if (qus.Qus_Type == 2)
            {
                Song.Entities.QuesAnswer[] ans = Business.Do<IQuestions>().QuestionsAnswer(qus, null);
                for (int i = 0; i < ans.Length; i++)
                {
                    if (ans[i].Ans_IsCorrect)
                        ansStr += (char)(65 + i) + "、";
                }
                ansStr = ansStr.Substring(0, ansStr.LastIndexOf("、"));
            }
            if (qus.Qus_Type == 3)
                ansStr = qus.Qus_IsCorrect ? "正确" : "错误";
            if (qus.Qus_Type == 4)
            {
                if (qus != null && !string.IsNullOrEmpty(qus.Qus_Answer))
                    ansStr = qus.Qus_Answer;
            }
            if (qus.Qus_Type == 5)
            {
                //当前试题的答案
                Song.Entities.QuesAnswer[] ans = Business.Do<IQuestions>().QuestionsAnswer(qus, null);
                for (int i = 0; i < ans.Length; i++)
                    ansStr += (char)(65 + i) + "、" + ans[i].Ans_Context + "<br/>";
            }
            return ansStr;
        }

        /// <summary>
        /// 试题是否被当前学员收藏
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        protected object IsCollect(object[] objs)
        {
            int qid = 0;
            if (objs.Length > 0)
                qid = Convert.ToInt32(objs[0]);
            //当前收藏            
            if (collectQues == null)
            {
                Song.Entities.Course currCourse = Extend.LoginState.Accounts.Course();
                if (Extend.LoginState.Accounts.IsLogin)
                {
                    Song.Entities.Accounts st = Extend.LoginState.Accounts.CurrentUser;
                    collectQues = Business.Do<IStudent>().CollectAll4Ques(st.Ac_ID, 0, currCourse.Cou_ID, 0);
                }
                else
                {
                    collectQues = Business.Do<IStudent>().CollectAll4Ques(0, 0, currCourse.Cou_ID, 0);
                }
            }
            if (collectQues != null)
            {
                foreach (Song.Entities.Questions q in collectQues)
                {
                    if (qid == q.Qus_ID) return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取序号
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected object GetOrder(object[] index)
        {
            int tax = 0;
            if (index.Length > 0)
                tax = Convert.ToInt32(index[0]);
            return (char)(tax - 1 + 65);
        }
    }
}