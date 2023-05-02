using UnityEngine;

public class BordersSetter : MonoBehaviour
{
    private const float MinBorder = 0;
    
    [SerializeField] private float _topBorder = 10;
    [SerializeField] private float _bottomBorder = -10;
    [SerializeField] private float _leftBorder = -10;
    [SerializeField] private float _rightBorder = 10;

    public float TopBorder => transform.position.z + _topBorder;
    public float BottomBorder => transform.position.z + _bottomBorder;
    public float LeftBorder => transform.position.x + _leftBorder;
    public float RightBorder => transform.position.x + _rightBorder;
    
    public Vector3[] Corners => new [] { LeftTop, LeftBottom, RightTop, RightBottom };
    
    private Vector3 LeftTop => ToLocalSpace(new Vector3(_leftBorder, transform.position.y,_topBorder));
    private Vector3 RightTop => ToLocalSpace(new Vector3(_rightBorder, transform.position.y, _topBorder));
    private Vector3 LeftBottom => ToLocalSpace(new Vector3(_leftBorder, transform.position.y, _bottomBorder));
    private Vector3 RightBottom => ToLocalSpace(new Vector3(_rightBorder, transform.position.y, _bottomBorder));


    private void OnValidate()
    {
        ClampBorders();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(LeftTop, RightTop);
        Gizmos.DrawLine(LeftTop, LeftBottom);
        Gizmos.DrawLine(RightTop, RightBottom);
        Gizmos.DrawLine(LeftBottom, RightBottom);
    }

    private Vector3 ToLocalSpace(Vector3 position)
    {
        return transform.position + /*transform.rotation **/ position;
    }
    
    
    private void ClampBorders()
    {
        _topBorder = Mathf.Max(_topBorder, MinBorder);
        _rightBorder = Mathf.Max(_rightBorder, MinBorder);
        _bottomBorder = Mathf.Min(_bottomBorder, MinBorder);
        _leftBorder = Mathf.Min(_leftBorder, MinBorder);
    }
}
