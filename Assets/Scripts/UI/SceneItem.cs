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
        public SceneItemSelected ItemClick;

        public void SetValue(SceneData data)
        {
            sceneData = data;
            itemName.text = data.Name;
            itemIcon.sprite = data.Icon;
            itemTime.text = data.DateTime;
            itemNumber.text = data.ModelNum.ToString();
        }

        public void OnPointerClick(PointerEventData data)
        {
            ItemClick(this);
        }
    }
}
