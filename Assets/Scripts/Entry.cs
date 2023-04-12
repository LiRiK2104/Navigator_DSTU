using UnityEngine;

public class Entry : MonoBehaviour
{
    private DataBase DataBase => Global.Instance.DataBase;
    
    private void Start()
    {
        DataBase.Initialize();

        foreach (var point in DataBase.GetAllPoints())
            point.Initialize();
    }
}
