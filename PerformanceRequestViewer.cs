using FISCA.Presentation.Controls;
using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using K12.Behavior.CSP.UDT;
using K12.Data;

namespace K12.Behavior.CSP
{
    //2016/8/11 穎驊開始製作

    public partial class PerformanceRequestViewer : BaseForm
    {

        AccessHelper accessHelper = new AccessHelper();

        List<PerformanceData> list = new List<PerformanceData>();

        Dictionary<String, String> StudentID_to_StudentName = new Dictionary<string, string>();

        Dictionary<String, String> TeacherID_to_TeacherName = new Dictionary<string, string>();

        Dictionary<String, String> CourseID_to_CourseName = new Dictionary<string, string>();

        // 將未發送的List Index 定位到全部搜尋List 的index 使用的Dict
        Dictionary<int, int> NotSendList_to_AllList = new Dictionary<int, int>();

        // 將未發送的List Index 定位到全部搜尋List 的index 使用的Dict
        Dictionary<int, int> SendedList_to_AllList = new Dictionary<int, int>();


        public PerformanceRequestViewer()
        {
            InitializeComponent();
        }

        private void PerformanceRequestViewer_Load(object sender, EventArgs e)        
        {
            //設定預設時間區間，預設為 從今天到上禮拜一
            dateTimeInput1.Value = DateTime.Now.Date.AddDays(-7 - (int)DateTime.Now.DayOfWeek + 1);
            dateTimeInput2.Value = DateTime.Now.Date;
            
            //搜尋區間內的資料
            list = accessHelper.Select<PerformanceData>("occur_date >=" + "'" + dateTimeInput1.Value + "'" + "AND occur_date <=" + "'" + dateTimeInput2.Value+"'");

            // 整理順序的Code ，以後如果有需要再改
            //list.Sort(delegate(PerformanceItem PI1, PerformanceItem PI2)
            //{
            //    return PI1.Order.CompareTo(PI2.Order);
            //});

            //建立學生、老師、課程 ID 與 Name 的對照
            #region 建立對照
            var list_studentRec = K12.Data.Student.SelectAll();

            var list_teacherRec = K12.Data.Teacher.SelectAll();

            var list_courseRec = K12.Data.Course.SelectAll();

            foreach (var Rec in list_studentRec)
            {
                if (!StudentID_to_StudentName.ContainsKey(Rec.ID))
                {
                    StudentID_to_StudentName.Add(Rec.ID, Rec.Name);
                }

            }
            foreach (var Rec in list_teacherRec)
            {
                if (!TeacherID_to_TeacherName.ContainsKey(Rec.ID))
                {
                    TeacherID_to_TeacherName.Add(Rec.ID, Rec.Name);
                }
            }
            foreach (var Rec in list_courseRec)
            {
                if (!CourseID_to_CourseName.ContainsKey(Rec.ID))
                {
                    CourseID_to_CourseName.Add(Rec.ID, Rec.Name);
                }

            }
            #endregion

            comboBoxEx1.Items.Add("未發送");
            comboBoxEx1.Items.Add("已發送");
            comboBoxEx1.Items.Add("全選");

            //先預設選"全選"，順帶一提此時會觸發comboBoxEx1_SelectedIndexChanged()，雖然本案最後不用了
            comboBoxEx1.SelectedIndex = 2;

            //自動換行設定
            dataGridViewX1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            Search_Show_Data();                       
        }
           
        // 搜尋按鈕
        private void buttonX1_Click(object sender, EventArgs e)
        {
            Search_Show_Data();

        }

        // 詳細資料
        private void dataGridViewX1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
            Int32 selectedRowCount = dataGridViewX1.CurrentCell.RowIndex;

            Dictionary<String, String> ID_to_Name = new Dictionary<string, string>();


            #region 全選
            if (comboBoxEx1.Text == "全選")
            {
                ID_to_Name.Add("Student", StudentID_to_StudentName["" + list[selectedRowCount].RefStudentID]);

                ID_to_Name.Add("Teacher", TeacherID_to_TeacherName["" + list[selectedRowCount].RefTeacherID]);

                ID_to_Name.Add("Course", CourseID_to_CourseName["" + list[selectedRowCount].RefCourseID]);

                PerformanceReqestDetail PRD = new PerformanceReqestDetail(list[selectedRowCount], ID_to_Name);

                PRD.ShowDialog();
            } 
            #endregion

