using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class EnemyScript : MonoBehaviour
{
    bool isCharmed = false;
    [SerializeField] GameObject correctTune;
    float nextFire = 1.0f;
    float fireRate = 0.5f;
    
    [SerializeField] GameObject rubberDucky;
    [SerializeField] private Material angryDucky;
    [SerializeField] private Material charmedDucky;
    private Renderer rubberDuckyRenderer;

    public enum AISTATE { PATROL = 0, CHASE = 1, ATTACK = 2, CHARMED = 3 };
    private NavMeshAgent thisAgent = null;
    private Transform player = null;
    private float range = 2.0f;
    public AISTATE currentState = AISTATE.PATROL;
    

    // Start is called before the first frame update
    void Start()
    {
        thisAgent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").GetComponent<Transform>();
        rubberDuckyRenderer = rubberDucky.GetComponent<MeshRenderer>();
        rubberDuckyRenderer.material = angryDucky;

        ChangeState(AISTATE.PATROL);
    }

    public void ChangeState(AISTATE newState)
    {
        StopCoroutine(currentState.ToString());
        currentState = newState;
        
        switch (currentState)
        {
            case AISTATE.PATROL:
                StartCoroutine(StatePatrol());
                break;
            case AISTATE.CHASE:
                StartCoroutine(StateChase());
                break;
            case AISTATE.ATTACK:
                StartCoroutine(StateAttack());
                break;
            case AISTATE.CHARMED:
                StartCoroutine(StateCharmed());
                break;
        }
    }

    public IEnumerator StatePatrol()
    {
        GameObject[] waypoints = GameObject.FindGameObjectsWithTag("waypoint");
        GameObject currentWaypoint = waypoints[Random.Range(0, waypoints.Length)];
        float targetDistance = 2f;

        while(currentState == AISTATE.PATROL)
        {
            thisAgent.SetDestination(currentWaypoint.transform.position);

            if (Vector3.Distance(transform.position, currentWaypoint.transform.position) < targetDistance)
            {
                currentWaypoint = waypoints[Random.Range(0, waypoints.Length)];
            }
            if (Vector3.Distance(transform.position, player.transform.position) < 4.0f)
            {
                ChangeState(AISTATE.CHASE);

            }
            if (isCharmed)
            {
                ChangeState(AISTATE.CHARMED);
            }
            yield return null;
        }
    }
    public IEnumerator StateChase()
    {
        while(currentState == AISTATE.CHASE)
        {
            thisAgent.SetDestination(player.transform.position);
            if(Vector3.Distance(transform.position, player.transform.position) < range)
            {
                ChangeState(AISTATE.ATTACK);
                yield break;
            }
            if (Vector3.Distance(transform.position, player.transform.position) > 4.0f)
            {
                ChangeState(AISTATE.PATROL);
                yield break;
            }
            if (isCharmed)
            {
                ChangeState(AISTATE.CHARMED);
            }
            yield return null;
        }
    }
    public IEnumerator StateAttack()
    {
        float targetDistance = 2f;
        while (currentState == AISTATE.ATTACK)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > targetDistance)
            {
                thisAgent.SetDestination(player.transform.position);

            }

            if(isCharmed)
            {
                ChangeState(AISTATE.CHARMED);
                yield break;
            }
            if (Vector3.Distance(transform.position, player.transform.position) > range)
            {
                ChangeState(AISTATE.CHASE);
                yield break;
            }
            yield return null;
        }
    }

    public IEnumerator StateCharmed()
    {
        rubberDuckyRenderer.material = charmedDucky;
        GameObject[] waypoints = GameObject.FindGameObjectsWithTag("waypoint");
        GameObject currentWaypoint = waypoints[Random.Range(0, waypoints.Length)];
        float targetDistance = 2f;

        while (currentState == AISTATE.CHARMED)
        {
            thisAgent.SetDestination(currentWaypoint.transform.position);

            if (Vector3.Distance(transform.position, currentWaypoint.transform.position) < targetDistance)
            {
                currentWaypoint = waypoints[Random.Range(0, waypoints.Length)];
            }
            if(!isCharmed)
            {
                rubberDuckyRenderer.material = angryDucky;
                ChangeState(AISTATE.PATROL);
            }
            yield return null;
        }

    }

    void Update()
    {
        if(currentState == AISTATE.ATTACK)
        {
            Attack();
        }

        if(correctTune.GetComponent<AudioSource>().isPlaying)
        {
            isCharmed = true;
        }
        if(correctTune.GetComponent<AudioSource>().isPlaying == false)
        {
            isCharmed = false;
        }
    }

    void Attack()
    {
        if(Time.time > nextFire && Vector3.Distance(transform.position, player.position) < range)
        {
            nextFire = Time.time + fireRate;

            player.SendMessage("Shot", 2.0f);
        }
    }
}
