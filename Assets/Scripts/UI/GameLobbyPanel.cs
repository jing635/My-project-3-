
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameLobbyPanel : BasePanel
{
    public Button btnSetting;
    public Button btnCharacter;
    public Button btnDuelArenaReal;
    public Button btnDuelArenaAi;
    public Coroutine TickCorotine;
    

    public override void Init()
    {
        NetMgr.Instance.On<MatchRes>(OnMatchRes);

        btnSetting.onClick.AddListener(OnClickSetting);
        btnCharacter.onClick.AddListener(OnClickCharacter);
        btnDuelArenaReal.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayMusic("click");
            //向服务器发起联机
            NetMgr.Instance.Send(new MatchReq { UserId = GameData.UserId });

        });
        btnDuelArenaAi.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayMusic("click");
            UIManager.Instance.HidePanel<GameLobbyPanel>();
            //UIManager.Instance.ShowPanel<ChooseCharacterPanel>();
        });
    }

    private void OnClickSetting()
    {
        AudioManager.Instance.PlayMusic("click");
        UIManager.Instance.HidePanel<GameLobbyPanel>();
        UIManager.Instance.ShowPanel<SettingPanel>();
    }

    private void OnClickCharacter()
    {
        AudioManager.Instance.PlayMusic("click");
        UIManager.Instance.HidePanel<GameLobbyPanel>();
        UIManager.Instance.ShowPanel<CharacterPanel>();
    }

    private void OnMatchRes(MatchRes res)
    {
        MatchPanel panel = UIManager.Instance.ShowPanel<MatchPanel>();
        panel.btnCancel.interactable = false;
        if (res.code == 0)
        {
            int seconds = 0;
            TickCorotine = StartCoroutine(TickRoutine(res.message,seconds));
        }
        else if (res.code == 1)
        {
            //更新面板信息 连入战斗服 显示choosecharacterpanel
            panel.ChangeInfo(res.message);
            StopCoroutine(TickCorotine);
            panel.btnCancel.interactable = false;

        }
        
    }
    IEnumerator TickRoutine(string str, int seconds)
    {
        MatchPanel panel = UIManager.Instance.GetPanel<MatchPanel>();
        while(true)
        {
            panel.ChangeInfo(str + "(" + seconds.ToString() + ")");
            if (seconds == 3)
            {
                panel.btnCancel.interactable = true;
            }
            yield return new WaitForSeconds(1f);
            seconds++;
        }
        
    }
    public void StopTick()
    {
        StopCoroutine(TickCorotine);
    }
}

