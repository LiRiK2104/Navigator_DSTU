using UnityEngine;

namespace TargetsSystem.Points
{
    public class LockedRoom : MonoBehaviour, IRoom
    {
        [SerializeField] private string _id;
        [Space]
        [SerializeField] private Color _fieldColor = Color.white;
        [SerializeField] private SpriteRenderer _field;


        public string Id => _id;
        
        
        public void PaintField()
        {
            _field.color = _fieldColor;
        }
    }
}
