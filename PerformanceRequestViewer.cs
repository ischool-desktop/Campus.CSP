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

        AccessHelper _AccessHelper = new AccessHelper();

        //List<PerformanceData> list = new List<PerformanceData>();

        Dictionary<String, String> _DicStudentName = new Dictionary<string, string>();

        Dictionary<String, String> _DicTeacherName = new Dictionary<string, string>();

        Dictionary<String, String> _DicCourseName = new Dictionary<string, string>();


        public PerformanceRequestViewer()
        {
            InitializeComponent();

            //自動換行設定
            dataGridViewX1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void PerformanceRequestViewer_Load(object sender, EventArgs e)
        {
            //設定預設時間區間，預設為 從今天到上禮拜一
            dateTimeInput1.Value = DateTime.Now.Date.AddDays(-7 - (int)DateTime.Now.DayOfWeek + 1);
            dateTimeInput2.Value = DateTime.Now.Date;

            //建立學生、老師、課程 ID 與 Name 的對照
            #region 建立對照
            BackgroundWorker bkw = new BackgroundWorker();
            bkw.DoWork += delegate
            {
                var dicStudentName = new Dictionary<string, string>();
                var dicTeacherName = new Dictionary<string, string>();
                var dicCourseName = new Dictionary<string, string>();
                dicStudentName.Add("", "");
                dicTeacherName.Add("", "");
                dicCourseName.Add("", "");
                var studentList = K12.Data.Student.SelectAll();
                var teacherList = K12.Data.Teacher.SelectAll();
                var courseList = K12.Data.Course.SelectAll();
                foreach (var Rec in studentList)
                {
                    if (!dicStudentName.ContainsKey(Rec.ID))
                    {
                        dicStudentName.Add(Rec.ID, Rec.Name);
                    }
                }
                foreach (var Rec in teacherList)
                {
                    if (!dicTeacherName.ContainsKey(Rec.ID))
                    {
                        dicTeacherName.Add(Rec.ID, Rec.Name);
                    }
                }
                foreach (var Rec in courseList)
                {
                    if (!dicCourseName.ContainsKey(Rec.ID))
                    {
                        dicCourseName.Add(Rec.ID, Rec.Name);
                    }
                }
                lock (typeof(PerformanceRequestViewer))
                {
                    _DicStudentName = dicStudentName;
                    _DicTeacherName = dicTeacherName;
                    _DicCourseName = dicCourseName;
                }
            };
            bkw.RunWorkerCompleted += delegate
            {
                foreach (DataGridViewRow row in dataGridViewX1.Rows)
                {
                    lock (typeof(PerformanceRequestViewer))
                    {
                        PerformanceData item = row.Tag as PerformanceData;
                        if (_DicStudentName.ContainsKey("" + item.RefStudentID))
                            item.StudentName = _DicStudentName["" + item.RefStudentID];
                        if (_DicTeacherName.ContainsKey("" + item.RefTeacherID))
                            item.TeacherName = _DicTeacherName["" + item.RefTeacherID];
                        if (_DicCourseName.ContainsKey("" + item.RefCourseID))
                            item.CourseName = _DicCourseName["" + item.RefCourseID];
                    }
                    updateRowValue(row);
                }
            };
            bkw.RunWorkerAsync();
            #endregion

            comboBoxEx1.Items.Add("未發送");
            comboBoxEx1.Items.Add("已發送");
            comboBoxEx1.Items.Add("全選");

            //先預設選"全選"，順帶一提此時會觸發comboBoxEx1_SelectedIndexChanged()，雖然本案最後不用了
            comboBoxEx1.SelectedIndex = 2;

            fillData();

            if (FISCA.Authentication.DSAServices.AccountType != FISCA.Authentication.AccountType.Greening)
            {
                MessageBox.Show("必須要使用Greening帳號登入才能執行發送功能。");
            }
        }

        // 搜尋按鈕
        private void buttonX1_Click(object sender, EventArgs e)
        {
            fillData();

        }
        // 搜尋並為DataGridView 填值
        private void fillData()
        {
            //搜尋區間內的資料
            var list = _AccessHelper.Select<PerformanceData>("occur_date >=" + "'" + dateTimeInput1.Value + "'" + "AND occur_date <=" + "'" + dateTimeInput2.Value + "'");
            list.Reverse();

            // 將舊的資料清光
            dataGridViewX1.Rows.Clear();
            foreach (var item in list)
            {
                lock (typeof(PerformanceRequestViewer))
                {
                    if (_DicStudentName.ContainsKey("" + item.RefStudentID))
                        item.StudentName = _DicStudentName["" + item.RefStudentID];
                    if (_DicTeacherName.ContainsKey("" + item.RefTeacherID))
                        item.TeacherName = _DicTeacherName["" + item.RefTeacherID];
                    if (_DicCourseName.ContainsKey("" + item.RefCourseID))
                        item.CourseName = _DicCourseName["" + item.RefCourseID];
                }

                var row = dataGridViewX1.Rows[dataGridViewX1.Rows.Add(
                    item.OccurDate.ToString("yyyy/MM/dd"),
                    item.StudentName,
                    item.TeacherName,
                    item.CourseName,
                    item.PublishMessage.Trim(),
                    item.Published == false ? "" : "已發送")];
                row.Tag = item;
                row.MinimumHeight = 60;
                if (comboBoxEx1.Text == "未發送" && item.Published)
                    row.Visible = false;
                if (comboBoxEx1.Text == "已發送" && !item.Published)
                    row.Visible = false;
            }
        }

        private void comboBoxEx1_TextChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                PerformanceData item = row.Tag as PerformanceData;

                if (comboBoxEx1.Text == "未發送" && item.Published)
                    row.Visible = false;
                else if (comboBoxEx1.Text == "已發送" && !item.Published)
                    row.Visible = false;
                else
                    row.Visible = true;
            }
        }

        private void updateRowValue(DataGridViewRow row)
        {
            PerformanceData item = row.Tag as PerformanceData;

            row.SetValues(
                item.OccurDate.ToString("yyyy/MM/dd"),
                item.StudentName,
                item.TeacherName,
                item.CourseName,
                item.PublishMessage.Trim(),
                item.Published == false ? "" : "已發送"
            );

            if (comboBoxEx1.Text == "未發送" && item.Published)
                row.Visible = false;
            else if (comboBoxEx1.Text == "已發送" && !item.Published)
                row.Visible = false;
            else
                row.Visible = true;
        }

        // 詳細資料
        private void dataGridViewX1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)//Header點擊也會觸發事件
                return;

            DataGridViewRow row = dataGridViewX1.Rows[e.RowIndex];
            PerformanceData item = row.Tag as PerformanceData;
            new PerformanceReqestDetail(item).ShowDialog();
            updateRowValue(row);
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.buttonX2.Enabled = false;
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                PerformanceData item = row.Tag as PerformanceData;
                item.PublishAndSave();
                updateRowValue(row);
            }
            this.buttonX2.Enabled = true;
        }
    }
}
