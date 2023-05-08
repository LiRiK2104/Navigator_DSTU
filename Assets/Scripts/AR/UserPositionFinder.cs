using System.Collections;
using UnityEngine;

namespace AR
{
    public class UserPositionFinder : MonoBehaviour
    {
        public Vector3 UserPosition { get; private set; }
        public ARConnectingState State { get; private set; } = ARConnectingState.None;
        private ARMain ARMain => Global.Instance.ArMain;
        private AREnvironment AREnvironment => Global.Instance.ArEnvironment;
    

        private void OnDisable()
        {
            Unsubscribe();
        }


        public IEnumerator FindUserPosition()
        {
            UserPosition = Vector3.zero;
            State = ARConnectingState.None;

            if (ARMain.Active)
            {
                GetUserPosition();
            }
            else
            {
                Subscribe();
                State = ARConnectingState.Processing;
                ARMain.Enter(false);
            }
        
            yield return new WaitUntil(() => State is ARConnectingState.Completed or ARConnectingState.Failed);
            Unsubscribe();
        }

        private void Subscribe()
        {
            ARMain.Entered += GetUserPosition;
            ARMain.Exited += CancelSearch;
        }
    
        private void Unsubscribe()
        {
            ARMain.Entered += GetUserPosition;
            ARMain.Exited += CancelSearch;
        }
    
        private void GetUserPosition()
        {
            var userPosition = ARMain.CameraManager.transform.position;
            UserPosition = new Vector3(userPosition.x, AREnvironment.GetFloorHeight(ARMain.UserFloorIndex), userPosition.z);
            State = ARConnectingState.Completed;
        }
    
        private void CancelSearch()
        {
            State = ARConnectingState.Failed;
        }
    }

    public enum ARConnectingState
    {
        None,
        Processing,
        Completed,
        Failed
    }
}