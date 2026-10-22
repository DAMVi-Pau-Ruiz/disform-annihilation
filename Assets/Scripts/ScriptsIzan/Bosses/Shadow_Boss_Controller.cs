using UnityEngine;
using System.Collections;

public class Shadow_Boss_Controller : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private float velocidadFaseDos = 4f;
    [SerializeField] private float distanciaAtaque = 5f;

    [Header("Embestida")]
    [SerializeField] private float tiempoPreparacion = 0.8f;
    [SerializeField] private float velocidadEmbestida = 10f;
    [SerializeField] private float duracionEmbestida = 0.5f;
    [SerializeField] private float tiempoRecuperacion = 1f;
    [SerializeField] private float radioImpactoEmbestida = 1f;

    [Header("Golpe de sombra")]
    [SerializeField] private float radioGolpeSombra = 3f;
    [SerializeField] private float preparacionGolpeSombra = 1.2f;
    [SerializeField] private float duracionZonaDaño = 0.25f;
    [SerializeField] private float dañoGolpeSombra = 2f;
    [SerializeField, Range(0f, 1f)]
    private float probabilidadGolpeSombra = 0.35f;

    [Header("Desaparición")]
    [SerializeField] private float tiempoAntesDesaparecer = 0.5f;
    [SerializeField] private float tiempoDesaparecido = 1.5f;
    [SerializeField] private float tiempoReaparicion = 0.5f;

    [SerializeField, Range(0f, 1f)]
    private float probabilidadDesaparecer = 0.25f;

    [SerializeField] private float distanciaReaparicion = 3f;

    [Header("Daño")]
    [SerializeField] private float dañoContacto = 1f;
    [SerializeField] private float dañoEmbestida = 2f;
    [SerializeField] private float intervaloDañoContacto = 1f;

    [Header("Fases")]
    [SerializeField, Range(0.1f, 0.9f)]
    private float porcentajeSegundaFase = 0.5f;

    [Header("Mejoras de fase 2")]
    [SerializeField, Range(0.1f, 1f)]
    private float multiplicadorCooldownFaseDos = 0.65f;

    [SerializeField, Min(1f)]
    private float multiplicadorDañoFaseDos = 1.5f;

    [Header("Indicadores")]
    [SerializeField] private bool mostrarIndicadorEmbestida = true;
    [SerializeField] private float anchoIndicador = 1.5f;

    [Header("Depuración")]
    [SerializeField] private bool mostrarMensajes = true;

    private Transform jugador;
    private PlayerStats statsJugador;
    private Enemy_Controller vidaEnemigo;
    private Rigidbody rb;

    private LineRenderer indicadorEmbestida;
    private LineRenderer indicadorGolpeSombra;

    private Renderer[] renderizadores;
    private Collider[] colisionadores;

    private Vector3 direccionEmbestida;
    private Vector3 centroGolpeSombra;

    private bool estaAtacando = false;
    private bool estaDesaparecido = false;
    private bool segundaFase = false;
    private bool jugadorGolpeadoEnEmbestida = false;

    private float vidaMaxima;
    private float tiempoSiguienteContacto;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        vidaEnemigo = GetComponent<Enemy_Controller>();

        renderizadores = GetComponentsInChildren<Renderer>();
        colisionadores = GetComponentsInChildren<Collider>();

        GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");

        if (objetoJugador != null)
        {
            jugador = objetoJugador.transform;
            statsJugador = objetoJugador.GetComponentInParent<PlayerStats>();

            if (statsJugador == null)
            {
                statsJugador = objetoJugador.GetComponent<PlayerStats>();
            }
        }

        if (vidaEnemigo != null)
        {
            vidaMaxima = vidaEnemigo.health;
        }
        else
        {
            Debug.LogError(
                "SOMBRA: No se encuentra Enemy_Controller en el jefe."
            );
        }

        CrearIndicador();
        CrearIndicadorGolpe();

        if (mostrarMensajes)
        {
            Debug.Log("SOMBRA: Jefe inicializado.");
        }
    }


    private void Update()
    {
        if (jugador == null || vidaEnemigo == null)
        {
            return;
        }

        ComprobarFase();

        // No iniciar otro ataque mientras ya está realizando uno.
        if (estaAtacando || estaDesaparecido)
        {
            return;
        }

        float distancia = Vector3.Distance(
            PosicionPlano(transform.position),
            PosicionPlano(jugador.position)
        );

        bool dentroDelRango = distancia <= distanciaAtaque;

        if (dentroDelRango)
        {
            float eleccion = Random.value;

            if (eleccion < probabilidadDesaparecer)
            {
                StartCoroutine(Desaparecer());
            }
            else if (
                eleccion <
                probabilidadDesaparecer + probabilidadGolpeSombra
            )
            {
                StartCoroutine(GolpeSombra());
            }
            else
            {
                StartCoroutine(Embestida());
            }
        }
        else
        {
            PerseguirJugador();
        }
    }


    // --------------------------------------------------
    // MOVIMIENTO
    // --------------------------------------------------

    private void PerseguirJugador()
    {
        Vector3 direccion = jugador.position - transform.position;
        direccion.y = 0f;

        if (direccion.sqrMagnitude < 0.001f)
        {
            return;
        }

        direccion.Normalize();

        float velocidadActual = segundaFase
            ? velocidadFaseDos
            : velocidad;

        Mover(direccion * velocidadActual * Time.deltaTime);

        MirarDireccion(direccion);
    }


    private void Mover(Vector3 desplazamiento)
    {
        if (rb != null)
        {
            rb.MovePosition(rb.position + desplazamiento);
        }
        else
        {
            transform.position += desplazamiento;
        }
    }


    private Vector3 PosicionPlano(Vector3 posicion)
    {
        posicion.y = 0f;
        return posicion;
    }


    private void MirarDireccion(Vector3 direccion)
    {
        direccion.y = 0f;

        if (direccion.sqrMagnitude < 0.001f)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(
            direccion.normalized,
            Vector3.up
        );
    }


    // --------------------------------------------------
    // EMBESTIDA
    // --------------------------------------------------

    private IEnumerator Embestida()
    {
        estaAtacando = true;
        jugadorGolpeadoEnEmbestida = false;

        direccionEmbestida = jugador.position - transform.position;
        direccionEmbestida.y = 0f;

        if (direccionEmbestida.sqrMagnitude > 0.001f)
        {
            direccionEmbestida.Normalize();
        }
        else
        {
            direccionEmbestida = transform.forward;
        }

        MirarDireccion(direccionEmbestida);

        if (mostrarMensajes)
        {
            Debug.Log("SOMBRA: Preparando embestida.");
        }

        ActualizarIndicador(true);

        yield return new WaitForSeconds(
            AjustarCooldown(tiempoPreparacion)
        );

        if (mostrarMensajes)
        {
            Debug.Log("SOMBRA: ¡EMBESTIDA!");
        }

        float tiempo = 0f;

        while (tiempo < duracionEmbestida)
        {
            Mover(
                direccionEmbestida *
                velocidadEmbestida *
                Time.deltaTime
            );

            ComprobarImpactoEmbestida();

            tiempo += Time.deltaTime;

            yield return null;
        }

        ActualizarIndicador(false);

        yield return new WaitForSeconds(
            AjustarCooldown(tiempoRecuperacion)
        );

        estaAtacando = false;

        if (mostrarMensajes)
        {
            Debug.Log("SOMBRA: Embestida terminada.");
        }
    }


    private void ComprobarImpactoEmbestida()
    {
        if (jugadorGolpeadoEnEmbestida)
        {
            return;
        }

        Collider[] impactos = Physics.OverlapSphere(
            transform.position,
            radioImpactoEmbestida
        );

        foreach (Collider impacto in impactos)
        {
            PlayerStats stats = impacto.GetComponentInParent<PlayerStats>();

            if (stats != null)
            {
                stats.GetHurt(AjustarDaño(dañoEmbestida));

                jugadorGolpeadoEnEmbestida = true;

                if (mostrarMensajes)
                {
                    Debug.Log(
                        "SOMBRA: Embestida acertada. Daño: " +
                        AjustarDaño(dañoEmbestida)
                    );
                }

                return;
            }
        }
    }


    // --------------------------------------------------
    // GOLPE DE SOMBRA
    // --------------------------------------------------

    private void CrearIndicadorGolpe()
    {
        GameObject objeto = new GameObject(
            "Indicador_Golpe_Sombra"
        );

        objeto.transform.SetParent(transform, false);

        indicadorGolpeSombra = objeto.AddComponent<LineRenderer>();

        indicadorGolpeSombra.useWorldSpace = true;
        indicadorGolpeSombra.loop = true;
        indicadorGolpeSombra.positionCount = 49;

        indicadorGolpeSombra.startWidth = 0.08f;
        indicadorGolpeSombra.endWidth = 0.08f;

        indicadorGolpeSombra.enabled = false;

        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");

        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        if (shader != null)
        {
            indicadorGolpeSombra.material = new Material(shader);
        }

        indicadorGolpeSombra.startColor = Color.yellow;
        indicadorGolpeSombra.endColor = Color.yellow;
    }


    private void DibujarZonaGolpe(Vector3 centro, Color color)
    {
        if (indicadorGolpeSombra == null)
        {
            return;
        }

        indicadorGolpeSombra.startColor = color;
        indicadorGolpeSombra.endColor = color;

        for (int i = 0; i < indicadorGolpeSombra.positionCount; i++)
        {
            float angulo = i * Mathf.PI * 2f /
                (indicadorGolpeSombra.positionCount - 1);

            float x = Mathf.Cos(angulo) * radioGolpeSombra;
            float z = Mathf.Sin(angulo) * radioGolpeSombra;

            Vector3 punto = new Vector3(
                centro.x + x,
                centro.y + 0.05f,
                centro.z + z
            );

            indicadorGolpeSombra.SetPosition(i, punto);
        }
    }


    private IEnumerator GolpeSombra()
    {
        estaAtacando = true;

        centroGolpeSombra = transform.position;

        if (mostrarMensajes)
        {
            Debug.Log("SOMBRA: Preparando golpe de sombra.");
        }

        if (indicadorGolpeSombra != null)
        {
            indicadorGolpeSombra.enabled = true;

            DibujarZonaGolpe(
                centroGolpeSombra,
                Color.yellow
            );
        }

        yield return new WaitForSeconds(
            AjustarCooldown(preparacionGolpeSombra)
        );

        if (mostrarMensajes)
        {
            Debug.Log("SOMBRA: ¡GOLPE DE SOMBRA!");
        }

        if (indicadorGolpeSombra != null)
        {
            DibujarZonaGolpe(
                centroGolpeSombra,
                Color.red
            );
        }

        Vector3 posicionJugador = PosicionPlano(jugador.position);
        Vector3 centroPlano = PosicionPlano(centroGolpeSombra);

        float distancia = Vector3.Distance(
            posicionJugador,
            centroPlano
        );

        if (distancia <= radioGolpeSombra && statsJugador != null)
        {
            statsJugador.GetHurt(AjustarDaño(dañoGolpeSombra));

            if (mostrarMensajes)
            {
                Debug.Log(
                    "SOMBRA: Golpe acertado. Daño: " +
                    AjustarDaño(dañoGolpeSombra)
                );
            }
        }
        else if (mostrarMensajes)
        {
            Debug.Log("SOMBRA: El jugador ha esquivado el golpe.");
        }

        yield return new WaitForSeconds(
            AjustarCooldown(duracionZonaDaño)
        );

        if (indicadorGolpeSombra != null)
        {
            indicadorGolpeSombra.enabled = false;
        }

        if (mostrarMensajes)
        {
            Debug.Log("SOMBRA: Recuperándose después del golpe.");
        }

        yield return new WaitForSeconds(
            AjustarCooldown(tiempoRecuperacion)
        );

        estaAtacando = false;

        if (mostrarMensajes)
        {
            Debug.Log("SOMBRA: Preparada para volver a atacar.");
        }
    }


    // --------------------------------------------------
    // DESAPARICIÓN Y TELETRANSPORTE
    // --------------------------------------------------

    private IEnumerator Desaparecer()
    {
        estaAtacando = true;

        if (mostrarMensajes)
        {
            Debug.Log("SOMBRA: Preparando desaparición.");
        }

        yield return new WaitForSeconds(
            AjustarCooldown(tiempoAntesDesaparecer)
        );

        estaDesaparecido = true;

        // Ocultar el modelo.
        foreach (Renderer renderizador in renderizadores)
        {
            if (renderizador != null)
            {
                renderizador.enabled = false;
            }
        }

        // Desactivar las colisiones mientras está oculto.
        foreach (Collider colisionador in colisionadores)
        {
            if (colisionador != null)
            {
                colisionador.enabled = false;
            }
        }

        if (mostrarMensajes)
        {
            Debug.Log("SOMBRA: Ha desaparecido.");
        }

        yield return new WaitForSeconds(
            AjustarCooldown(tiempoDesaparecido)
        );

        // Buscar una posición cerca del jugador.
        Vector3 direccion = transform.position - jugador.position;
        direccion.y = 0f;

        if (direccion.sqrMagnitude < 0.01f)
        {
            direccion = Vector3.forward;
        }

        direccion.Normalize();

        Vector3 nuevaPosicion = jugador.position
            + direccion * distanciaReaparicion;

        // Mantener la altura original.
        nuevaPosicion.y = transform.position.y;

        // Teletransportar el Rigidbody o el Transform.
        if (rb != null)
        {
            rb.position = nuevaPosicion;
        }
        else
        {
            transform.position = nuevaPosicion;
        }

        // Orientar al jefe hacia el jugador.
        MirarDireccion(jugador.position - transform.position);

        // Mostrar de nuevo el modelo.
        foreach (Renderer renderizador in renderizadores)
        {
            if (renderizador != null)
            {
                renderizador.enabled = true;
            }
        }

        // Reactivar las colisiones.
        foreach (Collider colisionador in colisionadores)
        {
            if (colisionador != null)
            {
                colisionador.enabled = true;
            }
        }

        estaDesaparecido = false;

        if (mostrarMensajes)
        {
            Debug.Log("SOMBRA: Ha reaparecido en otra posición.");
        }

        yield return new WaitForSeconds(
            AjustarCooldown(tiempoReaparicion)
        );

        estaAtacando = false;
    }


    // --------------------------------------------------
    // SEGUNDA FASE
    // --------------------------------------------------

    private void ComprobarFase()
    {
        if (segundaFase)
        {
            return;
        }

        if (
            vidaEnemigo.GetCurrentHealth()
            <= vidaMaxima * porcentajeSegundaFase
        )
        {
            segundaFase = true;

            if (mostrarMensajes)
            {
                Debug.Log(
                    "SOMBRA: ¡FASE 2! Más velocidad, más daño y ataques más rápidos."
                );
            }

            ActualizarColorIndicador();
        }
    }


    private float AjustarCooldown(float tiempo)
    {
        if (segundaFase)
        {
            return tiempo * multiplicadorCooldownFaseDos;
        }

        return tiempo;
    }


    private float AjustarDaño(float daño)
    {
        if (segundaFase)
        {
            return daño * multiplicadorDañoFaseDos;
        }

        return daño;
    }


    // --------------------------------------------------
    // INDICADOR DE EMBESTIDA
    // --------------------------------------------------

    private void CrearIndicador()
    {
        GameObject objeto = new GameObject(
            "Indicador_Embestida"
        );

        objeto.transform.SetParent(transform, false);

        indicadorEmbestida = objeto.AddComponent<LineRenderer>();

        indicadorEmbestida.useWorldSpace = true;
        indicadorEmbestida.positionCount = 2;

        indicadorEmbestida.startWidth = anchoIndicador;
        indicadorEmbestida.endWidth = 0.1f;

        indicadorEmbestida.enabled = false;

        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");

        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        if (shader != null)
        {
            indicadorEmbestida.material = new Material(shader);
        }

        indicadorEmbestida.startColor = Color.red;
        indicadorEmbestida.endColor = Color.red;

        ActualizarColorIndicador();
    }


    private void ActualizarIndicador(bool mostrar)
    {
        if (indicadorEmbestida == null)
        {
            return;
        }

        if (!mostrarIndicadorEmbestida)
        {
            indicadorEmbestida.enabled = false;
            return;
        }

        indicadorEmbestida.enabled = mostrar;

        if (!mostrar)
        {
            return;
        }

        Vector3 inicio = transform.position;
        inicio.y += 0.1f;

        Vector3 final = inicio
            + direccionEmbestida * velocidadEmbestida
            * AjustarCooldown(duracionEmbestida);

        indicadorEmbestida.SetPosition(0, inicio);
        indicadorEmbestida.SetPosition(1, final);
    }


    private void ActualizarColorIndicador()
    {
        if (indicadorEmbestida == null)
        {
            return;
        }

        Color color = segundaFase
            ? new Color(1f, 0.4f, 0f)
            : Color.red;

        indicadorEmbestida.startColor = color;
        indicadorEmbestida.endColor = color;
    }


    // --------------------------------------------------
    // DAÑO POR CONTACTO
    // --------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        ComprobarDañoContacto(collision.collider);
    }


    private void OnCollisionStay(Collision collision)
    {
        ComprobarDañoContacto(collision.collider);
    }


    private void ComprobarDañoContacto(Collider otro)
    {
        if (estaAtacando || estaDesaparecido)
        {
            return;
        }

        if (Time.time < tiempoSiguienteContacto)
        {
            return;
        }

        PlayerStats stats = otro.GetComponentInParent<PlayerStats>();

        if (stats == null)
        {
            return;
        }

        stats.GetHurt(AjustarDaño(dañoContacto));

        tiempoSiguienteContacto = Time.time
            + AjustarCooldown(intervaloDañoContacto);

        if (mostrarMensajes)
        {
            Debug.Log(
                "SOMBRA: Daño por contacto: " +
                AjustarDaño(dañoContacto)
            );
        }
    }


    // --------------------------------------------------
    // GIZMOS PARA EL EDITOR
    // --------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        // Rango de detección y ataque.
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            distanciaAtaque
        );

        // Radio de impacto de la embestida.
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            radioImpactoEmbestida
        );

        // Área del golpe de sombra.
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            radioGolpeSombra
        );

        // Dirección estimada de la embestida.
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                transform.position,
                transform.position
                + direccionEmbestida * velocidadEmbestida
                * duracionEmbestida
            );
        }
    }
}