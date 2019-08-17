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
using System.Data.OleDb;

using WeiSha.Common;

using Song.ServiceInterfaces;
using Song.Entities;
using System.Reflection;
using System.Collections.Generic;

namespace Song.Site.Manage.Admin
{
    public partial class Student_Input : Extend.CustomPage
    {
        
        //������
        Song.Entities.StudentSort[] sorts = null;
        Song.Entities.Organization org = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (org == null) org = Business.Do<IOrganization>().OrganCurrent();
        }
        

        protected void ExcelInput1_OnInput(object sender, EventArgs e)
        {
            //�������е�����
            DataTable dt = ExcelInput1.SheetDataTable;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    //throw new Exception();
                    //���������е������ݿ�
                    _inputData(dt.Rows[i]);
                }
                catch
                {
                    //��������������з��ظ��ؼ�
                    ExcelInput1.AddError(dt.Rows[i]);
                }
            }
        }

        #region ��������
       
        /// <summary>
        /// ��ĳһ�����ݼ��뵽���ݿ�
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="dl"></param>
        private void _inputData(DataRow dr)
        {
            //ȡ���з���           
            if (this.sorts == null) this.sorts = Business.Do<IStudent>().SortCount(org.Org_ID, null, 0);
            Song.Entities.Accounts obj = null;
            bool isExist = false;
            foreach (KeyValuePair<String, String> rel in ExcelInput1.DataRelation)
            {
                //Excel���е�ֵ
                string column = dr[rel.Key].ToString();
                //���ݿ��ֶε�����
                string field = rel.Value;
                if (field == "Ac_AccName")
                {
                    obj = Business.Do<IAccounts>().AccountsSingle(column, -1);
                    isExist = obj != null;
                    continue;
                }
            }
            if (obj == null) obj = new Entities.Accounts();
            foreach (KeyValuePair<String, String> rel in ExcelInput1.DataRelation)
            {
                //Excel���е�ֵ
                string column = dr[rel.Key].ToString();
                //���ݿ��ֶε�����
                string field = rel.Value;
                if (field == "Ac_Sex")
                {
                    obj.Ac_Sex = (short)(column == "��" ? 1 : (column == "Ů" ? 2 : 0));
                    continue;
                }
                PropertyInfo[] properties = obj.GetType().GetProperties();
                for (int j = 0; j < properties.Length; j++)
                {
                    PropertyInfo pi = properties[j];
                    if (field == pi.Name && !string.IsNullOrEmpty(column))
                    {
                        pi.SetValue(obj, Convert.ChangeType(column,pi.PropertyType), null);                        
                    }
                }               
            }
            //���÷���
            if (!string.IsNullOrWhiteSpace(obj.Sts_Name)) obj.Sts_ID = _getSortsId(sorts, obj.Sts_Name);
            if (!string.IsNullOrWhiteSpace(obj.Ac_Pw)) obj.Ac_Pw = new WeiSha.Common.Param.Method.ConvertToAnyValue(obj.Ac_Pw).MD5;
            obj.Org_ID = org.Org_ID;
            obj.Ac_IsPass = true;
            obj.Ac_IsUse = true;
            if (isExist)
            {
                Business.Do<IAccounts>().AccountsSave(obj);
            }
            else
            {
                Business.Do<IAccounts>().AccountsAdd(obj);
            }
        }
        /// <summary>
        /// ��ȡ����id
        /// </summary>
        /// <param name="sorts"></param>
        /// <param name="departName"></param>
        /// <returns></returns>
        private int _getSortsId(Song.Entities.StudentSort[] sorts, string sortName)
        {
            try
            {
                int sortId = 0;
                foreach (Song.Entities.StudentSort s in sorts)
                {
                    if (sortName.Trim() == s.Sts_Name)
                    {
                        sortId = s.Sts_ID;
                        break;
                    }
                }
                if (sortId == 0 && sortName.Trim() != "")
                {
                    Song.Entities.StudentSort nwsort = new Song.Entities.StudentSort();
                    nwsort.Sts_Name = sortName;
                    nwsort.Sts_IsUse = true;
                    nwsort.Org_ID = org.Org_ID;
                    Business.Do<IStudent>().SortAdd(nwsort);
                    sortId = nwsort.Sts_ID;
                    this.sorts = this.sorts = Business.Do<IStudent>().SortCount(org.Org_ID, null, 0);
                }
                return sortId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        
        #endregion
      
    }
}
