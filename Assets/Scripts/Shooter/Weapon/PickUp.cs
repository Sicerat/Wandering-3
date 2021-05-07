using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PickUp : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        print("I collided with " + other.gameObject);
        if (gameObject.tag == "Weapon")
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponentInParent<WeaponTemplate>().InitializeNewWeapon(GetComponent<WeaponInstance>());
                Destroy(gameObject);
            }
        }
    }
}
