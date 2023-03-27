using System;
using TargetsSystem.Points;
using TMPro;
using UnityEngine;

namespace UI.StateSystem.States
{
    public class SearchResultsState : State
    {
        [SerializeField] private TextMeshProUGUI _fakeInputFieldText;
        
        private DataBase DataBase => Global.Instance.DataBase;
        
        
        public override void Initialize() { }

        public override void OnClose() { }
        
        public void Initialize(string input, PointsGroup pointsGroup)
        {
            SetText(input);
            DeselectAllPoints();
            Select(pointsGroup);
        }

        private void SetText(string text)
        {
            _fakeInputFieldText.text = text;
        }

        private void SetEmptyText()
        {
            SetText(string.Empty);
        }

        private void DeselectAllPoints()
        {
            foreach (var point in DataBase.GetAllPoints())
                point.SignCreator.Sign.Deselect();
        }
        
        private void Select(PointsGroup pointsGroup)
        {
            foreach (var point in pointsGroup.Points)
                point.SignCreator.Sign.Select();
        }
    }
}
