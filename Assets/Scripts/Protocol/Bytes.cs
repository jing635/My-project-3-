using System;
using System.Collections.Generic;
using System.Text;

// 往 byte[] 里写数据
public class ByteWriter
{
    private readonly List<byte> list = new List<byte>();

    public void WriteInt(int v)
    {
        list.AddRange(BitConverter.GetBytes(v));
    }

    public void WriteFloat(float v)
    {
        list.AddRange(BitConverter.GetBytes(v));
    }

    public void WriteString(string s)
    {
        if (s == null) s = "";
        byte[] b = Encoding.UTF8.GetBytes(s);
        WriteInt(b.Length);         
        list.AddRange(b);
    }
    public void WriteUser_Characters(List<User_CharacterData> user_characters)
    {
        WriteInt(user_characters.Count);
        for (int i = 0; i < user_characters.Count; i++)
        {
            WriteUser_Character(user_characters[i]);
        }
    }

    private void WriteUser_Character(User_CharacterData data)
    {
        WriteString(data.CharacterName);
        WriteInt(data.CharacterLevel);
        WriteInt(data.CharacterShards);
    }



    public byte[] ToArray()
    {
        return list.ToArray();
    }
}

// 从 byte[] 里读数据
public class ByteReader
{
    private readonly byte[] data;
    private int pos;

    public ByteReader(byte[] data)
    {
        this.data = data;
        pos = 0;
    }

    public int ReadInt()
    {
        int v = BitConverter.ToInt32(data, pos);
        pos += 4;
        return v;
    }

    public float ReadFloat()
    {
        float v = BitConverter.ToSingle(data, pos);
        pos += 4;
        return v;
    }

    public string ReadString()
    {
        int len = ReadInt();
        string s = Encoding.UTF8.GetString(data, pos, len);
        pos += len;
        return s;
    }

    public List<User_CharacterData> ReadUser_Characters()
    {
        int count = ReadInt();
        List<User_CharacterData> user_characterdatas = new List<User_CharacterData>();
        for (int i = 0; i < count; i++)
        {
            user_characterdatas.Add(ReadUser_Character());
        }
        return user_characterdatas;

    }
    private User_CharacterData ReadUser_Character()
    {
        User_CharacterData data = new User_CharacterData();
        
        data.CharacterName = ReadString();
        data.CharacterLevel = ReadInt();
        data.CharacterShards = ReadInt();
        return data;
    }
}