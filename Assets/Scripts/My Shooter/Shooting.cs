using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shooting : MonoBehaviour
{
    Shot shot;
    Camera playerCamera;
    public GameObject hitDecal;
    Vector3 shootDirection;
    int mask;
    public GameObject textRandomizer;
    test_Story shootStory;
    
    
    

    // Start is called before the first frame update
    void Start()
    {
        shot = FindObjectOfType<Shot>();
        playerCamera = Camera.main;
        var from = transform.position;
        var direction = transform.forward;
        mask = LayerMask.GetMask("Default", "Enemies");
        shootStory = this.GetComponent<test_Story>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, mask))
            {
                shootDirection = hit.point - Camera.main.transform.position;
                shot.Show(transform.position, hit.point);
                if(hit.transform.GetComponent<Enemy>() != null)
                {
                    var enemy = hit.transform.GetComponent<Enemy>();
                    enemy.Hit(hit, shootDirection);
                }
                else
                {
                    SpawnDecal(hit);
                }
            }
        }
    }

    private void SpawnDecal(RaycastHit hitInfo)
    {
        textRandomizer.GetComponent<test_RandomPhrases>().GetNewText();
        var decal = Instantiate(hitDecal);
        decal.GetComponent<TextMeshPro>().text = shootStory.CurrentText;
        
        decal.transform.forward = hitInfo.normal * -1f;
        decal.transform.position = hitInfo.point;
    }
}
