using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum MyMessageBoxButton
{
    AbortRetryIgnore = 0, OK, OKCancel, RetryCancel, YesNo, YesNoCancel
}

public enum MyMessageBoxIcon
{
    Asterisk = 0, Error, Exclamation, Information, None, Question, Stop, Warning
}

public enum MyDialogResult
{
    None = 0, OK, Cancel, Abort, Retry, Ignore, Yes, No
}

public class MyMessageBox : MonoBehaviour {
    public Text TitleText;
    public Text ContentText;
    public Image IconImage;
    public Text YesButtonText;
    public Text NoButtonText;
    public Text CancelButtonText;
    public Sprite[] IconArray;
    public Toggle ShowToggle;

    MyMessageBoxButton buttonType;
    MyMessageBoxIcon iconType;

    public delegate void DialogHandlerCallback(MyDialogResult result, System.Object refer, bool flag);
    DialogHandlerCallback msgHandler;

    System.Object passRefer;

    private static MyMessageBox mSingleton;
    public static MyMessageBox current
    {
        get
        {
            return mSingleton;
        }
    }
    void Awake()
    {
        mSingleton = this;
    }

    public void Show(string caption, string content, DialogHandlerCallback callback)
    {
        Show(caption, content, MyMessageBoxButton.OK, MyMessageBoxIcon.Information, callback, null, false);
    }

    public void Show(string caption, string content, DialogHandlerCallback callback, System.Object refer)
    {
        Show(caption, content, MyMessageBoxButton.OK, MyMessageBoxIcon.Information, callback, refer, false);
    }

    public void Show(string caption, string content, DialogHandlerCallback callback, System.Object refer, bool flag)
    {
        Show(caption, content, MyMessageBoxButton.OK, MyMessageBoxIcon.Information, callback, refer, flag);
    }

    public void Show(string caption, string content, MyMessageBoxButton button, DialogHandlerCallback callback, System.Object refer = null, bool flag = false)
    {
        Show(caption, content, button, MyMessageBoxIcon.None, callback, flag);
    }

    public void Show(string caption, string content, MyMessageBoxButton button, MyMessageBoxIcon icon, DialogHandlerCallback callback, System.Object refer = null, bool flag = false)
    {
        Init();
        TitleText.text = caption;
        ContentText.text = content;
        buttonType = button;
        iconType = icon;
        SetButton(button);
        SetIcon(icon);
        ShowToggle.gameObject.SetActive(flag);
        passRefer = refer;
        msgHandler = callback;
    }

    void Init()
    {
        gameObject.SetActive(true);
        YesButtonText.transform.parent.gameObject.SetActive(true);
        NoButtonText.transform.parent.gameObject.SetActive(true);
        IconImage.transform.parent.gameObject.SetActive(true);
    }

    void SetButton(MyMessageBoxButton btn)
    {
        switch (btn)
        {
            case MyMessageBoxButton.AbortRetryIgnore: 
                YesButtonText.text = "Abort";
                NoButtonText.text = "Retry";
                CancelButtonText.text = "Ignore";
                break;
            case MyMessageBoxButton.OK:
                YesButtonText.transform.parent.gameObject.SetActive(false);
                NoButtonText.transform.parent.gameObject.SetActive(false);
                CancelButtonText.text = "OK";
                break;
            case MyMessageBoxButton.OKCancel:
                YesButtonText.transform.parent.gameObject.SetActive(false);
                NoButtonText.text = "OK";
                CancelButtonText.text = "Cancel";
                break;
            case MyMessageBoxButton.RetryCancel:
                YesButtonText.transform.parent.gameObject.SetActive(false);
                NoButtonText.text = "Retry";
                CancelButtonText.text = "Cancel";
                break;
            case MyMessageBoxButton.YesNo:
                YesButtonText.transform.parent.gameObject.SetActive(false);
                NoButtonText.text = "Yes";
                CancelButtonText.text = "No";
                break;
            case MyMessageBoxButton.YesNoCancel:
                YesButtonText.text = "Yes";
                NoButtonText.text = "No";
                CancelButtonText.text = "Cancel";
                break;
        }
    }

    void SetIcon(MyMessageBoxIcon icon)
    {
        switch (icon)
        {
            case MyMessageBoxIcon.Asterisk:
                IconImage.sprite = IconArray[0];
                break;
            case MyMessageBoxIcon.Error:
                IconImage.sprite = IconArray[1];
                break;
            case MyMessageBoxIcon.Exclamation:
                IconImage.sprite = IconArray[2];
                break;
            case MyMessageBoxIcon.Information:
                IconImage.sprite = IconArray[3];
                break;
            case MyMessageBoxIcon.None:
                IconImage.gameObject.SetActive(false);
                break;
            case MyMessageBoxIcon.Question:
                IconImage.sprite = IconArray[4];
                break;
            case MyMessageBoxIcon.Stop:
                IconImage.sprite = IconArray[5];
                break;
            case MyMessageBoxIcon.Warning:
                IconImage.sprite = IconArray[6];
                break;
        }
    }

    public void OnYesBtnClicked()
    {
        MyDialogResult result = MyDialogResult.None;
        switch (buttonType)
        {
            case MyMessageBoxButton.AbortRetryIgnore:
                result = MyDialogResult.Abort;
                break;
            case MyMessageBoxButton.YesNoCancel:
                result = MyDialogResult.Yes;
                break;
        }
        if(msgHandler != null)
            msgHandler(result, passRefer, ShowToggle.isOn);
        gameObject.SetActive(false);
    }

    public void OnNoBtnClicked()
    {
        MyDialogResult result = MyDialogResult.None;
        switch (buttonType)
        {
            case MyMessageBoxButton.AbortRetryIgnore:
                result = MyDialogResult.Retry;
                break;
            case MyMessageBoxButton.OKCancel:
                result = MyDialogResult.OK;
                break;
            case MyMessageBoxButton.RetryCancel:
                result = MyDialogResult.Retry;
                break;
            case MyMessageBoxButton.YesNo:
                result = MyDialogResult.Yes;
                break;
            case MyMessageBoxButton.YesNoCancel:
                result = MyDialogResult.No;
                break;
        }
        if (msgHandler != null)
            msgHandler(result, passRefer, ShowToggle.isOn);
        gameObject.SetActive(false);
    }

    public void OnCancelBtnClicked()
    {
        MyDialogResult result = MyDialogResult.None;
        switch (buttonType)
        {
            case MyMessageBoxButton.AbortRetryIgnore:
                result = MyDialogResult.Ignore;
                break;
            case MyMessageBoxButton.OK:
                result = MyDialogResult.OK;
                break;
            case MyMessageBoxButton.YesNo:
                result = MyDialogResult.No;
                break;
            default: result = MyDialogResult.Cancel;
                break;
        }
        if (msgHandler != null)
            msgHandler(result, passRefer, ShowToggle.isOn);
        gameObject.SetActive(false);
    }

}
