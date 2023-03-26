using System;
using Navigation;
using TargetsSystem.Points;
using UI;
using UI.SearchableDropDown;
using UI.SearchableDropDown.Options;
using UI.States.Setters;
using UnityEngine;

namespace Map
{
    public class SearchResultsSelector : MonoBehaviour
    {
        [SerializeField] private SearchableDropDown _searchableDropDown;

        private DataBase DataBase => Global.Instance.DataBase;
        private StateSetter StateSetter => Global.Instance.UISetterV2.StateSetter;
        private MapPointerSetter MapPointerSetter => Global.Instance.Navigator.MapPointerSetter;


        private void OnEnable()
        {
            _searchableDropDown.OptionSelected += Select;
        }

        private void OnDisable()
        {
            _searchableDropDown.OptionSelected -= Select;
        }
        

        private void Select(IOptionInfo optionInfo)
        {
            switch (optionInfo)
            {
                case PointInfo pointInfo:
                    Select(pointInfo);
                    break;
                
                case PointsGroup pointsGroup:
                    Select(pointsGroup);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(optionInfo));
            }
        }

        private void Select(PointInfo pointInfo)
        {
            if (DataBase.TryGetPoint(pointInfo, out Point point))
            {
                var pointerSetRequest = new PointerSetRequest(point.transform.position, PointerState.Default);
                MapPointerSetter.SetPointer(pointerSetRequest);

                int pointInfoStateIndex = 4;
                StateSetter.SetState(pointInfoStateIndex);
            }
            
        }
        
        private void Select(PointsGroup pointsGroup)
        {
            foreach (var point in pointsGroup.Points)
                point.SignCreator.Sign.Select();
            
            int searchResultStateIndex = 3;
            StateSetter.SetState(searchResultStateIndex);
        }
    }
}
