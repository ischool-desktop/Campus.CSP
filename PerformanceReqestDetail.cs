using System;
using FISCA.Presentation.Controls;
using FISCA.UDT;

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
    //2016/8/11 穎驊開始製作，預計兩天內完成

    public partial class PerformanceReqestDetail : BaseForm
    {
        // 儲存用item 
        PerformanceData item_to_save = new PerformanceData();

         internal PerformanceReqestDetail(PerformanceData item,Dictionary<String,String> ID_to_Name)
        {
            InitializeComponent();
                     
            labelX8.Text = "" + item.OccurDate.ToString("yyyy/MM/dd");
            labelX9.Text = "" + ID_to_Name["Student"];
            labelX10.Text = "" + ID_to_Name["Teacher"];
            labelX11.Text = "" + ID_to_Name["Course"];
            labelX12.Text = "" + (item.Published ==false ? "未發送":"已發送");
            textBoxX2.Text = "" + item.Content.Text;
            
            item_to_save = item;                     
        }

         private void PerformanceReqestDetail_Load(object sender, EventArgs e)
         {
             if (item_to_save.PublishMessage != "")
             {
                 textBoxX1.Text = item_to_save.PublishMessage;
             }
             else {
                 textBoxX1.Text = textBoxX2.Text;
             }
             
         }



        //儲存
         private void buttonX1_Click(object sender, EventArgs e)
         {
             
             item_to_save.PublishMessage = textBoxX1.Text;

             item_to_save.Published = true;

             item_to_save.Save();

             this.Close();
         }


         //發送
         private void buttonX2_Click(object sender, EventArgs e)
         {
             this.Close();

         }
    }
}
