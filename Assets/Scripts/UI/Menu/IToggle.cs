using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public interface IToggle
    {
        void SetToggle(bool flag);
    }

    [System.Serializable]
    public class TextToggle: IToggle
    {
        public Text targetText;
        public string toggleOnString;
        public string toggleOffString;

        public void SetToggle(bool flag)
        {
            if (flag)
                targetText.text = toggleOnString;
            else
                targetText.text = toggleOffString;
        }
    }
}
