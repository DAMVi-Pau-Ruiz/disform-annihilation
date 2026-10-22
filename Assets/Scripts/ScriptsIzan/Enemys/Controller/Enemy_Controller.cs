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
    private Coroutine corrutinaDestello;

    private void Awake()
    {
        currentHealth = health;

        renderizadores = GetComponentsInChildren<Renderer>();
        bloqueMaterial = new MaterialPropertyBlock();
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

            MaterialPropertyBlock bloque = new MaterialPropertyBlock();

            renderizador.GetPropertyBlock(bloque);

            // Para materiales URP/Lit.
            bloque.SetColor("_BaseColor", colorDaño);
            bloque.SetColor("_Color", colorDaño);

            // Para nuestro shader OrganicMetaballs3D.
            bloque.SetFloat("_HitFlash", 1f);

            renderizador.SetPropertyBlock(bloque);
        }

        yield return new WaitForSeconds(duracionDestello);

        foreach (Renderer renderizador in renderizadores)
        {
            if (renderizador != null)
            {
                renderizador.SetPropertyBlock(null);
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