using UnityEngine;

namespace Followers
{
    public class RigidFollower : Follower
    {
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
