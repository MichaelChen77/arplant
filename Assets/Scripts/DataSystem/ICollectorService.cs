using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV.Service
{
    public interface ICollectorService
    {
        IEnumerator TopCategories(Action<string> callback);

        IEnumerator GetProductsInCategory(long categoryId, Action<string> callback);

        IEnumerator GetProductDetail(string sku, ProductDownloadCallback callback);

        IEnumerator GetProductImage(string sku, ImageDownloadCallback callback);

        IEnumerator GetProductTexture(string sku, TextureDownloadCallback callback);

        IEnumerator GetCategoryImage(long categoryId, ImageDownloadCallback callback);

        IEnumerator GetCategoryTexture(long categoryId, TextureDownloadCallback callback);

        IEnumerator DownloadAssetBundle(string sku, AssetBundleDownloadCallback callback);
    }
}
