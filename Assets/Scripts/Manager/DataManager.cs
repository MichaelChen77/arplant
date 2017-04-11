﻿using UnityEngine;
using System;
using System.IO;
using IMAV.UI;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace IMAV
{
    public class DataManager : MonoBehaviour
    {
        public SearchForm searchform;
        public SceneForm sceneform;
        public SpinUI loadingImage;
        //List<GameObject> sceneObjects = new List<GameObject>();
        Dictionary<int, FurnitureData> furnitureDict = new Dictionary<int, FurnitureData>();
        public Dictionary<int, FurnitureData> FurnitureDatas
        {
            get { return furnitureDict; }
        }
        Dictionary<string, bool> tempFiles = new Dictionary<string, bool>();
        SceneSet scenes;
        public SceneSet Scenes
        {
            get { return scenes; }
        }

        int loadedCount = 0;
        int loadedModelID = 0;

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
                FurnitureSet fs = JsonConvert.DeserializeObject<FurnitureSet>(str);
                foreach (FurnitureData f in fs.Data)
                {
                    if (!furnitureDict.ContainsKey(f.id))
                    {
                        furnitureDict[f.id] = f;
                        loadedCount++;
                        StartCoroutine(WebManager.Singleton.DownloadBinary(f.id, Tags.basefileUrl + f.thumbnail_path, DownloadImageCallback));
                        //ServerScriptUtil.DownloadImageFile(f.thumbnail_path, DownloadImageCallback);
                    }
                    searchform.SearchResult.Add(furnitureDict[f.id]);
                }
                //ServerScript.Singleton.StartPHPRequests();
            }
            catch (IndexOutOfRangeException ioex)
            {
                Debug.Log("DataManager->Load Data->IndexOutofRange: " + ioex.Message);
            }
            catch (Exception ex)
            {
                Debug.Log("DataManager->Load Data->Exception: " + ex.Message);
            }
        }

        public void SaveScene()
        {
            if (!WebManager.CurrentUser.IsNull())
            {
                StartCoroutine(WebManager.Singleton.SaveScene(WebManager.CurrentUser.userKey, ResourceManager.Singleton.GetSceneString(), PostSaveScene));
            }
            else
            {
                Debug.Log("null user");
            }
        }

        public void PostSaveScene(string str)
        {
            Debug.Log("save: " + str);
        }

        public void LoadSceneList()
        {
            if (!WebManager.CurrentUser.IsNull())
            {
                StartCoroutine(WebManager.Singleton.GetSceneList(WebManager.CurrentUser.userKey, PostLoadSceneList));
            }
            else
            {
                Debug.Log("null user");
            }
        }

        public void GetTest()
        {
            StartCoroutine(WebManager.Singleton.GetObjectInfo(13, InfoType.Product, PostLoadScene));
        }

        void PostLoadScene(string str)
        {
            Debug.Log("post load scene: " + str);
        }

        public void PostLoadSceneList(string str)
        {
            try
            {
                scenes = JsonConvert.DeserializeObject<SceneSet>(str);
                sceneform.Refresh();
                sceneform.Open();
            }
            catch (Exception ex)
            {
                Debug.Log("error: " + ex.Message);
            }
        }

        public void LoadSceneItem(SceneData item)
        {
            ResourceManager.Singleton.Clear();
            foreach (SceneObjectData sd in item.Objects)
            {
                GameObject newObj = null;
                if (sd.isLocal())
                {
                    string[] strs = sd.ID.Split('-');
                    ResType rt = (ResType)Enum.Parse(typeof(ResType), strs[0]);
                    GameObject obj = ResourceManager.Singleton.GetGameObject(rt, strs[1]);
                    if (obj != null)
                    {
                        newObj = Instantiate(obj);
                        ResourceManager.Singleton.AddMarkerlessLocalObject(sd.ID, obj, true);
                    }
                }
                else
                {
                    int _id = Convert.ToInt32(sd.ID);
                    if (furnitureDict.ContainsKey(_id))
                    {
                        LoadDataModle(furnitureDict[_id]);
                        newObj = ResourceManager.Singleton.CurrentObject;
                    }
                }
                if (newObj != null)
                {
                    newObj.transform.position = sd.position;
                    newObj.transform.eulerAngles = sd.rotation;
                    newObj.transform.localScale = sd.scale;
                }
            }
        }

        //void LoadData(string str)
        //{
        //    try
        //    {
        //        string[] strs = str.Split(';');
        //        foreach (string s in strs)
        //        {
        //            if (s != "")
        //            {
        //                string[] subs = s.Split('>');
        //                uint _id = Convert.ToUInt32(subs[0]);
        //                if (!furnitureDict.ContainsKey(_id))
        //                {
        //                    FurnitureData _data = new FurnitureData(_id, subs[1]);
        //                    _data.category_name = subs[2];
        //                    _data.brand_name = subs[3];
        //                    _data.manufacturer_name = subs[4];
        //                    _data.production_site = subs[5];
        //                    _data.production_date = subs[6];
        //                    _data.price = Convert.ToSingle(subs[7]);
        //                    ServerScriptUtil.DownloadImageFile(DataUtility.GetImagePath(_data), DownloadImageCallback);
        //                    loadedCount++;
        //                    furnitureDict[_id] = _data;
        //                }
        //                searchform.SearchResult.Add(furnitureDict[_id]);
        //            }
        //        }
        //        ServerScript.Singleton.StartPHPRequests();
        //    }
        //    catch(IndexOutOfRangeException ioex)
        //    {
        //        Debug.Log("DataManager->Load Data->IndexOutofRange: " + ioex.Message);
        //    }
        //    catch(Exception ex)
        //    {
        //        Debug.Log("DataManager->Load Data->Exception: " + ex.Message);
        //    }
        //}

        public void DownloadImageCallback(int _id, byte[] content)
        {
            try
            {
                if (furnitureDict.ContainsKey(_id))
                {
                    furnitureDict[_id].Thumbnail = DataUtility.CreateSprit(content);
                }
                loadedCount--;
                if (loadedCount == 0)
                {
                    searchform.Refresh();
                }
            }
            catch (Exception ex)
            {
                Debug.Log("DataManager DownloadImageCallback Exception: " + ex.Message);
            }
        }

        public void Search(string _name)
        {
            StartCoroutine(WebManager.Singleton.SearchFurniture(_name, "", LoadData));
        }

        public void LoadDataModle(FurnitureData _data)
        {
            searchform.Close();
            loadedModelID = _data.id;
            if (loadingImage != null)
                loadingImage.Show();
            if (_data.Model == null)
            {
                StartCoroutine(WebManager.Singleton.DownloadAssetBundle(_data.id, Tags.basefileUrl + _data.object_path, DownloadModelCallback));
            }
            else
            {
                GameObject obj = Instantiate(_data.Model);
                ResourceManager.Singleton.AddMarkerlessRemoteObject(_data.id, obj, true);
                if (loadingImage != null)
                    loadingImage.Hide();
            }
        }

        public void DownloadModelCallback(int _id, UnityEngine.Object[] content)
        {
            try
            {
                loadedCount = 0;
                //tempFiles.Clear();
                if (furnitureDict.ContainsKey(_id) && content.Length > 0)
                {
                    furnitureDict[_id].Model = (GameObject)content[0];
                    GameObject obj = Instantiate(furnitureDict[_id].Model);
                    obj.transform.position = Vector3.zero;
                    ResourceManager.Singleton.AddMarkerlessRemoteObject(_id, obj, true);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("DownloadModelCallback Exception: " + ex.Message);
            }
            if (loadingImage != null)
                loadingImage.Hide();
        }

        //public void DownloadModelCallback(int _id, string content)
        //{
        //    try
        //    {
        //        loadedCount = 0;
        //        tempFiles.Clear();
        //        if (furnitureDict.ContainsKey(_id))
        //        {
        //            string[] files = content.Split(';');
        //            //Debug.Log("data path: " + Application.dataPath + "; path2: " + Application.persistentDataPath + "; path3: " + Application.streamingAssetsPath + "; path4: " + Application.temporaryCachePath);
        //            string mpath = DataUtility.GetLocalModelPath(furnitureDict[loadedModelID]);
        //            if (!Directory.Exists(mpath))
        //            {
        //                Directory.CreateDirectory(mpath);
        //            }
        //            foreach(string str in files)
        //            {
        //                if (str != "")
        //                {
        //                    string urlPath = DataUtility.GetModelPath(furnitureDict[loadedModelID]) + str;
        //                    tempFiles[urlPath] = false;
        //                    ServerScriptUtil.DownloadBinaryFile(urlPath, DownloadFileCallback);
        //                    loadedCount++;
        //                }
        //            }
        //            ServerScript.Singleton.StartPHPRequests();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Log("DownloadModelCallback Exception: " + ex.Message);
        //    }
        //}

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
            catch (Exception ex)
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
            furnitureDict[loadedModelID].Model = obj;
            if (loadingImage != null)
                loadingImage.Hide();
            // cschen ResourceManager.Singleton.AddMarkerlessObject (obj);
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
}