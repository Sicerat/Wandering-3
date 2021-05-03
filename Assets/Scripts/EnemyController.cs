using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

enum STATUS {
    Idle = 0,
    Chase = 1,
    Patrol = 2
}

public class EnemyController : MonoBehaviour
{

    public float lookRadius = 10f;
    public List<Waypoint> waypoints;
    private Transform _target;
    private NavMeshAgent _agent;
    private Animator _animator;
    private int _patrolIndex = 0;
    private STATUS _status = STATUS.Idle;
    private static readonly int Status = Animator.StringToHash("Status");

    // Start is called before the first frame update
    void Start()
    {
        _target = PlayerManager.instance.player.transform;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent <Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(_target.position, transform.position);

        if (distance <= lookRadius)
        {
            _status = STATUS.Chase;
            
            if (distance <= _agent.stoppingDistance)
            {
                FaceTarget();
            }
        }
        else
        {
            _status = STATUS.Patrol;
        }
        
        HandleStatus();
    }

    void HandleStatus()
    {
        switch (_status)
        {
            case STATUS.Idle:
                Idle();
                break;
            case STATUS.Chase:
                Chase();
                break;
            case STATUS.Patrol:
                Patrol();
                break;
            default:
                _status = STATUS.Idle;
                break;
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = (_target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void Idle()
    {
            _agent.SetDestination(transform.position);
            _animator.SetInteger(Status, (int)STATUS.Idle);
    }

    private void Chase()
    {
            _agent.speed = 3.5f;
            _agent.SetDestination(_target.position);
            _animator.SetInteger(Status, (int)STATUS.Chase);
    }

    private void Patrol()
    {
        Vector3 target = waypoints[_patrolIndex].transform.position;
        float distance = Vector3.Distance(target, transform.position);
        
        _agent.SetDestination(target);
        _agent.speed = 0.5f;
        _animator.SetInteger(Status, (int)STATUS.Patrol);

        if (distance < 10)
        {
            if (_patrolIndex < waypoints.Count - 1)
            {
                _patrolIndex++;
            }
            else
            {
                _patrolIndex = 0;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
