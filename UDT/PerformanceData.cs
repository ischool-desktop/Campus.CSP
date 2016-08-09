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
