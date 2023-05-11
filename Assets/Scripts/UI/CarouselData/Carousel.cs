using System.Collections.Generic;
using TargetsSystem.Points;
using UnityEngine;

namespace UI.CarouselData
{
    public class Carousel : MonoBehaviour
    {
        [SerializeField] private List<Point> _points;
        [SerializeField] private List<PointsGroup> _groups;
        [SerializeField] private Transform _contentParent;
        [SerializeField] private CarouselCard _cardTemplate;


        public void Initialize()
        {
            foreach (var point in _points)
                CreateCard().Initialize(point);
            
            foreach (var group in _groups)
                CreateCard().Initialize(group);
        }

        private CarouselCard CreateCard()
        {
            return Instantiate(_cardTemplate, _contentParent);
        }
    }
}
