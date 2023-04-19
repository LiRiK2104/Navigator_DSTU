using System.Collections;
using Map;
using UnityEngine;
using UnityEngine.UI;

namespace Followers
{
    public class SoftFollower : Follower
    {
        [SerializeField] private bool _shouldYRotate;
    
        private bool _shouldFollow;

        private Button FollowButton => Global.Instance.UiSetter.TrackingMenu.FollowButton;
        private MapControl MapControl => Global.Instance.UISetterV2.MapHandlePanel.MapControl;


        private void OnEnable()
        {
            MapControl.StartedDrag += StopFollow;
            FollowButton.onClick.AddListener(FlyAndFollow);
        }

        private void OnDisable()
        {
            MapControl.StartedDrag -= StopFollow;
            FollowButton.onClick.RemoveListener(FlyAndFollow);
        }
    

        private void StopFollow()
        {
            _shouldFollow = false;
        }
    
        private void FlyAndFollow()
        {
            if (_shouldFollow)
                return;
        
            _shouldFollow = true;
            MapControl.GoToTarget(Target.transform, true, false, Bind);
        }

        protected override void Bind()
        {
            base.Bind();
            StartCoroutine(Follow());
        }
        
        private IEnumerator Follow()
        {
            while (_shouldFollow)
            {
                if (_shouldYRotate)
                    FollowYRotation();
                else
                    FreezeRotation();

                FollowByPosition();
                yield return null;
            }
        }
    }
}
