using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class altFireBase : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void DoActionsLMB();

    public abstract void DoActionsLMBDown();

    public abstract void DoActionsLMBUp();

    public abstract void DoActionsRMB();

    public abstract void DoActionsRMBUp();

    public abstract void DoActionsMMB();

    public abstract void DoActionsMMBUp();
}
