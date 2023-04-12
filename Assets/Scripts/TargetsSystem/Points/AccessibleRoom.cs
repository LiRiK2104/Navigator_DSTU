using Map.Signs;
using UnityEngine;

namespace TargetsSystem.Points
{
    public class AccessibleRoom : Point, IRoom
    {
        [SerializeField] private string _id;
        [Space]
        [SerializeField] private Color _fieldColor = Color.white;
        [SerializeField] private SpriteRenderer _field;

        public string Id => _id;


        public override void Initialize()
        {
            base.Initialize();
            PaintField();
        }
        
        public void PaintField()
        {
            _field.color = _fieldColor;
        }
    }

    public interface IRoom
    {
        public string Id { get; }

        public void PaintField();
    }
}
