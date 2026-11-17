using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaMgr 
{
   private static LuaMgr instance;
   public LuaTable Global
    {
        get
        {
            return luaEnv.Global;
        }
    }
   public static LuaMgr Instance()
    {
        if(instance == null)
        {
            instance = new LuaMgr();
        }
        return instance;
    }

    private LuaEnv luaEnv;

    public void Init()
    {
        if(luaEnv != null)
        {
            return;
        }
        luaEnv = new LuaEnv();
        //加载脚本 重定向
        luaEnv.AddLoader(MyCustomLoader);
        luaEnv.AddLoader(MyCustomABLoader);
    }

    private byte[] MyCustomLoader(ref string filepath)
    {
        string path = Application.dataPath + "/Lua/" + filepath + ".lua";
        if (System.IO.File.Exists(path))
        {
            return System.IO.File.ReadAllBytes(path);
        }
        else
        {
            Debug.LogError("Lua file not found: " + filepath);
        }
        return null;
    }

    private byte[] MyCustomABLoader(ref string filepath)
    {
        TextAsset textAsset = ABMgr.Instance().LoadRes<TextAsset>("lua",filepath + ".lua");
        if (textAsset != null)
        {
            return textAsset.bytes;
        }
        else
        {
            Debug.LogError("Lua file not found in AssetBundle: " + filepath);
        }
        return null;
    }
 
    public void DoString(string luaStr)
    {
        if (luaEnv == null)
        {
            Debug.LogError("LuaEnv is not initialized. Call Init() first.");
            return;
        }
        luaEnv.DoString(luaStr);
    }

    public void DoLuaFile(string filePath)
    {
        string str = string.Format("require '{0}'", filePath);
        DoString(str);
    }

    public void Tick()
    {
        if (luaEnv == null)
        {
            Debug.LogError("LuaEnv is not initialized. Call Init() first.");
            return;
        }
        luaEnv.Tick();
    }

    public void DisPose()
    {
        luaEnv.Dispose();
        luaEnv = null;
    }
}
