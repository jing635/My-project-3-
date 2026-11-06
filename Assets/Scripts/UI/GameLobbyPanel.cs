using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLobbyPanel : BasePanel
{
    public Button btnSetting;
    public Button btnCharacter;



    public override void Init()
    {
        btnSetting.onClick.AddListener(OnClickSetting);
        btnCharacter.onClick.AddListener(OnClickCharacter);
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
        //UIManager.Instance.ShowPanel<CharacterPanel>();
    }

    
}
