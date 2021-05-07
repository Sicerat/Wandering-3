using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInstance : MonoBehaviour
{
    public GameObject weaponObj;

    public float hitDamage = 20f;
    public float fireRate = 15f;
    public float clipSize = 15f;
    public float currentAmmo = 15f;
    public float reloadTime = 2f;

    public bool isAutomatic = false;

    public float spreading = 0.01f;
    public float normalBurstDuration = 0.5f;
    public float burstPenaltySpeed = 0.001f;
    public float maxBurstPenalty = 0.01f;

    public float zoomValue = 50f;
    public float zoomSpreadingModifier = 0.5f;
    public float zoomSpeedModifier = 0.8f;
    public float zoomRotateModifier = 0.6f;
    public float impactForce = 30f;

    public GameObject altFireController;

    public ParticleSystem tracerEffect;
    public ParticleSystem muzzleFlash;
    public GameObject hitDecal;
    public GameObject impactEffect;
    public float recoilAngle = 30f;

}
