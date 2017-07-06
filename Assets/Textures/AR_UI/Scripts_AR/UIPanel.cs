using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV.UI
{
    public class UIPanel : UIControl
    {
        public UISwipe[] controls;

        public override void Open()
        {
            base.Open();
            for(int i=0; i<controls.Length; i++)
            {
                controls[i].Open();
            }
        }

        public override void Close()
        {
            base.Close();
            for (int i = 0; i < controls.Length; i++)
            {
                //controls[i].Close();
            }
        }
    }
}
