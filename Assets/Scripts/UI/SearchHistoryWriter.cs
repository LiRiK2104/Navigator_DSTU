using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Helpers;
using Map;
using Navigation;
using Newtonsoft.Json;
using TargetsSystem.Points;
using UI.Search.Options;
using UI.StateSystem;
using UI.StateSystem.States;
using UnityEngine;

namespace UI
{
    public class SearchHistoryWriter : MonoBehaviour
    {
        private const int MaxStoryLength = 10;
        private const string PointsInfosKey = "POINTS_INFOS";
        private const string PointsGroupsKey = "POINTS_GROUPS";
        
        private List<PointsGroup> _groups = new List<PointsGroup>();
        private List<PointInfo> _pointsInfos = new List<PointInfo>();
        private PathSearchState _pathSearchState;

        public bool HasHistory
        {
            get
            {
                Load();
                return _pointsInfos.Count > 0 || _groups.Count > 0;
            }
        }
        
        public ReadOnlyCollection<PointsGroup> Groups
        {
            get
            {
                LoadPointsGroups();
                return _groups.AsReadOnly();
            }
        }

        public ReadOnlyCollection<PointInfo> PointsInfos
        {
            get
            {
                LoadPointsInfos();
                return _pointsInfos.AsReadOnly();
            }
        }

        private DataBase DataBase => Global.Instance.DataBase;
        private SearchResultsSelector SearchResultsSelector => Global.Instance.UISetterV2.MapView.SearchResultsSelector;
        private UIStatesStorage UIStatesStorage => Global.Instance.UISetterV2.MapView.UIStatesStorage;
        private PathSearchState PathSearchState
        {
            get
            {
                if (_pathSearchState == null)
                    UIStatesStorage.TryGetState(StateType.PathSearch, out _pathSearchState);

                return _pathSearchState;
            }
        }


        private void OnEnable()
        {
            SearchResultsSelector.OptionSelected += AddPointInfo;
            PathSearchState.PathSearchView.OptionSelected += AddPointInfo;
        }

        private void OnDisable()
        {
            SearchResultsSelector.OptionSelected -= AddPointInfo;
            PathSearchState.PathSearchView.OptionSelected -= AddPointInfo;
        }

        private void Start()
        {
            Load();
        }


        private void AddPointInfo(IOptionInfo optionInfo)
        {
            switch (optionInfo)
            {
                case PointInfo pointInfo:
                    AddPointInfo(pointInfo);
                    break;
                
                case PointsGroup pointsGroup:
                    AddPointsGroup(pointsGroup);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(optionInfo));
            }
        }

        private void AddPointInfo(PointInfo pointInfo)
        {
            if (_pointsInfos.Contains(pointInfo))
            {
                int index = _pointsInfos.IndexOf(pointInfo);
                _pointsInfos.RemoveAt(index);
            }
            
            _pointsInfos.Add(pointInfo);
            RemoveOld();
            SavePointsInfos();
        }
        
        private void AddPointsGroup(PointsGroup pointsGroup)
        {
            if (_groups.Contains(pointsGroup))
            {
                int index = _groups.Select(group => group.Name).ToList().IndexOf(pointsGroup.Name);
                _groups.RemoveAt(index);
            }
            
            _groups.Add(pointsGroup);
            RemoveOld();
            SavePointsGroups();
        }

        private void RemoveOld()
        {
            if (_pointsInfos.Count + _groups.Count <= MaxStoryLength) 
                return;
            
            if (_pointsInfos.Count > _groups.Count)
                _pointsInfos.RemoveAt(0);
            else
                _groups.RemoveAt(0);
        }

        private void SavePointsInfos()
        {
            string json = JsonConvert.SerializeObject(_pointsInfos);
            PlayerPrefs.SetString(PointsInfosKey, json);
        }
        
        private void SavePointsGroups()
        {
            string[] names = _groups.Select(group => group.Name).ToArray();
            string json = JsonConvert.SerializeObject(names);
            PlayerPrefs.SetString(PointsGroupsKey, json);
        }

        private void Load()
        {
            LoadPointsInfos();
            LoadPointsGroups();
        }
        
        private void LoadPointsInfos()
        {
            _pointsInfos = ExtendedPlayerPrefs.TryGetString(PointsInfosKey, out string json) ? 
                JsonConvert.DeserializeObject<List<PointInfo>>(json) : 
                new List<PointInfo>();
        }
        
        private void LoadPointsGroups()
        {
            string[] names = ExtendedPlayerPrefs.TryGetString(PointsGroupsKey, out string json) ? 
                JsonConvert.DeserializeObject<string[]>(json) : 
                Array.Empty<string>();

            _groups = new List<PointsGroup>();

            foreach (var name in names)
            {
                if (DataBase.TryGetPointsGroup(name, out PointsGroup group)) 
                    _groups.Add(group);
            }
        }
    }
}
