using System.Collections;
using System.Collections.Generic;
using Gameplay;
using NPC;
using UnityEngine;

namespace Lighting
{
    public class Level2CitizensLightController : MonoBehaviour
    {
        [SerializeField] private SpawnManager _spawnManager;
        [SerializeField] private float _colorTransitionSpeed = 5f;
        
        private const float MinIntensity = 0.3f;

        private readonly List<LightningSource> _sources = new ();
        
        private bool _isActive;
        
        public void ActivateCalculation()
        {
            _isActive = true;
            _sources.Clear();
            DarkAllNpc();
        }

        public void DeactivateCalculation()
        {
            _isActive = false;
            _sources.Clear();
        }

        public void AddLightSource(LightningSource source) => 
            _sources.Add(source);

        public void AddTemporaryLightSource(LightningSource source, float duration)
        {
            _sources.Add(source);
            StartCoroutine(RemoveLightSourceAfterDelay(source, duration));
        }

        private IEnumerator RemoveLightSourceAfterDelay(LightningSource source, float duration)
        {
            yield return new WaitForSeconds(duration);
            _sources.Remove(source);
        }
        
        private void Update()
        {
            if(_isActive)
                CalculateLightIntensity();
        }
        
        private void CalculateLightIntensity() => 
            _spawnManager.Npcs.ForEach(CalculateLightIntensityForNpc);

        private void CalculateLightIntensityForNpc(NPCController npc)
        {
            var targetColor = CalculateIntensityValue(npc.transform.position);
            npc.SpriteRenderer.color = Color.Lerp(npc.SpriteRenderer.color, targetColor, Time.deltaTime * _colorTransitionSpeed);
        }
        
        private void DarkAllNpc()
        {
            _spawnManager.Npcs.ForEach(DarkNpc);

            void DarkNpc(NPCController npc)
            {
                npc.SpriteRenderer.color = new Color(MinIntensity, MinIntensity, MinIntensity, 1f);
            }
        }

        private Color CalculateIntensityValue(Vector3 npcPosition)
        {
            if (_sources.Count == 0)
                return new Color(MinIntensity, MinIntensity, MinIntensity, 1f);
            
            var finalIntensity = 0f;
            var blendedColor = Color.black;
            
            foreach (var source in _sources)
            {
                var distance = Vector3.Distance(npcPosition, source.Position);
                
                if (distance > source.Range)
                    continue;
                
                var distanceFactor = 1f - (distance / source.Range);
                var lightContribution = source.Intensity * distanceFactor;
                
                finalIntensity += lightContribution;
                blendedColor += source.Color * lightContribution;
            }

            if (finalIntensity > 0f)
                blendedColor /= finalIntensity;
            else
                blendedColor = Color.white;

            finalIntensity = Mathf.Max(MinIntensity, Mathf.Clamp01(finalIntensity));
            
            var finalColor = blendedColor * finalIntensity;
            finalColor.a = 1f;
            return finalColor;
        }
        
        public class LightningSource
        {
            public Vector3 Position;
            public Color Color;
            public float Intensity;
            public float Range;
        }
    }
}
