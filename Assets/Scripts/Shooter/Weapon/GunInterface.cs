using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunInterface : MonoBehaviour
{
    public TextMeshPro mainText, subText;

    private void Awake()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMainText(string setText)
    {
        mainText.text = setText;
    }

    public void SetSubText(string setText)
    {
        subText.text = setText;
    }
}
