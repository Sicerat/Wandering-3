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
            //print(transform.localRotation.eulerAngles.x + " " + recoilRot.eulerAngles.x);

            if( transform.localRotation.eulerAngles.x - 10f <= recoilRot.eulerAngles.x & !goingBack & transform.localRotation.eulerAngles.x > 180f)
            {
                //print(1);
                goingBack = true;
            }

            if(!goingBack)
            {
                print(2);
                desiredRot = recoilRot;
                //print("Desired Rot: " + desiredRot.eulerAngles);
            }
            else
            {
                print(3);
                desiredRot = defaultRot;
            }

            print("default Rot: " + defaultRot.eulerAngles + " | current Rot: " + transform.localRotation.eulerAngles + " | desired Rot: " + desiredRot.eulerAngles);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, desiredRot, Time.deltaTime * rotationSpeed);
        }

        

        if(goingBack && transform.localRotation == defaultRot)
        {
            inRecoil = false;
            goingBack = false;
        }
    }

    public void DoRecoil()
    {
        goingBack = false;
        inRecoil = true;
        recoilRot = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(-recoilAngle, 0));
    }
}
