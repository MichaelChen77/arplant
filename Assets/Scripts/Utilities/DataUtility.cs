using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace IMAV
{
    public class DataUtility
    {
        public const float ProductRotateSpeed = 1000f;
        public const float ProductStationaryTouchTimeThreshold = 1f;

        public static ARProduct CurrentObject = null;
        public static DontDestroy dontdestroy = null;
        public static ARTrackingMode TrackingMode = ARTrackingMode.Markerless;
        public static bool WorkOnLocal = false;
        public static string ProjectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 7);

        public static Sprite CreateSprite(byte[] bytes)
        {
            if (bytes == null)
                return null;
            Texture2D tex = new Texture2D(10, 10);
            tex.LoadImage(bytes);
            Rect _rect = new Rect(0, 0, tex.width, tex.height);
            Sprite newSprite = Sprite.Create(tex, _rect, new Vector2(0.5f, 0.5f));
            return newSprite;
        }

        public static Sprite CreateSprite(Texture2D tex)
        {
            if (tex == null)
            {
                Debug.Log("text null");
                return null;
            }
            Rect _rect = new Rect(0, 0, tex.width, tex.height);
            Sprite newSprite = Sprite.Create(tex, _rect, new Vector2(0.5f, 0.5f));
            return newSprite;
        }

        public static Sprite CreateSprite(string str)
        {
            if (str != string.Empty)
            {
                try
                {
                    FileInfo fi = new FileInfo(str);
                    if (fi.Exists)
                    {
                        byte[] content = File.ReadAllBytes(str);
                        return CreateSprite(content);
                    }
                }
                catch { }
            }
            return null;
        }

        public static Texture2D CreateTexture(string str)
        {
            if (str != string.Empty)
            {
                try
                {
                    FileInfo fi = new FileInfo(str);
                    if (fi.Exists)
                    {
                        byte[] content = File.ReadAllBytes(str);
                        Texture2D tex = new Texture2D(10, 10);
                        tex.LoadImage(content);
                        return tex;
                    }
                }
                catch { }
            }
            return null;
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

        public static string GetScreenShotPath()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return ProjectPath +"/TestSample/ScreenShots/";
#else
            return Application.persistentDataPath + "/ScreenShots/";
#endif
        }

        public static string GetScreenThumbnailPath()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return ProjectPath + "/TestSample/ScreenThumbnails/";
#else
            return Application.persistentDataPath + "/ScreenThumbnails/";
#endif
            
        }

        public static string GetScreenVideoPath()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return ProjectPath + "/TestSample/Videos/";
#else
            return Application.persistentDataPath + "/Videos/";
#endif
        }

        public static string GetSessionPath()
        {
            string basePath = Application.temporaryCachePath;
#if UNITY_ANDROID
            string[] sParts = basePath.Split('/');
            basePath = "/sdcard/Android/data/" + sParts[sParts.Length - 2] + "/cache/sessions/";
#elif UNITY_IOS
  basePath = basePath.Substring (0,basePath.Length-15);
  basePath += "/tmp/Everyplay/session/";
#endif
            return basePath;
        }

        public static string GetCategoryPath()
        {
#if UNITY_EDITOR
            return ProjectPath + "/TestSample/Cats/";
#else
            return Application.persistentDataPath + "/Cats/";
#endif
        }

		public static string GetSubCategoryPath()
		{
#if UNITY_EDITOR
			return ProjectPath + "/TestSample/SubCats/";
#else
			return Application.persistentDataPath + "/SubCats/";
#endif
		}

		public static string GetSubCategoryIconPath()
		{
			#if UNITY_EDITOR
			return ProjectPath + "/TestSample/SubCats/Icons/";
			#else
			return Application.persistentDataPath + "/SubCats/Icons/";
			#endif
		}

        public static string GetProductPath()
        {
#if UNITY_EDITOR
            return ProjectPath + "/TestSample/Products/";
#else
            return Application.persistentDataPath + "/Products/";
#endif
        }

        public static string GetProductIconPath()
        {
#if UNITY_EDITOR
            return ProjectPath + "/TestSample/Products/Icons/";
#else
            return Application.persistentDataPath + "/Products/Icons/";
#endif
        }

        public static string GetProductModelPath()
        {
#if UNITY_EDITOR
            return ProjectPath + "/TestSample/Products/Models/";
#else
            return Application.persistentDataPath + "/Products/Models/";
#endif
        }

