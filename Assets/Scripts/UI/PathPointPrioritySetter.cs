using Navigation;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PathPointPrioritySetter : MonoBehaviour
    {
        [SerializeField] private DestinationPoint _destinationPoint;
        
        private Button _button;

        private PathFinder PathFinder => Global.Instance.Navigator.PathFinder;
        
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SetPriority);
        }

        private void SetPriority()
        {
            PathFinder.PriorityPoint = _destinationPoint;
        }
    }
}
