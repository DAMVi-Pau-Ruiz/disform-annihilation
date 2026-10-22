
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

    private void Awake()
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

        if (corrutinaDestello != null)
        {
            StopCoroutine(corrutinaDestello);
        }

        corrutinaDestello = StartCoroutine(DestelloDaño());
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

            // Materiales URP/Lit.
            bloqueMaterial.SetColor("_BaseColor", colorDaño);
            bloqueMaterial.SetColor("_Color", colorDaño);

            // Shader OrganicMetaballs3D.
            bloqueMaterial.SetFloat("_HitFlash", 1f);

            // Shader de la sombra roja.
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
        estaMuerto = true;

        Debug.Log("El enemigo ha muerto.");

        Destroy(gameObject);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
