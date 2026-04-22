using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    CarHandler carHandler;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction resetAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction  = playerInput.actions["Move"];
        resetAction = playerInput.actions["Reset"];
    }

    void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        carHandler.SetInput(input);

        if (resetAction.WasPressedThisFrame())
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}