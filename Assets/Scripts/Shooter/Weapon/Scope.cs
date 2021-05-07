using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope : MonoBehaviour
{

    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoScope()
    {
        animator.SetBool("IsScoped", true);
    }

    public void DoUnscope()
    {
        animator.SetBool("IsScoped", false);
    }
}
