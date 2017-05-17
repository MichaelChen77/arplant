using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

#if UNITY_STANDALONE_WIN || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
public class UnityWinFileSystem : IMLFileSystem
{
    #region IMLFileSystem Members
    private VirtualFileSystem vfs;

    public UnityWinFileSystem()
    {
        Initialize();
    }

    public void Initialize()
    {
        vfs = new VirtualFileSystem();
        //string[] filenames = Directory.GetFiles(PathManager.GetUserPath(), "*.xml", SearchOption.AllDirectories);

        //foreach (string path in filenames)
        //{
        //    string content = File.ReadAllText(path);
        //    //Debug.Log("Cache file: " + path);
        //    vfs.Write(path, content);
        //}
    }

    public void CreateFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    public void DeleteFolder(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, true);
        }
    }

    public void WriteFile(string path, string content)
    {
        File.WriteAllText(path, content);
    }

    public void WriteFile(string path, byte[] content)
    {
        File.WriteAllBytes(path, content);
    }

    public string ReadFile(string path)
    {
        return File.ReadAllText(path, Encoding.UTF8);
    }

    public byte[] ReadBinaryFile(string path)
    {
        return File.ReadAllBytes(path);
    }

    public bool DeleteFile(string filePath)
    {
        bool deleted = false;
        try
        {
            File.Delete(filePath);
            deleted = true;
        }
        catch (Exception)
        {
            Debug.LogWarning("File deletion failed: " + filePath);
        }
        return deleted;
    }

    public string[] GetSubfolderNames(string basePath)
    {
        DirectoryInfo baseInfo = new DirectoryInfo(basePath);
        DirectoryInfo[] dirInfo = baseInfo.GetDirectories();

        return dirInfo.Select(info => info.Name).ToArray();

    }

    public string[] GetSubfolderNamesWithFilter(string basePath, string avoidName)
    {
        string[] folders = GetSubfolderNames(basePath);
        return folders.SkipWhile(name => name.Equals(avoidName, StringComparison.OrdinalIgnoreCase)).ToArray();
    }

    public string[] GetFiles(string basePath)
    {
        return Directory.GetFiles(basePath);
    }

    public bool ExistsFolder(string folderPath)
    {
        return Directory.Exists(folderPath);
    }

    public bool ExistsFile(string filePath)
    {
        return File.Exists(filePath);
    }

    #endregion
}
#endif
