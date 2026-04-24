using UnityEngine;
using Unity.Cinemachine;

public class PlayerCarSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] carPrefabs;

    [Header("Camera")]
    [SerializeField]
    CinemachineCamera cinemachineCamera;

    [Header("Menu")]
    [SerializeField]
    bool isMainMenu = false;

    //Instantiated car
    GameObject instantiatedPlayerCar = null;

    //Which car is selected
    int carIndex = 0;

    //Selected car from menu
    static GameObject selectedCarPrefab = null;

    Quaternion carRotation = Quaternion.identity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isMainMenu)
        {
            instantiatedPlayerCar = Instantiate(carPrefabs[carIndex].GetComponent<CarHandler>().CarMeshRender.gameObject);
            selectedCarPrefab = carPrefabs[carIndex];
        }
        else
        {
            if (selectedCarPrefab != null)
                instantiatedPlayerCar = Instantiate(selectedCarPrefab);
            else instantiatedPlayerCar = Instantiate(carPrefabs[0]);
        }

        if (cinemachineCamera != null)
            cinemachineCamera.Follow = instantiatedPlayerCar.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMainMenu)
        {
            instantiatedPlayerCar.transform.Rotate(new Vector3(0, 0, 20) * Time.deltaTime);

            carRotation = instantiatedPlayerCar.transform.rotation;
        }

    }

    public void ChangeCar()
    {
        Destroy(instantiatedPlayerCar);

        instantiatedPlayerCar = Instantiate(carPrefabs[carIndex].GetComponent<CarHandler>().CarMeshRender.gameObject);

        selectedCarPrefab = carPrefabs[carIndex];

        instantiatedPlayerCar.transform.rotation = carRotation;

    }

    public void OnNextCarClicked()
    {
        carIndex++;

        if (carIndex > carPrefabs.Length - 1)
            carIndex = 0;

        ChangeCar();
    }

    public void OnPreviousCarClicked()
    {
        carIndex--;

        if (carIndex < 0)
            carIndex = carPrefabs.Length - 1;

        ChangeCar();
    }
}
