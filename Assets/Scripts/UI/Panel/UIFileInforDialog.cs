using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace IMAV.UI
{
    public class UIFileInforDialog : UIControl
    {
        public Text timeText;
        public Text pathText;
        public Text detailText;
        public Text lengthText;

        public void Open(string str)
        {
            Open();
            FileInfo f = new FileInfo(str);
            timeText.text = f.CreationTime.ToLongTimeString() + "\n" + f.CreationTime.ToLongDateString();
            pathText.text = f.FullName;
            string sizeStr = "";
            float fileSize = ((float)f.Length)/1024;
            if (fileSize > 1024)
            {
                fileSize = fileSize / 1024;
                sizeStr = fileSize.ToString("f3") + " MB";
            }
            else
                sizeStr = fileSize.ToString("f3") + " KB";
            detailText.text = sizeStr;
        }
    }
}
