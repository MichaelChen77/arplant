using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UnityWebFileSystem : IMLFileSystem
{
    #region IMLFileSystem Members
    private VirtualFileSystem vfs;
    public UnityWebFileSystem()
    {
        Initialize();
    }

    public void Initialize()
    {
        UnityEngine.Debug.Log("Initialising web file system");
        // request all agents and other configuration files from server..
        vfs = new VirtualFileSystem();
        //GameObject serverScript = new GameObject("ServerScript");
        //serverScript.AddComponent<ServerScript>();
            
        //PhpDownloadRequest getAgentsRequest = new PhpDownloadRequest(
        //    "GetAllList", null, GetUserListCallBack);
        //ServerScript.Singleton.AppendRequest(getAgentsRequest);
    }

    public void CreateFolder(string folderPath)
    {
        // do nothing, since no folder concept in virtual file system
    }

    public void DeleteFolder(string folderPath)
    {
        vfs.DeleteFolder(folderPath);
    }

    public void WriteFile(string path, string content)
    {
        vfs.Write(path, content);
    }

    public VirtualFile.FileType GetFileType(string path)
    {
        return vfs.GetFileType(path);
    }

    public string ReadFile(string path)
    {
        return vfs.ReadStringContent(path);
    }

    public byte[] ReadBinaryFile(string path)
    {
        return vfs.ReadBinaryContent(path);
    }

    public bool DeleteFile(string filePath)
    {
        return vfs.DeleteFile(filePath);
    }

    public string[] GetSubfolderNames(string basePath)
    {
        return vfs.GetSubfolderNames(basePath);
    }

    public string[] GetSubfolderNamesWithFilter(string basePath, string avoidName)
    {
        string[] folders = GetSubfolderNames(basePath);
        return folders.SkipWhile(name => name.Equals(avoidName, StringComparison.OrdinalIgnoreCase)).ToArray();
    }

    public string[] GetFiles(string basePath)
    {
        return vfs.GetFiles(basePath);
    }

    public bool ExistsFolder(string folderPath)
    {
        return vfs.ExistsFolder(folderPath);
    }

    public bool ExistsFile(string filePath)
    {
        return vfs.ExistsFile(filePath);
    }

    #endregion

    private static bool isInitialized = false;
    private static int totalFilesToDownload = 0;
    private static int totalFilesDownloaded = 0;

    public static int GetNumFilesToDownload()
    {
        return totalFilesToDownload;
    }
    public static int GetNumFilesDownloaded()
    {
        return totalFilesDownloaded;
    }

    //public void GetUserListCallBack(string path, string content)
    //{
    //    try
    //    {
    //        AppManager.Singleton.RefreshUsers(content);
    //    }
    //    catch (Exception)
    //    {
    //        ServerScript.ServerError = true;
    //    }
    //}

    public void DownloadByteFilesCallback(string path, byte[] content)
    {
        vfs.WriteByte(path, content);
        if (!isInitialized)
        {
            if (++totalFilesDownloaded == totalFilesToDownload)
            {
                isInitialized = true;

                MLFileSystem.RefreshFileSystem();
                //WorldSaveLoad_SideBar.RefreshFiles();
                Debug.Log("MLFileSystem: refreshed AgentDesignUI_Load");
            }
            vfs.SetFileDirty(path, false);
        }
    }

    public void DownloadFilesCallback(string path, string content)
    {
        vfs.Write(path, content);
        if (!isInitialized)
        {
            if (++totalFilesDownloaded == totalFilesToDownload)
            {
                isInitialized = true;

                MLFileSystem.RefreshFileSystem();
                //WorldSaveLoad_SideBar.RefreshFiles();
                Debug.Log("MLFileSystem: refreshed AgentDesignUI_Load");
            }
            vfs.SetFileDirty(path, false);
        }
    }

    public void WriteFile(string path, byte[] content)
    {
        Debug.Log("Write file");
        vfs.WriteByte(path, content);
        if (!isInitialized)
        {
            if (++totalFilesDownloaded == totalFilesToDownload)
            {
                isInitialized = true;
                MLFileSystem.RefreshFileSystem();
                //WorldSaveLoad_SideBar.RefreshFiles();
                Debug.Log("MLFileSystem: refreshed AgentDesignUI_Load");
            }
            vfs.SetFileDirty(path, true);
        }
    }

    int SyncFileCount = 0;
    int uploadedFileCount = 0;
    public void Sync()
    {
        lock (typeof(MLFileSystem))
        {
            // Update dirty files
            foreach (string dirtyFile in vfs.DirtyFiles)
            {
                if (GetFileType(dirtyFile) == VirtualFile.FileType.String)
                {
                    string content = vfs.ReadStringContent(dirtyFile);
                    PhpUploadRequest ur = new PhpUploadRequest(dirtyFile, content, OnSyncUploaded, VirtualFile.FileType.String);
                    ServerScript.Singleton.AppendRequest(ur);
                    SyncFileCount++;
                }
                else if (GetFileType(dirtyFile) == VirtualFile.FileType.Binary)
                {
                    byte[] content = vfs.ReadBinaryContent(dirtyFile);
                    PhpUploadRequest ur = new PhpUploadRequest(dirtyFile, content, OnSyncUploaded, VirtualFile.FileType.Binary);
                    ServerScript.Singleton.AppendRequest(ur);
                    SyncFileCount++;
                }
            }

            Debug.Log("delete sync0");
            // Execute file deletion
            foreach (string path in vfs.DeletedFiles)
            {
                Debug.Log("delete sync");
                PhpDeleteRequest dr = new PhpDeleteRequest("DeleteFile", path, OnSyncDeleted, VirtualFile.FileType.String);
                ServerScript.Singleton.AppendRequest(dr);
            }
        }
    }

    public void DeleteSync(string path, OnDeleted callback)
    {
        lock (typeof(MLFileSystem))
        {
            PhpDeleteRequest dr = new PhpDeleteRequest("DeleteFile", path, callback, VirtualFile.FileType.String);
            ServerScript.Singleton.AppendRequest(dr);
        }
    }

    


    public void OnSyncUploaded(string path)
    {
        lock (typeof(MLFileSystem))
        {
            vfs.SetFileDirty(path, false);
            uploadedFileCount++;
            if (uploadedFileCount == SyncFileCount)
            {
                uploadedFileCount = 0;
                SyncFileCount = 0;
            }
            Debug.Log("Updated dirty file: " + path);
        }
    }

    public void OnSyncDeleted(string path)
    {
        lock (typeof(MLFileSystem))
        {
            vfs.RemovedDeleted(path);
            Debug.Log("Deleted file: " + path);
        }
    }

}
