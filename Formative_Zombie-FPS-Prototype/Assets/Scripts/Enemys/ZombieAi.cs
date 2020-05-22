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
    //private AudioSource audioSource;
    
    public GameObject Target;// test
    public float Damage = 100.0f;// test

    //for random walking
    [SerializeField] private float width = 5;// for a square around the enemy

    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool gotHit;
    [SerializeField] private bool walks;
    [SerializeField] private float maxSeeDistance = 20f;

    [SerializeField] private bool isInLateUpdate;
    [SerializeField] private bool haveToUpdate = true;
    [SerializeField] private bool shouldChase;
    private NavMeshAgent agent;
    //private EnemyUnit agent; TODO It doesnt work with astar yet, so I need to fix it for the summative assignment
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip diengSound;
    
    [SerializeField] private float normalSpeed = 4f;
    [SerializeField] private float runSpeed = 8f;
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
        //audioSource = GetComponent<AudioSource>();

        agent.speed = normalSpeed;
    }

    IEnumerator distUpdateCo = null;
    
    private void Update()
    {
        float distanceToPlayer = GetActualDistanceFromTarget();

        if (!isAttacking)
        {
            if (distanceToPlayer >= maxSeeDistance) // if the distanceToPlayer is too hight, the zombie will walk around slowly
            {
                shouldChase = false;
                animator.SetBool("Run", false);
                agent.speed = normalSpeed;
            }
            else
            {
                shouldChase = true;
                animator.SetBool("Run", true);
                agent.speed = runSpeed;
            }

            if (distanceToPlayer <= 2.0f)
            {
                Attack();
            }
            else if (shouldChase) // if zombie should chase, it follows the player and tries to attack him
            {
                agent.destination = Target.transform.position;
            }
            else if (!shouldChase && distanceToPlayer > 2.0f)
            {
                agent.destination = NotChaseTarget(); // random target based on the current pos from the zombie
            }
        }
    }

    private void Attack()
    {
        // Calculate actual distance from target
        float distanceFromTarget = GetActualDistanceFromTarget();
		
        // Calculate direction is toward player
        Vector3 direction = Target.transform.position - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);

        if(!isAttacking && distanceFromTarget <= 5.0f && angle <= 60f) 
        {
            isAttacking = true;
            shouldChase = false;

            agent.speed = 0;

            animator.SetBool("Run", false);
            //audioSource.PlayOneShot(attackSound);
            animator.SetTrigger("Attack");
            new WaitForSeconds(0.5f);
            
            HealthManager targetHealthManager = Target.GetComponent<HealthManager>();
            if(targetHealthManager)
            {
                targetHealthManager.ApplyDamage(Damage);
            }

            StartCoroutine(ResetAttacking());
        }
    }

    private Vector3 NotChaseTarget()// randomly get the next target if is not attacking the player
    { 
        float rx = Random.Range(-1, 1);
        float rz = Random.Range(-1, 1);
    
        Vector3 nDestination = new Vector3(transform.position.x + (rx * width), 0, transform.position.z + (rz * width));

        return nDestination;
    }

    IEnumerator LateDistanceUpdate(float duration)
    {
        isInLateUpdate = true;
        agent.destination = Target.transform.position;
        yield return new WaitForSeconds(duration);
		
        isInLateUpdate = false;
        distUpdateCo = null;
        yield break;
    }
    
    float GetActualDistanceFromTarget()
    {
        return GetDistanceFrom(Target.transform.position, this.transform.position);
    }

    float GetDistanceFrom(Vector3 src, Vector3 dist)
    {
        return Vector3.Distance(src, dist);
    }
    
    IEnumerator ResetAttacking()
    {
        yield return new WaitForSeconds(2.4f);

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
