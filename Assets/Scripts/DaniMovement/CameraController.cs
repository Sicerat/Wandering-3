using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float normalScope;
    public float currentScope;
    public float smooth = 5f;
    public Camera camera;

    private void Awake()
    {
        normalScope = Camera.main.fieldOfView;
        currentScope = normalScope;
    }

    void Update()
    {
        transform.position = player.transform.position;

        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, currentScope, Time.deltaTime * smooth);
    }

    public void SetScope(float newScope)
    {
        currentScope = newScope;
    }
}
