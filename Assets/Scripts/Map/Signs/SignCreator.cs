using Navigation;
using UnityEngine;

namespace Map.Signs
{
    public class SignCreator : MonoBehaviour
    {
        [SerializeField] private Sign _signPrefab;
        [SerializeField] private SignPreset _signPreset;

        private Sign _sign;

        public Sign Sign => _sign;
        public SignPreset SignPreset => _signPreset;
        private AREnvironment AREnvironment => Global.Instance.ArEnvironment;
        
        
        public void Create(PointInfo pointInfo)
        {
            _sign = Instantiate(_signPrefab, transform.position, Quaternion.identity, GetSignContainer(pointInfo));
            _sign.Initialize(pointInfo, _signPreset);
        }

        private Transform GetSignContainer(PointInfo pointInfo)
        {
            return AREnvironment.FirstBuilding.Floors[pointInfo.Address.FloorIndex].SignsContainer.transform;
        }
    }
}
