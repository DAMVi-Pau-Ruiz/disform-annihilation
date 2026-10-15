
using UnityEngine;

public class MoverBloque : MonoBehaviour
{
    public float velocidad = 3f;
    public float distancia = 5f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        float movimiento = Mathf.PingPong(Time.time * velocidad, distancia);

        transform.position = posicionInicial +
                             Vector3.right * movimiento;
    }
}
