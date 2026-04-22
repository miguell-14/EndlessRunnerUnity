using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHandler : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    
    //Multipliers
    float accelerationMultiplier = 3;
    float breaksmultiplier = 15;
    float steeringMultiplier = 5;

    //Input
    Vector2 input = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (input.y > 0)
            Accelerate();
        else 
            rb.linearDamping = 0.2f;
        
        if (input.y < 0)
            Brake();

        Steer();
    }

    void Accelerate()
    {
       rb.linearDamping = 0;

       rb.AddForce(rb.transform.forward * 10 * accelerationMultiplier * input.y);
    }

    void Brake()
    {
        //Dont brake unless the car is moving forward
        if (rb.linearVelocity.z <= 0)
        {
            return;
        }

        rb.AddForce(rb.transform.forward * breaksmultiplier * input.y);
    }

    void Steer()
    {
        if(Mathf.Abs(input.x) > 0)
        {
           rb.AddForce(rb.transform.right * steeringMultiplier * input.x);
        }
    }

    public void SetInput(Vector2 inputVector)
    {
        inputVector.Normalize();
        input = inputVector;
    }
}
