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
    public float maxDistance = 100f;
    public float maxHookSpeed = 60f;
    private SpringJoint joint;
    public float jointSpring = 4.5f, jointDamper = 7f, jointMassScale = 4.5f;
    public GameObject gunInterface;
    [Space(10)]
    public float simplifiedGrapplePullForce = 100f;
    public float simplifiedGrappleSlapForce = 100f;
    private bool _isGrappling = false;
    private bool _isSlapping = false;

    Vector3 shootDirection;

    private Vector3 shootDir;

    private PlayerController _playerController;
    public float sphereCastRadius = 0.1f;
    [Space(10)]
    public float hookSpeed = 0.1f;
    public float distanceToGrapplePoint = 0f;
    public float zeroGravityTime = 1f;
    private bool _needsZeroGravity = false;
    private bool _hookedEnemy = false;


    void Awake() {
        lr = GetComponent<LineRenderer>();
        camera = GetComponentInParent<PlayerInput>().camera;
        player = GetComponentInParent<PlayerInput>().player;
        gunTip = GetComponentInParent<WeaponTemplate>().gunTip;
        _playerController = GetComponentInParent<PlayerController>();
    }

    void Update() {

    }

    private void FixedUpdate()
    {
        if(_playerController.hookGrappling && _isGrappling)
        {
            if (distanceToGrapplePoint > 1f)
            {
                if (grappleAnchor != null) grapplePoint = grappleAnchor.transform.position;
                if (_playerController.playerRigidbody.velocity.magnitude < maxHookSpeed || true)
                {
                    //_playerController.playerRigidbody.velocity = (grapplePoint - camera.position).normalized * hookSpeed;
                    _playerController.playerRigidbody.AddForce((grapplePoint - camera.position).normalized * hookSpeed * 1000f * Time.fixedDeltaTime);
                }
                distanceToGrapplePoint = (grapplePoint - camera.position).magnitude;
            }
            else
            {
                _needsZeroGravity = true;
                StopHookGrapple();
            }
        }
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
        if (!_playerController.simplifiedGrappling && !_playerController.hookGrappling) StartGrapple();
        else if (_playerController.simplifiedGrappling) StartSimplifiedGrapple();
        else if (_playerController.hookGrappling) DoHookGrapple();
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
        if(_playerController.hookGrappling)
        {
            StopHookGrapple();
        }
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
        //if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        if (Physics.SphereCast(camera.position, sphereCastRadius, camera.forward, out hit, maxDistance, whatIsGrappleable))
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

    void StartSimplifiedGrapple()
    {
        RaycastHit hit;
        if (Physics.SphereCast(camera.position, sphereCastRadius, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            if (hit.transform.gameObject.GetComponent<Grappable>() != null)
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForce((hit.point - transform.position).normalized * simplifiedGrappleSlapForce);
                currentGrapplePosition = gunTip.position;
                grapplePoint = hit.point;
                lr.positionCount = 2;
                _isGrappling = true;
                Invoke("StopSimplifiedGrapple", 0.25f);
            }
            else
            {
                _playerController.playerRigidbody.AddForce(Vector3.up * 1000f);
                Invoke("DoSimplifiedGrapple", 0.1f);
            }
        }
    }

    void DoSimplifiedGrapple()
    {
        RaycastHit hit;
        if (Physics.SphereCast(camera.position, sphereCastRadius, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            if (hit.transform.gameObject.GetComponent<Grappable>() != null)
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForce((hit.point - transform.position).normalized * simplifiedGrappleSlapForce);
            }
            else
            {
                _playerController.playerRigidbody.AddForce((hit.point - camera.position).normalized * simplifiedGrapplePullForce);
            }
            currentGrapplePosition = gunTip.position;
            grapplePoint = hit.point;
            lr.positionCount = 2;
            _isGrappling = true;
            Invoke("StopSimplifiedGrapple", 0.25f);
        }
    }

    void StopSimplifiedGrapple()
    {
        _isGrappling = false;
        lr.positionCount = 0;
    }

    void DoHookGrapple()
    {
        if (_isGrappling) return;

        RaycastHit hit;
        if (Physics.SphereCast(camera.position, sphereCastRadius, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            currentGrapplePosition = gunTip.position;
            grapplePoint = hit.point;
            lr.positionCount = 2;

            if (hit.transform.gameObject.GetComponent<Grappable>() != null)
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForce((hit.point - transform.position).normalized * simplifiedGrappleSlapForce);
                _isSlapping = true;
                Invoke("StopHookSlapping", 0.2f);
            }
            else
            {
                if (hit.transform.gameObject.GetComponent<EnemyController>())
                {
                    _playerController.playerRigidbody.AddForce(Vector3.up * 500f);
                    _hookedEnemy = true;
                    grappleAnchor = new GameObject();
                    grappleAnchor.transform.position = hit.point;
                    grappleAnchor.transform.SetParent(hit.transform);
                }
                distanceToGrapplePoint = (hit.point - camera.position).magnitude;
                if (_playerController.resetVelocityOnHookStart) _playerController.playerRigidbody.velocity = Vector3.zero;
                _playerController.playerRigidbody.useGravity = false;
                _isGrappling = true;
                _playerController.isGrappling = true;
            }
        }
    }

    void StopHookGrapple()
    {
        _isGrappling = false;
        _hookedEnemy = false;
        _playerController.isGrappling = false;
        if (grappleAnchor != null) Destroy(grappleAnchor);
        if(!_playerController.isGrounded && _playerController.playerRigidbody.velocity.y < -_playerController.playerMovement.maxFallSpeed)
        {
            _playerController.inFreeFlight = true;
        }
        lr.positionCount = 0;
        Invoke("ActivateGravity", zeroGravityTime);
        if(_needsZeroGravity && !_hookedEnemy)_playerController.playerRigidbody.velocity = Vector3.zero;
    }

    void StopHookSlapping()
    {
        _isSlapping = false;
        lr.positionCount = 0;
    }

    void ActivateGravity()
    {
        _playerController.playerRigidbody.useGravity = true;
        _needsZeroGravity = false;
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
        if (!joint && !_isGrappling && !_isSlapping) return;

        if (grappleAnchor != null)
        {
            if ((currentGrapplePosition - grappleAnchor.transform.position).magnitude < 1)
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
        if (!_playerController.simplifiedGrappling) return joint != null;
        else return _isGrappling;

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
