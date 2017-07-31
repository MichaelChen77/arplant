using UnityEngine;
using System.Collections;

public class MagentoServiceSample : MonoBehaviour {

	// Use this for initialization
	void Start () {

        //StartCoroutine(MagentoService.Instance.TopCategories(categories =>
        //{
        //    foreach (Category c in categories)
        //    {
        //        Debug.Log("Downloaded category: " + c.name + " (id: " + c.id + ")");
        //    }

        //    DownloadProductsInCategory(categories[0].id);
        //    DownloadCategoryImage(categories[0].id);
        //    DownloadCategoryTexture(categories[0].id);
        //}));
        //DownloadProductsInCategory(3);

    }

    //private void DownloadProductsInCategory(long categoryId)
    //{
    //    StartCoroutine(MagentoService.Instance.GetProductsInCategory(categoryId, (catId, products) => {
    //        for(int i=0; i < 6 && i < products.Count; ++i)
    //        {
    //            CategoryProduct categoryProduct = products[i];
    //            Debug.Log("Download products in category " + categoryId + ": " + categoryProduct.sku);
    //            DownloadProductDetail(categoryProduct.sku);
    //            if (i == 5)
    //            {
    //                DownloadProductImage(categoryProduct.sku);
    //                DownloadProductTexture(categoryProduct.sku);
    //            }
    //        }
            
    //    }));
    //}

    private void DownloadProductDetail(string sku)
    {
        StartCoroutine(MagentoService.Instance.GetProductDetail(sku, (product) => {
            Debug.Log("Name for sku " + product.sku + ": " + product.name);
            Debug.Log("Price for sku " + product.sku + ": " + product.price);
            Debug.Log("Weight for sku " + product.sku + ": " + product.weight);
        }));
    }

    private void DownloadProductImage(string sku)
    {
        StartCoroutine(MagentoService.Instance.GetProductImage(sku, (bytes) => {
            Debug.Log("Bytes for product " + sku + " is " + bytes.Length);
        }));
    }

    private void DownloadProductTexture(string sku)
    {
        StartCoroutine(MagentoService.Instance.GetProductTexture(sku, (texture) =>
        {
            Debug.Log("texture for product " + sku + " is " + (texture != null));
        }));
    }

    private void DownloadCategoryImage(long categoryId)
    {
        StartCoroutine(MagentoService.Instance.GetCategoryImage(categoryId, (bytes) => {
            Debug.Log("Bytes for category " + categoryId + " is " + bytes.Length);
        }));
    }

    private void DownloadCategoryTexture(long categoryId)
    {
        StartCoroutine(MagentoService.Instance.GetCategoryTexture(categoryId, (texture) =>
        {
            Debug.Log("texture for category " + categoryId + " is " + (texture != null));
        }));
    }

	// Update is called once per frame
	void Update () {
	
	}
}
