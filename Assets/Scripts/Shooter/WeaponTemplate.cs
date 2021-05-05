using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTemplate : MonoBehaviour
{
    public GameObject weaponObj;
    public GameObject instantiatedWeapon;
    public Transform weaponPos;
    public LayerMask hittableLayers;

    public float hitDamage = 20f;
    public float spreading = 0.01f;
    public float impactForce = 30f;

    public int currentMode = 1;
    public GameObject altFireController;
    public GameObject instantiatedAltFireController;
    public altFireBase altFire;
    public Transform gunTip;

    public ParticleSystem tracerEffect;
    public ParticleSystem instantiatedTracer;
    public ParticleSystem muzzleFlash;
    public ParticleSystem instantiatedMuzzleFlash;
    public GameObject hitDecal;
    public GameObject impactEffect;
    public float recoilAngle = 30f;


    private void Awake()
    {
        instantiatedWeapon = Instantiate(weaponObj, weaponPos);
        gunTip = instantiatedWeapon.GetComponentInChildren<GunTip>().gameObject.transform;
        instantiatedTracer = Instantiate(tracerEffect, gunTip);
        instantiatedMuzzleFlash = Instantiate(muzzleFlash, gunTip);

        instantiatedAltFireController = Instantiate(altFireController, weaponPos);
        altFire = instantiatedAltFireController.GetComponent<altFireController>().altFireScript as altFireBase;

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}