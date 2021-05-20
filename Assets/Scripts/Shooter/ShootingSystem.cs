using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponTemplate))]
public class ShootingSystem : MonoBehaviour
{

    public WeaponTemplate weapon;

    Vector3 shootDir;
    public RecoilGun gunRecoil;
    public float nextTimeToFire = 0f;
    public float burstStartTime = 0f;


    void Start()
    {
        weapon = GetComponent<WeaponTemplate>();
        gunRecoil = GetComponentInChildren<RecoilGun>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanShoot()
    {
        return nextTimeToFire <= Time.time && weapon.CurrentAmmo > 0;
    }

    //Universal weapon shot, using values from Weapon Template
    public void Shoot(Vector3 raycastStart, Vector3 shootDirection)
    {
        if (!CanShoot())
        {
            return;
        }
        
        RaycastHit hit;
        //Calculating "hit cooldown" based on weapon's fire rate. Multiplication by timeScale is needed to make fire rate independent from timeScale, which is highly questionable.
        nextTimeToFire = Time.time + (1f / weapon.fireRate) * Time.timeScale;
        weapon.CurrentAmmo--;

        //Calculating max spreading for this hit
        float spreading = weapon.Spreading + weapon.CurrentBurstPenalty;
        if(Time.time - burstStartTime > weapon.normalBurstDuration && weapon.CurrentBurstPenalty < weapon.maxBurstPenalty)
        {
            weapon.CurrentBurstPenalty += weapon.burstPenaltySpeed;
        }

        //Actual spreading randomization
        float dispersionX = Random.Range(-spreading, +spreading);
        float dispersionY = Random.Range(-spreading, +spreading);
        float dispersionZ = Random.Range(-spreading, +spreading);
        shootDir = new Vector3(shootDirection.x + dispersionX, shootDirection.y + dispersionY, shootDirection.z + dispersionZ);

        if (weapon.instantiatedTracer != null) weapon.instantiatedTracer.Emit(1);
        if (weapon.instantiatedMuzzleFlash != null) weapon.instantiatedMuzzleFlash.Play();

        if (gunRecoil != null) gunRecoil.DoRecoil(weapon.recoilAngle);

        if (Physics.Raycast(raycastStart, shootDir, out hit, Mathf.Infinity, weapon.hittableLayers))
        {
            shootDirection = hit.point - raycastStart;

            GameObject impactOnHit = Instantiate(weapon.impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactOnHit, 2);

            if (hit.transform.GetComponent<RagdollEnemy>() != null || hit.transform.GetComponentInParent<RagdollEnemy>() != null)
            {
                RagdollEnemy enemy;
                if (hit.transform.GetComponent<RagdollEnemy>() != null)
                {
                    enemy = hit.transform.GetComponent<RagdollEnemy>();
                }
                else
                {
                    enemy = hit.transform.GetComponentInParent<RagdollEnemy>();
                }
                enemy.ReceiveDamage(weapon.hitDamage, hit.transform.gameObject, shootDir);
            }
            else if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * weapon.impactForce);
                SpawnDecal(hit);
            }
            else
            {
                SpawnDecal(hit);
            }

        }
    }

    private void SpawnDecal(RaycastHit hitInfo)
    {
        var decal = Instantiate(weapon.hitDecal);

        decal.transform.forward = hitInfo.normal * -1f;
        decal.transform.position = hitInfo.point;
        decal.transform.parent = hitInfo.transform;
    }

    public void SwitchMode(float direction)
    {
        if (weapon.altFire != null)
        {
            if (direction > 0)
            {
                if (weapon.Mode == 2)
                {
                    weapon.Mode = 0;
                    return;
                }
                else
                {
                    weapon.Mode++;
                    return;
                }
            }
            else
            {
                if (weapon.Mode == 0)
                {
                    weapon.Mode = 2;
                    return;
                }
                else
                {
                    weapon.Mode--;
                    return;
                }
            }
        }
    }

    public void StartReload()
    {
        if (!weapon.IsReloading)
        {
            weapon.IsReloading = true;
            Invoke("FinishReload", weapon.reloadTime);
        }
    }

    private void FinishReload()
    {
        weapon.CurrentAmmo = weapon.clipSize;
        weapon.IsReloading = false;
    }

    public void Zoom()
    {
        weapon.IsZoomed = true;
    }

    public void Unzoom()
    {
        weapon.IsZoomed = false;
    }



}
