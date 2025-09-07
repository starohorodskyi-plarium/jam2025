using System;
using System.Collections;
using UnityEngine;

public class SnailController : MonoBehaviour
{
    [Header("Reaction Prefab")]
    [SerializeField] private GameObject reactionPrefab;
    [SerializeField] private SnailLetter letter;
    [SerializeField] private SpriteRenderer snailSprite;


    public void Activate() => 
        snailSprite.enabled = true;

    public void Deactivate()
    {
        if(snailSprite)
            snailSprite.enabled = false;
    }
       
    
    public void Hit(float impactDelay, Vector3 collisionPoint)
    {
        Debug.Log($"{gameObject.name} was hit!");
       
        StartCoroutine(HitRoutine());

        IEnumerator HitRoutine()
        {
            yield return new WaitForSeconds(impactDelay);
            CreateReaction(collisionPoint);
        }
    }
    
    public void CreateReaction(Vector3 collisionPoint)
    {
        if (!reactionPrefab)
            Debug.LogWarning($"{nameof(ObstacleReaction)}: reactionPrefab is not assigned.", this);
        else
        {
            Instantiate(reactionPrefab, collisionPoint, Quaternion.identity); 
            gameObject.SetActive(false);
            SnailProgress.LetterOpen?.Invoke(letter);
        }
           
    }
}
