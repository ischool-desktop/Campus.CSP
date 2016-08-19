using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FISCA;
using FISCA.Permission;

namespace K12.Behavior.CSP
{
    public static class Program
    {
        [FISCA.MainMethod]
        public static void Main()
        {
            {
                Catalog permission = RoleAclSource.Instance["教務作業"]["課堂表現"];
                permission.Add(new RibbonFeature("{B243BA3C-978A-4D57-A8B5-1C79E4078F09}", "紀錄推送"));

                var btn = FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "課堂表現"]["紀錄推送"];
                btn.Size = FISCA.Presentation.RibbonBarButton.MenuButtonSize.Large;
                btn.Image = Properties.Resources.cabinet_up_128;
                btn.Enable = FISCA.Permission.UserAcl.Current["{B243BA3C-978A-4D57-A8B5-1C79E4078F09}"].Executable;
                btn.Click += delegate
                {
                    PerformanceRequestViewer PRV = new PerformanceRequestViewer();

                    PRV.ShowDialog();
                };
            }

            {
                Catalog permission = RoleAclSource.Instance["教務作業"]["課堂表現"];
                permission.Add(new RibbonFeature("{491040CA-05BD-461A-AACD-0572473E57A3}", "樣板設定"));

                var btn = FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "課堂表現"]["樣板設定"];
                btn.Size = FISCA.Presentation.RibbonBarButton.MenuButtonSize.Large;
                btn.Image = Properties.Resources.設定;
                btn.Enable = FISCA.Permission.UserAcl.Current["{491040CA-05BD-461A-AACD-0572473E57A3}"].Executable;

                btn.Click += delegate
                {
                    PerformanceSetting PS = new PerformanceSetting();

                    PS.ShowDialog();
                };
            }
        }

    }
}
