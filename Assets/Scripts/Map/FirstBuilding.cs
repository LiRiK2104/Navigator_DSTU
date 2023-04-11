using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Map
{
    public class FirstBuilding : MonoBehaviour
    {
        [SerializeField] private List<Floor> _floors = new List<Floor>();

        public ReadOnlyCollection<Floor> Floors => _floors.AsReadOnly();
    }
}
