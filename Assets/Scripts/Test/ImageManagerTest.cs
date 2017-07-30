using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMAV.UI;

namespace IMAV
{
    public class ImageManagerTest : MonoBehaviour {
        public void DeleteFolderTest()
        {
            ImageManager.Singleton.DeleteImageFolder("20170726");
        }

        public void GetVideoFileName()
        {
            string str1 = ImageManager.Singleton.GetVideoFileName("20170726/WhizHome 20170726_203921_video.jpg");
            string str2 = ImageManager.Singleton.GetVideoFileName("20170726/WhizHome 20170726_203921.jpg");
            string str3 = ImageManager.Singleton.GetVideoFileName("20170726/WhizHome 20170.jpg");
            if (str1 == string.Empty)
                str1 = "empty";
            if (str2 == string.Empty)
                str2 = "empty";
            if (str3 == string.Empty)
                str3 = "empty";
            Debug.Log("str1: " + str1 + " ; str2: " + str2 + " ; str3: " + str3);

            ImageManager.Singleton.SaveVideo("20170726/WhizHome 20170726_203921_video.jpg");
            ImageManager.Singleton.SaveVideo("20170726/WhizHome 20170726_203921.jpg");
            ImageManager.Singleton.SaveVideo("20170726/WhizHome 20170.jpg");
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
            UIImage im = ImageManager.Singleton.imageGallery.GetImage("20170727/WhizHome 20170727_114540.jpg");
            if (im == null)
                Debug.Log("empty image");
            else
                Debug.Log("imagetag: " + im.ImageTag + " ; ");
        }

        public void RenameImageTest()
        {
            ImageManager.Singleton.RenameFile("20170727/WhizHome 20170727_175334.jpg", "HelloWorld3");
        }

        public void ChangeTest()
        {
            //string str =ImageManager.Singleton.ChangeJsonContent(@"D:\WorkSpace\AR\Android\WhizHome 20170728_105941_video.json", "hello.mp4");
            //Debug.Log("str: " + str);
        }
    }
}
