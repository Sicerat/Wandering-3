using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grappable : MonoBehaviour
{
    Rigidbody rb;
    public GameObject grappleDecal;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grappleDecal = GameObject.Find("GrappleDecal");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject CreateJointAnchor(RaycastHit grapplePoint)
    {
        var decal = Instantiate(grappleDecal, this.transform);

        decal.transform.forward = grapplePoint.normal * -1f;
        decal.transform.position = grapplePoint.point;

        return decal;
    }
}
