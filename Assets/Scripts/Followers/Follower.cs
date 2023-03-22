using UnityEngine;

namespace Followers
{
    public abstract class Follower : MonoBehaviour
    {
        [SerializeField] protected GameObject Target;
    
        private Vector3 _differentPosition;
        private Quaternion _differentRotation;


        protected virtual void Bind()
        {
            BindPosition();
            BindRotation();
        }

        protected void FollowByPosition()
        {
            transform.position = Target.transform.position + _differentPosition;
        }
    
        protected void FreezeRotation()
        {
            var targetRotation = _differentRotation.eulerAngles;
            transform.rotation = Quaternion.Euler(targetRotation);
        }
    
        protected void FollowYRotation()
        {
            var targetRotation = _differentRotation.eulerAngles + new Vector3(0, Target.transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(targetRotation);
        }
    
        private void BindPosition()
        {
            _differentPosition = transform.position - Target.transform.position;
        }
    
        private void BindRotation()
        {
            _differentRotation = Quaternion.Inverse(Target.transform.rotation) * transform.rotation;
        }
    }
}
