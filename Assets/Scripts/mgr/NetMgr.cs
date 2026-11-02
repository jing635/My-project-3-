using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetMgr : MonoBehaviour
{

    private static NetMgr instance;
    public static NetMgr Instance => instance;

    //客户端的socket
    private Socket socket;

    //用于发送消息的队列
    private Queue<BaseMsg> sendMsgQueue = new Queue<BaseMsg>();
    //用于接收消息的队列
    private Queue<BaseMsg> receiveQueue = new Queue<BaseMsg>();

    private Thread sendthread;
    private Thread receivethread;

    private byte[] receiveBytes = new byte[1024 * 5];
    private int receiveNum;

    public bool isConnect = false;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (receiveQueue.Count > 0)
        {
            print(receiveQueue.Dequeue());
        }
    }

    //连接服务端
    public void Connect(string ip, int port)
    {
        if (socket == null)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        try
        {
            isConnect = true;
            socket.Connect(ipPoint);
            sendthread = new Thread(SendMsg);
            sendthread.Start();
            receivethread = new Thread(ReceiveMsg);
            receivethread.Start();
        }
        catch (SocketException ex)
        {
            if (ex.ErrorCode == 10061)
            {
                print("服务器拒绝连接");
            }
            else
            {
                print("连接失败");
            }

        }
    }

    //发送消息
    public void Send(BaseMsg msg) 
    {
        sendMsgQueue.Enqueue(msg);
    }

    private void SendMsg()
    {
        while (isConnect)
        {
            if (sendMsgQueue.Count > 0)
            {
                socket.Send(sendMsgQueue.Dequeue().Write());
            }
        }
    }

    //接收消息
    private void ReceiveMsg()
    {
        while (isConnect)
        {
            if (socket.Available > 0)
            {
                receiveNum = socket.Receive(receiveBytes);
                
                int msgid = BitConverter.ToInt32 (receiveBytes, 0);
                switch (msgid)
                {
                    case 1:
                        LoginMsg1 msg = new LoginMsg1();
                        msg.Reading(receiveBytes, 4); 
                    
                        break;
                }
                receiveQueue.Enqueue(Encoding.UTF8.GetString(receiveBytes, 0, receiveNum));
            }
        }
    }

    public void Close()
    {
        if (socket != null)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            isConnect = false;
            sendthread = null;
            receivethread = null;
        }
    }

    private void OnDestroy()
    {
        Close();
    }
}
