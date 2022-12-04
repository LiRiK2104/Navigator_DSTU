using UnityEngine;

namespace Navigation
{
    public class Navigator : MonoBehaviour
    {
        [SerializeField] private PathFinder _pathFinder;
        [SerializeField] private DestinationSetter _destinationSetter;

        public PathFinder PathFinder => _pathFinder;
        public DestinationSetter DestinationSetter => _destinationSetter;
    }
}
