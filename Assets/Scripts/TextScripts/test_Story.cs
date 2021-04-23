using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class test_Story : MonoBehaviour
{
    public TextAsset inkAsset;
    Story _inkStory;
    private string currentText;

    public string CurrentText
    {
        get
        {
            if (_inkStory.canContinue)
            {
                _inkStory.Continue();
            }
            else
            {
                _inkStory.ResetState();
                _inkStory.Continue();
            }
            currentText = _inkStory.currentText;
            return currentText;
        }
    }

    private void Awake()
    {
        _inkStory = new Story(inkAsset.text);
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


