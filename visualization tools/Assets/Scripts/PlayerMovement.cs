using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The code in this class is taken from:
// https://www.youtube.com/watch?v=_QajrabyTJc&ab_channel=Brackeys
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [SerializeField] private float speed = 31f;

    private void Update()
    {
        float horIn = Input.GetAxis("Horizontal");
        float verIn = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horIn + transform.forward * verIn;

        controller.Move(move*speed*Time.deltaTime);
    }
}
