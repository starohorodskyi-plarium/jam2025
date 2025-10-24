using System;
using System.Collections.Generic;
using System.Linq;
using MonoControllers.Camera;
using UnityEngine;

namespace Gun
{
    public class SpriteSwapper : MonoBehaviour
    {
        public enum AngleWeapon
        {
            Left90,
            Left,
            Middle,
            Right,
            Right90
        }
        
        [Serializable]
        public class SpriteSwap
        {
            public AngleWeapon Angle;
            public SpriteRenderer Gun;
            public GameObject Muzzle;
            public Vector2Int ActiveAnglesRange;
            public Vector2Int ActiveAnglesZoomRange;
            
            public Vector2Int GetActiveAnglesRange(bool isZoom) => 
                isZoom ? ActiveAnglesZoomRange : ActiveAnglesRange;
        }
        
        [SerializeField] private List<SpriteSwap> _spriteSwaps;
        [SerializeField] private Transform _rotatedParent;
        [Space]
        [SerializeField] private ZoomController _zoom;
        
        private AngleWeapon _currentAngle;

        public SpriteRenderer ActiveSprite;
        
        public List<SpriteRenderer> AllSprites => 
            _spriteSwaps.Select(x => x.Gun).ToList();

        public static event Action SpriteChanged;

        private void OnEnable()
        {
            _currentAngle = AngleWeapon.Middle;
            SetSprite(_currentAngle);
        }

        private void Update()
        {
            var angle = Angle();
            GetAngleWeapon(angle);
        }

        public Transform ActiveMuzzle() => 
            (from spriteSwap in _spriteSwaps where spriteSwap.Angle == _currentAngle select spriteSwap.Muzzle.transform)
            .FirstOrDefault();

        private float Angle()
        {
            var angle = _rotatedParent.localEulerAngles.y;
            
            if (angle > 180)
                angle -= 360;
            
            return angle;
        }
        
        private void GetAngleWeapon(float angle)
        {
            foreach (var spriteSwap in _spriteSwaps.Where(spriteSwap => spriteSwap.Angle == _currentAngle))
            {
                var range = spriteSwap.GetActiveAnglesRange(_zoom.IsZoomed);
                if (angle >= range.x && angle <= range.y)
                    return;
                
                break;
            }
            
            foreach (var spriteSwap in _spriteSwaps.Where(spriteSwap => 
                         angle >= spriteSwap.GetActiveAnglesRange(_zoom.IsZoomed).x 
                         && angle <= spriteSwap.GetActiveAnglesRange(_zoom.IsZoomed).y))
            {
                if (_currentAngle != spriteSwap.Angle)
                {
                    _currentAngle = spriteSwap.Angle;
                    SetSprite(_currentAngle);
                }
                
                break;
            }
        }
        
        private void SetSprite(AngleWeapon angleWeapon)
        {
            Transform activeMuzzle = null;
            List<Transform> muzzleChildren = new();
            
            foreach (var spriteSwap in _spriteSwaps)
            {
                var isActive = spriteSwap.Angle == angleWeapon;
                spriteSwap.Gun.enabled = isActive;

                if (isActive)
                {
                    ActiveSprite = spriteSwap.Gun;
                    activeMuzzle = spriteSwap.Muzzle.transform;
                }
                else
                {
                    muzzleChildren.AddRange(StoreMuzzleChildren(spriteSwap.Muzzle));
                }
            }
            
            if(activeMuzzle != null)
                muzzleChildren.ForEach(child => ResetMuzzleChildren(child, activeMuzzle));
            
            SpriteChanged?.Invoke();
        }

        private static IEnumerable<Transform> StoreMuzzleChildren(GameObject muzzle)
        {
            if (muzzle.transform.childCount <= 0) 
                yield break;
                    
            for (var i = 0; i < muzzle.transform.childCount; i++) 
                yield return muzzle.transform.GetChild(i);
        }

        private static void ResetMuzzleChildren(Transform child , Transform activeMuzzle)
        {
            child.SetParent(activeMuzzle);
            child.localPosition = Vector3.zero;
        }
    }
}
