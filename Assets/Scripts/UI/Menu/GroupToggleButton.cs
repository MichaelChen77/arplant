﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class GroupToggleButton : MonoBehaviour
    {
        public TextToggle[] targetTexts;

        RectTransform rect;

        [SerializeField]
        bool toggle = true;
        public bool IsToggle
        {
            get { return toggle; }
        }

        void Awake()
        {
            SetToggle(toggle);
        }

        public void SetToggle(bool flag)
        {
            toggle = flag;
            float w = 10f;
            if(rect == null)
                rect = GetComponent<RectTransform>();
            foreach (TextToggle t in targetTexts)
            {
                t.SetToggle(toggle);
                w += t.targetText.preferredWidth;
            }
            rect.sizeDelta = new Vector2(w, rect.sizeDelta.y);
        }

        public void SetToggle()
        {
            SetToggle(!toggle);
        }
    }
}
