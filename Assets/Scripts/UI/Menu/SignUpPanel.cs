using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class SignUpPanel : MonoBehaviour
    {
        public Text title;
        public InputField firstnameInput;
        public InputField surnameInput;
        public InputField emailInput;
        public InputField passwordInput;
        public GroupToggleButton signUpToggle;

        Animator anim;
        Action PostLogin;

        private void Awake()
        {
            if (anim == null)
                anim = GetComponent<Animator>();
            
        }

        void Open(Action run)
        {
            if (anim == null)
                anim = GetComponent<Animator>();
            anim.SetBool("Show", true);
            PostLogin = run;
        }

        public void Open(Action run, bool isSignUp)
        {
            signUpToggle.SetToggle(isSignUp);
            ChangeSignStyle();
            Open(run);
        }

        void Clear()
        {
            firstnameInput.text = "";
            surnameInput.text = "";
            emailInput.text = "";
            passwordInput.text = "";
        }

        public void Close()
        {
            anim.SetBool("Show", false);
        }

        public void ChangeSignStyle()
        {
            if (signUpToggle.IsToggle)
            {
                title.text = "Create an Account";
            }
            else
            {
                title.text = "Sign In";
            }
            firstnameInput.gameObject.SetActive(signUpToggle.IsToggle);
            surnameInput.gameObject.SetActive(signUpToggle.IsToggle);
        }

        public void HidePassword(bool flag)
        {
            if (flag)
                passwordInput.contentType = InputField.ContentType.Password;
            else
                passwordInput.contentType = InputField.ContentType.Standard;
            passwordInput.ForceLabelUpdate();
        }

        void AfterLoginSubmit(MessageData data)
        {
            if (data != null)
            {
                if (data.status == 1)
                {
                    WebManager.CurrentUser.userKey = data.key;
                    if (PostLogin != null)
                        PostLogin();
                    Close();
                }
                else
                {

                }
            }
        }

        public void SignUp()
        {
            if(signUpToggle.IsToggle)
            {
                StartCoroutine(WebManager.Singleton.UserRegister(firstnameInput.text, surnameInput.text, emailInput.text, passwordInput.text, AfterSignUpSubmit));
            }
            else
            {
                StartCoroutine(WebManager.Singleton.UserLogin(emailInput.text, passwordInput.text, AfterLoginSubmit));
            }
        }

        void AfterSignUpSubmit(string str)
        {
            if (PostLogin != null)
                PostLogin.Invoke();
            Close();
        }
    }
}
