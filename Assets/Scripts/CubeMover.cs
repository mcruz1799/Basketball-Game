using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    private Vector3 movementVector; 
    public float movementSpeed = 1.0f;
    public float jumpPower = 500;
    private float startY;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        startY = transform.position.y;
        movementVector = Vector3.zero;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time % 2 == 0)Debug.Log(rb.velocity);
        movementVector.x = Input.GetAxis("LeftJoystickX") * movementSpeed;
        movementVector.z = Input.GetAxis("LeftJoystickY") * movementSpeed;
        //if (transform.position.y == startY){
        if (Input.GetButtonDown("A")){
            if (rb.velocity.y < .25 && rb.velocity.y > -.25)
            rb.AddForce(Vector3.up * jumpPower);
            }
       // }
       // else Debug.Log("naw");
        //movementVector.y -= gravity * Time.deltaTime;   
        transform.Translate(movementVector);
    }
}
