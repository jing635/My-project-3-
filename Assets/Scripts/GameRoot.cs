using UnityEngine;

public class GameRoot : MonoBehaviour
{
    public string serverIP = "127.0.0.1";
    public int serverPort = 8080;

    private void Start()
    {
        if (NetMgr.Instance == null)
        {
            Debug.LogError("场景里没有 NetMgr，检查 GameRoot 物体上挂的组件");
            return;
        }

        // 顶号通知是全局的，放在这里注册，不管在哪个界面都能收到
        NetMgr.Instance.On<KickNotify>(OnKicked);

        NetMgr.Instance.Connect(serverIP, serverPort);

        NetMgr.Instance.On<SaveSettingRes>(res =>
        {
            Debug.Log("设置已保存：" + (res.code == Code.Ok ? "成功" : "失败"));
        });
    }

    private void OnKicked(KickNotify msg)
    {
        Debug.LogWarning("被顶号：" + msg.reason);

        GameData.Clear();
        NetMgr.Instance.Close();

        //UIManager.Instance.HidePanel<LobbyPanel>();       // 阶段二才有，现在没有也能调
        //UIManager.Instance.HidePanel<CharacterPanel>();

        UIManager.Instance.ShowPanel<LoginPanel>();
        UIManager.Instance.ShowPanel<TipPanel>().Show(msg.reason);

        // 重新连上，方便玩家立刻再登录
        NetMgr.Instance.Connect(serverIP, serverPort);
    }
}