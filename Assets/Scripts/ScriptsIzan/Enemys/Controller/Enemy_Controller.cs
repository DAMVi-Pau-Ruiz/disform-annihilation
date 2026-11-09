using UnityEngine;
using System.Collections;

public class Enemy_Controller : MonoBehaviour, IHealthSystem
{
    [Header("Vida del enemigo")]
    [SerializeField] public int health = 3;

    [Header("Feedback de daño")]
    [SerializeField] private float duracionDestello = 0.1f;
    [SerializeField] private Color colorDaño = Color.white;

    private float currentHealth;
    private bool estaMuerto = false;

    private Renderer[] renderizadores;
    private MaterialPropertyBlock bloqueMaterial;
    private MaterialPropertyBlock[] bloquesOriginales;
    private Coroutine corrutinaDestello;

    // Avisamos al spawner cuando cambia la vida o el estado del enemigo.
    public event System.Action OnStateChanged;

    public bool IsDead()
    {
        return estaMuerto;
    }

    protected virtual void Awake()
    {
        currentHealth = health;

        renderizadores = GetComponentsInChildren<Renderer>();
        bloqueMaterial = new MaterialPropertyBlock();

        bloquesOriginales = new MaterialPropertyBlock[renderizadores.Length];

        for (int i = 0; i < renderizadores.Length; i++)
        {
            bloquesOriginales[i] = new MaterialPropertyBlock();
            renderizadores[i].GetPropertyBlock(bloquesOriginales[i]);
        }
    }

    public void GetHurt(float quantity)
    {
        Debug.Log("DAÑO RECIBIDO: " + quantity + " | OBJETO: " + gameObject.name);

        if (estaMuerto || quantity <= 0)
        {
            return;
        }

        currentHealth -= quantity;

        Debug.Log("Enemigo dañado. Vida restante: " + Mathf.Max(0, currentHealth));

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return;
        }

        OnStateChanged?.Invoke();

        if (corrutinaDestello != null)
        {
            StopCoroutine(corrutinaDestello);
        }

        corrutinaDestello = StartCoroutine(DestelloDaño());
    }

    // Permite restaurar la vida guardada.
    public void SetCurrentHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0, health);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DestelloDaño()
    {
        foreach (Renderer renderizador in renderizadores)
        {
            if (renderizador == null)
            {
                continue;
            }

            renderizador.GetPropertyBlock(bloqueMaterial);

            bloqueMaterial.SetColor("_BaseColor", colorDaño);
            bloqueMaterial.SetColor("_Color", colorDaño);
            bloqueMaterial.SetFloat("_HitFlash", 1f);
            bloqueMaterial.SetColor("_ColorSombra", colorDaño);

            renderizador.SetPropertyBlock(bloqueMaterial);
        }

        yield return new WaitForSeconds(duracionDestello);

        for (int i = 0; i < renderizadores.Length; i++)
        {
            if (renderizadores[i] != null)
            {
                renderizadores[i].SetPropertyBlock(bloquesOriginales[i]);
            }
        }

        corrutinaDestello = null;
    }

    private void Die()
    {

        Debug.LogError("EL BOSS HA MUERTO: " + gameObject.name);
        if (estaMuerto)
        {
            return;
        }

        estaMuerto = true;

        Debug.Log("El enemigo ha muerto.");

        OnStateChanged?.Invoke();

        Destroy(gameObject);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}