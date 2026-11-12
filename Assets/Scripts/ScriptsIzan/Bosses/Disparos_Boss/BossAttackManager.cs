
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class BossAttackManager : MonoBehaviour
{
    [Header("Jugador")]
    private Transform jugador;

    [Header("Disparo")]
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private GameObject proyectilRadial;
    [SerializeField] private GameObject proyectilFlecha;
    [SerializeField] private float velocidadProyectil = 10f;

    [Header("Ataques")]
    [SerializeField] private float tiempoEntreAtaques = 2.5f;
    [SerializeField] private float radioDeteccionCuerpo = 3f;
    [SerializeField] private BossMeleeAttack ataqueCuerpo;



    private int ultimoAtaque = -1;
    private bool atacando;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            jugador = player.transform;
        }
        else
        {
            Debug.LogError("No se ha encontrado el jugador con tag Player.");
        }
    }

    public void IniciarBoss()
    {
        StartCoroutine(CicloAtaques());
    }

    private IEnumerator CicloAtaques()
    {
        while (true)
        {
            if (jugador == null)
            {
                yield return null;
                continue;
            }

            if (atacando)
            {
                yield return null;
                continue;
            }

            atacando = true;

            float distancia = Vector3.Distance(
                transform.position,
                jugador.position
            );

            if (distancia <= radioDeteccionCuerpo && ataqueCuerpo != null)
            {
                yield return StartCoroutine(
                    ataqueCuerpo.EjecutarAtaque()
                );
            }
            else
            {
                int ataque;

                do
                {
                    ataque = Random.Range(0, 2);
                }
                while (ataque == ultimoAtaque);

                ultimoAtaque = ataque;

                if (ataque == 0)
                {
                    yield return StartCoroutine(AtaqueRadial());
                }
                else
                {
                    yield return StartCoroutine(AtaqueFlechas());
                }
            }

            yield return new WaitForSeconds(tiempoEntreAtaques);

            atacando = false;
        }
    }

    private IEnumerator AtaqueRadial()
    {
        if (proyectilRadial == null || puntoDisparo == null)
            yield break;

        int cantidadProyectiles = 24;
        int huecos = 3;

        float anguloPorProyectil = 360f / cantidadProyectiles;
        float rotacionInicial = Random.Range(0f, 360f);

        for (int i = 0; i < cantidadProyectiles; i++)
        {
            // Dejamos tres huecos para que el jugador pueda esquivar.
            if (i % (cantidadProyectiles / huecos) == 0)
                continue;

            float angulo = rotacionInicial + i * anguloPorProyectil;

            float radianes = angulo * Mathf.Deg2Rad;

            Vector3 direccion = new Vector3(
                Mathf.Sin(radianes),
                0f,
                Mathf.Cos(radianes)
            );

            CrearProyectil(proyectilRadial, direccion);
        }

        yield return null;
    }

    private IEnumerator AtaqueFlechas()
    {
        if (proyectilFlecha == null || puntoDisparo == null)
            yield break;

        int cantidadFlechas = 5;
        float separacion = 0.2f;

        for (int i = 0; i < cantidadFlechas; i++)
        {
            if (jugador == null)
                yield break;

            Vector3 direccion = jugador.position - puntoDisparo.position;
            direccion.y = 0f;

            if (direccion.sqrMagnitude > 0.001f)
            {
                CrearProyectil(proyectilFlecha, direccion.normalized);
            }

            yield return new WaitForSeconds(separacion);
        }
    }

    private void CrearProyectil(GameObject prefab, Vector3 direccion)
    {
        if (prefab == null || direccion.sqrMagnitude < 0.001f)
            return;

        Vector3 posicion = puntoDisparo.position + direccion.normalized * 1.5f;

        GameObject nuevo = Instantiate(
            prefab,
            posicion,
            Quaternion.LookRotation(direccion)
        );

        ProjectileBase script = nuevo.GetComponent<ProjectileBase>();

        if (script != null)
        {
            script.ConfigurarPropietario(transform);
        }

        Rigidbody rb = nuevo.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = direccion.normalized * velocidadProyectil;
        }

        Collider[] collidersBoss = GetComponentsInChildren<Collider>();
        Collider[] collidersProyectil = nuevo.GetComponentsInChildren<Collider>();

        foreach (Collider bossCollider in collidersBoss)
        {
            foreach (Collider proyectilCollider in collidersProyectil)
            {
                Physics.IgnoreCollision(bossCollider, proyectilCollider);
            }
        }
    }
}
