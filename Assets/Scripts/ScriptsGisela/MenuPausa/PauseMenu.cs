using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("Paneles UI OPCIONES")]
    [SerializeField] GameObject MenuPausa;
    [SerializeField] GameObject MenuOpciones;
    [Header("Input Action")]
    [SerializeField] InputActionReference pauseAction;
    private bool IsPaused = false;

    void Start()
    {
        Resume();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        pauseAction.action.Enable();
        pauseAction.action.performed += OnPausePerformed;
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPausePerformed;
        pauseAction.action.Disable();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {

        //cogemos la llamada de input y entonces lo llamamos
        TogglePause();
    }
    private void TogglePause()
    {
        //si es falso saldrá de la pausa y si es true entrará en esta
        if (IsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
    /// <summary>
    /// Método para cuando salgamos de la pausa
    /// desactivar el canvas, poner el bool en false 
    /// y restaurar el tiempo
    /// </summary>
    private void Resume()
    {
        MenuPausa.SetActive(false);
        IsPaused = false;
        Time.timeScale = 1f;
    }
    /// <summary>
    ///método de pausa donde cuando entremos en esta
    ///activaremos el menú(canvas)
    ///y pondremos en true IsPaused además de dejar el tiempo
    ///en 0, pararlo.
    /// </summary>
    private void Pause()
    {
        MenuPausa.SetActive(true);
        IsPaused = true;
        Time.timeScale = 0.0f;
    }
    //OPCIONES DE PAUSA
    public void OpenOptions()
    {
        MenuPausa.SetActive(false);
        MenuOpciones.SetActive(true);
        
    }
    public void CloseOptions()
    {
   
        MenuPausa.SetActive(true);
        MenuOpciones.SetActive(false);
    }

    public void ResumeButton()
    {
        MenuPausa.SetActive(false);
    }
    public void QuitGame()
    {
      
        Application.Quit();
    }
}
