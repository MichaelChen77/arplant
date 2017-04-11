using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DataImageType
{
    Thumnail, Logo, Factory, Brand
}

[Serializable]
public struct DataViewItem
{
    public string name;
    public Text text;
}

namespace IMAV.UI
{
    public class DataPresentForm : MonoBehaviour
    {

        public DataViewItem[] presentDatas;
        public Image dataIcon;
        public Camera uiCam;
        public RectTransform lineRect;
        public int showFrame = 20;
        public Vector2 range = new Vector2(5f, 3f);
        public DataImageType iconType = DataImageType.Thumnail;

        Mask formMask;
        Image formImage;
        RectTransform formRect;
        Quaternion mStart;
        Vector2 mRot = Vector2.zero;
        float height = 0;
        float lineLength = 0;
        Color tranperantColor;
        bool isShowed = false;

        // Use this for initialization
        void Awake()
        {
            formMask = GetComponent<Mask>();
            formImage = GetComponent<Image>();
            formRect = GetComponent<RectTransform>();
            mStart = transform.localRotation;
            height = formRect.sizeDelta.y;
            lineLength = lineRect.sizeDelta.x;
            tranperantColor = new Color(0, 0, 0, 0);
        }

        // Update is called once per frame
        void Update()
        {
            if (isShowed)
            {
                Vector3 pos = Input.mousePosition;

                float halfWidth = Screen.width * 0.5f;
                float halfHeight = Screen.height * 0.5f;
                float x = Mathf.Clamp((pos.x - halfWidth) / halfWidth, -1f, 1f);
                float y = Mathf.Clamp((pos.y - halfHeight) / halfHeight, -1f, 1f);
                mRot = Vector2.Lerp(mRot, new Vector2(x, y), Time.deltaTime * 5f);

                transform.localRotation = mStart * Quaternion.Euler(-mRot.y * range.y, mRot.x * range.x, 0f);
            }
        }

        public void LoadData(Dictionary<string, string> values, Sprite icon)
        {
            foreach (DataViewItem item in presentDatas)
            {
                if (values.ContainsKey(item.name))
                {
                    item.text.text = values[item.name];
                }
            }
            if (dataIcon != null)
            {
                dataIcon.sprite = icon;
            }
        }

        public void Show(ARObject obj)
        {
            if (obj != null)
            {
                gameObject.SetActive(true);
                isShowed = true;
                GetDirectionTo(obj.transform.position);
                formRect.sizeDelta = new Vector2(formRect.sizeDelta.x, 0);
                formMask.enabled = true;
                formImage.color = Color.white;
                StartCoroutine(showout());
            }
            else
                Hide();
        }

        public void Hide()
        {
            isShowed = false;
            gameObject.SetActive(false);
        }

        IEnumerator showout()
        {
            float uh = height / showFrame;
            float h = 0;
            for (int i = 0; i < showFrame; i++)
            {
                yield return new WaitForFixedUpdate();
                h += uh;
                formRect.sizeDelta = new Vector2(formRect.sizeDelta.x, h);
            }
            formRect.sizeDelta = new Vector2(formRect.sizeDelta.x, height);
            formMask.enabled = false;
            formImage.color = tranperantColor;
            float ul = lineLength / showFrame;
            h = 0;
            for (int j = 0; j < showFrame; j++)
            {
                yield return new WaitForFixedUpdate();
                h += ul;
                lineRect.sizeDelta = new Vector2(h, lineRect.sizeDelta.y);
            }
            lineRect.sizeDelta = new Vector2(lineLength, lineRect.sizeDelta.y);
        }

        public void GetDirectionTo(Vector3 pos)
        {
            Vector3 viewPos = uiCam.WorldToViewportPoint(pos);
            Vector3 dir = viewPos - transform.position;
            dir.z = 0;
            float rot = Vector2.Angle(Vector2.right, new Vector2(dir.x, dir.y));
            if (dir.y < 0)
                rot = -rot;
            lineRect.transform.eulerAngles = new Vector3(0, 0, rot);
            float f = formRect.rect.max.magnitude;
            lineRect.anchoredPosition = dir.normalized * f;
            lineRect.sizeDelta = new Vector2(0, lineRect.sizeDelta.y);
        }
    }
}
