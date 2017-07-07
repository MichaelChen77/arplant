using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Collections.Generic;

namespace IMAV.UI
{
    public class ImageGallery : MonoBehaviour
    {
        public GameObject header;
        public Text nameText;
        public Image currentImage;
        public float prevThold;
        public float nextThold;

        List<string> files = new List<string>();
        int currentIndex;
        float dragDistance = 0;
        bool startDrag = false;
        string prevfile = "";

        void Start()
        {
        }

        // Use this for initialization
        public void Open(string str)
        {
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

    }
}
