using UnityEngine;
using System.Collections;

public class Tags{
    public const string baseUrl = "http://155.69.151.176/imav/";
    public const string SearchUrl = "http://155.69.150.59/IMAV/Search.php";
    public const string GetInfoUrl = baseUrl + "product/m/get-product-info?id=2";
    public const string UploadSceneUrl = baseUrl + "design/m/create?auth_key=&raw_design=";
    //public const string SearchUrl = baseUrl + "product/m/search";
    public const string UserRegUrl = baseUrl + "site/signup";
    public const string UserLoginUrl = baseUrl + "site/login";
    public const float ThumbnailSize = 80;
    public static readonly string[] FurnitureCategory = { "All", "Car", "Electronics", "Furniture" };

}
