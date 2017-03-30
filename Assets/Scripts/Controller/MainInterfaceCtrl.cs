using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMAV.UI;
using UnityEngine.SceneManagement;

namespace IMAV
{
    public class MainInterfaceCtrl : MonoBehaviour
    {

        public LoginForm loginform;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Play()
        {
        }

        public void Login()
        {
            loginform.Open(GotoARScene);
        }

        public void GotoARScene()
        {
            SceneManager.LoadSceneAsync("test");
        }
    }
}
