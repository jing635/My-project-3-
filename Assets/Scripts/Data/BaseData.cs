using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class BaseData 
{
    //序列化
    public abstract byte[] Write();

    //反序列化
    public abstract int Reading(byte[] bytes,int beginindex = 0);
    
    //得到字节数组长度
    public abstract int GetBytesNum();
    
    //存储int到指定的字节数组
    protected void WriteInt(byte[] bytes,int value,ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += 4;
    }
    protected void WriteShort(byte[] bytes, short value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(short);
    }
    protected void WriteLong(byte[] bytes, long value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(long);
    }
    protected void WriteFloat(byte[] bytes, float value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(float);
    }
    protected void WriteByte(byte[] bytes, byte value, ref int index)
    {
        bytes[index] = value;
        index += 1;
    }
    protected void WriteBool(byte[] bytes, bool value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(bool);
    }
    protected void WriteString(byte[] bytes, string value, ref int index)
    {
        byte[] stringbytes = Encoding.UTF8.GetBytes(value);
        WriteInt(bytes, stringbytes.Length, ref index);

        stringbytes.CopyTo(bytes, index);
        index += stringbytes.Length;
    }
    protected void WriteData(byte[] bytes,BaseData data, ref int index)
    {
        data.Write().CopyTo(bytes, index);
        index += data.GetBytesNum();
    }


    protected int ReadInt(byte[] bytes,ref int index)
    {
        int value = BitConverter.ToInt32(bytes, index);
        index += sizeof(int);
        return value;
    }
    protected bool Readbool(byte[] bytes, ref int index)
    {
        bool value = BitConverter.ToBoolean(bytes, index);
        index += sizeof(bool);
        return value;
    }
    protected float Readfloat(byte[] bytes, ref int index)
    {
        float value = BitConverter.ToSingle(bytes, index);
        index += sizeof(float);
        return value;
    }
    protected byte Readbyte(byte[] bytes, ref int index)
    {
        byte value = bytes[index];
        index += 1;
        return value;
    }
    protected string Readstring(byte[] bytes, ref int index)
    {
        int length = ReadInt(bytes, ref index);
        string value = Encoding.UTF8.GetString(bytes, index, length);
        index += length;
        return value;
    }
    protected T Readdata<T>(byte[] bytes, ref int index) where T:BaseData,new()
    {
        T data = new T();
        index += data.Reading(bytes,index);
        return data;
    }
}
