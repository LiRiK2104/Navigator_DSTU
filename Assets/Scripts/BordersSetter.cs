using UnityEngine;

public class BordersSetter : MonoBehaviour
{
    private const float MinBorder = 0;
    
    [SerializeField] private float _topBorder = 10;
    [SerializeField] private float _bottomBorder = -10;
    [SerializeField] private float _leftBorder = -10;
    [SerializeField] private float _rightBorder = 10;

    public float TopBorder => _topBorder;
    public float BottomBorder => _bottomBorder;
    public float LeftBorder => _leftBorder;
    public float RightBorder => _rightBorder;


    private void OnValidate()
    {
        ClampBorders();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var height = transform.position.y;
        
        Vector3 leftTop = new Vector3(_leftBorder, height,_topBorder);
        Vector3 rightTop = new Vector3(_rightBorder, height, _topBorder);
        Vector3 leftBottom = new Vector3(_leftBorder, height, _bottomBorder);
        Vector3 rightBottom = new Vector3(_rightBorder, height, _bottomBorder);
        
        Gizmos.DrawLine(leftTop, rightTop);
        Gizmos.DrawLine(leftTop, leftBottom);
        Gizmos.DrawLine(rightTop, rightBottom);
        Gizmos.DrawLine(leftBottom, rightBottom);
    }
    
    
    private void ClampBorders()
    {
        _topBorder = Mathf.Max(_topBorder, MinBorder);
        _rightBorder = Mathf.Max(_rightBorder, MinBorder);
        _bottomBorder = Mathf.Min(_bottomBorder, MinBorder);
        _leftBorder = Mathf.Min(_leftBorder, MinBorder);
    }
}
