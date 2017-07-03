using System;
using UnityEngine;
using UnityEngine.UI;

namespace IMAV.UI
{
    public class SignUpForm : MonoBehaviour
    {
        public Image headerImage;
        public Text headerText;
        public Color errorColor;
        public CustomInputField firstNameInput;
        public CustomInputField surNameInput;
        public CustomInputField emailInput;
        public CustomInputField pwInput;
        Action PostSignUP;
        Color normalColor;

        // Use this for initialization
        void Awake()
        {
            normalColor = headerImage.color;
        }

        public void Open(Action run)
        {
            Clear();
            PostSignUP = run;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            PostSignUP = null;
            gameObject.SetActive(false);
        }

        void Clear()
        {
            headerImage.color = normalColor;
            firstNameInput.Clear();
            surNameInput.Clear();
            emailInput.Clear();
            pwInput.Clear();
        }

        public void SignUp()
        {
            StartCoroutine(WebManager.Singleton.UserRegister(firstNameInput.text, surNameInput.text, emailInput.text, pwInput.text, AfterSignUpSubmit));
        }

        void AfterSignUpSubmit(string str)
        {
            if (PostSignUP != null)
                PostSignUP.Invoke();
        }

        public void Login()
        {
            Close();
        }
    }
}
