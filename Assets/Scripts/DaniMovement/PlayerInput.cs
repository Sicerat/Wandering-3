using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShootingSystem))]
public class PlayerInput : MonoBehaviour
{
    public Transform camera, player;

    public ShootingSystem shootingSystem;
    public Scope scope;
    public CameraController cameraController;
    public PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        shootingSystem = GetComponent<ShootingSystem>();
        scope = GetComponentInChildren<Scope>();
        cameraController = GetComponentInChildren<CameraController>();
        playerMovement = GetComponentInChildren<PlayerMovement>();
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

        if (Input.GetMouseButton(0))
        {
            DoActionsLMB();
        }

        if(Input.GetMouseButtonDown(1))
        {
            DoActionsRMBDown();
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
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            DoActionsRPressed();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            DoActionsFPressed();
        }
    }


    public void DoActionsLMBDown()
    {
        if (shootingSystem.weapon.Mode == 1 && Time.time >= shootingSystem.nextTimeToFire && shootingSystem.weapon.CurrentAmmo > 0)
        {
            shootingSystem.burstStartTime = Time.time;
            shootingSystem.Shoot(camera.transform.position, camera.forward);
        }
        else if (shootingSystem.weapon.Mode == 2) shootingSystem.weapon.altFire.DoActionsLMBDown();
        else if (shootingSystem.weapon.CurrentAmmo <= 0) shootingSystem.StartReload();
    }

    public void DoActionsLMB()
    {
        if (shootingSystem.weapon.Mode == 1 && Time.time >= shootingSystem.nextTimeToFire && shootingSystem.weapon.isAutomatic)
        {
            if (shootingSystem.weapon.CurrentAmmo > 0) shootingSystem.Shoot(camera.transform.position, camera.forward);
            else shootingSystem.StartReload();
        }
    }

    public void DoActionsLMBUp()
    {
        if (shootingSystem.weapon.Mode == 1) shootingSystem.weapon.CurrentBurstPenalty = 0f;
        else shootingSystem.weapon.altFire.DoActionsLMBUp();
    }

    public void DoActionsRMBDown()
    {
        if (shootingSystem.weapon.Mode == 1)
        {
            shootingSystem.Zoom();
            scope.DoScope();
            cameraController.SetScope(shootingSystem.weapon.zoomValue);
            playerMovement.SetScopedSpeedModifier(shootingSystem.weapon.zoomSpeedModifier);
            playerMovement.SetScopedSensModifier(shootingSystem.weapon.zoomRotateModifier);
        }
    }

    public void DoActionsRMB()
    {
        if (shootingSystem.weapon.Mode == 1) return;
        else shootingSystem.weapon.altFire.DoActionsRMB();
    }

    public void DoActionsRMBUp()
    {
        if (shootingSystem.weapon.Mode == 1)
        {
            shootingSystem.Unzoom();
            scope.DoUnscope();
            cameraController.SetScope(cameraController.normalScope);
            playerMovement.SetScopedSpeedModifier(1f);
            playerMovement.SetScopedSensModifier(1f);
        }
        else shootingSystem.weapon.altFire.DoActionsRMBUp();
    }

    public void DoActionsMMB()
    {
        if (shootingSystem.weapon.Mode == 1) return;
        else shootingSystem.weapon.altFire.DoActionsMMB();
    }

    public void DoActionsMMBUp()
    {
        if (shootingSystem.weapon.Mode == 1) return;
        else shootingSystem.weapon.altFire.DoActionsMMBUp();
    }

    public void DoActionsMMBScroll()
    {
        shootingSystem.SwitchMode(Input.GetAxis("Mouse ScrollWheel"));
    }

    public void DoActionsRPressed()
    {
        if (shootingSystem.weapon.CurrentAmmo != shootingSystem.weapon.clipSize) shootingSystem.StartReload();
    }

    public void DoActionsFPressed()
    {
        if(GetComponent<PA_Slowmotion>() != null)
        {
            GetComponent<PA_Slowmotion>().DoTempSlowmotion();
        }
    }
}

