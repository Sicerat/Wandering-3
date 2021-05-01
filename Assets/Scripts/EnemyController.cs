using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public float lookRadius = 10f;
    private Transform _target;
    private NavMeshAgent _agent;
    private Animator _animator;
    private static readonly int Chase = Animator.StringToHash("Chase");

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
            _agent.SetDestination(_target.position);
            _animator.SetBool(Chase, true);

            if (distance <= _agent.stoppingDistance)
            {
                FaceTarget();
            }
        }
        else
        {
            _agent.SetDestination(transform.position);
            _animator.SetBool(Chase, false);
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = (_target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
