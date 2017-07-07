using UnityEngine;
using System;

namespace IMAV
{
    public class DataUtility
    {
        public static GameObject CurrentObject = null;
        public static DontDestroy dontdestroy = null;
        public static int VirtualModeInt = 0;
        public static bool WorkOnLocal = false;

        public static Sprite CreateSprit(byte[] bytes)
        {
            if (bytes == null)
                return null;
            Texture2D tex = new Texture2D(10, 10);
            tex.LoadImage(bytes);
            Rect _rect = new Rect(0, 0, tex.width, tex.height);
            Sprite newSprite = Sprite.Create(tex, _rect, new Vector2(0.5f, 0.5f));
            return newSprite;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static Vector3 ConvertToVector3(string str1, string str2, string str3)
        {
            try
            {
                float f1 = Convert.ToSingle(str1);
                float f2 = Convert.ToSingle(str2);
                float f3 = Convert.ToSingle(str3);
                Vector3 vec = new Vector3(f1, f2, f3);
                return vec;
            }
            catch (Exception ex)
            {
                Debug.Log("error: " + ex.Message);
                return Vector3.zero;
            }
        }

        public static string GetModelPath(FurnitureData _data)
        {
            return _data.category_name + "/" + _data.id + "/OBJ/";
        }

        public static string GetLocalModelPath(FurnitureData _data)
        {
            return Application.persistentDataPath + "/Data/" + _data.category_name + "/" + _data.id + "/OBJ/";
        }

        public static string GetLocalModelFile(FurnitureData _data)
        {
            return GetLocalModelPath(_data) + _data.name + ".obj";
        }

        public static string GetImagePath(FurnitureData _data)
        {
            return _data.category_name + "/" + _data.id + "/" + _data.name;
        }

        public static string GetScreenShotPath()
        {
            return Application.persistentDataPath + "/ScreenShots/";
        }

        public static string GetScreenThumbnailPath()
        {
            return Application.persistentDataPath + "/ScreenThumbnails/";
        }

		public static ARModel SetAsMarkerlessObject(GameObject obj, bool init, bool isLocal, string _content)
        {
            obj.transform.parent = ResourceManager.Singleton.markerlessTransform;
            if (init)
            {
                obj.layer = 8;
                obj.transform.localScale = obj.transform.localScale * 100;
                if (ResourceManager.Singleton.VMode == VirtualMode.Markerless)
                {
                    obj.transform.localPosition = Vector3.zero;
                    Quaternion quat = obj.transform.rotation;
                    obj.transform.localRotation = quat;
                }
                else if(ResourceManager.Singleton.VMode == VirtualMode.Placement)
                {
                    obj.transform.position = ResourceManager.Singleton.TrackPos;
                    Quaternion quat = obj.transform.rotation;
                    obj.transform.rotation = ResourceManager.Singleton.TrackRotation * quat;
                }

                ResourceManager.Singleton.DebugString("# object " + obj.name + " rot: " + obj.transform.rotation + " ; " + obj.transform.localRotation + " ; " + obj.transform.localPosition+";" + LayerMask.LayerToName(obj.layer));
				return obj.AddComponent<ARModel>();
            }
			return obj.GetComponent<ARModel>();
        }
    }
}