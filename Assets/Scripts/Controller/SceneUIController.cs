using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using IMAV.UI;

namespace IMAV.Controller
{
    public class SceneUIController : MonoBehaviour
    {
        public UIMenuPanel mainMenu;
        public UIProductMenu productMenu;
        UIControl currentPanel = null;
        public UIControl CurrentPanel
        {
            get { return currentPanel; }
            set { currentPanel = value; }
        }

        private void Start()
        {
            SceneController.Singleton.ProductChoosedEvent += OpenProductMenu;
        }

        #region UI
        public void OpenMenu()
		{
			closeCurrentPanel();
            mainMenu.OpenTrigger();
		}

		void closeCurrentPanel()
		{
			if (currentPanel != null)
			{
				currentPanel.Close();
				currentPanel = null;
			}
		}

		public void OpenUI(UIControl ui)
		{
			mainMenu.Close();
			currentPanel = ui;
			ui.Open();
		}

		public void CloseUI(bool openMenu)
		{
			closeCurrentPanel();
			if (openMenu)
				mainMenu.Open();
		}

		public void HideUI()
		{
			if (currentPanel is UIMediaCapture)
				return;
			mainMenu.Close();
			closeCurrentPanel();
		}

        public void OpenProductMenu(bool choose)
        {
            if (choose)
            {
                if (currentPanel != null)
                {
                    currentPanel.Close();
                    currentPanel = null;
                }
                productMenu.Open();
            }
            else
                productMenu.Close();
        }
        #endregion

        private void Update()
        {
            CheckTouch();
        }

        void CheckTouch()
		{
			if (Input.touchCount > 0)
			{
				foreach (Touch touch in Input.touches)
				{
					int id = touch.fingerId;
					if (EventSystem.current.IsPointerOverGameObject(id))
					{
                        SceneController.Singleton.SetCurrentObjectState(SelectState.Pause);
						return;
					}
				}

				if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit))
					{
						GameObject touchedObject = hit.transform.gameObject;
                        if (touchedObject != null || touchedObject.layer == SceneController.ProductLayer)
                        {
							SceneController.Singleton.SetCurrentObject(touchedObject);
							Debug.Log("Unity: touch on object : " + touchedObject);
						}
                        else
                        {
							SceneController.Singleton.SetCurrentProduct(null);
							Debug.Log("Unity: touch on null");
                        }
					}
                    else
                        SceneController.Singleton.SetCurrentProduct(null);
					HideUI();
				}

			}
		}
	}
}