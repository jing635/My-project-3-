using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLobbyPanel : BasePanel
{
    public Button btnSetting;
    public Button btnCharacter;
    public Button btnDuelArenaReal;
    public Button btnDuelArenaAi;


    public override void Init()
    {
        btnSetting.onClick.AddListener(OnClickSetting);
        btnCharacter.onClick.AddListener(OnClickCharacter);
        btnDuelArenaReal.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayMusic("click");
            //向服务器发起联机

            UIManager.Instance.HidePanel<GameLobbyPanel>();
            //UIManager.Instance.ShowPanel<ChooseCharacterPanel>();
        });
        btnDuelArenaReal.onClick.AddListener(() =>
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

    
}
