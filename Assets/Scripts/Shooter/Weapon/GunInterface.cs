using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunInterface : MonoBehaviour
{
    TextMeshPro text;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string addText)
    {
        text.text = addText;
    }
}
