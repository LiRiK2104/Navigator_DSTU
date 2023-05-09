using UnityEngine;

namespace Followers
{
    public class RigidFollower : Follower
    {
        [SerializeField] protected GameObject _target;

        protected override GameObject Target => _target;


        private void Start()
        {
            Bind();
        }

        private void Update()
        {
            Follow();
        }
        
        
        private void Follow()
        {
            FollowYRotation();
            FollowByPosition();
        }
    }
}
