using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Xml;

public static class ServerScriptUtil
{

    public static void PrintXML(string path, string xml)
    {
        UnityEngine.Debug.Log("path = " + path);
        UnityEngine.Debug.Log("xml = " + xml);
    }

    public static string[] GetFiles(string xml)
    {
        List<string> fileList = new List<string>();
        XmlDocument xmlDoc = new XmlDocument();
        UnityEngine.Debug.Log(xml);

        xmlDoc.LoadXml(xml);
        foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
        {
            RecursiveGetFiles(fileList, node, "./");
            //RecursiveGetFiles(fileList, node, ".\\" + xmlDoc.DocumentElement.Name + "\\");
        }
        return fileList.ToArray();
    }


    public static void DownloadBinaryFiles(string[] files, OnDownloadedBinary binaryFileCallback)
    {
        foreach (string path in files)
        {
            DownloadBinaryFile(path, binaryFileCallback);
        }
    }

    public static void DownloadBinaryFile(string path, OnDownloadedBinary binaryFileCallback)
    {
        PhpDownloadRequest r = new PhpDownloadRequest("GetFile", path, binaryFileCallback, VirtualFile.FileType.Binary);
        ServerScript.Singleton.AppendRequest(r);
    }

    public static void DownloadImageFile(string path, OnDownloadedBinary binaryFileCallback)
    {
        PhpDownloadRequest r = new PhpDownloadRequest("GetImageFile", path, binaryFileCallback, VirtualFile.FileType.Binary);
        ServerScript.Singleton.AppendRequest(r);
    }

    public static void DownloadTextFile(string path, OnDownloaded textFileCallback)
    {
        PhpDownloadRequest r = new PhpDownloadRequest("GetObjFile", path, textFileCallback, VirtualFile.FileType.String);
        ServerScript.Singleton.AppendRequest(r);
    }

    private static void RecursiveGetFiles(List<string> fileList, XmlNode node, string path)
    {
        if (node.Name.Equals("Directory"))
        {
            string directoryName = node.Attributes.GetNamedItem("Name").InnerText;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                //explore deeper
                RecursiveGetFiles(fileList, childNode, path + directoryName + "/");
            }
        }
        else if (node.Name.Equals("File"))
        {
            //Debug.Log(path + node.Attributes.GetNamedItem("Name").InnerText);
            // store the full path of the file
            fileList.Add(path + node.Attributes.GetNamedItem("Name").InnerText);
        }
    }

    public static WWWForm CreateUploadForm(string path, byte[] content)
    {
        WWWForm form = new WWWForm();
        string directory = path.Substring(0, path.LastIndexOf("/") + 1);
        byte[] filebyte = content;
        form.AddBinaryData("file", filebyte, path);
        Debug.Log("directory: " + directory);
        form.AddField("directory", directory);
        return form;
    }

    //public static WWWForm CreateUploadForm(string path, string content)
    //{
    //    WWWForm form = new WWWForm();
    //    string directory = path.Substring(0, path.LastIndexOf(@"\") + 1);
    //    byte[] filebyte = ConvertStringToByte(content);
    //    form.AddBinaryData("file", filebyte, path);
    //    form.AddField("directory", directory);
    //    return form;
    //}

}
