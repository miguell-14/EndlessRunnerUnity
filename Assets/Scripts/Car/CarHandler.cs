using System.Collections;
using UnityEngine;
using System;

public class CarHandler : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    public Transform gameModel;

    [SerializeField] MeshRenderer carMeshRender;
    public MeshRenderer CarMeshRender => carMeshRender;

    [SerializeField] ExplodeHandler explodeHandler;

    [Header("Car Settings")]
    [SerializeField] float maxSpeed = 30f;
    [SerializeField] float acceleration = 15f;
    [SerializeField] float braking = 20f;
    [SerializeField] float steering = 130f;
    [SerializeField] float grip = 5f;

    [Header("SFX")]
    [SerializeField] AudioSource carEngineAS;
    [SerializeField] AnimationCurve carPitchAnimationCurve;
    [SerializeField] AudioSource carSkidAS;
    [SerializeField] AudioSource carCrashAS;

    Vector2 input = Vector2.zero;
    bool isExploded = false;
    bool isPlayer = true;

    float carMaxSpeedPercentage = 0;

    // Stats
    float carStartPositionZ;
    float distanceTravelled = 0;
    public float DistanceTravelled => distanceTravelled;

    public event Action<CarHandler> OnPlayerCrashed;

    // Emissive
    float emissiveColorMultiplier = 0f;
    Color emissiveColor = Color.white;
    int _EmissionColor = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        isPlayer = CompareTag("Player");

        if (isPlayer && carEngineAS != null)
            carEngineAS.Play();

        carStartPositionZ = transform.position.z;
    }

    void Update()
    {
        if (isExploded)
        {
            FadeOutCarAudio();
            return;
        }

        if (gameModel != null)
            gameModel.rotation = Quaternion.Euler(0, rb.linearVelocity.x * 5, 0);

        // Emissive
        if (carMeshRender != null)
        {
            float target = input.y < 0 ? 4f : 0f;

            emissiveColorMultiplier = Mathf.Lerp(
                emissiveColorMultiplier,
                target,
                Time.deltaTime * 4
            );

            carMeshRender.material.SetColor(
                _EmissionColor,
                emissiveColor * emissiveColorMultiplier
            );
        }

        UpdateCarAudio();

        distanceTravelled = transform.position.z - carStartPositionZ;
    }

    void FixedUpdate()
    {
        if (isExploded) return;

        ApplyAcceleration();
        ApplySteering();
        ApplyGrip();
        LimitSpeed();
    }

    // -------- MOVEMENT (segundo script) --------

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

    // -------- AUDIO (primeiro script) --------

    void UpdateCarAudio()
    {
        if (!isPlayer) return;

        carMaxSpeedPercentage = rb.linearVelocity.magnitude / maxSpeed;

        carEngineAS.pitch = carPitchAnimationCurve.Evaluate(carMaxSpeedPercentage);

        if (input.y < 0 && carMaxSpeedPercentage > 0.2f)
        {
            if (!carSkidAS.isPlaying)
                carSkidAS.Play();

            carSkidAS.volume = Mathf.Lerp(carSkidAS.volume, 1f, Time.deltaTime * 10);
        }
        else
        {
            carSkidAS.volume = Mathf.Lerp(carSkidAS.volume, 0, Time.deltaTime * 30);
        }
    }

    void FadeOutCarAudio()
    {
        if (!isPlayer) return;

        carEngineAS.volume = Mathf.Lerp(carEngineAS.volume, 0, Time.deltaTime * 10);
        carSkidAS.volume = Mathf.Lerp(carSkidAS.volume, 0, Time.deltaTime * 10);
    }

    // -------- INPUT --------

    public void SetInput(Vector2 inputVector)
    {
        inputVector.Normalize();
        input = inputVector;
    }

    public void SetMaxSpeed(float newMaxSpeed)
    {
        maxSpeed = newMaxSpeed;
    }

    // -------- TIME SLOW --------

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

    // -------- COLLISION --------

    private void OnCollisionEnter(Collision collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce < 5f) return;

        if (!isPlayer)
        {
            if (collision.transform.root.CompareTag("Untagged"))
                return;

            if (collision.transform.root.CompareTag("CarAI"))
                return;
        }

        Vector3 velocity = rb.linearVelocity;

        if (explodeHandler != null)
            explodeHandler.Explode(velocity * 45);

        isExploded = true;

        carCrashAS.volume = Mathf.Clamp(carMaxSpeedPercentage, 0.25f, 1.0f);
        carCrashAS.pitch = Mathf.Clamp(carMaxSpeedPercentage, 0.3f, 1.0f);
        carCrashAS.Play();

        OnPlayerCrashed?.Invoke(this);
        StartCoroutine(SlowDownTimeCO());
    }
}