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

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        transform.localRotation = Quaternion.Euler(xRot, 0, 0);

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

      

        playerBody.Rotate(Vector3.up * mouseX);
    }

}
