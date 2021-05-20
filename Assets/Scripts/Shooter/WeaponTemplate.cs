using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShootingSystem))]
//[ExecuteInEditMode]
[DisallowMultipleComponent]
public class WeaponTemplate : MonoBehaviour
{
    [Header("Base settings (REQUIRED!)")]
    [Tooltip("Already existing weapon prefab")]
    public GameObject weaponObj;
    [HideInInspector] public GameObject instantiatedWeapon;
    [Tooltip("Gun position where to snap a weapon")]
    public Transform weaponPos;
    [Tooltip("Layers this weapon can hit")]
    public LayerMask hittableLayers;

    [Header("Weapon stats")]
    [Tooltip("Damage done by weapon per hit")]
    public float hitDamage = 20f;
    [Tooltip("Bullets per second")]
    public float fireRate = 15f;
    public float clipSize = 15f;
    public float spreading = 0.01f;
    [Tooltip("Reloading time in seconds")]
    public float reloadTime = 2f;
    [Tooltip("How much force is applied to the hitted object")]
    public float impactForce = 30f;

    public bool isAutomatic = false;
    [Tooltip("How long in seconds burst should be")]
    public float normalBurstDuration = 0.5f;
    [Tooltip("How fast will spreading increase after exceeding normalBurstDuration")]
    public float burstPenaltySpeed = 0.001f;
    [Tooltip("Max spreading penalty in case of exceeding normalBurstDuration")]
    public float maxBurstPenalty = 0.01f;

    private float _currentAmmo;

    private bool _isZoomed;
    public bool IsZoomed { get; set; } = false;

    private float _currentBurstPenalty;
    public float CurrentBurstPenalty { get; set; } = 0f;

    [Header("Zoom Settings")]
    [Tooltip("How much it zooms in %")]
    public float zoomValue = 50f;
    [Tooltip("Spreading is multiplied by this value while zooming")]
    public float zoomSpreadingModifier = 0.5f;
    [Tooltip("Move speed is multiplied by this value while zooming")]
    public float zoomSpeedModifier = 0.8f;
    [Tooltip("Rotate speed is multiplied by this value while zooming")]
    public float zoomRotateModifier = 0.6f;

    private int _currentMode = 1;

    [HideInInspector] public Transform gunTip;
    private Transform _gunEffectsHolder;
    private GunInterface _gunInterface;

    [Header("Visual Effects")]
    public ParticleSystem tracerEffect;
    [HideInInspector] public ParticleSystem instantiatedTracer;
    public ParticleSystem muzzleFlash;
    [HideInInspector] public ParticleSystem instantiatedMuzzleFlash;
    public GameObject hitDecal;
    public GameObject impactEffect;
    public float recoilAngle = 30f;

    [Header("Alt fire (only for player)")]
    public GameObject altFireController;
    [HideInInspector] public GameObject instantiatedAltFireController;
    public altFireBase altFire;

    private bool _isReloading = false;

    private void Awake()
    {
        instantiatedWeapon = Instantiate(weaponObj, weaponPos);
        SetLayerRecursively(instantiatedWeapon, weaponPos.gameObject.layer);
        gunTip = instantiatedWeapon.GetComponentInChildren<GunTip>().gameObject.transform;
        _gunEffectsHolder = instantiatedWeapon.GetComponentInChildren<GunEffectsHolder>().gameObject.transform;
        _gunInterface = GetComponentInChildren<GunInterface>();
        _gunInterface.SetSubText(_currentAmmo.ToString());
        CurrentAmmo = clipSize;

        if (tracerEffect != null) instantiatedTracer = Instantiate(tracerEffect, _gunEffectsHolder);
        if (muzzleFlash != null) instantiatedMuzzleFlash = Instantiate(muzzleFlash, _gunEffectsHolder);

        if (altFireController != null)
        {
            instantiatedAltFireController = Instantiate(altFireController, weaponPos);
            altFire = instantiatedAltFireController.GetComponent<altFireController>().altFireScript as altFireBase;
        }

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public int Mode
    {
        get
        {
            return _currentMode;
        }

        set
        {
            _currentMode = value;
            _gunInterface.SetMainText(value.ToString());
        }
    }

    public float CurrentAmmo
    {
        get
        {
            return _currentAmmo;
        }

        set
        {
            _currentAmmo = value;
            _gunInterface.SetSubText(value.ToString());
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
            if (value) _gunInterface.SetSubText("R");
        }
    }

    public float Spreading
    {
        get
        {
            if (_isZoomed) return spreading * zoomSpreadingModifier;
            else return spreading;
        }
    }

    public void InitializeNewWeapon(WeaponInstance weapon)
    {
        weaponObj = weapon.weaponObj;

        hitDamage = weapon.hitDamage;
        fireRate = weapon.fireRate;
        clipSize = weapon.clipSize;
        CurrentAmmo = weapon.currentAmmo;
        reloadTime = weapon.reloadTime;
        isAutomatic = weapon.isAutomatic;

        _isReloading = false;
        IsZoomed = false;

        spreading = weapon.spreading;
        normalBurstDuration = weapon.normalBurstDuration;
        burstPenaltySpeed = weapon.burstPenaltySpeed;
        maxBurstPenalty = weapon.maxBurstPenalty;
        CurrentBurstPenalty = 0f;

        zoomRotateModifier = weapon.zoomRotateModifier;
        zoomSpeedModifier = weapon.zoomSpeedModifier;
        zoomSpreadingModifier = weapon.zoomSpreadingModifier;
        zoomValue = weapon.zoomValue;
        impactForce = weapon.impactForce;

        _currentMode = 1;
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
        _gunEffectsHolder = GetComponentInChildren<GunEffectsHolder>().gameObject.transform;
        _gunInterface = instantiatedWeapon.GetComponentInChildren<GunInterface>();
        _gunInterface.SetSubText(CurrentAmmo.ToString());

        instantiatedTracer = Instantiate(tracerEffect, _gunEffectsHolder);
        instantiatedMuzzleFlash = Instantiate(muzzleFlash, _gunEffectsHolder);

        if (altFireController != null)
        {
            instantiatedAltFireController = Instantiate(altFireController, weaponPos);
            altFire = instantiatedAltFireController.GetComponent<altFireController>().altFireScript as altFireBase;
        }

        GetComponent<ShootingSystem>().gunRecoil = instantiatedWeapon.GetComponentInChildren<RecoilGun>(); 
    }

    public static void SetLayerRecursively(GameObject gameObject, int layerNumber)
    {
        foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(true))
        {
            transform.gameObject.layer = layerNumber;
        }
    }
}
