using Navigation;
using TargetsSystem.Points;
using TargetsSystem.Rooms;
using TMPro;
using UnityEngine;

namespace Map.Signs
{
    public class RoomNumber : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshPro;

        
        public void Initialize(PointInfo pointInfo)
        {
            SetNumber(pointInfo.Name);
        }
        
        private void SetNumber(string number)
        {
            _textMeshPro.text = FormatNumber(number);
        }

        private string FormatNumber(string number)
        {
            char splitValue = '-';
            string[] subs = number.Split(splitValue);
            int successSplitCount = 2;

            if (subs.Length == successSplitCount)
                return subs[1];

            return number;
        }
    }
}
