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

    //Timing
    WaitForSeconds wait = new WaitForSeconds(0.2f);

    private void Awake()
    {
        if(CompareTag("Player"))
        {
            Destroy(this);
            return;
        }
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(UpdateLessOfTenCO());
    }

    // Update is called once per frame
    void Update()
    {
        float accelerationInput = 1.0f;
        float steerInput = 0.0f;

        if (isCarAhead)
            accelerationInput = -1;
       
        steerInput = Mathf.Clamp(steerInput, -1f, 1.0f);

        carHandler.SetInput(new Vector2(steerInput, accelerationInput));
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

        int numberOfHits = Physics.BoxCastNonAlloc(transform.position, Vector3.one * 0.25f, transform.forward, raycastHits, Quaternion.identity, 2, otherCarsLayerMask);

        meshCollider.enabled = true;

        if(numberOfHits > 0)
            return true;
        
        return false;
    }

    //Events
    private void onEnable()
    {
        carHandler.SetMaxSpeed(Random.Range(2, 4));
    }
}
