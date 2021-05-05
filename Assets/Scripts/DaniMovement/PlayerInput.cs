using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShootingSystem))]
public class PlayerInput : MonoBehaviour
{
    public Transform camera, player;

    public ShootingSystem shootingSystem;
    // Start is called before the first frame update
    void Start()
    {
        shootingSystem = GetComponent<ShootingSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DoActionsLMBDown();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            DoActionsLMBUp();
        }

        if (Input.GetMouseButton(1))
        {
            DoActionsRMB();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            DoActionsRMBUp();
            print("RMB released!");
        }

        if (Input.GetMouseButton(2))
        {
            DoActionsMMB();
            print("RMB pressed!");
        }
        else if (Input.GetMouseButtonUp(2))
        {
            DoActionsMMBUp();
            print("RMB released!");
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            DoActionsMMBScroll();
            //SwitchMode(Input.GetAxis("Mouse ScrollWheel"));
        }
    }


    public void DoActionsLMBDown()
    {
        if (shootingSystem.weapon.currentMode == 1) shootingSystem.Shoot(camera.transform.position, camera.forward);
        else shootingSystem.weapon.altFire.DoActionsLMBDown();
    }

    public void DoActionsLMBUp()
    {
        if (shootingSystem.weapon.currentMode == 1) return;
        else shootingSystem.weapon.altFire.DoActionsLMBUp();
    }

    public void DoActionsRMB()
    {
        if (shootingSystem.weapon.currentMode == 1) return;
        else shootingSystem.weapon.altFire.DoActionsRMB();
    }

    public void DoActionsRMBUp()
    {
        if (shootingSystem.weapon.currentMode == 1) return;
        else shootingSystem.weapon.altFire.DoActionsRMBUp();
    }

    public void DoActionsMMB()
    {
        if (shootingSystem.weapon.currentMode == 1) return;
        else shootingSystem.weapon.altFire.DoActionsMMB();
    }

    public void DoActionsMMBUp()
    {
        if (shootingSystem.weapon.currentMode == 1) return;
        else shootingSystem.weapon.altFire.DoActionsMMBUp();
    }

    public void DoActionsMMBScroll()
    {
        shootingSystem.SwitchMode(Input.GetAxis("Mouse ScrollWheel"));
    }
}
