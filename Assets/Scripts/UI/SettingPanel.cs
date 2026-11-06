using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    public Button btncancel;
    public Button btnreturn;
    public Slider sdmusic;
    public Slider sdSound;
    public Toggle tgmusic;
    public Toggle tgsound;



    public override void Init()
    {
        btncancel.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayMusic("click");
            UIManager.Instance.HidePanel<SettingPanel>();
            UIManager.Instance.ShowPanel<GameLobbyPanel>();
        });

        btnreturn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayMusic("click");
            UIManager.Instance.HidePanel<SettingPanel>();
            UIManager.Instance.ShowPanel<LoginPanel>();
        });

        tgmusic.onValueChanged.AddListener((isOn) =>
        {
            //设置音乐开关 保存
        });

        tgsound.onValueChanged.AddListener((isOn) =>
        {
            //设置音效开关
        });

        sdmusic.onValueChanged.AddListener((value) =>
        {
            //设置音乐大小
        });

        sdSound.onValueChanged.AddListener((value) =>
        {
            //设置音效大小
        });
    }

    private void Start()
    {
        tgmusic.isOn = GameData.IsPlayMusic;
        tgsound.isOn = GameData.IsPlaySound;
        sdmusic.value = GameData.Music;
        sdSound.value = GameData.Sound;
    }



}
