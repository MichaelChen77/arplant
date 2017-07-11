using System;
using System.Collections.Generic;
using UnityEngine;

namespace IMAV.UI
{
    public class UIControl : MonoBehaviour
    {
        protected UIContainer parent;
        public virtual UIContainer Parent
        {
            get { return parent; }
            set {
                if (parent != value)
                {
                    if (parent != null)
                        parent.Items.Remove(this);
                    parent = value;
                    if (parent != null && !parent.Items.Contains(this))
                    {
                        parent.Items.Add(this);
                    }  
                }
            }
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }

        public virtual void Delete()
        {
            Parent = null;
            Destroy(gameObject);
        }

        public virtual void Refresh()
        {

        }
    }

    public class UIContainer: UIControl
    {
        protected List<UIControl> items = new List<UIControl>();
        public List<UIControl> Items
        {
            get { return items; }
        }

        public int ChildCount
        {
            get { return items.Count; }
        }

        public virtual void DeleteItem(UIControl ctrl)
        {
            ctrl.Delete();
        }

        public virtual void Refresh()
        {

        }
    }
}
