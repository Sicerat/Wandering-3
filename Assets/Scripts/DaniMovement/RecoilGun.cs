using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilGun : MonoBehaviour
{
    public GrapplingGun grappling;

    public float rotationSpeed = 5f;
    private bool inRecoil = false;
    private bool goingBack = false;

    private Quaternion defaultRot = Quaternion.Euler(0, 0, 0);
    private Quaternion desiredRot;
    private Quaternion recoilRot;

    private void Awake()
    {
        defaultRot = transform.localRotation;
    }

    void Update()
    {
        //defaultRot = transform.parent.rotation;

        if (inRecoil)
        {

            if(transform.localRotation.eulerAngles.x - 10f <= recoilRot.eulerAngles.x & !goingBack & transform.localRotation.eulerAngles.x > 180f)
            {
                goingBack = true;
            }

            if(!goingBack)
            {
                desiredRot = recoilRot;
            }
            else
            {
                desiredRot = defaultRot;
            }

            //print("default Rot: " + defaultRot.eulerAngles + " | current Rot: " + transform.localRotation.eulerAngles + " | desired Rot: " + desiredRot.eulerAngles);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, desiredRot, Time.deltaTime * rotationSpeed);
        }

        

        if(goingBack && transform.localRotation == defaultRot)
        {
            inRecoil = false;
            goingBack = false;
        }
    }

    public void DoRecoil(float recoilAngle)
    {
        goingBack = false;
        inRecoil = true;
        recoilRot = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(-recoilAngle, 0, 0));
    }
}
