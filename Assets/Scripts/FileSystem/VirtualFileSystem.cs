using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class VirtualFile
{
    private bool isDirty;
    private object content;
    public enum FileType
    {
        Invalid,
        Binary,
        String,
    }
    private FileType type = FileType.Invalid;
    public FileType VirtualFileType
    {
        get
        {
            return type;
        }
    }
    public bool IsDirty
    {
        get { return isDirty; }
        set { isDirty = value; }
    }

    public object Content
    {
        get { return content; }
        set { content = value; }
    }

    public VirtualFile(string content)
    {
        type = FileType.String;
        this.content = content;
        this.isDirty = true;
    }

    public VirtualFile(byte[] content)
    {
        type = FileType.Binary;
        this.content = content;
        this.IsDirty = true;
    }
}

public class VirtualFileSystem
{
    private Dictionary<string, VirtualFile> files = new Dictionary<string, VirtualFile>();
    private Dictionary<string, VirtualFile> filesDeleted = new Dictionary<string, VirtualFile>();

    public string[] DirtyFiles
    {
        get
        {
            return files.Where(pair => pair.Value.IsDirty).Select(pair => pair.Key).ToArray();
        }
    }

    public string[] DeletedFiles
    {
        get
        {
            return filesDeleted.Keys.ToArray();
        }
    }

    public VirtualFile this[string path]
    {
        get
        {
            return files[path];
        }
    }

    public bool RemovedDeleted(string path)
    {
        lock (this)
        {
            return filesDeleted.Remove(path);
        }
    } 

    public void SetFileDirty(string path, bool dirty)
    {
        files[path].IsDirty = dirty;
    }

    public void WriteByte(string path, byte[] content){
        lock (this)
        {
            if (files.ContainsKey(path))
            {
                Debug.Log("VFS binary file written: " + path);
                //Tags.isFileExisted = true;
                return;
            }
            else
            {
                VirtualFile newFile = null;
                try
                {
                    newFile = new VirtualFile(content);
                    files.Add(path, newFile);
                    Debug.Log("normal");
                }
                catch (ArgumentException)
                {
                    //if (!files[path].Equals(content))
                    //{
                    //    files.Remove(path);
                    //    files.Add(path, newFile);
                    //    newFile.IsDirty = true;
                    //}
                    Debug.Log("exception");
                }
            }
        }
    }
    public void Write(string path, string content)
    {
        lock (this)
        {
            if (files.ContainsKey(path))
            {
                Debug.Log("VFS string file written: " + path);
                //Tags.isFileExisted = true;
                return;
            }
            else
            {
                VirtualFile newFile = null;
                try
                {
                    newFile = new VirtualFile(content);
                    files.Add(path, newFile);
                }
                catch (ArgumentException)
                {
                    //if (!files[path].Equals(content))
                    //{
                    //    files.Remove(path);
                    //    files.Add(path, newFile);
                    //    newFile.IsDirty = true;
                    //}
                }
            }
        }
    }

    public byte[] ReadBinaryContent(string path)
    {
        try
        {
            if (files[path].VirtualFileType == VirtualFile.FileType.Binary)
            {
                return (byte[])files[path].Content;
            }
            else
            {
                throw new Exception("Invalid type");
            }
        }
        catch (KeyNotFoundException)
        {
            Debug.Log("File path not found: " + path);
            return null;
        }
    }

    public string ReadStringContent(string path)
    {
        try {
            if (files[path].VirtualFileType == VirtualFile.FileType.String)
            {
                return (string)files[path].Content;
            }
            else
            {
                throw new Exception("Invalid type");
            }
        }
        catch(KeyNotFoundException)
        {
            Debug.Log("File path not found: " + path);
            return null;
        }
    }
    public VirtualFile.FileType GetFileType(string path)
    {
        try {
            return files[path].VirtualFileType;
        }
        catch(KeyNotFoundException)
        {
            Debug.Log("File path not found: " + path);
            return VirtualFile.FileType.Invalid;
        }
    }

    public bool DeleteFile(string path)
    {
        lock (this)
        {
            VirtualFile deletedFile = files[path];
            try
            {
                filesDeleted.Add(path, deletedFile);
            }
            catch (ArgumentException)
            {
                filesDeleted.Remove(path);
                filesDeleted.Add(path, deletedFile);
            }
            return files.Remove(path);
        }
    }

    public void DeleteFolder(string path)
    {
        lock (this)
        {
            List<string> pathToDelete = files.Keys.Where(
                p => p.StartsWith(path, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (string p in pathToDelete)
            {
                DeleteFile(p);
            }
        }
    }

    // returns the folder name of a file path. e.g. "AAA" for ".\AAA\BBB.xml"
    private string GetFolderName(string basePath, string filePath)
    {
        int headingCount = basePath.Length;
        if (basePath[basePath.Length - 1] != '/')
        {
            headingCount++;
        }
        string folderName = null;
        string partialFilePath = filePath.Remove(0, headingCount);
        if (partialFilePath.Contains("/"))
        {
            folderName = partialFilePath.Split(new char[]{'/'})[0];
        }
        return folderName;
    }

    public string[] GetSubfolderNames(string basePath)
    {
        IEnumerable<string> targetFiles = files.Keys.Where(
            p => p.StartsWith(basePath, StringComparison.OrdinalIgnoreCase));
        List<string> folders = new List<string>();
        foreach (string filePath in targetFiles)
        {
            string folderName = GetFolderName(basePath, filePath);
            if (folderName != null)
            {
                folders.Add(folderName);
            }
        }
        return folders.Distinct().ToArray();
    }

    public string[] GetFiles(string basePath)
    {
        IEnumerable<string> targetFiles = files.Keys.Where(
            p => p.StartsWith(basePath, StringComparison.OrdinalIgnoreCase));
        return targetFiles.ToArray();
    }

    public bool ExistsFolder(string folderPath)
    {
        string existing = files.Keys.FirstOrDefault(
            p => p.StartsWith(folderPath, StringComparison.OrdinalIgnoreCase)
                && p[folderPath.Length-1+1] == '/'
            );
        return existing != default(string);
    }

    public bool ExistsFile(string filePath)
    {
        string existing = files.Keys.FirstOrDefault(p => p.Equals(filePath, StringComparison.OrdinalIgnoreCase));
        return existing != default(string);
    }


}
