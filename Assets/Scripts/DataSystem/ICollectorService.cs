﻿using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV.Service
{
    public interface ICollectorService
    {
        IEnumerator TopCategories(Action<string> callback);

        IEnumerator GetProductsInCategory(long categoryId, Action<string> callback);
		IEnumerator GetProductsInSubCategory(string subCategoryName, Action<string> callback);
		//test
		IEnumerator GetSubCategoryDetail(long subCategoryId, string subCategoryName, SubCategoryDownloadCallback callback);
		IEnumerator GetSubCategoryTexture(long subCategoryId, string subCategoryName, TextureDownloadCallback callback);
		IEnumerator GetSubCategoryImage(long subCategoryId, string subCategoryName, ImageDownloadCallback callback);



        IEnumerator GetProductDetail(string sku, ProductDownloadCallback callback);

        IEnumerator GetProductImage(string sku, ImageDownloadCallback callback);

        IEnumerator GetProductTexture(string sku, TextureDownloadCallback callback);

		//test
		IEnumerator GetSubCategoryProductDetail (long subCategoryId, string sku, ProductDownloadCallback callback);
		IEnumerator GetSubCategoryProductTexture(long subCategoryId, string sku, TextureDownloadCallback callback);
		IEnumerator GetSubCategoryProductImage(long subCategoryId, string sku, ImageDownloadCallback callback);

		IEnumerator GetCategoryImage(long categoryId, ImageDownloadCallback callback);

        IEnumerator GetCategoryTexture(long categoryId, TextureDownloadCallback callback);

        IEnumerator DownloadAssetBundle(string sku, AssetBundleDownloadCallback callback);

    }
}
