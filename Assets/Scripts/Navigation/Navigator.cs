using UI;
using UnityEngine;

namespace Navigation
{
    public class Navigator : MonoBehaviour
    {
        [SerializeField] private PathFinder _pathFinder;
        [SerializeField] private MapPointerSetter _mapPointerSetter;


        public PathFinder PathFinder => _pathFinder;
        public MapPointerSetter MapPointerSetter => _mapPointerSetter;
    }
}
