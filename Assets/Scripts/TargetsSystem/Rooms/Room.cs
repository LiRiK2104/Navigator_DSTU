using System;
using UnityEngine;

namespace TargetsSystem.Rooms
{
    public abstract class Room : MonoBehaviour
    {
        [SerializeField] private string _id;
        [SerializeField] private Color _fieldColor = Color.white;
        [Space]
        [SerializeField] private SpriteRenderer _field;

        public string Id => _id;


        private void Start()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            PaintField();
        }
        
        private void PaintField()
        {
            _field.color = _fieldColor;
        }
    }
}
