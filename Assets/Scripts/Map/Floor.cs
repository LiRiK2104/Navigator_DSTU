using UnityEngine;

namespace Map
{
    public class Floor : MonoBehaviour
    {
        [SerializeField] private GameObject _signsContainer;

        public GameObject SignsContainer => _signsContainer;
    }
}