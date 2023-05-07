using System.Collections;
using UnityEngine;

public class UserPositionFinder : MonoBehaviour
{
    public Vector3 UserPosition { get; private set; }
    public ARConnectingState State { get; private set; }
    private ARMain ARMain => Global.Instance.ArMain;
    

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
            
            //TODO: Включить AR без установки Worldview
            ARMain.Enter();
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
        UserPosition = ARMain.CameraManager.transform.position;
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
