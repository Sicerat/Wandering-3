using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PA_Slowmotion : MonoBehaviour
{
    TimeManager timeManager;
    PlayerMovement playerMovement;
    ShootingSystem shootingSystem;
    public float slowmotionFactor = 0.05f;
    public float slowmotionDuration = 5f;


    private void Start()
    {
        timeManager = TimeManager.instance;
        playerMovement = GetComponentInChildren<PlayerMovement>();
        shootingSystem = GetComponent<ShootingSystem>();
    }

    public void DoTempSlowmotion()
    {

        timeManager.SetTimeScale(slowmotionFactor);
        playerMovement.ScaleSensitivity(1 / slowmotionFactor);
        Invoke("StopTempSlowdown", slowmotionDuration * slowmotionFactor);
    }

    public void StopTempSlowdown()
    {
        timeManager.SetTimeScale(1f);
        playerMovement.ScaleSensitivity(slowmotionFactor);
    }
}
