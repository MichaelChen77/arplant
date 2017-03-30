using System;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class LoginForm : MonoBehaviour
    {
        public Image header;
        public Text headerText;
        public CustomInputField emailInput;
        public CustomInputField pwInput;
        public SignUpForm signupForm;
        public Color errorColor;
        Color normalColor;
        Action PostLogin;

        // Use this for initialization
        void Awake()
        {
            normalColor = header.color;
        }

        public void Open(Action run)
        {
            PostLogin = run;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            PostLogin = null;
            gameObject.SetActive(false);
        }

        void Clear()
        {
            header.color = normalColor;
            headerText.text = "User Login";
            emailInput.Clear();
            pwInput.Clear();
        }

        public void Login()
        {
            StartCoroutine(WebManager.Singleton.UserLogin(emailInput.text, pwInput.text, AfterLoginSubmit));
        }

        void AfterLoginSubmit(string str)
        {
            PostLogin();
            Close();
        }

        public void GotoSignUpPage()
        {
            Close();
        }

        public void LoginByFacebook()
        {

        }

        public void LoginByGoogle()
        {

        }
    }
}
