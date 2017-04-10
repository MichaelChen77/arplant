using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace IMAV.UI
{
    public class SceneItem : MonoBehaviour, IPointerClickHandler
    {
        public Image itemIcon;
        public Text itemName;
        public Text itemTime;
        public Text itemNumber;

        SceneData sceneData;
        public SceneData Data
        {
            get { return sceneData; }
        }
        public delegate void SceneItemSelected(SceneItem _item);
        SceneItemSelected ItemClick;

        public void SetValue(SceneData data, SceneItemSelected _click)
        {
            sceneData = data;
            itemName.text = data.Name;
            //itemIcon.sprite = data.Icon;
            itemTime.text = DataUtility.UnixTimeStampToDateTime((double)data.created_at).ToString();
            itemNumber.text = data.ModelNum.ToString();
            ItemClick = _click;
        }

        public void OnPointerClick(PointerEventData data)
        {
            ItemClick(this);
        }
    }
}
