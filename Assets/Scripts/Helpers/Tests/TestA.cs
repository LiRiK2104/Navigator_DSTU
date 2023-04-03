using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Helpers
{
    public class TestA : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void MakeRed()
        {
            _image.color = Color.red;
        }
    }
}
