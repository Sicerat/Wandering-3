using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

enum STATUS
{
    Idle = 0,
    Chase = 1,
    Patrol = 2,
    Cover = 3,
    Attack = 4
}

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 10f;
    public List<Waypoint> waypoints;
    private Transform _target;
    private NavMeshAgent _agent;
    private Animator _animator;
    private int _patrolIndex = 0;
    private ShootingSystem _shootingSystem;
    private STATUS _status = STATUS.Idle;
    private static readonly int Status = Animator.StringToHash("Status");
    private static readonly int IsShooting = Animator.StringToHash("isShooting");

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerManager.instance.player)
        {
            _target = PlayerManager.instance.player.transform;
        }

        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _shootingSystem = GetComponent<ShootingSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_target)
        {
            HandleStatus();
            _status = STATUS.Patrol;

            return;
        }

        float distance = Vector3.Distance(_target.position, transform.position);

        if (distance <= lookRadius)
        {
            _status = STATUS.Attack;

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
        _animator.SetInteger(Status, (int) _status);

        if (_status != STATUS.Attack)
        {
            _animator.SetBool(IsShooting, false);
        }

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
            case STATUS.Attack:
                Attack();
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
    }

    private void Chase()
    {
        _agent.speed = 3.5f;
        _agent.SetDestination(_target.position);
    }

    private void Patrol()
    {
        if (_patrolIndex >= waypoints.Count)
        {
            return;
        }

        Vector3 target = waypoints[_patrolIndex].transform.position;
        float distance = Vector3.Distance(target, transform.position);

        _agent.SetDestination(target);
        _agent.speed = 0.5f;

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

    private void Attack()
    {
        _agent.ResetPath();
        _animator.SetBool(IsShooting, false);

        if (_shootingSystem.weapon.CurrentAmmo <= 0)
        {
            _shootingSystem.StartReload();
            return;
        }

        if (_shootingSystem.CanShoot())
        {
            FaceTarget();
            _animator.SetBool(IsShooting, true);
        }
    }

    public void Fire()
    {
        Vector3 direction = (_target.position - transform.position).normalized;
        _shootingSystem.Shoot(transform.position, direction);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
