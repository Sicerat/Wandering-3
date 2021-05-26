using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RagdollEnemy : MonoBehaviour
{
    public List<Collider> RagdollParts;
    public float health = 100f;
    public float killVelocity;
    public LayerMask whatCanHitMe;
    public float deathHitForce = 10000f;
    public float hitOverlapSphereRadius = 1f;
    public LayerMask hitOverlapSphereMask;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        SetRagdollParts();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((whatCanHitMe.value & (1 << collision.gameObject.layer)) != 0)
        {
            if(collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude >= killVelocity)
            {
                Die();
                Collider[] hitColliders = Physics.OverlapSphere(collision.GetContact(0).point, hitOverlapSphereRadius, hitOverlapSphereMask);
                foreach (Collider c in hitColliders)
                {
                    if (c.attachedRigidbody != null)
                    {
                        c.attachedRigidbody.AddForce(new Vector3(collision.gameObject.GetComponent<Rigidbody>().velocity.x, 0, collision.gameObject.GetComponent<Rigidbody>().velocity.z).normalized * deathHitForce / hitColliders.Length);
                    }
                }
            }
        }
    }

    private void SetRagdollParts()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();

        foreach(Collider c in colliders)
        {
            if (c.gameObject != this.gameObject)
            {
                c.isTrigger = true;
                c.attachedRigidbody.isKinematic = true;
                RagdollParts.Add(c);
            }
        }
    }

    public void TurnOnRagdoll()
    {
        rb.useGravity = false;
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        this.gameObject.GetComponent<Animator>().enabled = false;

        foreach(Collider c in RagdollParts)
        {
            c.isTrigger = false;
            c.attachedRigidbody.isKinematic = false;
            c.attachedRigidbody.velocity = Vector3.zero;
        }
    }

    public void TurnOffRagdoll()
    {
        rb.useGravity = true;
        this.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        this.gameObject.GetComponent<Animator>().enabled = true;

        foreach (Collider c in RagdollParts)
        {
            c.isTrigger = true;
            c.attachedRigidbody.isKinematic = true;
        }
    }

    public void ReceiveDamage(float damage, GameObject partOfBody, Vector3 hitDirection)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
            partOfBody.GetComponent<Rigidbody>().AddForce(hitDirection.normalized * deathHitForce);
        }
    }

    private void Die()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        EnemyController controller = GetComponent<EnemyController>();
        controller.enabled = false;
        agent.enabled = false;
        TurnOnRagdoll();
    }
}
