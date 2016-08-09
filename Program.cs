using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Behavior.CSP
{
    public static class Program
    {
        [FISCA.MainMethod]
        public static void Main()
        {
            {
                var btn = FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "課堂表現"]["紀錄推送"];
                btn.Size = FISCA.Presentation.RibbonBarButton.MenuButtonSize.Large;
                //btn.Image = Properties.Resources.請假;
                btn.Click += delegate
                {

                };
            }
        }

    }
}
