using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public enum Faction
    {
        Ally,
        Enemy
    }

    public enum Gender
    {
        Male,
        Female
    }

    [SerializeField] private Faction faction = Faction.Enemy;
    [SerializeField] private Gender gender = Gender.Male;
    [SerializeField] private GameObject allyHitEffect;
    [SerializeField] private GameObject enemyHitEffect;
    [SerializeField] private GameObject soundPlayerPrefab;
    public void Hit(float impactDelay)
    {
        Debug.Log($"{gameObject.name} was hit!");
       
        StartCoroutine(HitRoutine());

        IEnumerator HitRoutine()
        {
            yield return new WaitForSeconds(impactDelay);
            PlayDeathEffects();
            Destroy(gameObject);
        }
    }
    
    private void PlayDeathEffects()
    {
        var effectPrefab = faction == Faction.Ally ? allyHitEffect : enemyHitEffect;
        if (effectPrefab)
            Instantiate(effectPrefab, transform.position, Quaternion.identity);

        if (!soundPlayerPrefab) 
            return;
        
        var soundObj = Instantiate(soundPlayerPrefab, transform.position, Quaternion.identity);
        var reaction = soundObj.GetComponent<NPCShotAudioReaction>();
        
        if (!reaction) 
            return;
        
        if (faction == Faction.Ally)
            reaction.PlayHumanDeath(gender);
        else
            reaction.PlayDemonDeath(gender);
    }
}
