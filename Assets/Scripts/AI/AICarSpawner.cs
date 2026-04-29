using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [SerializeField]
    GameObject[] carAIPreFabs;

    GameObject[] carAIPool = new GameObject[30];

    Transform playerCarTransform;

    // NOVO: posições das lanes (podes editar no Inspector)
    [SerializeField]
    float[] lanePositions = new float[] { -2f, 0f, 2f };

    //Timing
    float timeLastCarSpawned = 0;
    WaitForSeconds wait = new WaitForSeconds(0.5f);

    void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;

        int prefabIndex = 0;

        for (int i = 0; i < carAIPool.Length; i++)
        {
            carAIPool[i] = Instantiate(carAIPreFabs[prefabIndex]);
            carAIPool[i].SetActive(false);

            prefabIndex++;

            if (prefabIndex > carAIPreFabs.Length - 1)
                prefabIndex = 0;
        }

        StartCoroutine(UpdateLessOfTenCO());
    }

    IEnumerator UpdateLessOfTenCO()
    {
        while (true)
        {
            CleanUpCarsBeyondView();
            SpawnNewCars();

            yield return wait;
        }
    }

    void SpawnNewCars()
    {
        if (Time.time - timeLastCarSpawned < 1)
            return;

        GameObject carToSpawn = null;

        foreach (GameObject aiCar in carAIPool)
        {
            if (aiCar.activeInHierarchy)
                continue;

            carToSpawn = aiCar;
            break;
        }

        if (carToSpawn == null)
            return;

        // ESCOLHER LANE ALEATÓRIA
        float randomX = lanePositions[Random.Range(0, lanePositions.Length)];

        Vector3 spawnPosition = new Vector3(randomX, 0, playerCarTransform.position.z + 100);

        carToSpawn.transform.position = spawnPosition;
        carToSpawn.SetActive(true);

        timeLastCarSpawned = Time.time;
    }

    void CleanUpCarsBeyondView()
    {
        foreach (GameObject aiCar in carAIPool)
        {
            if (!aiCar.activeInHierarchy)
                continue;

            if (aiCar.transform.position.z - playerCarTransform.position.z > 200)
                aiCar.SetActive(false);

            if (aiCar.transform.position.z - playerCarTransform.position.z < -50)
                aiCar.SetActive(false);
        }
    }

    void Update()
    {

    }
}