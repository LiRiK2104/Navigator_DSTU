using System;
using System.Collections;
using AR;
using Map;
using UnityEngine;

namespace Followers
{
    public class SoftARFollower : Follower
    {
        private bool _shouldFollow;

        protected override GameObject Target => ARMain.CameraManager.gameObject;
        private ARMain ARMain => Global.Instance.ArMain;
        private MapControl MapControl => Global.Instance.UISetterV2.MapView.MapHandlePanel.MapControl;


        private void OnEnable()
        {
            MapControl.StartedDrag += StopFollow;
        }

        private void OnDisable()
        {
            MapControl.StartedDrag -= StopFollow;
        }


        public void FlyAndFollow()
        {
            if (_shouldFollow)
                return;
        
            _shouldFollow = true;
            MapControl.GoToTarget(Target.transform, ARMain.UserFloorIndex,true, false, Bind);
        }
        
        protected override void Bind()
        {
            StartCoroutine(FollowAndBind());
        }

        private void StopFollow()
        {
            _shouldFollow = false;
        }

        private IEnumerator FollowAndBind()
        {
            base.Bind();
            yield return Follow();
        }
        
        private IEnumerator Follow()
        {
            while (_shouldFollow)
            {
                FollowByPosition();
                yield return null;
            }
        }
    }
}
