using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollEnemy : MonoBehaviour
{
    public List<Collider> RagdollParts;
    public float health = 100f;
    public float killVelocity;
    public LayerMask whatCanHitMe;

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
            print("I collided with the player with velocity of " + collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude);
            if(collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude >= killVelocity)
            {
                Die();

                print("My velocity after death: " + this.GetComponent<Rigidbody>().velocity);
                foreach (Collider c in RagdollParts)
                {
                    print(c.attachedRigidbody.velocity);
                }
                //GetComponent<Rigidbody>().AddForce(new Vector3(collision.gameObject.GetComponent<Rigidbody>().velocity.x, 0, collision.gameObject.GetComponent<Rigidbody>().velocity.z));
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
        }
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        TurnOnRagdoll();
    }
}
