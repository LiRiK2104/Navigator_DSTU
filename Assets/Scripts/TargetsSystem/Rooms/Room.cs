using System;
using TargetsSystem.Signs;
using UnityEngine;

namespace TargetsSystem.Rooms
{
    public abstract class Room : MonoBehaviour
    {
        [SerializeField] private string _id;
        [SerializeField] private Color _fieldColor = Color.white;
        [Space]
        [SerializeField] private SpriteRenderer _field;
        [SerializeField] private RoomSign _signPrefab;

        public string Id => _id;
        private Transform SignContainer => Global.Instance.ArEnvironment.RoomSignsContainer;


        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            PaintField();
            var sign = Instantiate(_signPrefab, transform.position, Quaternion.identity, SignContainer);
            sign.Initialize(this);
        }
        
        private void PaintField()
        {
            _field.color = _fieldColor;
        }
    }
}
