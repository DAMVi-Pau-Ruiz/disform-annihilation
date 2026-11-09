
using UnityEngine;
using System.Collections;

public class BossAttackManager : MonoBehaviour
{
   

    private Transform jugador;

    [Header("Referencias")]
    [SerializeField] private Transform puntoDisparo;

    [Header("Proyectiles")]
    [SerializeField] private GameObject proyectilRicochet;
    [SerializeField] private GameObject proyectilRadial;
    [SerializeField] private GameObject proyectilFlecha;

    [Header("Ataque cuerpo a cuerpo")]
    [SerializeField] private BossMeleeAttack ataqueCuerpo;

    [Header("Configuración")]
    [SerializeField] private float tiempoEntreAtaques = 2f;
    [SerializeField] private float velocidadProyectil = 10f;
    [SerializeField] private float radioDeteccionCuerpo = 3f;

    private int ultimoAtaque = -1;
    private bool atacando;

    public void IniciarBoss()
    {
        StartCoroutine(CicloAtaques());
    }
    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            jugador = player.transform;
        }
        else
        {
            Debug.LogError("No se ha encontrado ningún objeto con el tag Player.");
        }
    }

    private IEnumerator CicloAtaques()
    {
        while (true)
        {
            if (jugador == null || atacando)
            {
                yield return null;
                continue;
            }

            atacando = true;

            float distancia = Vector3.Distance(
                transform.position,
                jugador.position
            );

            if (distancia <= radioDeteccionCuerpo)
            {
                yield return StartCoroutine(ataqueCuerpo.EjecutarAtaque());
            }
            else
            {
                int ataque = ObtenerAtaqueAleatorio();

                switch (ataque)
                {
                    case 0:
                        AtaqueRicochet();
                        break;

                    case 1:
                        AtaqueRadial();
                        break;

                    case 2:
                        AtaqueFlechas();
                        break;
                }
            }

            yield return new WaitForSeconds(tiempoEntreAtaques);

            atacando = false;
        }
    }

    private int ObtenerAtaqueAleatorio()
    {
        int ataque;

        do
        {
            ataque = Random.Range(0, 3);
        }
        while (ataque == ultimoAtaque);

        ultimoAtaque = ataque;

        return ataque;
    }

    private void AtaqueRicochet()
    {
        if (proyectilRicochet == null) return;

        int cantidad = 12;

        for (int i = 0; i < cantidad; i++)
        {
            float angulo = i * 360f / cantidad;

            Vector3 direccion = Quaternion.Euler(0, angulo, 0)
                * transform.forward;

            CrearProyectil(proyectilRicochet, direccion);
        }
    }

    private void AtaqueRadial()
    {
        if (proyectilRadial == null) return;

        int cantidad = 24;
        int huecos = 3;
        int inicioHueco = Random.Range(0, cantidad);

        for (int i = 0; i < cantidad; i++)
        {
            bool esHueco = false;

            for (int j = 0; j < huecos; j++)
            {
                if (i == (inicioHueco + j) % cantidad)
                {
                    esHueco = true;
                    break;
                }
            }

            if (esHueco) continue;

            float angulo = i * 360f / cantidad;

            Vector3 direccion = Quaternion.Euler(0, angulo, 0)
                * transform.forward;

            CrearProyectil(proyectilRadial, direccion);
        }
    }


    private void AtaqueFlechas()
    {
        if (proyectilFlecha == null || jugador == null)
            return;

        StartCoroutine(DispararRafagaFlechas());
    }

    private IEnumerator DispararRafagaFlechas()
    {
        int cantidadFlechas = 5;
        float separacion = 0.2f;

        for (int i = 0; i < cantidadFlechas; i++)
        {
            Vector3 direccion = jugador.position - puntoDisparo.position;
            direccion.y = 0f;
            direccion.Normalize();

            CrearProyectil(proyectilFlecha, direccion);

            yield return new WaitForSeconds(separacion);
        }
    }



    private void CrearProyectil(GameObject prefab, Vector3 direccion)
    {
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

        // Ignorar las colisiones entre los proyectiles y el boss.
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
