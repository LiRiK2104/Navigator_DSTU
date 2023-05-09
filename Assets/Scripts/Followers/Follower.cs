using Helpers;
using UnityEngine;

namespace Followers
{
    public abstract class Follower : MonoBehaviour
    {
        private Vector3 _differentPosition;
        private Quaternion _differentRotation;
        
        protected abstract GameObject Target { get; }
        
        
        protected virtual void Bind()
        {
            BindPosition();
            BindRotation();
        }

        protected void FollowByPosition()
        {
            transform.position = Target.transform.position + _differentPosition;
        }

        protected void FollowYRotation()
        {
            transform.rotation = Target.transform.rotation.ClearDimensions(true, false,true) * 
                                 _differentRotation.ClearDimensions(true, false,true);
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
