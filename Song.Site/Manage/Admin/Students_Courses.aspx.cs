using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using WeiSha.Common;

using Song.ServiceInterfaces;
using Song.Entities;
using System.Collections.Generic;



namespace Song.Site.Manage.Admin
{
    public partial class Students_Courses : Extend.CustomPage
    {
        //ѧԱid
        protected int id = WeiSha.Common.Request.QueryString["id"].Int32 ?? 0;
        //Ա���ϴ����ϵ�����·��
        private string _uppath = "Course";
        //ѧϰ��¼��datatable
        DataTable dtLog = null;
        Song.Entities.Organization org;
        protected void Page_Load(object sender, EventArgs e)
        {
            org = Business.Do<IOrganization>().OrganCurrent();
            dtLog = Business.Do<IStudent>().StudentStudyCourseLog(id);
            if (!this.IsPostBack)
            {
                fill();
            }
           
        }
        private void fill()
        {
            //��ǰѧ���Ŀγ�
            Song.Entities.Accounts st = Business.Do<IAccounts>().AccountsSingle(id);
            if (st == null) return;
            //��ǰѧԱ������
            lbAccName.Text = st.Ac_Name;
            Title = st.Ac_Name;
            //����Ŀγ�(�������õģ�
            List<Song.Entities.Course> cous = Business.Do<ICourse>().CourseForStudent(st.Ac_ID, null, 0, null, -1);
            foreach (Song.Entities.Course c in cous)
            {
                //�γ�ͼƬ
                if (!string.IsNullOrEmpty(c.Cou_LogoSmall) && c.Cou_LogoSmall.Trim() != "")
                    c.Cou_LogoSmall = Upload.Get[_uppath].Virtual + c.Cou_LogoSmall;
                c.Cou_IsStudy = true;
            }
            rptCourse.DataSource = cous;
            rptCourse.DataBind();
            plNoCourse.Visible = cous.Count < 1;     
        }
        #region ������Ҫ�ķ���
        /// <summary>
        /// ȡ���γ�ѧϰ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbSelected_Click(object sender, EventArgs e)
        {
            LinkButton lb = (LinkButton)sender;
            int couid = Convert.ToInt32(lb.CommandArgument);    //�γ�id 
            //ȡ���γ�
            Business.Do<ICourse>().DelteCourseBuy(id, couid);
            //���ص�ǰ��
            this.fill();
        }
        /// <summary>
        /// ��ȡ�γ̵Ĺ�����Ϣ
        /// </summary>
        /// <param name="objid"></param>
        /// <returns></returns>
        protected string getBuyInfo(object objid)
        {
            int couid = 0;
            int.TryParse(objid.ToString(), out couid);
            Student_Course sc = Business.Do<ICourse>().StudentCourse(id, couid);
            if (sc == null) return "";
            if (sc.Stc_IsFree) return "��ѣ������ڣ�";
            if (sc.Stc_IsTry) return "����";
            return sc.Stc_StartTime.ToString("yyyy��MM��dd��") + " - " + sc.Stc_EndTime.ToString("yyyy��MM��dd��");
        }
        /// <summary>
        /// �����ۼ�ѧϰʱ��
        /// </summary>
        /// <param name="studyTime"></param>
        /// <returns></returns>
        protected string CaleStudyTime(string studyTime)
        {
            int num = 0;
            int.TryParse(studyTime, out num);
            if (num < 60) return num + "����";
            //�������
            num = num / 60;
            int ss = num % 60;
            if (num < 60) return num + "����";
            //����Сʱ
            int hh = num / 60;
            int mm = num % 60;
            return string.Format("{0}Сʱ{1}����", hh, mm);
        }
        /// <summary>
        /// ��ȡ�ۼ�ѧϰʱ��
        /// </summary>
        /// <param name="studyTime"></param>
        /// <returns></returns>
        protected string GetstudyTime(string couid)
        {
            string studyTime = "0";
            if (dtLog != null)
            {
                foreach (DataRow dr in dtLog.Rows)
                {
                    if (dr["Cou_ID"].ToString() == couid)
                    {
                        studyTime = dr["studyTime"].ToString();
                    }
                }
                return CaleStudyTime(studyTime);
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// �γ���ɶ�
        /// </summary>
        /// <param name="studyTime"></param>
        /// <returns></returns>
        protected string GetComplete(string couid)
        {
            string complete = "0";
            if (dtLog != null)
            {
                foreach (DataRow dr in dtLog.Rows)
                {
                    if (dr["Cou_ID"].ToString() == couid)
                    {
                        complete = dr["complete"].ToString();
                    }
                }
                return complete;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// ��ȡ���ѧϰʱ��
        /// </summary>
        /// <param name="couid"></param>
        /// <returns></returns>
        protected string GetLastTime(string couid)
        {
            DateTime? lastTime = null;
            if (dtLog != null)
            {
                foreach (DataRow dr in dtLog.Rows)
                {
                    if (dr["Cou_ID"].ToString() == couid)
                    {
                        lastTime = Convert.ToDateTime(dr["LastTime"]);
                    }
                }
            }
            if (lastTime == null) return "����û��ѧϰ��";
            return ((DateTime)lastTime).ToString();
        }
        /// <summary>
        /// ��ȡѧԱѧϰ�Ŀγ̼�¼
        /// </summary>
        /// <param name="couidstr"></param>
        /// <returns></returns>
        protected Song.Entities.Student_Course GetStc(string couidstr)
        {
            int couid = 0;
            int.TryParse(couidstr, out couid);
            return Business.Do<ICourse>().StudentCourse(id, couid);
        }
        #endregion
    }
}
