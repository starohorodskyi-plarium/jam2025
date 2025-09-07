using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

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

    private Transform point1;
    private Transform point2;
    private Transform target;
    
    private float moveSpeed = 0.5f;
    private float reachThreshold = 0.1f; // how close is "arrived"
    
    private SpriteRenderer spriteRenderer;
    
    [Header("Character Settings")]
    [SerializeField] private Faction faction = Faction.Enemy;
    [SerializeField] private Gender gender = Gender.Male;
    [SerializeField] private GameObject allyHitEffect;
    [SerializeField] private GameObject enemyHitEffect;
    [SerializeField] private GameObject soundPlayerPrefab;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    private void Update()
    {
        Move();
    }

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
    
    public void SetWaypoints(Transform p1, Transform p2)
    {
        point1 = p1;
        point2 = p2;

        // pick 0 or 1
        int random = Random.Range(0, 2);

        if (random == 0)
        {
            transform.position = point1.position;
            target = point2;
        }
        else
        {
            transform.position = point2.position;
            target = point1;
        }

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Move()
    {
        if (point1 == null || point2 == null || target == null)
            return;

        // Move
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        // Flip sprite based on movement direction (x axis)
        Vector3 dir = target.position - transform.position;
        if (spriteRenderer != null)
        {
            if (dir.x > 0.01f)
                spriteRenderer.flipX = false; // facing right
            else if (dir.x < -0.01f)
                spriteRenderer.flipX = true;  // facing left
        }

        // Switch target if reached
        if (Vector3.Distance(transform.position, target.position) < reachThreshold)
        {
            target = target == point1 ? point2 : point1;
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
