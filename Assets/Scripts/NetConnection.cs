using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

// 一条 TCP 连接。大厅服、战斗服各用一个实例
public class NetConnection
{
    private Socket socket;
    private readonly byte[] buffer = new byte[64 * 1024];
    private int bufferLen;

    private readonly Dictionary<int, Action<byte[]>> handlers = new Dictionary<int, Action<byte[]>>();
    private readonly List<int> pendingIds = new List<int>();
    private readonly List<byte[]> pendingBodies = new List<byte[]>();

    private readonly string name;

    public NetConnection(string name)
    {
        this.name = name;
    }

    public bool IsConnected
    {
        get { return socket != null && socket.Connected; }
    }

    public bool Connect(string ip, int port)
    {
        Close();

        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.Connect(IPAddress.Parse(ip), port);
            bufferLen = 0;
            Debug.Log("[" + name + "] 已连接 " + ip + ":" + port);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("[" + name + "] 连接失败：" + e.Message);
            socket = null;
            return false;
        }
    }

    public void Send(BaseMsg msg)
    {
        if (!IsConnected)
        {
            Debug.LogWarning("[" + name + "] 还没连上，发不出去");
            return;
        }

        try
        {
            socket.Send(Frame.Pack(msg));
        }
        catch (Exception e)
        {
            Debug.LogError("[" + name + "] 发送失败：" + e.Message);
            Close();
        }
    }

    public void On<T>(Action<T> callback) where T : BaseMsg, new()
    {
        int id = new T().Id();
        handlers[id] = delegate (byte[] body)
        {
            T msg = new T();
            msg.Read(body);
            callback(msg);
        };
    }

    // 注销某个消息的处理函数。面板销毁时一定要调用
    public void Off<T>() where T : BaseMsg, new()
    {
        handlers.Remove(new T().Id());
    }

    // 每帧由 NetMgr 统一驱动
    public void Update()
    {
        if (socket == null) return;
        ReceiveAll();
        DispatchAll();
    }

    public void Close()
    {
        if (socket != null)
        {
            try { socket.Close(); } catch { }
            socket = null;
        }
        bufferLen = 0;
    }

    private void ReceiveAll()
    {
        try
        {
            while (socket.Poll(0, SelectMode.SelectRead))
            {
                if (bufferLen >= buffer.Length)
                {
                    Debug.LogError("[" + name + "] 缓冲区满了，数据异常");
                    Close();
                    return;
                }

                int n = socket.Receive(buffer, bufferLen, buffer.Length - bufferLen, SocketFlags.None);
                if (n <= 0)
                {
                    Debug.Log("[" + name + "] 服务器断开了");
                    Close();
                    return;
                }

                bufferLen += n;

                int msgId;
                byte[] body;
                while (Frame.TryUnpack(buffer, ref bufferLen, out msgId, out body))
                {
                    pendingIds.Add(msgId);
                    pendingBodies.Add(body);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[" + name + "] 接收出错：" + e.Message);
            Close();
        }
    }

    private void DispatchAll()
    {
        try
        {
            for (int i = 0; i < pendingIds.Count; i++)
            {
                int id = pendingIds[i];
                if (handlers.ContainsKey(id)) handlers[id](pendingBodies[i]);
                else Debug.LogWarning("[" + name + "] 收到没注册处理函数的消息号：" + id);
            }
        }
        finally
        {
            pendingIds.Clear();
            pendingBodies.Clear();
        }
    }
}
