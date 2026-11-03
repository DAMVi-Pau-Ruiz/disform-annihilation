
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class EnemySpawnerSaveData : SaveData
{
    private const int MAX_ENEMIES = 1000;

    public bool hasSpawned;
    public List<EnemySpawnData> enemies = new List<EnemySpawnData>();

    public override string GetDataType()
    {
        return "ENEMY_SPAWNER";
    }

    public override void Write(BinaryWriter writer)
    {
        base.Write(writer);

        writer.Write(hasSpawned);
        writer.Write(enemies.Count);

        foreach (EnemySpawnData enemy in enemies)
        {
            writer.Write(enemy.hasSpawned);
            writer.Write(enemy.isAlive);
            writer.Write(enemy.enemyIndex);

            writer.Write(enemy.position.x);
            writer.Write(enemy.position.y);
            writer.Write(enemy.position.z);

            writer.Write(enemy.currentHealth);
        }
    }

    public override void Read(BinaryReader reader)
    {
        base.Read(reader);

        hasSpawned = reader.ReadBoolean();

        int count = reader.ReadInt32();

        if (count < 0 || count > MAX_ENEMIES)
        {
            throw new InvalidDataException(
                "La cantidad de enemigos guardados no es válida."
            );
        }

        enemies.Clear();

        for (int i = 0; i < count; i++)
        {
            EnemySpawnData enemy = new EnemySpawnData();

            enemy.hasSpawned = reader.ReadBoolean();
            enemy.isAlive = reader.ReadBoolean();
            enemy.enemyIndex = reader.ReadInt32();

            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();

            enemy.position = new Vector3(x, y, z);

            enemy.currentHealth = reader.ReadSingle();

            enemies.Add(enemy);
        }
    }
}

public class EnemySpawnData
{
    public bool hasSpawned;
    public bool isAlive;
    public int enemyIndex;
    public Vector3 position;
    public float currentHealth;
}
