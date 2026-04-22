using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeObjects : MonoBehaviour
{
    [SerializeField]
    Vector3 localRotationMin = Vector3.zero;
    [SerializeField]
    Vector3 localRotationMax = Vector3.zero;

    [SerializeField]
    float localscalemultiplierMin = 0.8f;
    [SerializeField]

    Vector3 localscaleoriginal = Vector3.one;

    private void Start()
    {
        localscaleoriginal = transform.localScale;
    }

    float localscalemultiplierMax = 1.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void onEnabled()
    {
        transform.localRotation = Quaternion.Euler(Random.Range(localRotationMin.x, localRotationMax.x), Random.Range(localRotationMin.y, localRotationMax.y), Random.Range(localRotationMin.z, localRotationMax.z));    
        transform.localScale = localscaleoriginal * Random.Range(localscalemultiplierMin, localscalemultiplierMax);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
