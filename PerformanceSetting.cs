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
using K12.Data;
using K12.Behavior.CSP.UDT;


namespace K12.Behavior.CSP
{
    public partial class PerformanceSetting : BaseForm
    {
        List<PerformanceItem> list = new List<PerformanceItem>();

        AccessHelper accessHelper = new AccessHelper();
        
        public PerformanceSetting()
        {
            InitializeComponent();

           
        }


        private void PerformanceSetting_Load(object sender, EventArgs e)
        {
            //自動換行
            //dgvResult.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            
             list = accessHelper.Select<PerformanceItem>();

            // 以Order整理順序，否則list 的順序將是由id 排
             list.Sort(delegate(PerformanceItem PI1, PerformanceItem PI2)
             {
                 return PI1.Order.CompareTo(PI2.Order);
             });
             
            // 加入項目
            for (int i = 0; i < list.Count; i++)
            {
                DataGridViewRow DGR = dgvResult.Rows[dgvResult.Rows.Add( list[i].Item,"上移","下移")];

                DGR.Tag = list[i];
                
            }
         
            // 幫新的預設新增空白行的按鈕加上 "上移"、"下移"
            DataGridViewButtonCell DGVBC1 = new DataGridViewButtonCell();
            DataGridViewButtonCell DGVBC2 = new DataGridViewButtonCell();

            DGVBC1 = (DataGridViewButtonCell)dgvResult.Rows[dgvResult.Rows.Count - 1].Cells[1];
            DGVBC2 = (DataGridViewButtonCell)dgvResult.Rows[dgvResult.Rows.Count - 1].Cells[2];

            DGVBC1.Value = "上移";
            DGVBC2.Value = "下移";
            
        }

        // 儲存設定按鈕
        private void buttonX1_Click(object sender, EventArgs e)
        {
            // 先預設每一項抓下來的PerformanceItem List 的每一項PI Item 都是要刪除，後面在反向把有讀到的合理資料設定為不刪除
            // 如此使用者就可以直接在UI上刪除一整條Row
            foreach (var item in list) 
            {
                item.Deleted = true;                                    
            }
              
            // 全部的Row 進行檢定
            foreach (DataGridViewRow dgvr in dgvResult.Rows)
            {                
                    PerformanceItem PI = new PerformanceItem();

                    PI = (PerformanceItem)dgvr.Tag;

                    //假如他有PI，且課堂表現不為空
                    if (dgvr.Tag != null && dgvr.Cells[2].Value != "" && dgvr.Cells[2].Value !=null)
                    {
                     
                            //假如順序有改變，針對Order調整，其值= Rowindex
                            if (""+dgvr.Index != "" + PI.Order)
                            {
                                PI.Order = Int32.Parse("" + dgvr.Index);
                            }
                            //假如課堂表現有改變，針對Item調整，其值= Cells[2].Value
                            if (""+dgvr.Cells[2].Value != "" + PI.Item) 
                            {
                                PI.Item = (String)dgvr.Cells[2].Value;
                            }

                            //不管值有沒有改變，只要PI 還存在於UI 上，就不會刪除
                            PI.Deleted = false;
                    }

                        // 假如使用者是用BackSpace 將 課堂表現內容刪光，就算他有PI ，但仍然將該項刪除
                    else if (dgvr.Tag != null && ("" + dgvr.Cells[2].Value == "" || dgvr.Cells[2].Value == null))
                    {
                        PI.Deleted = true;
                        
                    }
                        // 在UI ROW上的項目，假如沒有PI 代表是是使用者新增的項目，主動將它家道list
                    else
                    {
                        PerformanceItem PI_New = new PerformanceItem();
                        if (dgvr.Cells[2].Value != null )
                        {
                            PI_New.Order = Int32.Parse("" + dgvr.Index);
                            PI_New.Item = (String)dgvr.Cells[2].Value;

                            PI_New.Deleted = false;

                            list.Add(PI_New);
                        }
                    }                                        
            }

            // Update、Insert、Delete 都在這裡處理
            list.SaveAll();

        }

        private void dgvResult_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {            
            // 上移
            #region 上移
            if (e.ColumnIndex == 1)
            {
                // 取得總行數
                int totalRows = dgvResult.Rows.Count;

                //取得當前列數
                int rowIndex = e.RowIndex;

                // 項目在最上面的話不能在往上了
                if (rowIndex == 0)
                {
                    return;
                }

                //取得當前行數
                int colIndex = dgvResult.SelectedCells[0].OwningColumn.Index;

                //取得當前Row
                DataGridViewRow selectedRow = dgvResult.Rows[rowIndex];

                // 刪除原本的Row
                dgvResult.Rows.Remove(selectedRow);

                //在上一列的位子加入Row，看起來就像是原地往上移
                dgvResult.Rows.Insert(rowIndex - 1, selectedRow);

                //選擇的列數跟隨一起往上移
                dgvResult.ClearSelection();
                dgvResult.Rows[rowIndex - 1].Cells[colIndex].Selected = true;


            }

            #endregion
            // 下移
            #region 下移
            if (e.ColumnIndex == 2)
            {
                int totalRows = dgvResult.Rows.Count;

                int rowIndex = e.RowIndex;

                // 項目在最後一項的話不能在往下了
                if (rowIndex == totalRows - 2)
                {
                    return;
                }

                int colIndex = dgvResult.SelectedCells[0].OwningColumn.Index;

                DataGridViewRow selectedRow = dgvResult.Rows[rowIndex];

                dgvResult.Rows.Remove(selectedRow);

                dgvResult.Rows.Insert(rowIndex + 1, selectedRow);

                dgvResult.ClearSelection();

                dgvResult.Rows[rowIndex + 1].Cells[colIndex].Selected = true;

            } 
            #endregion

            //其他的的鍵 不需要理會
            else 
            {
                return;
                        
            }
        }

    }
}
