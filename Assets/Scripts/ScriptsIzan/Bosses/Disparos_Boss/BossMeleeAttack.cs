
using UnityEngine;
using System.Collections;

public class BossMeleeAttack : MonoBehaviour
{
    [Header("Ataque")]
    [SerializeField] private float alcance = 3f;
    [SerializeField] private float angulo = 105f;
    [SerializeField] private float daño = 3f;
    [SerializeField] private float preparacion = 0.6f;
    [SerializeField] private float recuperacion = 1f;


     private Transform jugador;


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

    public IEnumerator EjecutarAtaque()
    {
        yield return new WaitForSeconds(preparacion);

        if (jugador != null)
        {
            Vector3 direccion = jugador.position - transform.position;
            direccion.y = 0f;

            float distancia = direccion.magnitude;

            if (distancia <= alcance)
            {
                float diferenciaAngular = Vector3.Angle(
                    transform.forward,
                    direccion.normalized
                );

                if (diferenciaAngular <= angulo / 2f)
                {
                    IHealthSystem salud =
                        jugador.GetComponentInParent<IHealthSystem>();

                    if (salud != null)
                    {
                        salud.GetHurt(daño);
                    }
                }
            }
        }

        yield return new WaitForSeconds(recuperacion);
    }
}
