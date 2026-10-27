
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IHealthSystem
{
    [Header("Health")]
    [SerializeField] private int totalHealth = 10;
    [SerializeField] private float invincibleTime = 1f;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float slowMultiplier = 0.5f;

    [Header("Combat")]
    [SerializeField] private float  damage = 1;
    [SerializeField] private float damageMultiplier = 1;
    [SerializeField] private float bulletsPerSecond = 2f;
    [SerializeField] private float bulletSpeed = 15;

    private float currentHealth;
    private bool isInvincible;

    private void Awake()
    {
        currentHealth = totalHealth;
    }
    //#####################################################
    //Sistema de vida
    //#####################################################
    
    //Getter de la vida actual
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    //Getter de la vida total del player
    public int GetTotalHealth()
    {
        return totalHealth;
    }

    //#####################################################
    //Movimiento
    //#####################################################

    //Getter de la velocidad base
    public float GetBaseSpeed()
    {
        return baseSpeed;
    }

    //Getter de la velocidad reducida
    public float GetSlowSpeed()
    {
        return baseSpeed * slowMultiplier;
    }

    //#####################################################
    //Daño
    //#####################################################

    //Getter de daño total
    public float GetTotalDamage()
    {
        return CalculateTotalDamage();
    }
    
    //Calculo de daño * multi
    private float CalculateTotalDamage()
    {
        return damage * damageMultiplier;
    }

    //Getter del multi de daño
    public float GetDamageMultiplier()
    {
        return damageMultiplier;
    }

    //Getter del daño base
    public float GetBaseDamage()
    {
        return damage;
    }

    //Añade daño base
    public void AddBaseDamage(float amount)
    {
        damage += amount;
    }

    //Añade multi
    public void AddDamageMultiplier(float amount)
    {
        damageMultiplier += amount;
    }

    //Getter de cadencia de disparo
    public float GetBulletsPerSecond()
    {
        return bulletsPerSecond;
    }

    //Añade balas por segundo
    public void AddBulletsPerSecond(float amount)
    {
        bulletsPerSecond += amount;
    }

    //Quita balas por segnudo
    public void SubsBulletsPerSecond(float amount)
    {
        bulletsPerSecond -= amount;
    }

    //Getter de la velocidad de la bala
    public float GetBulletSpeed()
    {
        return bulletSpeed;
    }

    //Añade velocidad a la bala
    public void AddBulletSpeed(float amount)
    {
        bulletSpeed += amount;
    }

    //Quita velocidad a la bala
    public void SubsBulletSpeed(float amount)
    {
        bulletSpeed -= amount;
    }

    //GetHurt de la interfaz de combate
    public void GetHurt(float quantity)
    {
        if (isInvincible || currentHealth <= 0)
        {
            return;
        }

        currentHealth -= quantity;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log("Te has hecho daño. Vida: " + currentHealth);

        if (currentHealth > 0)
        {
            StartCoroutine(CooldownCoroutine());
        }
        else
        {
            // Aquí puedes gestionar la muerte del jugador.
        }
    }

    //Cura al player
    public void AddHealth(float health)
    {
        currentHealth = Mathf.Clamp(currentHealth + health, 0, totalHealth);
    }

    //Invulnerabilidad al recibir daño
    private IEnumerator CooldownCoroutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
    }
}
