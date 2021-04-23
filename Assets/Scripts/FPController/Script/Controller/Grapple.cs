using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour

    
{
    SpringJoint joint;
    Vector3 grapplePoint;
    Camera playerCam;
    LayerMask whatIsGround;
    // Start is called before the first frame update
    void Awake()
    {
        playerCam = GetComponentInChildren<Camera>();
        whatIsGround = LayerMask.GetMask("Default");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartGrapple();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StopGrapple();
        }
    }

    private void StopGrapple()
    {
        Destroy(joint);
    }

    private void StartGrapple()
    {
        //print("grapple");
        RaycastHit[] hits = Physics.RaycastAll(playerCam.transform.position, playerCam.transform.forward, Mathf.Infinity, whatIsGround);
        if (hits.Length < 1) return;
        grapplePoint = hits[0].point;
        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        //joint.connectedAnchor = hits[0].rigidbody;
        joint.spring = 100f;
        joint.damper = 10f;
    }
}
