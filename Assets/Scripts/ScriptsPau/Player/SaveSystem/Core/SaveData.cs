using System.IO;

public abstract class SaveData
{
    public bool isActive;

    public virtual void Write(BinaryWriter writer)
    {
        writer.Write(isActive);
    }

    public virtual void Read(BinaryReader reader)
    {
        isActive = reader.ReadBoolean();
    }

    public abstract string GetDataType();
}