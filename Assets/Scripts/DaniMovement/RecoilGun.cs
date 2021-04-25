using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilGun : MonoBehaviour
{
    public GrapplingGun grappling;

    public float recoilAngle = 30f;
    public float rotationSpeed = 5f;
    private bool inRecoil = false;
    private bool goingBack = false;

    private Quaternion defaultRot = Quaternion.Euler(0, 0, 0);
    private Quaternion desiredRot;
    private Quaternion recoilRot;

    void Update()
    {
        //defaultRot = transform.parent.rotation;

        if (inRecoil & grappling.Mode != 2)
        {

            if( grappling.transform.localRotation.eulerAngles.x - 10f <= recoilRot.eulerAngles.x & !goingBack & grappling.transform.localRotation.eulerAngles.x > 180f)
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
            grappling.transform.localRotation = Quaternion.Slerp(grappling.transform.localRotation, desiredRot, Time.deltaTime * rotationSpeed);
        }

        

        if(goingBack && grappling.transform.localRotation == defaultRot)
        {
            inRecoil = false;
            goingBack = false;
        }
    }

    public void DoRecoil()
    {
        goingBack = false;
        inRecoil = true;
        recoilRot = Quaternion.Euler(grappling.transform.localRotation.eulerAngles + new Vector3(-recoilAngle, 0));
    }
}
