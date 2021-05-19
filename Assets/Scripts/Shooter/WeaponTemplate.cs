using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShootingSystem))]
public class WeaponTemplate : MonoBehaviour
{
    public GameObject weaponObj;
    public GameObject instantiatedWeapon;
    public Transform weaponPos;
    public LayerMask hittableLayers;

    public float hitDamage = 20f;
    public float fireRate = 15f;
    public float clipSize = 15f;
    public float currentAmmo = 15f;
    public float reloadTime = 2f;
    public bool isZoomed = false;
    public bool isAutomatic = false;
    public float spreading = 0.01f;
    public float normalBurstDuration = 0.5f;
    public float burstPenaltySpeed = 0.001f;
    public float maxBurstPenalty = 0.01f;
    public float currentBurstPenalty = 0f;

    public float zoomValue = 50f;
    public float zoomSpreadingModifier = 0.5f;
    public float zoomSpeedModifier = 0.8f;
    public float zoomRotateModifier = 0.6f;
    public float impactForce = 30f;

    public int currentMode = 1;
    public GameObject altFireController;
    public GameObject instantiatedAltFireController;
    public altFireBase altFire;

    public Transform gunTip;
    public Transform gunEffectsHolder;
    public GunInterface gunInterface;

    public ParticleSystem tracerEffect;
    public ParticleSystem instantiatedTracer;
    public ParticleSystem muzzleFlash;
    public ParticleSystem instantiatedMuzzleFlash;
    public GameObject hitDecal;
    public GameObject impactEffect;
    public float recoilAngle = 30f;

    private bool _isReloading = false;

    private void Awake()
    {
        instantiatedWeapon = Instantiate(weaponObj, weaponPos);
        gunTip = instantiatedWeapon.GetComponentInChildren<GunTip>().gameObject.transform;
        gunEffectsHolder = GetComponentInChildren<GunEffectsHolder>().gameObject.transform;
        gunInterface = GetComponentInChildren<GunInterface>();
        gunInterface.SetSubText(currentAmmo.ToString());

        instantiatedTracer = Instantiate(tracerEffect, gunEffectsHolder);
        instantiatedMuzzleFlash = Instantiate(muzzleFlash, gunEffectsHolder);

        if (altFireController != null)
        {
            instantiatedAltFireController = Instantiate(altFireController, weaponPos);
            altFire = instantiatedAltFireController.GetComponent<altFireController>().altFireScript as altFireBase;
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int Mode
    {
        get
        {
            return currentMode;
        }

        set
        {
            currentMode = value;
            gunInterface.SetMainText(value.ToString());
        }
    }

    public float CurrentAmmo
    {
        get
        {
            return currentAmmo;
        }

        set
        {
            currentAmmo = value;
            gunInterface.SetSubText(value.ToString());
        }
    }

    public bool IsReloading
    {
        get
        {
            return _isReloading;
        }
        set
        {
            _isReloading = value;
            if (value) gunInterface.SetSubText("R");
        }
    }

    public float Spreading
    {
        get
        {
            if (isZoomed) return spreading * zoomSpreadingModifier;
            else return spreading;
        }
    }

    public void InitializeNewWeapon(WeaponInstance weapon)
    {
        weaponObj = weapon.weaponObj;

        hitDamage = weapon.hitDamage;
        fireRate = weapon.fireRate;
        clipSize = weapon.clipSize;
        currentAmmo = weapon.currentAmmo;
        reloadTime = weapon.reloadTime;
        isAutomatic = weapon.isAutomatic;

        _isReloading = false;
        isZoomed = false;

        spreading = weapon.spreading;
        normalBurstDuration = weapon.normalBurstDuration;
        burstPenaltySpeed = weapon.burstPenaltySpeed;
        maxBurstPenalty = weapon.maxBurstPenalty;
        currentBurstPenalty = 0f;

        zoomRotateModifier = weapon.zoomRotateModifier;
        zoomSpeedModifier = weapon.zoomSpeedModifier;
        zoomSpreadingModifier = weapon.zoomSpreadingModifier;
        zoomValue = weapon.zoomValue;
        impactForce = weapon.impactForce;

        currentMode = 1;
        altFireController = weapon.altFireController;

        tracerEffect = weapon.tracerEffect;
        muzzleFlash = weapon.muzzleFlash;
        impactEffect = weapon.impactEffect;
        hitDecal = weapon.hitDecal;
        recoilAngle = weapon.recoilAngle;

        Destroy(instantiatedWeapon);
        Destroy(instantiatedAltFireController);

        instantiatedWeapon = Instantiate(weaponObj, weaponPos);
        gunTip = instantiatedWeapon.GetComponentInChildren<GunTip>().gameObject.transform;
        gunEffectsHolder = GetComponentInChildren<GunEffectsHolder>().gameObject.transform;
        gunInterface = instantiatedWeapon.GetComponentInChildren<GunInterface>();
        gunInterface.SetSubText(currentAmmo.ToString());

        instantiatedTracer = Instantiate(tracerEffect, gunEffectsHolder);
        instantiatedMuzzleFlash = Instantiate(muzzleFlash, gunEffectsHolder);

        if (altFireController != null)
        {
            instantiatedAltFireController = Instantiate(altFireController, weaponPos);
            altFire = instantiatedAltFireController.GetComponent<altFireController>().altFireScript as altFireBase;
        }

        GetComponent<ShootingSystem>().gunRecoil = instantiatedWeapon.GetComponentInChildren<RecoilGun>(); 
    }
}
