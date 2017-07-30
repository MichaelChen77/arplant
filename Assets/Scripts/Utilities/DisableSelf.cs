using UnityEngine;
using UnityEngine.UI;

namespace IMAV
{
    public class DisableSelf : MonoBehaviour
    {
        public Text hintText;
        public float duration = 1f;

        Color originColor;

        public void Open()
        {
			gameObject.SetActive (true);
            originColor = hintText.color;
            LeanTween.alphaText(GetComponent<RectTransform>(), 0, duration).setOnComplete(Close);
            Invoke("Close", duration);
        }

        public void Open(string str)
        {
            hintText.text = str;
            Open();
        }

        public void Open(float t)
        {
            duration = t;
            Open();
        }

        void Close()
        {
            gameObject.SetActive(false);
            hintText.color = originColor;
        }
    }
}
