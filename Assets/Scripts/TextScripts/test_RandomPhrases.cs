using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;

public class test_RandomPhrases : MonoBehaviour
{

    public TextAsset inkAsset;
    public Story _inkStory;
    // Start is called before the first frame update
    private void Awake()
    {
        _inkStory = new Story(inkAsset.text);
        _inkStory.Continue();
        this.GetComponent<Text>().text = _inkStory.currentText;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetNewText()
    {
        _inkStory.ResetState();
        _inkStory.Continue();
        this.GetComponent<Text>().text = _inkStory.currentText;
    }
}
