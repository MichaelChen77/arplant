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
        public GameObject videoCover;

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

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public void LoadImage(string dir, FileInfo f, float animated = 0)
        {
            Clear();
            imageTag = dir + "/" + f.Name;
            if(MediaCenter.Singleton.IsVideoImage(imageTag))
                Instantiate(videoCover, transform);
            name = f.Name;
            if (animated > 0)
            {
                transform.localScale = Vector3.zero;
                LeanTween.scale(gameObject, Vector3.one, animated);
            }
            StartCoroutine(Load(imageTag));
        }

        public void RenameTag(string fileName)
        {
            imageTag = fileName;
            name = imageTag.Substring(imageTag.IndexOf('/') + 1);
            int index = imageTag.IndexOf('/');
            imageTag = imageTag.Substring(0, index + 1) + name;
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (OnClickHandler != null)
                OnClickHandler(this);
        }

        IEnumerator Load(string str)
        {
            yield return null;
            image.sprite = MediaCenter.Singleton.GetImage(str, true);
        }

        public void Delete(bool removeFile)
        {
            if (removeFile)
                MediaCenter.Singleton.DeleteFile(imageTag);
            Clear();
            Delete();
        }


        public void Clear()
        {
            if (image.sprite != null)
            {
                Destroy(image.sprite.texture);
                Destroy(image.sprite);
            }
        }
    }
}
