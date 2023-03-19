using System.Collections.Generic;
using System.Linq;
using Navigation;
using UnityEngine;

namespace TargetsSystem.Rooms
{
    public class MultiRoom : MonoBehaviour
    {
        [SerializeField] private string _commonId;
        [SerializeField] private List<AccessibleRoom> _rooms;

        public string CommonId => _commonId;
        private PathFinder PathFinder => Global.Instance.Navigator.PathFinder;
        
        
        public AccessibleRoom GetNearestRoom()
        {
            return _rooms.OrderBy(room =>
                PathFinder.CalculatePathDistance(PathFinder.GetLocalPath(room.TargetPointPosition))).First();
        }

        public bool TryGetRoom(string id, out AccessibleRoom foundRoom)
        {
            foundRoom = _rooms.First(room => room.Id == id);
            return foundRoom != null;
        }

        public List<string> GetAllIds()
        {
            List<string> ids = new List<string>();
            
            ids.Add(_commonId);
            _rooms.ForEach(room => ids.Add(room.Id));

            return ids;
        }
    }
}
