using UnityEngine;
using System.Collections;

public class Tags{
    public const string baseUrl = "http://155.69.151.176/imav/";
    //public const string SearchUrl = "http://155.69.150.59/IMAV/Search.php";
    public const string basefileUrl = "http://155.69.151.176/imav/admin/file/index?path=";
    public const string GetInfoUrl = baseUrl + "product/m/get-product-info?id=2";
    public const string UploadSceneUrl = baseUrl + "design/m/create";
    public const string GetScenesUrl = baseUrl + "design/m/get-all-designs";

    public const string SearchUrl = baseUrl + "product/m/search";
    public const string UserRegUrl = baseUrl + "site/signup";
    public const string UserLoginUrl = baseUrl + "site/login";
    public const float ThumbnailSize = 80;
    public static readonly string[] FurnitureCategory = { "All", "Car", "Electronics", "Furniture" };

}
