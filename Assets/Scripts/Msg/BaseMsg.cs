using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMsg : BaseData
{
    public override int GetBytesNum()
    {
        throw new System.NotImplementedException();
    }

    public override int Reading(byte[] bytes, int beginindex = 0)
    {
        throw new System.NotImplementedException();
    }

    public override byte[] Write()
    {
        throw new System.NotImplementedException();
    }

    public virtual int GetId()
    {
        return 0;
    }
}
