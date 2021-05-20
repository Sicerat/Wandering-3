using UnityEngine;
using TMPro;

public class GrapplingGun : altFireBase {

    private LineRenderer lr;
    private Vector3 grapplePoint;
    private GameObject grappleAnchor;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    public float defaultPullSpeed, defaultStretchSpeed;
    public float pullAcceleration = 0.01f;
    public float stretchAcceleration = 0.01f;
    private float currentPullSpeed, currentStretchSpeed;
    private float maxDistance = 100f;
    private SpringJoint joint;
    public float jointSpring = 4.5f, jointDamper = 7f, jointMassScale = 4.5f;
    public GameObject gunInterface;

    Vector3 shootDirection;

    private Vector3 shootDir;


    void Awake() {
        lr = GetComponent<LineRenderer>();
        camera = GetComponentInParent<PlayerInput>().camera;
        player = GetComponentInParent<PlayerInput>().player;
        gunTip = GetComponentInParent<WeaponTemplate>().gunTip;
    }

    void Update() {

    }

    //Called after Update
    void LateUpdate() {
        DrawRope();
    }

    public override void DoActionsLMB()
    {
        throw new System.NotImplementedException();
    }


    public override void DoActionsLMBDown() {
        StartGrapple();
    }

    public override void DoActionsLMBUp()
    {
        lr.positionCount = 0;
        Destroy(joint);
        if (grappleAnchor != null) Destroy(grappleAnchor);
    }

    public override void DoActionsMMB()
    {
        StretchGrapple();
    }

    public override void DoActionsMMBUp()
    {
        if (joint != null) currentStretchSpeed = defaultStretchSpeed;
    }

    public override void DoActionsRMB()
    {
        PullOnGrapple();
    }

    public override void DoActionsRMBUp()
    {
        if (joint != null) currentPullSpeed = defaultPullSpeed;
    }
    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void DoActionLMBUp() {
        lr.positionCount = 0;
        Destroy(joint);
        if (grappleAnchor != null) Destroy(grappleAnchor);
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {

            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;

            if (hit.transform.gameObject.GetComponent<Grappable>() != null)
            {
                joint.connectedBody = hit.rigidbody;
                joint.connectedMassScale = 100;
                grappleAnchor = hit.transform.gameObject.GetComponent<Grappable>().CreateJointAnchor(hit);
                grapplePoint = grappleAnchor.transform.localPosition;
            }
            else
            {
                grapplePoint = hit.point;
            }
            joint.connectedAnchor = grapplePoint;


            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = 0.25f;

            //Set desiredLenght for pulling function to "default value" of start max joint distance
            //Set pull speed to default pull speed
            print("Previous pull speed: " + currentPullSpeed);
            currentPullSpeed = joint.maxDistance * 0.01f * defaultPullSpeed;
            print("Set pulling parameters to default: " + currentPullSpeed);
            print("Start max distance: " + joint.maxDistance);
            print("Start min distance: " + joint.minDistance);
            print("Start connected mass scale: " + joint.connectedMassScale);


            //Adjust these values to fit your game.
            joint.spring = jointSpring;
            joint.damper = jointDamper;
            joint.massScale = jointMassScale;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }

    /// <summary>
    /// Pull up while grappling
    /// </summary>
    void PullOnGrapple()
    {
        if (joint == null) return;

        currentPullSpeed += pullAcceleration;

        joint.spring += currentPullSpeed * Time.fixedDeltaTime;
        joint.maxDistance -= currentPullSpeed * Time.fixedDeltaTime;
        print("Current joint max distance: " + joint.maxDistance);
    }

    void StretchGrapple()
    {
        if (joint == null) return;

        currentStretchSpeed += stretchAcceleration;

        joint.maxDistance += currentStretchSpeed * Time.fixedDeltaTime;
    }

    private Vector3 currentGrapplePosition;
    
    void DrawRope() {
        //If not grappling, don't draw rope
        if (!joint) return;

        if(grappleAnchor != null)
        {
            if ( (currentGrapplePosition-grappleAnchor.transform.position).magnitude < 1)
            {
                //currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grappleAnchor.transform.position, Time.deltaTime * 8f);
                currentGrapplePosition = grappleAnchor.transform.position;
            }
            else
            {
                //float u = Vector3.Angle(grappleAnchor.transform.position - currentGrapplePosition, grappleAnchor.transform.position - gunTip.position);
                //float hyp1 = (grappleAnchor.transform.position - currentGrapplePosition).magnitude * Mathf.Sin(u * Mathf.Deg2Rad);

                //float yComp = hyp1 * Mathf.Cos(u * Mathf.Deg2Rad);
                //float xComp = hyp1 * Mathf.Sin(u * Mathf.Deg2Rad);

                

                //Vector3 movementCorrection = new Vector3(yComp, 0, xComp);
                currentGrapplePosition = currentGrapplePosition + (grappleAnchor.transform.position - currentGrapplePosition).normalized * 0.8f;
                //+ (grappleAnchor.transform.position - currentGrapplePosition).normalized * 0.1f
            }

        }
        else
        {
            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        }
        
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        if (grappleAnchor != null)
        {
            return grappleAnchor.transform.position;
        }
        else
        {
            return grapplePoint;
        }
        
    }

}
