using Navigation;
using TMPro;
using UnityEngine;

namespace Map.Signs.States
{
    public class SearchResultState : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        
        
        public void Initialize(PointInfo pointInfo)
        {
            _name.text = pointInfo.Name;
        }
    }
}
