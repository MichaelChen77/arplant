using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMAV.UI;
using UnityEngine.SceneManagement;

namespace IMAV
{
    public class MainInterfaceCtrl : MonoBehaviour
    {
        public SignUpPanel signupPanel;

        private void Start()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

        public void ClickGuestButton()
        {
            DataUtility.WorkOnLocal = true;
            GotoARScene();
        }

        public void ClickStartButton()
        {
            signupPanel.Open(GotoARScene, true);
        }

        public void ClickSignInButton()
        {
            signupPanel.Open(GotoARScene, false);
        }

        public void GotoARScene()
        {
            Screen.orientation = ScreenOrientation.AutoRotation;
            SceneManager.LoadSceneAsync("ARPlugin");
            //SceneManager.LoadSceneAsync("AR");
        }
    }
}
