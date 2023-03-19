using TargetsSystem.Rooms;
using TMPro;
using UnityEngine;

namespace TargetsSystem.Signs
{
    public class RoomNumber : RoomSign
    {
        [SerializeField] private TextMeshProUGUI _textMeshPro;

        public override void Initialize(Room room)
        {
            SetNumber(room.Id);
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
