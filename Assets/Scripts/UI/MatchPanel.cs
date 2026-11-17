using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchPanel : BasePanel
{
    public TextMeshProUGUI textMatchState;
    public Button btnCancel;
    public override void Init()
    {
        btnCancel.onClick.AddListener(() =>
        {
            //发送取消匹配消息
            NetMgr.Instance.Send(new MatchCancelReq());
            UIManager.Instance.HidePanel<MatchPanel>();
            GameLobbyPanel panel = UIManager.Instance.GetPanel<GameLobbyPanel>();
            panel.StopTick();
        });
    }

    public void ChangeInfo(string info)
    {
        textMatchState.text = info;
    }
}
