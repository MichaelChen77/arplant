using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class UISwipeToggleGroup : UIControl
    {
        public ToggleGroup targetGroup;
        public UISwipe swipe;
        public bool reset = false;

        private Toggle[] listToggles;
        private int totalPages;
        private int curPage;

        void Awake()
        {
            if (listToggles == null)
                listToggles = targetGroup.GetComponentsInChildren<Toggle>();
            swipe.OnSwipeCompleted = UpdatePos;
        }

        public override void Open()
        {
            base.Open();
            if (listToggles == null)
                listToggles = targetGroup.GetComponentsInChildren<Toggle>();
            swipe.PageCount = listToggles.Length;
            swipe.Open();
        }

        private void UpdatePos()
        {
            listToggles[swipe.CurrentPage].isOn = true;
        }

        public override void Close()
        {
            if (reset)
                ResetPages();
            base.Close();
        }

        public void ResetPages()
        {
            swipe.CurrentPage = 0;
            listToggles[0].isOn = true;
        }
    }
}
