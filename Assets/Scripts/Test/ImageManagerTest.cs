using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMAV.UI;

namespace IMAV
{
    public class ImageManagerTest : MonoBehaviour {
        public void DeleteFolderTest()
        {
            MediaCenter.Singleton.DeleteImageFolder("20170726");
        }

        public void GetVideoFileName()
        {
            string str1 = MediaCenter.Singleton.GetVideoFileName("20170726/WhizHome 20170726_203921_video.png");
            string str2 = MediaCenter.Singleton.GetVideoFileName("20170726/WhizHome 20170726_203921.png");
            string str3 = MediaCenter.Singleton.GetVideoFileName("20170726/WhizHome 20170.png");
            if (str1 == string.Empty)
                str1 = "empty";
            if (str2 == string.Empty)
                str2 = "empty";
            if (str3 == string.Empty)
                str3 = "empty";
            Debug.Log("str1: " + str1 + " ; str2: " + str2 + " ; str3: " + str3);

            MediaCenter.Singleton.SaveVideo("20170726/WhizHome 20170726_203921_video.png");
            MediaCenter.Singleton.SaveVideo("20170726/WhizHome 20170726_203921.png");
            MediaCenter.Singleton.SaveVideo("20170726/WhizHome 20170.png");
        }

        public void FileIndexTest()
        {
            List<string> files = new List<string>();
            int index1 = files.IndexOf("a");
            files.Add("a");
            int index2 = files.IndexOf("a");
            int index3 = files.IndexOf(string.Empty);
            files.Add("b");
            int index4 = files.IndexOf("a");
            int index5 = files.IndexOf("b");
            Debug.Log("int: " + index1 + " ; " + index2 + " ; " + index3 + " ; " + index4 + " ; " + index5);
        }

        public void GetImageTest()
        {
            UIImage im = MediaCenter.Singleton.imageGallery.GetImage("20170727/WhizHome 20170727_114540.png");
            if (im == null)
                Debug.Log("empty image");
            else
                Debug.Log("imagetag: " + im.ImageTag + " ; ");
        }

        public void RenameImageTest()
        {
            MediaCenter.Singleton.RenameFile("20170727/WhizHome 20170727_175334.png", "HelloWorld3");
        }

        public void ChangeTest()
        {
            //string str =ImageManager.Singleton.ChangeJsonContent(@"D:\WorkSpace\AR\Android\WhizHome 20170728_105941_video.json", "hello.mp4");
            //Debug.Log("str: " + str);
        }
    }
}
