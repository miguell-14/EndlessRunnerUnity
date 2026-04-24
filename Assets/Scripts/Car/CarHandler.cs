using System.Collections;
using UnityEngine;

public class CarHandler : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    public Transform gameModel;
    public MeshRenderer carMeshRenderer;

    [SerializeField] ExplodeHandler explodeHandler;

    [Header("Car Settings")]
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float acceleration = 15f;
    [SerializeField] float braking = 20f;
    [SerializeField] float steering = 130f;
    [SerializeField] float grip = 5f;

    float maxSteerVelocity = 2f;
    float maxForwardVelocity = 30f;

    Vector2 input = Vector2.zero;
    bool isExploded = false;

    //Stats
    float carStartPositionZ;
    float distanceTravelled = 0;
    public float DistanceTravelled => distanceTravelled;

    // Emissive
    float emissiveColorMultiplier = 0f;
    Color emissiveColor = Color.red;
    int _EmissionColor = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        carStartPositionZ = transform.position.z;
    }

    void Update()
    {
        if (isExploded) return;

        if (gameModel != null)
            gameModel.transform.rotation = Quaternion.Euler(0, rb.linearVelocity.x * 5, 0);

        if (carMeshRenderer != null)
        {
            float desiredCarEmissiveColorMultiplier = 0f;

            if (input.y < 0)
                desiredCarEmissiveColorMultiplier = 4.0f;

            emissiveColorMultiplier = Mathf.Lerp(
                emissiveColorMultiplier,
                desiredCarEmissiveColorMultiplier,
                Time.deltaTime * 4
            );

            carMeshRenderer.material.SetColor(
                _EmissionColor,
                emissiveColor * emissiveColorMultiplier
            );
        }

        distanceTravelled = transform.position.z - carStartPositionZ;
    }

    private void FixedUpdate()
    {
        if (isExploded) return;

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
            rb.linearDamping = 0.5f;
            rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
        }
        else if (input.y < 0)
        {
            rb.linearDamping = 0.5f;

            if (localVelocityZ > 0.1f)
                rb.AddForce(-transform.forward * braking, ForceMode.Acceleration);
        }
        else
        {
            rb.linearDamping = 3f;
        }
    }

    void ApplySteering()
    {
        float speed = rb.linearVelocity.magnitude;

        if (speed < 0.3f) return;

        float speedFactor = Mathf.Clamp01(speed / maxSpeed);
        float steerAmount = input.x * steering * speedFactor * Time.fixedDeltaTime;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, steerAmount, 0f));
    }

    void ApplyGrip()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        localVelocity.x = Mathf.Lerp(localVelocity.x, 0f, grip * Time.fixedDeltaTime);
        rb.linearVelocity = transform.TransformDirection(localVelocity);

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
        maxSpeed = newMaxSpeed;
        maxForwardVelocity = newMaxSpeed;
    }

    IEnumerator SlowDownTimeCO()
    {
        while (Time.timeScale > 0.2f)
        {
            Time.timeScale -= Time.deltaTime * 2;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        while (Time.timeScale < 1.0f)
        {
            Time.timeScale += Time.deltaTime;
            yield return null;
        }

        Time.timeScale = 1.0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Hit {collision.collider.name}");

        Vector3 velocity = rb.linearVelocity;

        if (explodeHandler != null)
            explodeHandler.Explode(velocity * 45);

        isExploded = true;

        StartCoroutine(SlowDownTimeCO());
    }
}