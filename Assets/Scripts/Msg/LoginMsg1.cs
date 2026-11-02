using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LoginMsg1 : BaseMsg
{
    public string username;
    public bool IsExistUsername = false;
    public override byte[] Write()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteInt(bytes,GetId(),ref index);
        WriteBool(bytes,IsExistUsername,ref index);
        WriteString(bytes,username,ref index);
        return bytes;
    }

    public override int Reading(byte[] bytes, int beginindex = 0)
    {
        int index = beginindex;
        IsExistUsername = Readbool(bytes,ref index);
        username = Readstring(bytes,ref index);
        return index - beginindex;
    }

    public override int GetBytesNum()
    {
        return 4 + 1 + 4 + Encoding.UTF8.GetBytes(username).Length;
    }

    public override int GetId()
    {
        return 1;
    }
}
