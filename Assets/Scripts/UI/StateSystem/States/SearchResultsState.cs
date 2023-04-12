using System;
using Map;
using TargetsSystem.Points;
using TMPro;
using UnityEngine;

namespace UI.StateSystem.States
{
    public class SearchResultsState : State
    {
        [SerializeField] private FakeInputField _fakeInputField;
        
        private DataBase DataBase => Global.Instance.DataBase;
        private MapHandlePanel MapHandlePanel => Global.Instance.UISetterV2.MapHandlePanel;


        public override void Initialize()
        {
            MapHandlePanel.MapControllingActive = true;
            MapHandlePanel.SignSelectorActive = true;
        }

        public override void OnClose()
        {
            MapHandlePanel.MapControllingActive = false;
            MapHandlePanel.SignSelectorActive = false;
        }
        
        public void Initialize(string input, PointsGroup pointsGroup)
        {
            SetText(input);
            DeselectAllPoints();
            Select(pointsGroup);
        }

        private void SetText(string text)
        {
            _fakeInputField.SetText(text);
        }

        private void SetEmptyText()
        {
            _fakeInputField.Clear();
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
