using UnityEngine;
using TMPro;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI distanceTravelledText;

    [SerializeField]
    TextMeshProUGUI gameOverText; 

    [SerializeField]
    CanvasGroup gameOverCanvasGroup;

    // Reference
    CarHandler playerCarHandler;

    void Awake()
    {
        playerCarHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<CarHandler>();

        // ✅ Correto
        playerCarHandler.OnPlayerCrashed += PlayerCarHandler_OnPlayerCrashed;
    }

    void Start()
    {
        gameOverCanvasGroup.interactable = false;
        gameOverCanvasGroup.alpha = 0;
    }

    void Update()
    {
        distanceTravelledText.text = playerCarHandler.DistanceTravelled.ToString("000000");
    }

    IEnumerator StartGameOverCO()
    {
        yield return new WaitForSecondsRealtime(3.0f); // ✅ corrigido

        gameOverCanvasGroup.interactable = true;

        while (gameOverCanvasGroup.alpha < 0.8f)
        {
            gameOverCanvasGroup.alpha = Mathf.MoveTowards(
                gameOverCanvasGroup.alpha,
                1.0f,
                Time.deltaTime * 2
            );

            yield return null;
        }
    }

    // Events
    void PlayerCarHandler_OnPlayerCrashed(CarHandler obj)
    {
        gameOverText.text = $"DISTANCE {distanceTravelledText.text}";
        StartCoroutine(StartGameOverCO());
    }

    public void OnRestartClicked()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}