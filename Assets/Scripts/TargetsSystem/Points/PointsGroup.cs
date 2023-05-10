using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Navigation;
using UI.Search.Options;
using UnityEngine;

namespace TargetsSystem.Points
{
    public class PointsGroup : MonoBehaviour, IOptionInfo
    {
        [SerializeField] private string _name;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private List<Point> _points;

        public string Name => _name;
        public Sprite Sprite => _sprite;
        public ReadOnlyCollection<Point> Points => _points.AsReadOnly();
        private DataBase DataBase => Global.Instance.DataBase;


        public int[] GetFloorsIndexes()
        {
            var floorsIndexes = new List<int>();
            
            foreach (var point in _points)
            {
                if (DataBase.TryGetPointInfo(point, out PointInfo pointInfo) && 
                    floorsIndexes.Contains(pointInfo.Address.FloorIndex) == false)
                {
                    floorsIndexes.Add(pointInfo.Address.FloorIndex);
                }
            }

            return floorsIndexes.ToArray();
        } 
        
        public bool TryGetPoint(PointInfo myPointInfo, out Point foundPoint)
        {
            foundPoint = null;
            
            foreach (var point in _points)
            {
                if (DataBase.TryGetPointInfo(point, out PointInfo pointInfo) && 
                    pointInfo.Equals(myPointInfo))
                {
                    foundPoint = point;
                    return true;
                }
            }
            
            return false;
        }

        public List<string> GetAllIds()
        {
            List<string> ids = new List<string>();
            
            ids.Add(_name);
            //_points.ForEach(room => ids.Add(room.Id));

            return ids;
        }
    }
}
