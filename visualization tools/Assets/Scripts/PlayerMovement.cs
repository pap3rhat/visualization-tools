using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The code in this class is taken from:
// https://www.youtube.com/watch?v=_QajrabyTJc&ab_channel=Brackeys
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [SerializeField] private float speed = 31f;

    // method that is being called every frame
    private void Update()
    {
        // getting user input
        float horIn = Input.GetAxis("Horizontal"); // horizontal input
        float verIn = Input.GetAxis("Vertical"); // vertical input

        Vector3 move = transform.right * horIn + transform.forward * verIn; // motion vector in correct direction

        controller.Move(move*speed*Time.deltaTime); // moving player
    }
}
