using System;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV
{
    [Serializable]
    public class ErrorMessage
    {
        public IList<string> email { get; set; }
        public IList<string> first_name { get; set; }
        public IList<string> password { get; set; }
    }


    public class MessageData
    {
        public int status { get; set; }
        public string key { get; set; }
        public ErrorMessage errors { get; set; }


        public void ReadString(string str)
        {
        }
    }

    public class UserData
    {
        public string userName;
        public string userKey;
        public string userEmail;

        public bool IsNull()
        {
            return userKey == null || userKey == "";
        }
    }
}
