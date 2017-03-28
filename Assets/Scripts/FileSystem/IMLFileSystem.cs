using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IMLFileSystem
{
    //        public static event RefreshHandler RefreshEvent = delegate { };
    //        public delegate void RefreshHandler();
    void Initialize();
    //void GetFileListCallBack(string path, string content);

    //        void RegisterRefresh(RefreshHandler refreshMethod);
    //        void UnRegisterRefresh(RefreshHandler refreshMethod);
    //void DownloadFilesCallback(string path, string content);
    void CreateFolder(string folderPath);
    void DeleteFolder(string folderPath);
    void WriteFile(string path, string content);
    void WriteFile(string path, byte[] content);
    string ReadFile(string path);
    byte[] ReadBinaryFile(string path);
    bool DeleteFile(string filePath);

    string[] GetSubfolderNames(string basePath);
    string[] GetSubfolderNamesWithFilter(string basePath, string avoidName);
    string[] GetFiles(string basePath);
    bool ExistsFolder(string folderPath);
    bool ExistsFile(string filePath);
}
