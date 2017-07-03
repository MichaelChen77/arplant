using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UITestControls : MonoBehaviour {

    public GameObject goMenu;
    public GameObject goHelp;
    public GameObject goCatalogue;
    public GameObject goNotification;
    public GameObject goSelectedObject;
    public GameObject goScreenshot;
    public GameObject goARVRScenesList;
    public GameObject goGallery;

    public RectTransform rtMenu;
    public RectTransform rtShowHideMenuSprite;

    private bool isMenuShown = false;

    private void Awake()
    {
        ToggleAllOff();
        goMenu.SetActive(true);
    }

    private void ToggleAllOff()
    {
        goMenu.SetActive(false);
        goHelp.SetActive(false);
        goCatalogue.SetActive(false);
        goNotification.SetActive(false);
        goSelectedObject.SetActive(false);
        goScreenshot.SetActive(false);
        goARVRScenesList.SetActive(false);
        goGallery.SetActive(false);
    }
    
    public void ToggleScreen_Notification(bool b)
    {
        goNotification.SetActive(b);
    }

    //============================================= MENU
    public void Button_ShowHideMenu()
    {
        if (!isMenuShown)
        {
            //Show
            //rtMenu.anchoredPosition = Vector2.zero;
            LeanTween.move(rtMenu, Vector2.zero, 0.25f).setEaseOutQuad();
            //rtShowHideMenuSprite.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            //Hide
            //rtMenu.anchoredPosition = new Vector2(rtMenu.rect.width, 0f);
            LeanTween.move(rtMenu, new Vector2(rtMenu.rect.width, 0f), 0.25f).setEaseInQuad();
            //rtShowHideMenuSprite.localScale = Vector3.one;
        }

        isMenuShown = !isMenuShown;
    }

    public void Button_ReturnToMenuList()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void Button_Screenshot()
    {
        ToggleAllOff();
        goScreenshot.SetActive(true);
    }

    public void Button_ARVRScenesList()
    {
        ToggleAllOff();
        goARVRScenesList.SetActive(true);
    }

    public void Button_Gallery()
    {
        ToggleAllOff();
        goGallery.SetActive(true);
    }
    //============================================= HELP 
    public void ReturnToMenu()
    {
        ToggleAllOff();
        goMenu.SetActive(true);
    }

    public void Button_Help()
    {
        ToggleAllOff();
        goHelp.SetActive(true);
    }

    //============================================= CATALOGUE
    
    public void Button_Add()
    {
        ToggleAllOff();
        goCatalogue.SetActive(true);
    }

    public void DisableCatalogue()
    {
        ToggleAllOff();
        goMenu.SetActive(true);
    }

    //============================================= SELECT GIZMO 

    public void ToggleObjectUI(bool b)
    {
        goSelectedObject.SetActive(b);
    }

}
