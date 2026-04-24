using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIHandler : MonoBehaviour
{
    [SerializeField]
    CarHandler carHandler;

    //Collision detection
    [SerializeField]
    LayerMask otherCarsLayerMask;
    
    [SerializeField]
    MeshCollider meshCollider;

    RaycastHit[] raycastHits = new RaycastHit[1];
    bool isCarAhead = false;

    int drivingInLane = 0;

    //Timing
    WaitForSeconds wait = new WaitForSeconds(0.2f);

    private void Awake()
    {
        if (CompareTag("Player"))
        {
            Destroy(this);
            return;
        }
    }
    
    void Start()
    {
        StartCoroutine(UpdateLessOfTenCO());
    }

    void Update()
    {
        float accelerationInput = 1.0f;

        if (isCarAhead)
            accelerationInput = -1;

        float desiredPositionX = Utils.CarLanes[drivingInLane];

        // Mantém o carro na faixa (sem steering)
        Vector3 position = transform.position;
        position.x = Mathf.Lerp(position.x, desiredPositionX, Time.deltaTime * 3f);
        transform.position = position;

        // Mantém o carro sempre alinhado para a frente
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        // Sem steering → sempre em frente
        carHandler.SetInput(new Vector2(0f, accelerationInput));
    }

    IEnumerator UpdateLessOfTenCO()
    {
        while (true)
        {
            isCarAhead = CheckIfOtherCarsIsAhead();
            yield return wait;
        }
    }

    bool CheckIfOtherCarsIsAhead()
    {
        meshCollider.enabled = false;

        int numberOfHits = Physics.BoxCastNonAlloc(
            transform.position,
            Vector3.one * 0.25f,
            transform.forward,
            raycastHits,
            Quaternion.identity,
            2,
            otherCarsLayerMask
        );

        meshCollider.enabled = true;

        if (numberOfHits > 0)
            return true;
        
        return false;
    }

    //Events
    private void OnEnable()
    {
        //Set random speed
        carHandler.SetMaxSpeed(Random.Range(2, 4));

        //Set a random lane
        drivingInLane = Random.Range(0, Utils.CarLanes.Length);
    }
}