using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The code in this class in a combination of the following two videos:
// https://www.youtube.com/watch?v=f473C43s8nE&ab_channel=Dave%2FGameDevelopment
// https://www.youtube.com/watch?v=_QajrabyTJc&ab_channel=Brackeys
public class MouseLook : MonoBehaviour
{
    [SerializeField] private float sensX = 250f;
    [SerializeField] private float sensY = 250f;

    [SerializeField] private Transform playerBody;

    private float xRot;

    // metehod that is being called once script is enabled; before first Update()-call
    private void Start()
    {
        // TODO: check if ther is a way to use UI while locked 

       // Cursor.lockState = CursorLockMode.Locked;
       // Cursor.visible = false;
    }

    // method that is being called every frame
    private void Update()
    {
        UpdateRotation();
    }

    // updates player roation given mouse input
    private void UpdateRotation()
    {
        // getting user input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        xRot -= mouseY; // determining roatation
        xRot = Mathf.Clamp(xRot, -90f, 90f); // clamping rotation so player cannot look behind themselves

        transform.localRotation = Quaternion.Euler(xRot, 0, 0); // setting roation

        playerBody.Rotate(Vector3.up * mouseX); // rotating player
    }

}
