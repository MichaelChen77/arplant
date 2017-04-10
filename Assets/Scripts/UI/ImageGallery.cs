using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Collections.Generic;

namespace IMAV.UI
{
    public class ImageGallery : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
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
            ResetFiles();
        }

        void ResetFiles()
        {
            files.Clear();
            string[] strs = Directory.GetFiles(DataUtility.GetScreenShotPath());
            foreach (string s in strs)
            {
                files.Add(s);
            }
        }

        // Use this for initialization
        public void Open(string str)
        {
            gameObject.SetActive(true);
            if (!files.Contains(str))
                files.Add(str);
            Load(str);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            if (files.Count <= 0)
                ResetFiles();
            if (files.Count > 0)
            {
                if (currentIndex >= files.Count || currentIndex < 0)
                    currentIndex = 0;
                Load(files[currentIndex]);
            }
        }

        public void Close()
        {
            gameObject.SetActive(false);
            ResourceManager.Singleton.Resume();
        }

        public void Load(string str)
        {
            try
            {
                if (str != "")
                {
                    FileInfo fi = new FileInfo(str);
                    if (fi.Exists)
                    {
                        nameText.text = fi.Name;
                        byte[] content = File.ReadAllBytes(str);
                        currentImage.sprite = DataUtility.CreateSprit(content);
                        currentIndex = getFileIndex(str);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("error load: " + ex.Message);
            }
        }

        int getFileIndex(string path)
        {
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i] == path)
                    return i;
            }
            return 0;
        }

        public void LoadNext()
        {
            currentIndex++;
            if (currentIndex >= files.Count)
                currentIndex = 0;
            Load(files[currentIndex]);
        }

        public void LoadPrev()
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = files.Count - 1;
            Load(files[currentIndex]);
        }

        public void OnPointerClick(PointerEventData data)
        {
            header.SetActive(!header.activeSelf);
        }

        public void OnBeginDrag(PointerEventData data)
        {
            dragDistance = 0;
            startDrag = true;
        }

        public void OnDrag(PointerEventData data)
        {
            if (startDrag)
            {
                dragDistance += data.delta.x;
                if (dragDistance > nextThold)
                {
                    LoadNext();
                    startDrag = false;
                }
                else if (dragDistance < prevThold)
                {
                    LoadPrev();
                    startDrag = false;
                }
            }
        }

        public void OnEndDrag(PointerEventData data)
        {
            startDrag = false;
        }
    }
}
