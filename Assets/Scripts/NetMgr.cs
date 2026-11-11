using System;
using UnityEngine;

public class NetMgr : MonoBehaviour
{
    public static NetMgr Instance;

    // 大厅服：登录、设置、角色、匹配
    public NetConnection Lobby { get; private set; }

    // 战斗服：帧同步
    public NetConnection Battle { get; private set; }

    private void Awake()
    {
        Instance = this;
        Lobby = new NetConnection("大厅服");
        Battle = new NetConnection("战斗服");
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        Lobby.Update();
        Battle.Update();
    }

    private void OnDestroy()
    {
        Lobby.Close();
        Battle.Close();
    }

    // ===== 下面是给旧代码用的快捷入口，默认都走大厅服 =====
    public bool IsConnected { get { return Lobby.IsConnected; } }
    public bool Connect(string ip, int port) { return Lobby.Connect(ip, port); }
    public void Send(BaseMsg msg) { Lobby.Send(msg); }
    public void On<T>(Action<T> callback) where T : BaseMsg, new() { Lobby.On(callback); }
    public void Off<T>() where T : BaseMsg, new() { Lobby.Off<T>(); }
    public void Close() { Lobby.Close(); }
}