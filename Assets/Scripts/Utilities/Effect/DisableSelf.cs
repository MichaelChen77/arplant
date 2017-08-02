using UnityEngine;
using UnityEngine.UI;

namespace IMAV
{
    public enum AnimatedType
    {
        Alpha, AlphaText, AlphaVertex, Scale, ScaleX, ScaleY, ScaleZ, Size, Color
    }

    public class DisableSelf : MonoBehaviour
    {
        public RectTransform targetRect;
        public float duration = 1f;
        public Color targetColor;
        public AnimatedType disableType = AnimatedType.Alpha;

        Vector3 originVector;
        Color originColor;

        void Start()
        {
            animSave();
        }

        public void Open(float t)
        {
            duration = t;
            Open();
        }

        public void OpenBelow(RectTransform tran)
        {
            OpenBelow(tran, 1.4f);
        }

        public void OpenBelow(RectTransform tran, float rate)
        { 
            transform.SetParent(tran.parent);
            transform.SetSiblingIndex(tran.GetSiblingIndex());
            transform.position = tran.position;
            targetRect.sizeDelta = tran.rect.size * rate;
            Open();
        }

        public virtual void Open()
        {
			gameObject.SetActive (true);
            animDisable();
            Invoke("Close", duration);
        }

        public void SetColor(Color c)
        {
            Graphic g = targetRect.GetComponent<Graphic>();
            if (g != null)
            {
                g.color = c;
                originColor = c;
            }
        }

        protected virtual void animDisable()
        {
            switch (disableType)
            {
                case AnimatedType.Alpha: LeanTween.alpha(targetRect, 0, duration); break;
                case AnimatedType.AlphaText: LeanTween.alphaText(targetRect, 0, duration); break;
                case AnimatedType.AlphaVertex: LeanTween.alphaVertex(targetRect.gameObject, 0, duration); break;
                case AnimatedType.Scale: LeanTween.scale(targetRect, Vector3.zero, duration); break;
                case AnimatedType.ScaleX: LeanTween.scaleX(targetRect.gameObject, 0, duration); break;
                case AnimatedType.ScaleY: LeanTween.scaleY(targetRect.gameObject, 0, duration); break;
                case AnimatedType.ScaleZ: LeanTween.scaleZ(targetRect.gameObject, 0, duration); break;
                case AnimatedType.Size: LeanTween.size(targetRect, Vector2.zero, duration); break;
                case AnimatedType.Color: LeanTween.color(targetRect, targetColor, duration); break;
            }
        }

        public virtual void animSave()
        {
            bool scaleflag = disableType == AnimatedType.Scale || disableType == AnimatedType.ScaleX || disableType == AnimatedType.ScaleY || disableType == AnimatedType.ScaleZ;
            if (scaleflag)
                originVector = targetRect.localScale;
            else if (disableType == AnimatedType.Size)
                originVector = targetRect.rect.size;
            else
            {
                Graphic g = targetRect.GetComponent<Graphic>();
                if (g != null)
                    originColor = g.color;
            }
        }

        public virtual void animRestore()
        {
            bool scaleflag = disableType == AnimatedType.Scale || disableType == AnimatedType.ScaleX || disableType == AnimatedType.ScaleY || disableType == AnimatedType.ScaleZ;
            if (scaleflag)
                targetRect.localScale = originVector;
            else if (disableType == AnimatedType.Size)
                targetRect.sizeDelta = originVector;
            else
            {
                Graphic g = targetRect.GetComponent<Graphic>();
                if (g != null)
                    g.color = originColor;
            }
        }

        protected virtual void Close()
        {
            animRestore();
            gameObject.SetActive(false);
        }
    }
}
