using System.Collections;
using Map;
using UnityEngine;
using UnityEngine.UI;

namespace Followers
{
    public class FlyingFollower : Follower
    {
        [SerializeField] private bool _shouldYRotate;
    
        private bool _shouldFollow;

        private Camera Camera => Global.Instance.CameraContainer.MapCamera;
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
            if (_shouldFollow == false)
                return;
        
            _shouldFollow = false;
        }
    
        private void FlyAndFollow()
        {
            if (_shouldFollow)
                return;
        
            _shouldFollow = true;
            StartCoroutine(FlyAndBind());
        }
    
        private IEnumerator FlyAndBind()
        {
            yield return FlyToTarget();
            Bind();
        }
    
        protected override void Bind()
        {
            base.Bind();
            StartCoroutine(Follow());
        }
    
        private IEnumerator FlyToTarget()
        {
            float speed = 2;
            float minDistance = 0.1f;
            float distance;

            do
            {
                Vector3 targetPosition = new Vector3(Target.transform.position.x, Camera.transform.position.y,
                    Target.transform.position.z);
                Quaternion transformRotation = Quaternion.Euler(0, Target.transform.rotation.eulerAngles.y, 0) * Quaternion.Euler(90, 0, 0);
                
                Camera.transform.rotation = Quaternion.Lerp(Camera.transform.rotation, transformRotation, Time.deltaTime * speed);
                Camera.transform.position = Vector3.Lerp(Camera.transform.position, targetPosition, Time.deltaTime * speed);
            
                distance = Vector3.Distance(Camera.transform.position, targetPosition);
                yield return null;
            } 
            while (_shouldFollow && distance > minDistance);
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
