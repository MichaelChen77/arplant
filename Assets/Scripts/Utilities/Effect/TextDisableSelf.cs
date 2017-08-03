using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV
{
    public class TextDisableSelf : DisableSelf
    {
        public Text hintText;

        public void Open(string str)
        {
            hintText.text = str;
            Open();
        }
    }
}
