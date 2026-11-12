using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [Header("Paneles UI")]
    [SerializeField] GameObject MenuPausa;
    [SerializeField] GameObject MenuOpciones;

    [Header("Input System")]
    [SerializeField] InputActionReference pauseAction;
    [SerializeField] private PlayerInput playerInput;

    [Header("Nombres de los Action Maps")]
    [SerializeField] private string gameplayActionMapName = "Player";
    [SerializeField] private string uiActionMapName = "UI";

    [Header("Arrastra aquí el objeto del Jugador")]
    [SerializeField] private GameObject objetoJugador;

    private bool IsPaused = false;
    private bool isInOptions = false;
    private bool canProcessInput = true;

    private void Awake()
    {
        if (MenuPausa != null) MenuPausa.SetActive(false);
        if (MenuOpciones != null) MenuOpciones.SetActive(false);
    }

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.started += OnPauseStarted;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.started -= OnPauseStarted;
            pauseAction.action.Disable();
        }
    }

    private void OnPauseStarted(InputAction.CallbackContext context)
    {
        if (!canProcessInput) return;

        if (IsPaused && isInOptions)
        {
            MenuOpcionesManager opcionesManager = Object.FindFirstObjectByType<MenuOpcionesManager>();
            if (opcionesManager != null)
            {
                opcionesManager.OnClickIntentarSalir();
            }
            else
            {
                CloseOptions();
            }
            return;
        }

        TogglePause();
    }

    public void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        StartCoroutine(ResumeRoutine());
    }

    private IEnumerator ResumeRoutine()
    {
        if (MenuPausa != null) MenuPausa.SetActive(false);
        if (MenuOpciones != null) MenuOpciones.SetActive(false);

        IsPaused = false;
        isInOptions = false;

        ActivarScriptsJugador(true);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yield return null;

        if (playerInput != null)
        {
            var gameplayMap = playerInput.actions?.FindActionMap(gameplayActionMapName);
            if (gameplayMap != null) gameplayMap.Enable();

            playerInput.SwitchCurrentActionMap(gameplayActionMapName);
        }

        StartCoroutine(InputCooldown());
    }

    private void Pause()
    {
        if (MenuPausa != null) MenuPausa.SetActive(true);
        if (MenuOpciones != null) MenuOpciones.SetActive(false);

        IsPaused = true;
        isInOptions = false;
        Time.timeScale = 0.0f;

        ActivarScriptsJugador(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerInput != null)
        {
            var gameplayMap = playerInput.actions?.FindActionMap(gameplayActionMapName);
            var uiMap = playerInput.actions?.FindActionMap(uiActionMapName);

            if (gameplayMap != null) gameplayMap.Disable();
            if (uiMap != null) uiMap.Enable();

            playerInput.SwitchCurrentActionMap(uiActionMapName);
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        StartCoroutine(InputCooldown());
    }

    private void ActivarScriptsJugador(bool estado)
    {
        if (objetoJugador == null) return;

        CharacterController controller = objetoJugador.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = estado;

        Rigidbody rb = objetoJugador.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = !estado;

        // Filtramos de forma limpia los scripts tipo MonoBehaviour sin comprobaciones redundantes
        MonoBehaviour[] scripts = objetoJugador.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != null && script != playerInput && script != this)
            {
                script.enabled = estado;
            }
        }
    }

    public void OpenOptions()
    {
        isInOptions = true;
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        if (MenuPausa != null) MenuPausa.SetActive(false);
        if (MenuOpciones != null) MenuOpciones.SetActive(true);
        StartCoroutine(InputCooldown());
    }

    public void CloseOptions()
    {
        isInOptions = false;
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        if (MenuPausa != null) MenuPausa.SetActive(true);
        if (MenuOpciones != null) MenuOpciones.SetActive(false);
        StartCoroutine(InputCallerRefresh());
    }

    private IEnumerator InputCallerRefresh()
    {
        StartCoroutine(InputCooldown());
        yield return null;
    }

    public void ResumeButton()
    {
        Resume();
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    private IEnumerator InputCooldown()
    {
        canProcessInput = false;
        float timer = 0f;
        while (timer < 0.15f)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        canProcessInput = true;
    }
}