            #region 未發送
            if (comboBoxEx1.Text == "未發送")
            {
                ID_to_Name.Add("Student", StudentID_to_StudentName["" + list[NotSendList_to_AllList[selectedRowCount]].RefStudentID]);

                ID_to_Name.Add("Teacher", TeacherID_to_TeacherName["" + list[NotSendList_to_AllList[selectedRowCount]].RefTeacherID]);

                ID_to_Name.Add("Course", CourseID_to_CourseName["" + list[NotSendList_to_AllList[selectedRowCount]].RefCourseID]);

                PerformanceReqestDetail PRD = new PerformanceReqestDetail(list[NotSendList_to_AllList[selectedRowCount]], ID_to_Name);

                PRD.ShowDialog();
            } 
            #endregion

            #region 已發送
            if (comboBoxEx1.Text == "已發送")
            {
                ID_to_Name.Add("Student", StudentID_to_StudentName["" + list[SendedList_to_AllList[selectedRowCount]].RefStudentID]);

                ID_to_Name.Add("Teacher", TeacherID_to_TeacherName["" + list[SendedList_to_AllList[selectedRowCount]].RefTeacherID]);

                ID_to_Name.Add("Course", CourseID_to_CourseName["" + list[SendedList_to_AllList[selectedRowCount]].RefCourseID]);

                PerformanceReqestDetail PRD = new PerformanceReqestDetail(list[SendedList_to_AllList[selectedRowCount]], ID_to_Name);

                PRD.ShowDialog();

            } 
            #endregion
      
        }
  
        // 搜尋並為DataGridView 填值
        private void Search_Show_Data() {
           
            list = accessHelper.Select<PerformanceData>("occur_date >=" + "'" + dateTimeInput1.Value + "'" + "AND occur_date <=" + "'" + dateTimeInput2.Value + "'");

            // 將舊的資料清光
            dataGridViewX1.Rows.Clear();
            NotSendList_to_AllList.Clear();
            SendedList_to_AllList.Clear();


            #region 未發送
            if (comboBoxEx1.Text == "未發送")
            {
                int i_dict = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Published == false)
                    {
                        dataGridViewX1.Rows.Add(list[i].OccurDate.ToString("yyyy/MM/dd"), StudentID_to_StudentName["" + list[i].RefStudentID], TeacherID_to_TeacherName["" + list[i].RefTeacherID], CourseID_to_CourseName["" + list[i].RefCourseID], list[i].PublishMessage, "未發送");

                        // 將未發送的List Index 定位到全部搜尋List 的index
                        NotSendList_to_AllList.Add(i_dict, i);

                        i_dict++;
                    }
                }
            } 
            #endregion

            #region 已發送
            if (comboBoxEx1.Text == "已發送")
            {
                int i_dict = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Published == true)
                    {
                        dataGridViewX1.Rows.Add(list[i].OccurDate.ToString("yyyy/MM/dd"), StudentID_to_StudentName["" + list[i].RefStudentID], TeacherID_to_TeacherName["" + list[i].RefTeacherID], CourseID_to_CourseName["" + list[i].RefCourseID], list[i].PublishMessage, "已發送");

                        // 將已發送的List Index 定位到全部搜尋List 的index，比如說 已發送的第0項 是在全部List 的第15項
                        SendedList_to_AllList.Add(i_dict, i);

                        i_dict++;
                    }
                }

            } 
            #endregion

            #region 全選
            if (comboBoxEx1.Text == "全選")
            {
                for (int i = 0; i < list.Count; i++)
                {
                    dataGridViewX1.Rows.Add(list[i].OccurDate.ToString("yyyy/MM/dd"), StudentID_to_StudentName["" + list[i].RefStudentID], TeacherID_to_TeacherName["" + list[i].RefTeacherID], CourseID_to_CourseName["" + list[i].RefCourseID], list[i].PublishMessage, list[i].Published == false ? "未發送" : "已發送");

                }

            } 
            #endregion
                                
        }
    
    }


}
