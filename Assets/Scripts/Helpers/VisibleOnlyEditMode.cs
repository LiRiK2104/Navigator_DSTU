using System.Linq;
using UnityEngine;

namespace Helpers
{
    public class VisibleOnlyEditMode : MonoBehaviour
    {
        private void OnEnable()
        {
            var renderers = GetComponentsInChildren<Renderer>().ToList();
            renderers.AddRange(GetComponents<Renderer>());

            foreach (var renderer in renderers)
                renderer.enabled = false;
        }
    }
}
