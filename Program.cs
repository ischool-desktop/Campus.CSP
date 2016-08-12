using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FISCA;

namespace K12.Behavior.CSP
{
    public static class Program
    {
        [FISCA.MainMethod]
        public static void Main()
        {
            {
                var btn1 = FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "課堂表現"]["紀錄推送"];
                btn1.Size = FISCA.Presentation.RibbonBarButton.MenuButtonSize.Large;
                btn1.Image = Properties.Resources.cabinet_up_128;
                btn1.Click += delegate
                {
                    PerformanceRequestViewer PRV = new PerformanceRequestViewer();

                    PRV.ShowDialog();

                };

                var btn2 = FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "課堂表現"]["表現設定"];
                btn2.Size = FISCA.Presentation.RibbonBarButton.MenuButtonSize.Large;
                btn2.Image = Properties.Resources.設定;
             
                btn2.Click += delegate
                {
                    PerformanceSetting PS = new PerformanceSetting();

                    PS.ShowDialog();


                };

            }
        }

    }
}
