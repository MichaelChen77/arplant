using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace IMAV.UI
{
    [RequireComponent(typeof(Image))]
    public class UIImage : UIControl, IPointerClickHandler
    {
        public GameObject coverPrefab;

        public System.Action<UIImage> OnClickHandler;

        bool selected = false;
        public bool Selected
        {
            get { return selected; }
            set {
                if (selected != value)
                {
                    selected = value;
                    if(selected)
                    {
                        if (coverObject == null)
                            coverObject = Instantiate(coverPrefab, transform);
                        else
                            coverObject.SetActive(true);
                    }
                    else
                    {
                        if (coverObject != null)
                            coverObject.SetActive(false);
                    }
                }
            }
        }

        protected Image image;
        protected GameObject coverObject;
        string imageTag;
        public string ImageTag
        {
            get { return imageTag; }
        }
        FileInfo imageFile;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public void LoadImage(string dir, FileInfo f, float animated = 0)
        {
            imageTag = dir + @"\" + f.Name;
            imageFile = f;
            ImageManager.Singleton.AddImage(imageTag);
            if (animated > 0)
            {
                transform.localScale = Vector3.zero;
                LeanTween.scale(gameObject, Vector3.one, animated);
            }
            StartCoroutine(Load(f.FullName));
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (OnClickHandler != null)
                OnClickHandler(this);
        }

        IEnumerator Load(string str)
        {
            yield return null;
            try
            {
                if (str != string.Empty)
                {
                    byte[] content = File.ReadAllBytes(str);
                    if(image == null)
                        image = GetComponent<Image>();
                    image.sprite = DataUtility.CreateSprit(content);
                }
            }
            catch { }
        }

        public override void Delete()
        {
            ImageManager.Singleton.DeleteImage(imageTag);
            Destroy(image.sprite);
            base.Delete();
        }

        public override void Refresh()
        {
            if(imageFile == null || !imageFile.Exists)
            {
                Destroy(image.sprite);
                base.Delete();
            }
        }
    }
}
