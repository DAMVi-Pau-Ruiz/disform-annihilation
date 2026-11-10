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
        base.Write(writer);

        writer.Write(position.x);
        writer.Write(position.y);
        writer.Write(position.z);

        writer.Write(currentHealth);

        writer.Write(damage);
        writer.Write(damageMultiplier);
        writer.Write(bulletsPerSecond);
        writer.Write(bulletSpeed);
    }

    public override void Read(BinaryReader reader)
    {
        base.Read(reader);

        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        float z = reader.ReadSingle();

        position = new Vector3(x, y, z);

        currentHealth = reader.ReadSingle();

        damage = reader.ReadSingle();
        damageMultiplier = reader.ReadSingle();
        bulletsPerSecond = reader.ReadSingle();
        bulletSpeed = reader.ReadSingle();
    }
}