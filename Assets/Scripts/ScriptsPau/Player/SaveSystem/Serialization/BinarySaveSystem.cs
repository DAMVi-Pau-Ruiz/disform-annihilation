
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BinarySaveSystem
{
    private readonly string savePath =
        Application.persistentDataPath + "/save.dat";

    private readonly string temporaryPath =
        Application.persistentDataPath + "/save.tmp";

    private readonly string backupPath =
        Application.persistentDataPath + "/save.bak";

    private Dictionary<string, System.Func<SaveData>> dataFactories =
        new Dictionary<string, System.Func<SaveData>>();

    private const int SAVE_VERSION = 1;
    private const int MAX_OBJECTS = 10000;

    public BinarySaveSystem()
    {
        RegisterDataType("PLAYER", () => new PlayerSaveData());
        RegisterDataType("TEST_OBJECT", () => new TestObjectSaveData());
        RegisterDataType("ENEMY_SPAWNER", () => new EnemySpawnerSaveData());
    }

    public bool Save(Dictionary<string, SaveData> dataToSave)
    {
        if (dataToSave == null)
        {
            Debug.LogError("No se pueden guardar datos nulos.");
            return false;
        }

        try
        {
            using (FileStream stream = new FileStream(
                temporaryPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(SAVE_VERSION);
                    writer.Write(dataToSave.Count);

                    foreach (KeyValuePair<string, SaveData> entry in dataToSave)
                    {
                        if (string.IsNullOrEmpty(entry.Key) || entry.Value == null)
                        {
                            throw new InvalidDataException(
                                "Se ha encontrado un objeto con ID o datos inválidos."
                            );
                        }

                        string dataType = entry.Value.GetDataType();

                        if (string.IsNullOrEmpty(dataType))
                        {
                            throw new InvalidDataException(
                                "El tipo de datos no puede estar vacío."
                            );
                        }

                        writer.Write(entry.Key);
                        writer.Write(dataType);
                        entry.Value.Write(writer);
                    }

                    writer.Flush();
                    stream.Flush(true);
                }
            }

            if (File.Exists(savePath))
            {
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }

                File.Replace(temporaryPath, savePath, backupPath);
            }
            else
            {
                File.Move(temporaryPath, savePath);
            }

            Debug.Log("Guardado binario realizado: " + savePath);

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "Error al guardar la partida: " + exception.Message
            );

            return false;
        }
    }

    public Dictionary<string, SaveData> Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No existe ninguna partida principal.");

            return LoadBackup();
        }

        Dictionary<string, SaveData> loadedData = ReadSaveFile(savePath);

        if (loadedData != null)
        {
            Debug.Log("Partida cargada correctamente.");
            return loadedData;
        }

        Debug.LogWarning("La partida principal está dañada. Intentando recuperar backup.");

        return LoadBackup();
    }

    private Dictionary<string, SaveData> LoadBackup()
    {
        if (!File.Exists(backupPath))
        {
            Debug.LogWarning("No existe ninguna copia de seguridad.");

            return new Dictionary<string, SaveData>();
        }

        Dictionary<string, SaveData> backupData = ReadSaveFile(backupPath);

        if (backupData != null)
        {
            Debug.Log("Partida recuperada desde la copia de seguridad.");
            return backupData;
        }

        Debug.LogError("El archivo principal y el backup están dañados.");

        return new Dictionary<string, SaveData>();
    }

    private Dictionary<string, SaveData> ReadSaveFile(string path)
    {
        Dictionary<string, SaveData> loadedData =
            new Dictionary<string, SaveData>();

        try
        {
            using (FileStream stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int version = reader.ReadInt32();

                    if (version != SAVE_VERSION)
                    {
                        throw new InvalidDataException(
                            "La versión del archivo no es compatible."
                        );
                    }

                    int objectCount = reader.ReadInt32();

                    if (objectCount < 0 || objectCount > MAX_OBJECTS)
                    {
                        throw new InvalidDataException(
                            "La cantidad de objetos guardados no es válida."
                        );
                    }

                    for (int i = 0; i < objectCount; i++)
                    {
                        string id = reader.ReadString();
                        string dataType = reader.ReadString();

                        if (string.IsNullOrEmpty(id))
                        {
                            throw new InvalidDataException(
                                "Se ha encontrado un ID vacío."
                            );
                        }

                        if (loadedData.ContainsKey(id))
                        {
                            throw new InvalidDataException(
                                "ID duplicado: " + id
                            );
                        }

                        SaveData data = CreateSaveData(dataType);

                        if (data == null)
                        {
                            throw new InvalidDataException(
                                "Tipo de datos desconocido: " + dataType
                            );
                        }

                        data.Read(reader);

                        loadedData.Add(id, data);
                    }

                    if (stream.Position != stream.Length)
                    {
                        throw new InvalidDataException(
                            "El archivo contiene datos sobrantes."
                        );
                    }
                }
            }

            return loadedData;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "Error leyendo " + path + ": " + exception.Message
            );

            return null;
        }
    }

    public void RegisterDataType(string type, System.Func<SaveData> factory)
    {
        if (string.IsNullOrEmpty(type) || factory == null)
        {
            Debug.LogError("Registro de tipo inválido.");
            return;
        }

        if (dataFactories.ContainsKey(type))
        {
            Debug.LogError("Tipo de datos duplicado: " + type);
            return;
        }

        dataFactories.Add(type, factory);
    }

    private SaveData CreateSaveData(string dataType)
    {
        if (dataFactories.TryGetValue(dataType, out System.Func<SaveData> factory))
        {
            return factory();
        }

        return null;
    }
}
