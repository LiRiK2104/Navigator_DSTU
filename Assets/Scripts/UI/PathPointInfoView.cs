using Navigation;
using UnityEngine;
using UI.StateSystem.Setters;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(PathPointButtonStateSetter))]
    public class PathPointInfoView : PointInfoView
    {
        [SerializeField] private GameObject _defaultState;
        [SerializeField] private GameObject _infoState;


        public override void Initialize(PointInfo pointInfo)
        {
            _infoState.SetActive(true);
            _defaultState.SetActive(false);
            
            base.Initialize(pointInfo);
        }

        public void Initialize()
        {
            _infoState.SetActive(false);
            _defaultState.SetActive(true);
        }
    }
}
