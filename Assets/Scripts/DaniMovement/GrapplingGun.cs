using UnityEngine;
using TMPro;

public class GrapplingGun : MonoBehaviour {

    private LineRenderer lr;
    private Vector3 grapplePoint;
    private GameObject grappleAnchor;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    public float defaultPullSpeed, stretchSpeed;
    private float currentPullSpeed;
    private float maxDistance = 100f;
    private SpringJoint joint;
    public float jointSpring = 4.5f, jointDamper = 7f, jointMassScale = 4.5f;
    private int mode = 1;
    public GameObject gunInterface;

    Vector3 shootDirection;
    public Shot shot;
    public GameObject hitDecal;
    public ParticleSystem gunTracer;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public float gunDamage = 25f;
    public float impactForce = 30f;
    public float spreading = 0.1f;
    private Vector3 shootDir;
    private RecoilGun gunRecoil;

    public int Mode
    {
        get
        {
            return mode;
        }

        set
        {
            mode = value;
            gunInterface.GetComponent<TextMeshPro>().text = value.ToString();
        }
    }

    void Awake() {
        lr = GetComponent<LineRenderer>();
        gunRecoil = GetComponentInParent<RecoilGun>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            ChooseActionLMBDown();
        }
        else if (Input.GetMouseButtonUp(0)) {
            ChooseActionLMBUp();
        }

        if (Input.GetMouseButton(1))
        {
            ChooseActionRMB();
            print("RMB pressed!");
        }
        else if (Input.GetMouseButtonUp(1))
        {
            ChooseActionRMBUp();
            print("RMB released!");
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            SwitchMode(Input.GetAxis("Mouse ScrollWheel"));
        }
    }

    //Called after Update
    void LateUpdate() {
        DrawRope();
    }


    ///Choose function based on current gun mode
    void ChooseActionLMBDown()
    {
        switch (mode)
        {
            case 0:
                return;
            case 1:
                Shoot();
                break;
            case 2:
                StartGrapple();
                break;
        }
    }

    void ChooseActionLMBUp()
    {
        switch (mode)
        {
            case 0:
                return;
            case 1:
                return;
            case 2:
                StopGrapple();
                break;
        }
    }

    void ChooseActionRMB()
    {
        switch (mode)
        {
            case 0:
                return;
            case 1:
                Shoot();
                break;
            case 2:
                PullOnGrapple();
                break;
        }
    }

    void ChooseActionRMBUp()
    {
        switch (mode)
        {
            case 0:
                return;
            case 1:
                Shoot();
                break;
            case 2:
                if(joint != null) StretchGrapple();
                break;
        }
    }

    void SwitchMode(float direction)
    {
        if (direction > 0)
        {
            if (this.Mode == 2)
            {
                this.Mode = 0;
                return;
            }
            else
            {
                this.Mode++;
                return;
            }
        }
        else
        {
            if (this.Mode == 0)
            {
                this.Mode = 2;
                return;
            }
            else
            {
                this.Mode--;
                return;
            }
        }
    }

    ///Call whenever we want to shoot
    void Shoot()
    {
        RaycastHit hit;

        float dispersionX = Random.Range(-spreading, +spreading);
        float dispersionY = Random.Range(-spreading, +spreading);
        shootDir = camera.transform.TransformDirection(Vector3.forward).normalized + new Vector3(dispersionX, dispersionY, 0);

        gunTracer.Emit(1);
        muzzleFlash.Play();
        gunRecoil.DoRecoil();

        if (Physics.Raycast(camera.transform.position,shootDir, out hit, Mathf.Infinity, whatIsGrappleable))
        {
            shootDirection = hit.point - camera.transform.position;
            //shot.Show(transform.position, hit.point);
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2);
            if (hit.transform.GetComponent<RagdollEnemy>() != null || hit.transform.GetComponentInParent<RagdollEnemy>() != null)
            {
                RagdollEnemy enemy = null;
                if (hit.transform.GetComponent<RagdollEnemy>() != null)
                {
                    enemy = hit.transform.GetComponent<RagdollEnemy>();
                }
                else
                {
                    enemy = hit.transform.GetComponentInParent<RagdollEnemy>();
                }
                enemy.ReceiveDamage(gunDamage);
            }
            else if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
                SpawnDecal(hit);
            }
            else
            {
                SpawnDecal(hit);
            }
            
        }
    }
    

    //Function for spawning decals on everything
    private void SpawnDecal(RaycastHit hitInfo)
    {
        var decal = Instantiate(hitDecal);

        decal.transform.forward = hitInfo.normal * -1f;
        decal.transform.position = hitInfo.point;
        decal.transform.parent = hitInfo.transform;
    }


    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable)) {

            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            if (hit.transform.gameObject.GetComponent<Grappable>() != null)
            {
                joint.connectedBody = hit.rigidbody;
                joint.connectedMassScale = 100;
                grappleAnchor = hit.transform.gameObject.GetComponent<Grappable>().CreateJointAnchor(hit);
                grapplePoint = grappleAnchor.transform.position;
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

            //Adjust these values to fit your game.
            joint.spring = jointSpring;
            joint.damper = jointDamper;
            joint.massScale = jointMassScale;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple() {
        lr.positionCount = 0;
        Destroy(joint);
        if (grappleAnchor != null) Destroy(grappleAnchor);
    }

    /// <summary>
    /// Pull up while grappling
    /// </summary>
    void PullOnGrapple()
    {
        if (joint == null) return;
        //print("Joint exists!");
        currentPullSpeed += 0.01f;
        //print("Default pull speed: " + defaultPullSpeed);
        //print("Current pull speed: " + currentPullSpeed);
        joint.maxDistance -= currentPullSpeed * Time.fixedDeltaTime;
        //joint.maxDistance = Mathf.Lerp(joint.maxDistance, desiredLenght, 0.5f);
        print("Current joint max distance: " + joint.maxDistance);
    }

    void StretchGrapple()
    {
        currentPullSpeed = joint.maxDistance * 0.01f * defaultPullSpeed;
    }

    private Vector3 currentGrapplePosition;
    
    void DrawRope() {
        //If not grappling, don't draw rope
        if (!joint) return;

        if(grappleAnchor != null)
        {
            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grappleAnchor.transform.position, Time.deltaTime * 8f);
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
