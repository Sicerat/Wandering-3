using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponTemplate))]
public class ShootingSystem : MonoBehaviour
{

    public WeaponTemplate weapon;

    Vector3 shootDir;
    RecoilGun gunRecoil;
    public float nextTimeToFire = 0f;


    void Start()
    {
        weapon = GetComponent<WeaponTemplate>();
        gunRecoil = GetComponentInChildren<RecoilGun>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Universal weapon shot, using values from Weapon Template
    public void Shoot(Vector3 raycastStart, Vector3 shootDirection)
    {
        RaycastHit hit;
        nextTimeToFire = Time.time + 1f / weapon.fireRate;

        //Actual spreading randomization
        float dispersionX = Random.Range(-weapon.spreading, +weapon.spreading);
        float dispersionY = Random.Range(-weapon.spreading, +weapon.spreading);
        shootDir = shootDirection + new Vector3(dispersionX, dispersionY, 0);

        weapon.instantiatedTracer.Emit(1);
        weapon.instantiatedMuzzleFlash.Play();
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

}
