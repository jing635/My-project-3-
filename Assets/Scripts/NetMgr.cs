using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetMgr : MonoBehaviour
{
    public static NetMgr Instance;

    private Socket socket;

    // 收到的数据先攒着，够一条完整消息才处理
    private readonly byte[] buffer = new byte[64 * 1024];
    private int bufferLen;

    // 消息号 → 收到之后调哪个函数
    private readonly Dictionary<int, Action<byte[]>> handlers = new Dictionary<int, Action<byte[]>>();

    // 这一帧收到的消息，收完再统一处理
    private readonly List<int> pendingIds = new List<int>();
    private readonly List<byte[]> pendingBodies = new List<byte[]>();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsConnected
    {
        get { return socket != null && socket.Connected; }
    }

    public bool Connect(string ip, int port)
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.Connect(IPAddress.Parse(ip), port);
            bufferLen = 0;
            Debug.Log("已连接服务器 " + ip + ":" + port);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("连接失败：" + e.Message);
            socket = null;
            return false;
        }
    }

    public void Send(BaseMsg msg)
    {
        if (!IsConnected)
        {
            Debug.LogWarning("还没连接服务器，发不出去");
            return;
        }

        try
        {
            socket.Send(Frame.Pack(msg));
        }
        catch (Exception e)
        {
            Debug.LogError("发送失败：" + e.Message);
            Close();
        }
    }

    // 注册：收到 T 类型的消息时，调用 callback
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

    private void Update()
    {
        if (socket == null) return;

        ReceiveAll();
        DispatchAll();
    }

    // 把内核缓冲区里的数据全部读出来，拆成一条条消息
    private void ReceiveAll()
    {
        try
        {
            while (socket.Poll(0, SelectMode.SelectRead))
            {
                if (bufferLen >= buffer.Length)
                {
                    Debug.LogError("缓冲区满了，数据异常");
                    Close();
                    return;
                }

                int n = socket.Receive(buffer, bufferLen, buffer.Length - bufferLen, SocketFlags.None);
                if (n <= 0)
                {
                    Debug.Log("服务器断开了");
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
            Debug.LogError("接收出错：" + e.Message);
            Close();
        }
    }

    // 消息处理放在接收之后，这样处理函数里关闭/重连 socket 不会打乱接收循环
    private void DispatchAll()
    {
        try
        {
            for (int i = 0; i < pendingIds.Count; i++)
            {
                int id = pendingIds[i];
                if (handlers.ContainsKey(id)) handlers[id](pendingBodies[i]);
                else Debug.LogWarning("收到没注册处理函数的消息号：" + id);
            }
        }
        finally
        {
            pendingIds.Clear();
            pendingBodies.Clear();
        }
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

    private void OnDestroy()
    {
        Close();
    }
}