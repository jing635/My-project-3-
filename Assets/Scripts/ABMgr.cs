using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABMgr 
{
    //主包
    private AssetBundle mainAB = null;
    //依赖包获取用配置文件
    private AssetBundleManifest manifest = null;
    private static ABMgr instance;
    public Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();
    //路径名
    private string PathUrl
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }
    //主包名 方便修改
    private string ManifestPath
    {
        get
        {
#if UNITY_IOS
                return "IOS";
#elif UNITY_ANDROID
                return "Android";
#else
                return "PC";
#endif
        }
    }
    public static ABMgr Instance()
    {
        if (instance == null)
        {
            instance = new ABMgr();
        }
        return instance;
    }


    //加载AB包
    public void LoadAB(string abName)
    {
        //加载主包
        if( mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(PathUrl + ManifestPath);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        string[]  strs = manifest.GetAllDependencies(abName);
        //依赖包
        AssetBundle ab = null;
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                abDic.Add(strs[i], ab);
            }
        }
        //资源来源包
        if (!abDic.ContainsKey(abName))
        {
            ab = AssetBundle.LoadFromFile(PathUrl + abName);
            abDic.Add(abName, ab);
        }
    }
    //同步加载
    public Object LoadRes(string abName, string resName)
    {
        LoadAB(abName);
        //加载资源
        Object obj = abDic[abName].LoadAsset(resName);
        if(obj is GameObject)
            return Instantiate(obj);
        else
            return obj;
    }

    //指定类型
    public Object LoadRes(string abName, string resName, System.Type type)
    {
        LoadAB(abName);
        //加载资源
        Object obj = abDic[abName].LoadAsset(resName,type);
        if(obj is GameObject)
            return Instantiate(obj);
        else
            return obj;
    }

    public T LoadRes<T>(string abName, string resName) where T : Object
    {
        LoadAB(abName);
        //加载资源
        T obj = abDic[abName].LoadAsset<T>(resName);
        if(obj is GameObject)
            return Instantiate(obj);
        else
            return obj;
    }

    public void UnLoad(string abName)
    {
        if(abDic.ContainsKey(abName))
        {
            abDic[abName].Unload(false);
            abDic.Remove(abName);
        }
    }

    public void ClearAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        manifest = null;
    }
}
