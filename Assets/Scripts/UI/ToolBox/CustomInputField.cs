using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class CustomInputField : MonoBehaviour
    {
        public InputField input;
        public string text
        {
            get { return input.text; }
            set { input.text = value; }
        }

        Outline outline;

        public bool OutlineEnabled
        {
            get
            {
                if (outline == null)
                    outline = input.GetComponent<Outline>();
                if (outline != null)
                    return outline.enabled;
                else
                    return false;
            }
            set
            {
                if (outline == null)
                    outline = input.GetComponent<Outline>();
                if (outline != null)
                    outline.enabled = value;
            }
        }

        void Awake()
        {
            outline = input.GetComponent<Outline>();
        }

        public void SetOutlineColor(Color _color)
        {
            if (outline == null)
                outline = input.GetComponent<Outline>();
            if (outline != null)
                outline.effectColor = _color;
        }

        public void Clear()
        {
            input.text = "";
            outline.enabled = false;
        }
    }
}
