using System;
using System.Collections.Generic;
using System.Linq;
using Navigation;
using TargetsSystem.Points;
using UI.Search.Options.States;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Search.Options
{
    public class Option : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private PointState _pointState;
        [SerializeField] private GroupState _groupState;

        public OptionType ContentType { get; private set; }
        private List<string> _keyWords = new List<string>();


        public bool HasKeyWord(string arg)
        {
            return _keyWords.Any(keyWord => keyWord.ToLower() == arg.ToLower());
        }
        
        public bool HasInKeyWords(string arg)
        {
            return _keyWords.Any(keyWord => keyWord.ToLower().Contains(arg.ToLower()));
        }
        
        public void Initialize(IOptionInfo optionInfo, OptionsList.OnOptionSelectedDel callback, bool isStoryOption = false)
        {
            switch (optionInfo)
            {
                case PointInfo pointInfo:
                    Initialize(pointInfo, isStoryOption);
                    break;
                
                case PointsGroup pointsGroup:
                    Initialize(pointsGroup, isStoryOption);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(optionInfo));
            }

            InitializeButton(optionInfo, callback);
        }
        
        private void Initialize(PointInfo pointInfo, bool isStoryOption)
        {
            ContentType = OptionType.Point;
            _groupState.gameObject.SetActive(false);
            _pointState.gameObject.SetActive(true);

            _pointState.Initialize(pointInfo, isStoryOption);

            _keyWords.Add(pointInfo.Name);

            if (pointInfo.Address.HasRoomId && pointInfo.NameIsRoomId == false)
                _keyWords.Add(pointInfo.Address.RoomId);
        }
        
        private void Initialize(PointsGroup group, bool isStoryOption)
        {
            ContentType = OptionType.Group;
            _groupState.gameObject.SetActive(true);
            _pointState.gameObject.SetActive(false);
            
            _groupState.Initialize(group, isStoryOption);
            _keyWords.Add(group.Name);
        }

        private void InitializeButton(IOptionInfo optionInfo, OptionsList.OnOptionSelectedDel callback)
        {
            _button.onClick.AddListener(delegate { callback.Invoke(optionInfo); });
        }
    }

    public interface IOptionInfo
    {
        public string Name { get; }
    }

    public enum OptionType
    {
        Point,
        Group
    }
}
