using UnityEngine;
using System;

public class DataUtility{

	public static bool firstobject = true;

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
        catch(Exception ex)
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
		return Application.persistentDataPath+"/ScreenShots/";
	}

	public static void SetAsMarkerlessObject(GameObject obj, bool init)
	{
		obj.transform.parent = ResourceManager.Singleton.markerlessTransform;
		if (init) {
			obj.layer = 8;
			obj.transform.localScale = obj.transform.localScale * 100;
			if (firstobject) {
				obj.transform.localPosition = new Vector3 (0, 0, 0);
				firstobject = false;
			} else {
				GameObject arrow = GameObject.Find ("Arrow");
				Vector3 relativePoint = ResourceManager.Singleton.markerlessTransform.InverseTransformPoint (arrow.transform.localPosition.x, arrow.transform.localPosition.y, arrow.transform.localPosition.z);
				obj.transform.localPosition = relativePoint;
			} 
			Quaternion quat = obj.transform.rotation;
			obj.transform.localRotation = quat;
			Debug.Log ("# object " + obj.name + " rot: " + obj.transform.rotation + " ; " + obj.transform.localRotation + " ; " + LayerMask.LayerToName (obj.layer));
			//obj.transform.rotation = ResourceManager.Singleton.StartFloorOrientation;
			obj.AddComponent<MarkerlessTouchControl> ();
			//obj.AddComponent<Outline> ();
			if (obj.GetComponent<BoxCollider> () == null) {
				BoxCollider box = obj.AddComponent<BoxCollider> ();
				box.isTrigger = false;
			}
			ResourceManager.Singleton.SetDefaultSize (obj);
		}
	}
}
