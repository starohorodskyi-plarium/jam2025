using Gun;
using UnityEngine;

namespace MonoControllers
{
    public class ShootSoundController : MonoBehaviour
    {
        [SerializeField] private GameObject _shootSoundPrefab;
        [SerializeField] private SpriteSwapper _spawnPoint;
        
        public void PlaySound() => 
            Instantiate(_shootSoundPrefab, _spawnPoint.ActiveMuzzle());
    }
}
