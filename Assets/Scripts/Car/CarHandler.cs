using UnityEngine;

public class CarHandler : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [Header("Car Settings")]
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float acceleration = 15f;
    [SerializeField] float braking = 20f;
    [SerializeField] float steering = 130f;
    [SerializeField] float grip = 5f;

    //Max Values
    float maxSteerVelocity = 2;
    float maxForwardVelocity = 30;

    Vector2 input = Vector2.zero;

    private void FixedUpdate()
    {
        ApplyAcceleration();
        ApplySteering();
        ApplyGrip();
        LimitSpeed();
    }

    void ApplyAcceleration()
    {
        float localVelocityZ = transform.InverseTransformDirection(rb.linearVelocity).z;

        if (input.y > 0)
        {
            // Acelerar
            rb.linearDamping = 0.5f;
            rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
        }
        else if (input.y < 0)
        {
            // Travar apenas se estiver a mover para a frente
            rb.linearDamping = 0.5f;
            if (localVelocityZ > 0.1f)
                rb.AddForce(-transform.forward * braking, ForceMode.Acceleration);
        }
        else
        {
            // Sem input — abranda naturalmente
            rb.linearDamping = 3f;
        }
    }

    void ApplySteering()
    {
        float speed = rb.linearVelocity.magnitude;

        // Só vira se estiver em movimento
        if (speed < 0.3f) return;

        // Steering proporcional à velocidade mas com limite
        float speedFactor = Mathf.Clamp01(speed / maxSpeed);
        float steerAmount = input.x * steering * speedFactor * Time.fixedDeltaTime;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, steerAmount, 0f));
    }

    void ApplyGrip()
    {
        // Cancela velocidade lateral — simula aderência dos pneus
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        localVelocity.x = Mathf.Lerp(localVelocity.x, 0f, grip * Time.fixedDeltaTime);
        rb.linearVelocity = transform.TransformDirection(localVelocity);

        // Redireciona a velocidade para a frente do carro
        float speed = rb.linearVelocity.magnitude;
        if (speed > 0.1f)
        {
            rb.linearVelocity = Vector3.Lerp(
                rb.linearVelocity,
                transform.forward * speed,
                grip * Time.fixedDeltaTime
            );
        }
    }

    void LimitSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    public void SetInput(Vector2 inputVector)
    {
        inputVector.Normalize();
        input = inputVector;
    }

    public void SetMaxSpeed(float newMaxSpeed)
    {
        maxForwardVelocity = newMaxSpeed;
    }
}