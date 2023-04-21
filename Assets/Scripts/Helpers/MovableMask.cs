using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Helpers
{
    [RequireComponent(typeof(Mask))]
    public class MovableMask : MonoBehaviour
    {
        [SerializeField] private List<Image> _staticChildren = new List<Image>();
        
        private Mask _mask;

        private Mask Mask
        {
            get
            {
                _mask ??= GetComponent<Mask>();
                return _mask;
            }
        }

        public Vector2 MaskPosition
        {
            get { return Mask.transform.position; }
            set
            {
                SetParentToStaticChildren(null);
                Mask.transform.position = value;
                SetParentToStaticChildren(Mask.rectTransform);
            }
        }

        private void SetParentToStaticChildren(Transform parent)
        {
            foreach (var staticChild in _staticChildren)
            {
                if (staticChild == null)
                    return;
                
                staticChild.transform.SetParent(parent);
                staticChild.transform.SetAsFirstSibling();
            }
        }
    }
}
