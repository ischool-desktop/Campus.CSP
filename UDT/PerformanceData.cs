using FISCA.UDT;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace K12.Behavior.CSP.UDT
{
    [TableName("campus.csp.performance_data")]
    class PerformanceData : FISCA.UDT.ActiveRecord
    {
        [Field(Field = "occur_date")]
        public DateTime OccurDate { get; set; }
        [Field(Field = "ref_student_id")]
        public int? RefStudentID { get; set; }
        [Field(Field = "ref_teacher_id")]
        public int? RefTeacherID { get; set; }
        [Field(Field = "ref_course_id")]
        public int? RefCourseID { get; set; }

        [Field(Field = "publish_message")]
        public string PublishMessage { get; set; }
        [Field(Field = "published")]
        public bool Published { get; set; }

        private string _JSON = "";
        [Field(Field = "content")]
        public string JSON
        {
            get { return _JSON; }
            set
            {
                _JSON = value;
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes("{\"List\":" + _JSON + "}")))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PDList));
                    Content = (PDList)serializer.ReadObject(ms);
                    if (PublishMessage == "")
                    {
                        PublishMessage = Content.Text;
                    }
                }
            }
        }

        public PDList Content { get; private set; }


        public string TeacherName { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }

        public void PublishAndSave()
        {
            //必須要使用greening帳號登入才能用
            if (FISCA.Authentication.DSAServices.AccountType == FISCA.Authentication.AccountType.Greening)
            {
                var subject = K12.Data.Course.SelectByID("" + RefCourseID).Subject;

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                var root = doc.CreateElement("Request");
                //發送者名稱
                {
                    var ele = doc.CreateElement("DisplaySender");
                    ele.InnerText = "系統通知";
                    root.AppendChild(ele);
                }
                //標題
                {
                    var ele = doc.CreateElement("Title");
                    ele.InnerText = "課堂表現 " + OccurDate.ToShortDateString();
                    root.AppendChild(ele);
                }
                //內容
                {
                    var ele = doc.CreateElement("Message");
                    ele.AppendChild(doc.CreateCDataSection("課程：" + subject + "\n\n課堂表現內容：\n" + this.PublishMessage.Trim()));
                    root.AppendChild(ele);
                }
                //對象
                {
                    var ele = doc.CreateElement("TargetStudent");
                    ele.InnerText = "" + this.RefStudentID;//學生id
                    root.AppendChild(ele);
                }


                //送出
                FISCA.DSAClient.XmlHelper xmlHelper = new FISCA.DSAClient.XmlHelper(root);
                var conn = new FISCA.DSAClient.Connection();
                conn.Connect(FISCA.Authentication.DSAServices.AccessPoint, "1campus.notice.admin", FISCA.Authentication.DSAServices.PassportToken);
                conn.SendRequest("PushNotice", xmlHelper);


                this.Published = true;
                this.Save();
            }
        }
    }
    [DataContract]
    class PDList
    {
        [DataMember]
        public PDItem[] List { get; set; }

        public string Text
        {
            get
            {
                string result = "";
                foreach (var item in List)
                {
                    if (item.Text != "")
                    {
                        result += item.Text + "\r\n";
                    }
                }
                result.Trim();
                return result;
            }
        }
    }

    [DataContract]
    class PDItem
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string[] Values { get; set; }

        public string Text
        {
            get
            {
                return string.Join("", Values);
            }
        }
    }
}
