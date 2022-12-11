using UnityEngine;

public static class MyExtensions
{
    public static Vector2 GetPreviousTouchPosition(this Touch touch)
    {
        return touch.position - touch.deltaPosition;
    }
}
