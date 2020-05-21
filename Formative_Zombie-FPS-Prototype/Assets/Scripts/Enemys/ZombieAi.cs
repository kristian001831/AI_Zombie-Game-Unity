using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ZombieAi : MonoBehaviour
{
    private Animator animator;
    private HealthManager healthManager;
    private AudioSource audioSource;
    
    [SerializeField] private GameObject target;
    
    [SerializeField] private float damage = 100.0f;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool runs;
    [SerializeField] private bool gotHit;
    [SerializeField] private bool walks;

    [SerializeField] private bool isInLateUpdate;
    [SerializeField] private bool haveToUpdate = true;
    [SerializeField] private bool shouldChase;
    private NavMeshAgent agent;
    //private EnemyUnit agent; TODO It doesnt work with astar yet, so I need to fix it for the summative assignment
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip diengSound;
    
    [SerializeField] private float normalSpeed = 3;
    [SerializeField] private float runSpeed = 5;
    //[SerializeField] private float currentSpeed;
    [SerializeField] private GameObject noChaseObj;
    [SerializeField] private Vector3 notChaseTarget;
    private float distance;
    
    private Random rnd = new Random();// needed for random enemy notChase target

    void Start () 
    {
        animator = GetComponent<Animator>();
        healthManager = GetComponent<HealthManager>();
        agent = GetComponent<NavMeshAgent>();
        //agent = GetComponent<EnemyUnit>();
        audioSource = GetComponent<AudioSource>();

        agent.speed = normalSpeed;
    }

    IEnumerator distUpdateCo = null;
    
    private void Update()
    {
        agent.speed = normalSpeed;
        animator.SetBool("Run", false);
        
        float distanceToPlayer = GetActualDistanceFromTarget();
        Debug.Log(distanceToPlayer);// Debugs the current distance from the zombies
        

        if (distanceToPlayer >= 20f)// if the distanceToPlayer is too hight, the zombie will walk around slowly
        {
            shouldChase = false;
            
            
        }
        
        if (shouldChase)// if zombie should chase, it follows the player and tries to attack him
        {
            agent.destination = target.transform.position; 
            runs = true;
        }
        else if (!shouldChase)
        {
            agent.destination = NotChaseTarget();// random target based on the current pos from the zombie
            runs = false;
        }
    }

    private void Attack()
    {
        // Calculate actual distance from target
        float distanceFromTarget = GetActualDistanceFromTarget();
		
        // Calculate direction is toward player
        Vector3 direction = target.transform.position - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);

        if(!isAttacking && distanceFromTarget <= 2.0f && angle <= 60f) {
            isAttacking = true;
            shouldChase = false;

            agent.speed = 0;

            audioSource.PlayOneShot(attackSound);
            animator.SetTrigger("Attack");

            HealthManager targetHealthManager = target.GetComponent<HealthManager>();

            if(targetHealthManager) {
                targetHealthManager.ApplyDamage(damage);
            }

            StartCoroutine(ResetAttacking());
        }
    }

    private Vector3 NotChaseTarget()// randomly get the next target if is not attacking the player
    {
        Vector3 old = this.transform.position;
        noChaseObj.transform.position = old;
        
        notChaseTarget.x = Random.Range(old.x - 4, old.x + 4);
        notChaseTarget.y = old.y;
        notChaseTarget.z = Random.Range(old.z - 4, old.z + 4);
            
        notChaseTarget = new Vector3(notChaseTarget.x, notChaseTarget.y, notChaseTarget.z);
        return notChaseTarget;
    }

    IEnumerator LateDistanceUpdate(float duration)
    {
        isInLateUpdate = true;
        agent.destination = target.transform.position;
        yield return new WaitForSeconds(duration);
		
        isInLateUpdate = false;
        distUpdateCo = null;
        yield break;
    }
    
    float GetActualDistanceFromTarget()
    {
        return GetDistanceFrom(target.transform.position, this.transform.position);
    }

    float GetDistanceFrom(Vector3 src, Vector3 dist)
    {
        return Vector3.Distance(src, dist);
    }
    
    IEnumerator ResetAttacking()
    {
        yield return new WaitForSeconds(1.4f);

        isAttacking = false;
        shouldChase = true;

        if(!healthManager.IsDead) 
        {
            agent.speed = normalSpeed;
        }
        yield break;
    }

    IEnumerator RemoveGameObject() 
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
