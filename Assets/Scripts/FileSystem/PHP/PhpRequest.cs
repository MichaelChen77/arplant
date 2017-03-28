using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Xml;


public delegate void OnDownloaded(string path, string content);
public delegate void OnDownloadedBinary(string path, byte[] content);
public delegate void OnUploaded(string path);
public delegate void OnDeleted(string path);

public enum PhpAttributeType
{
    op,
    url,
    path,
    content
}

public class PhpAttribute
{
    public PhpAttributeType name;
    public string value;

    public PhpAttribute(PhpAttributeType name, string value)
    {
        this.name = name;
        this.value = value;
    }
    public override string ToString()
    {
        return name.ToString() + "=" + value;
    }
}

public abstract class PhpRequest
{
    protected List<PhpAttribute> attributes;
    protected Delegate callback;
    protected VirtualFile.FileType returnFileType = VirtualFile.FileType.Binary;
    public VirtualFile.FileType ReturnFileType
    {
        get { return returnFileType; }
    }

    protected PhpRequest(Delegate callback, VirtualFile.FileType _type)
    {
        this.attributes = new List<PhpAttribute>();
        this.returnFileType = _type;
        this.callback = callback;
    }

    public void AddAttribute(PhpAttribute newAttribute)
    {
        this.attributes.Add(newAttribute);
    }

    public virtual string GetAttribute(PhpAttributeType attributeType)
    {
        PhpAttribute attri = attributes.SingleOrDefault(a => a.name == attributeType);
        string s = null;
        if (attri != null)
        {
            s = attri.value;
        }
        return s;
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < attributes.Count; i++)
        {
            s += attributes[i].ToString().Replace(" ", "%20");
            if (i != attributes.Count - 1)
            {
                s += "&";
            }
        }
        return s;
    }
}

public class PhpDownloadRequest : PhpRequest
{
    public void InvokeOnDownloaded(string path, string content)
    {
        OnDownloaded onDownloaded = this.callback as OnDownloaded;
        onDownloaded(path, content);
    }
    public void InvokeOnDownloadedBinary(string path, byte[] content)
    {
        OnDownloadedBinary onDownloaded = this.callback as OnDownloadedBinary;
        onDownloaded(path, content);
    }

    public PhpDownloadRequest(string op, string path, OnDownloadedBinary callback, VirtualFile.FileType _type)
        : base(callback, _type)
    {
        attributes.Add(new PhpAttribute(PhpAttributeType.op, op));
        if (path != null)
        {
            attributes.Add(new PhpAttribute(PhpAttributeType.path, path));
        }
    }

    public PhpDownloadRequest(string op, string path, OnDownloaded callback, VirtualFile.FileType _type)
        : base(callback, _type)
    {
        attributes.Add(new PhpAttribute(PhpAttributeType.op, op));
        if (path != null)
        {
            attributes.Add(new PhpAttribute(PhpAttributeType.path, path));
        }
    }

    public override string ToString()
    {
        return ServerScript.Singleton.baseUrl + "?" + base.ToString();
    }
}

public class PhpDeleteRequest : PhpRequest
{
    public void InvokeOnDeleted(string path)
    {
        OnDeleted onDeleted = this.callback as OnDeleted;
        onDeleted(path);
    }

    public PhpDeleteRequest(string op, string path, OnDeleted callback, VirtualFile.FileType _type)
        : base(callback, _type)
    {
        attributes.Add(new PhpAttribute(PhpAttributeType.op, op));
        attributes.Add(new PhpAttribute(PhpAttributeType.path, path));
    }

    public override string ToString()
    {
        return ServerScript.Singleton.baseUrl + "?" + base.ToString();
    }
}

public class PhpUploadRequest : PhpRequest
{
    private string path;
    private byte[] content;

    public static byte[] ConvertStringToByte(string my_string)
    {
        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        Byte[] bytes = encoding.GetBytes(my_string);
        return bytes;
    }

    public WWWForm Form
    {
        get { return ServerScriptUtil.CreateUploadForm(path, content); }
    }

    public void InvokeOnUploaded(string path)
    {
        OnUploaded onUploaded = this.callback as OnUploaded;
        onUploaded(path);
    }

    public PhpUploadRequest(string path, byte[] content, OnUploaded callback, VirtualFile.FileType _type)
        : base(callback, _type)
    {
        this.path = path;
        this.content = content;
    }

    public PhpUploadRequest(string path, string content, OnUploaded callback, VirtualFile.FileType _type)
        : base(callback, _type)
    {
        this.path = path;
        this.content = ConvertStringToByte(content);
    }

    public override string GetAttribute(PhpAttributeType attributeType)
    {
        string retVal;
        if (attributeType == PhpAttributeType.path)
        {
            retVal = path;
        }
        else
        {
            throw new ArgumentException("PhpUploadRequest does not have attributes other than 'path'.");
        }
        return retVal;
    }

    public override string ToString()
    {
        return ServerScript.Singleton.postUrl;
    }
}