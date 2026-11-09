using UnityEngine;

public class Disparos_Boss_Controller : Enemy_Controller
{

    [Header("Componentes del Boss")]
    [SerializeField] private BossAttackManager attackManager;
    [SerializeField] private BossMovement bossMovement;


    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if(attackManager != null)
        {
            attackManager.IniciarBoss();
        }
    }
}
