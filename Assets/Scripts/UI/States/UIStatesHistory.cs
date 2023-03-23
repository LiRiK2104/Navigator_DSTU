using System.Collections.Generic;
using UnityEngine;

namespace UI.States
{
    public class UIStatesHistory : MonoBehaviour
    {
        private const int MaxHistoryLength = 10;
        
        private List<int> _statesIndexes = new List<int>();

        
        public void AddState(int index)
        {
            _statesIndexes.Add(index);

            if (_statesIndexes.Count > MaxHistoryLength)
                _statesIndexes.RemoveAt(0);
        }
        
        public bool TryStepBack(int index, out int previousIndex)
        {
            previousIndex = 0;
            int minHistoryLength = 2;

            if (_statesIndexes.Count < minHistoryLength)
                return false;

            _statesIndexes.RemoveAt(_statesIndexes.Count - 1);
            previousIndex = _statesIndexes.Count - 1;
            return true;
        }
    }
}
