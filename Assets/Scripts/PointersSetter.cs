using UnityEngine;

public class PointersSetter : MonoBehaviour
{
    [SerializeField] private Pointer _3dPointerPrefab;
    [SerializeField] private MinimapPointer _minimapPointerPrefab;
    [SerializeField] private GameObject _environment;
    [SerializeField] private Camera _minimapCamera;
    
    private Pointer _pointer;
    private MinimapPointer _minimapPointer;


    public void SetPointers(Vector3 targetPosition)
    {
        Set3DPointer(targetPosition);
        SetMinimapPointer(targetPosition);
    }
    
    private void Set3DPointer(Vector3 targetPosition)
    {
        if (_pointer == null)
            _pointer = Instantiate(_3dPointerPrefab, _environment.transform);

        _pointer.transform.position = targetPosition;
    }
    
    private void SetMinimapPointer(Vector3 targetPosition)
    {
        if (_minimapPointer == null)
        {
            _minimapPointer = Instantiate(_minimapPointerPrefab, _environment.transform);
            _minimapPointer.Initialize(_minimapCamera);
        }

        _minimapPointer.transform.position = targetPosition;
    }
}
