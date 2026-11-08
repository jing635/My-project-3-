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
        tgmusic.isOn = GameData.IsPlayMusic;
        tgsound.isOn = GameData.IsPlaySound;
        sdmusic.interactable = tgmusic.isOn;
        sdSound.interactable = tgsound.isOn;
        sdmusic.value = GameData.Music;
        sdSound.value = GameData.Sound;

        tgmusic.onValueChanged.AddListener((isOn)=>
        {
            GameData.IsPlayMusic =isOn;
            sdmusic.interactable = isOn;
            ApplyAudio(); 
        });
        tgsound.onValueChanged.AddListener((isOn)=>
        {
            GameData.IsPlaySound =isOn;
            sdSound.interactable = isOn;
            ApplyAudio();
        });
        sdmusic.onValueChanged.AddListener((v)=>
        {
            GameData.Music =v;
            ApplyAudio();
        });
        sdSound.onValueChanged.AddListener((v)=>
        {
            GameData.Sound =v;
            ApplyAudio();
        });

        btncancel.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayMusic("click");
            Save();
            UIManager.Instance.HidePanel<SettingPanel>();
            UIManager.Instance.ShowPanel<GameLobbyPanel>();
        });

        btnreturn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayMusic("click");
            Save();
            UIManager.Instance.HidePanel<SettingPanel>();
            GameData.Clear();
            UIManager.Instance.ShowPanel<LoginPanel>();
            AudioManager.Instance.StopMusic("bk2");
        });

    }

    private void ApplyAudio()
    {
        AudioManager.Instance.SetMusicVolume(GameData.IsPlayMusic ? GameData.Music : 0f);
        AudioManager.Instance.SetSoundVolume(GameData.IsPlaySound ? GameData.Sound : 0f);
    }

    private void Save()
    {
        if (!NetMgr.Instance.IsConnected) return;

        NetMgr.Instance.Send(new SaveSettingReq
        {
            isPlayMusic = GameData.IsPlayMusic,
            isPlaySound = GameData.IsPlaySound,
            music = GameData.Music,
            sound = GameData.Sound
        });
    }



}
