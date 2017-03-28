using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class DataManager : MonoBehaviour {
    public SearchForm searchform;
    public SpinUI loadingImage;
    //List<GameObject> sceneObjects = new List<GameObject>();
    Dictionary<uint, FurnitureData> furnitureDict = new Dictionary<uint, FurnitureData>();
    public Dictionary<uint, FurnitureData> FurnitureDatas
    {
        get { return furnitureDict; }
    }
    Dictionary<string, bool> tempFiles = new Dictionary<string, bool>();
    int loadedCount = 0;
    uint loadedModelID = 0;

    private static DataManager mSingleton;
    public static DataManager Singleton
    {
        get
        {
            return mSingleton;
        }
    }

    void Awake()
    {
        if (mSingleton)
        {
            Destroy(gameObject);
        }
        else
        {
            mSingleton = this;
        }
    }

    void LoadData(string str)
    {
        try
        {
            string[] strs = str.Split(';');
            foreach (string s in strs)
            {
                if (s != "")
                {
                    string[] subs = s.Split('>');
                    uint _id = Convert.ToUInt32(subs[0]);
                    if (!furnitureDict.ContainsKey(_id))
                    {
                        FurnitureData _data = new FurnitureData(_id, subs[1]);
                        _data.Category = subs[2];
                        _data.Brand = subs[3];
                        _data.Manufacturer = subs[4];
                        _data.ProductionSite = subs[5];
                        _data.ProductionDate = subs[6];
                        _data.Price = Convert.ToSingle(subs[7]);
                        ServerScriptUtil.DownloadImageFile(DataUtility.GetImagePath(_data), DownloadImageCallback);
                        loadedCount++;
                        furnitureDict[_id] = _data;
                    }
                    searchform.SearchResult.Add(furnitureDict[_id]);
                }
            }
            ServerScript.Singleton.StartPHPRequests();
        }
        catch(IndexOutOfRangeException ioex)
        {
            Debug.Log("DataManager->Load Data->IndexOutofRange: " + ioex.Message);
        }
        catch(Exception ex)
        {
            Debug.Log("DataManager->Load Data->Exception: " + ex.Message);
        }
    }

    public void DownloadImageCallback(string path, byte[] content)
    {
        try
        {
            string[] strs = path.Split('/');
            uint _id = Convert.ToUInt32(strs[1]);
            if (furnitureDict.ContainsKey(_id))
            {
                furnitureDict[_id].thumbnail = DataUtility.CreateSprit(content);
            }
            loadedCount--;
            if (loadedCount == 0)
            {
                searchform.Refresh();
            }
        }
        catch(Exception ex)
        {
            Debug.Log("DataManager DownloadImageCallback Exception: " + ex.Message);
        }
    }

    public void Search(string _type, string _name)
    {
        StartCoroutine(WebManager.Singleton.SearchFurniture(_type, _name, LoadData));
    }

    public void LoadDataModle(FurnitureData _data)
    {
        searchform.Close();
        loadedModelID = _data.ID;
        if (_data.model == null)
        {
            if (_data.CheckModelExist())
            {
                StartLoadOBjFile(_data);
            }
            else
            {
                PhpDownloadRequest r = new PhpDownloadRequest("GetList", DataUtility.GetModelPath(_data), DownloadModelCallback, VirtualFile.FileType.String);
                ServerScript.Singleton.AppendRequest(r);
                ServerScript.Singleton.StartPHPRequests();
            }
        }
        else
        {
            GameObject obj = Instantiate(_data.model);
			ResourceManager.Singleton.AddMarkerlessObject (obj);
        }
    }

    public void DownloadModelCallback(string path, string content)
    {
        try
        {
            string[] strs = path.Split('/');
            uint _id = Convert.ToUInt32(strs[1]);
            loadedCount = 0;
            tempFiles.Clear();
            if (furnitureDict.ContainsKey(_id))
            {
                string[] files = content.Split(';');
                //Debug.Log("data path: " + Application.dataPath + "; path2: " + Application.persistentDataPath + "; path3: " + Application.streamingAssetsPath + "; path4: " + Application.temporaryCachePath);
                string mpath = DataUtility.GetLocalModelPath(furnitureDict[loadedModelID]);
                if (!Directory.Exists(mpath))
                {
                    Directory.CreateDirectory(mpath);
                }
                foreach(string str in files)
                {
                    if (str != "")
                    {
                        string urlPath = DataUtility.GetModelPath(furnitureDict[loadedModelID]) + str;
                        tempFiles[urlPath] = false;
                        ServerScriptUtil.DownloadBinaryFile(urlPath, DownloadFileCallback);
                        loadedCount++;
                    }
                }
                ServerScript.Singleton.StartPHPRequests();
            }
        }
        catch (Exception ex)
        {
            Debug.Log("DownloadModelCallback Exception: " + ex.Message);
        }
    }

    public void DownloadFileCallback(string path, byte[] content)
    {
        try
        {
            FileInfo f = new FileInfo(path);
            if (tempFiles.ContainsKey(path))
            {
                tempFiles[path] = true;
                string mpath = DataUtility.GetLocalModelPath(furnitureDict[loadedModelID]) + f.Name;
                File.WriteAllBytes(mpath, content);
                loadedCount--;
                DownloadInvalidation();
            }
        }
        catch(Exception ex)
        {
            Debug.Log("error: " + ex.Message);
        }
    }

    void DownloadInvalidation()
    {
        if (loadedCount == 0)
        {
            int _count = 0;
            foreach (KeyValuePair<string, bool> pair in tempFiles)
            {
                if (!pair.Value)
                {
                    ServerScriptUtil.DownloadBinaryFile(pair.Key, DownloadFileCallback);
                }
            }
            if (_count != 0)
            {
                loadedCount = _count;
                ServerScript.Singleton.StartPHPRequests();
            }
            else
                StartLoadOBjFile(furnitureDict[loadedModelID]);
        }
    }


    void StartLoadOBjFile(FurnitureData _data)
    {
        if (loadingImage != null)
            loadingImage.Show();
        string file = DataUtility.GetLocalModelFile(_data);
        OBJLoader.Singleton.LoadOBJ(file, AfterLoadOBJ);
    }

    void AfterLoadOBJ(GameObject obj)
    {
        furnitureDict[loadedModelID].model = obj;
        if (loadingImage != null)
            loadingImage.Hide();
		ResourceManager.Singleton.AddMarkerlessObject (obj);
//        MainCamCtrl cam = Camera.main.GetComponent<MainCamCtrl>();
//        if (cam != null)
//            cam.orbitPivot = obj.transform;
    }

//    public void Clear()
//    {
//        foreach(GameObject obj in sceneObjects)
//        {
//            Destroy(obj);
//        }
//        sceneObjects.Clear();
//    }

//    public void Quit()
//    {
//        Application.Quit();
//    }
}
