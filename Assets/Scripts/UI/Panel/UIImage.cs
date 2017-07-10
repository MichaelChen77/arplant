using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace IMAV.UI
{
    [RequireComponent(typeof(Image))]
    public class UIImage : MonoBehaviour, IPointerClickHandler
    {
        public System.Action<UIImage> OnClickHandler;

        protected Image image;
        string imageTag;
        public string ImageTag
        {
            get { return imageTag; }
        }

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public void LoadImage(FileInfo f)
        {
            imageTag = f.Name;
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
    }
}
