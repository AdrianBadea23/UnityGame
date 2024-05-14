using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    private bool chasing;
    private Vector3 targetPoint, startPoint;
    public float distanceToChase = 10f, distanceToLose = 15f, distanceToStop = 2f;
    public NavMeshAgent agent;
    public float keepChasingTime;
    public float chaseCounter;
    public GameObject bullet;
    public Transform firePoint;
    public float fireRate, waitBetweenShots = 2f, timeToShoot = 1f;
    private float fireCount, shotWaitCounter, shootTimeCouner;
    public Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position;
        shootTimeCouner = timeToShoot;
        shotWaitCounter = waitBetweenShots;
    }

    // Update is called once per frame
    void Update()
    {
        targetPoint = PlayerController.instance.transform.position;
        targetPoint.y = transform.position.y;
        if (!chasing)
        {
            if (Vector3.Distance(transform.position, targetPoint) < distanceToChase)
            {
                chasing = true;
                shootTimeCouner = timeToShoot;
                shotWaitCounter = waitBetweenShots;
            }

            if (chaseCounter > 0)
            {
                chaseCounter -= Time.deltaTime;
                if (chaseCounter <= 0)
                {
                    agent.destination = startPoint;
                }
            }else if (agent.remainingDistance <= agent.stoppingDistance) // Add this condition to check if the agent reached its destination.
            {
                agent.destination = startPoint; // Reset destination to start point.
            }

            if (agent.remainingDistance < .25f)
            {
                anim.SetBool("isMoving", false);
            }
            else
            {
                anim.SetBool("isMoving", true);
            }
        }
        else
        {
            //transform.LookAt(targetPoint);
            //theRB.velocity = transform.forward * moveSpeed;
            if (Vector3.Distance(transform.position, targetPoint) > distanceToStop)
            {
                agent.destination = targetPoint;    
            }
            else
            {
                agent.destination = transform.position;
            }

            
            if (Vector3.Distance(transform.position, targetPoint) > distanceToLose)
            {
                chasing = false;
                chaseCounter = keepChasingTime;
            }

            if (shotWaitCounter > 0)
            {
                shotWaitCounter -= Time.deltaTime;

                if (shotWaitCounter <= 0)
                {
                    shootTimeCouner = timeToShoot;
                }
                anim.SetBool("isMoving", true);
            }
            else
            {
                if (PlayerController.instance.gameObject.activeInHierarchy)
                {
                    shootTimeCouner -= Time.deltaTime;
            
                    if (shootTimeCouner > 0)
                    {
                        fireCount -= Time.deltaTime;
                        if (fireCount <= 0)
                        {
                            fireCount = fireRate;
                        
                            firePoint.LookAt(PlayerController.instance.transform.position + new Vector3(0,0.25f,0));
                            //check the angle to the player
                            Vector3 targetDirection = PlayerController.instance.transform.position - transform.position;
                            float angle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);

                            if (Mathf.Abs(angle) < 30f)
                            {
                                Instantiate(bullet, firePoint.position, firePoint.rotation);
                                anim.SetTrigger("fireShot");
                            }
                            else
                            {
                                shotWaitCounter = waitBetweenShots;
                            }
                        
                        
                        }

                        agent.destination = transform.position;
                    }
                    else
                    {
                        shotWaitCounter = waitBetweenShots;
                    }
                }
                
                anim.SetBool("isMoving", false);
            }
        }
    }
}
