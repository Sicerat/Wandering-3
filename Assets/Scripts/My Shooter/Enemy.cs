using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    bool dead = false;
    BoxCollider boxCollider;
    ParticleSystem hitEffect;
    Rigidbody body;
    public GameObject textRandomizer;
    

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        hitEffect = GetComponentInChildren<ParticleSystem>();
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
            return;
    }

    public void Hit(RaycastHit hitPoint, Vector3 shootDirection)
    {
        if (!dead)
        {
            body.AddForceAtPosition(Vector3.Normalize(shootDirection)*100, hitPoint.point);

            textRandomizer.GetComponent<test_RandomPhrases>().GetNewText();
            hitEffect.transform.forward = hitPoint.normal;
            hitEffect.transform.position = hitPoint.point;
            hitEffect.Play();
        }
    }
}