//		public static void DeleteProductModelFile(string ProductName)
//		{
//			string path = "";
//#if UNITY_EDITOR
//			path = ProjectPath + "/TestSample/Products/Models/" + ProductName;
//#else
//			path = Application.persistentDataPath + "/Products/Models/" + ProductName;
//#endif
//			Debug.Log ("model file path = " + path);
//			FileAttributes attr = File.GetAttributes(path);
//			if (File.Exists (path)) {
//				Debug.Log ("delete model name = " + ProductName);
//				if (attr == FileAttributes.Directory) {
//					Directory.Delete (path, true);
//				} else {
//					File.Delete (path);
//				}
////				File.Delete (path);
//				Debug.Log ("delete successful");
//			} else {
//				Debug.Log ("model file path not exit ");
//			}
//		}



        public static string GetCategoryFile()
        {
#if UNITY_EDITOR
            return ProjectPath + "/TestSample/Cats/cats.json";
#else
            return Application.persistentDataPath + "/Cats/cats.json";
#endif
        }



        public static void SetObjectColliderLayer(GameObject obj, int layer)
        {
            obj.layer = layer;
			Collider[] cols = obj.GetComponentsInChildren<Collider>();
			for (int i = 0; i < cols.Length; i++)
			{
                cols[i].gameObject.layer = layer;
			}
        }

        public static void SetObjectLayer(GameObject obj, int layer)
        {
            obj.layer = layer;
            if(obj.transform.childCount > 0)
            {
                foreach(Transform child in obj.transform)
                {
                    SetObjectLayer(child.gameObject, layer);
                }
            }
        }

        public static int GetRelatedHeight(int w)
        {
            int h = w * Screen.height / Screen.width;
            int _t = h % 4;
            h = h - _t;
            return h;
        }

        public static string GetPathName(string str)
        {
            return str.Substring(0, str.LastIndexOf('/'));
        }

        public static void SetDirectory(string str)
        {
            if (!Directory.Exists(str))
            {
                Directory.CreateDirectory(str);
            }
        }

        public static void Swap<T>(List<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        public static Vector2 GetFitVector(bool isHorizontal, Vector2 source, Vector2 target)
        {
            if (isHorizontal)
            {
                float y = target.x * source.y / source.x;
                return new Vector2(target.x, y);
            }
            else
            {
                float x = target.y * source.x / source.y;
                return new Vector2(x, target.y);
            }
        }

        public static string CovertToTimeString(int t)
        {
            string str = "";
            if (t > 3600)
                str = (t / 3600).ToString("00")+":";
            else
                str = "00:";
            t = t % 3600;
            if (t > 60)
                str += (t / 60).ToString("00")+":";
            else
                str += "00:";
            t = t % 60;
            str += t.ToString("00");
            return str;
        }

        public static ARProduct InitARObject(GameObject obj, Transform parentTransform)
        {
            obj.transform.parent = parentTransform;
            obj.layer = 8;
            obj.transform.localScale = obj.transform.localScale * 164;
            if (TrackingMode != ARTrackingMode.Marker)
            {
                obj.transform.position = ResourceManager.Singleton.TrackPos;
            }
            else
            {
                obj.transform.localPosition = Vector3.zero;
            }
            Quaternion quat = obj.transform.rotation;
            obj.transform.localRotation = quat;
            return obj.AddComponent<ARProduct>();
        }

        public static ARProduct InitARObject(GameObject obj)
        {
            obj.layer = 8;
            obj.transform.localScale = obj.transform.localScale * 200;
            obj.transform.localPosition = Vector3.zero;
            return obj.AddComponent<ARProduct>();
        }
    }
}