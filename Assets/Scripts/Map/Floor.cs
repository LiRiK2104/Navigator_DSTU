using Plugins.ZenythStudios.Graphway.Assets.Scripts;
using UnityEngine;

namespace Map
{
    public class Floor : MonoBehaviour
    {
        [SerializeField] private GameObject _signsContainer;
        [SerializeField] private Graphway _graphway;

        public GameObject SignsContainer => _signsContainer;
        public Graphway Graphway => _graphway;
    }
}