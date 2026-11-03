using System.IO;
using UnityEngine;

public class PlayerSaveData : SaveData
{
    public Vector3 position;

    public float currentHealth;

    public float damage;
    public float damageMultiplier;
    public float bulletsPerSecond;
    public float bulletSpeed;

    public override string GetDataType()
    {
        return "PLAYER";
    }

    public override void Write(BinaryWriter writer)
    {
        // Datos comunes
        base.Write(writer);

        // Posición
        writer.Write(position.x);
        writer.Write(position.y);
        writer.Write(position.z);

        // Vida
        writer.Write(currentHealth);

        // Combate
        writer.Write(damage);
        writer.Write(damageMultiplier);
        writer.Write(bulletsPerSecond);
        writer.Write(bulletSpeed);
    }

    public override void Read(BinaryReader reader)
    {
        // Datos comunes
        base.Read(reader);

        // Posición
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        float z = reader.ReadSingle();

        position = new Vector3(x, y, z);

        // Vida
        currentHealth = reader.ReadSingle();

        // Combate
        damage = reader.ReadSingle();
        damageMultiplier = reader.ReadSingle();
        bulletsPerSecond = reader.ReadSingle();
        bulletSpeed = reader.ReadSingle();
    }
}