// 保存"当前登录的是谁"，其他界面要用
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class GameData
{
    public static int UserId;
    public static string UserName = "";
    public static bool IsPlayMusic = true;
    public static bool IsPlaySound = true;
    public static float Music = 0.7f;
    public static float Sound = 0.7f;
    public static List<User_CharacterData> user_CharacterDatas = new List<User_CharacterData>();
    public static CharacterDatas characterDatas = new CharacterDatas();
    public static int SelectedCharacterId = 0;

    
    
    static void Awake()
    {
        characterDatas = JsonUtility.FromJson<CharacterDatas>(ABMgr.Instance().LoadRes<TextAsset>("json","CharacterDatas").text);
    }
    public static void ApplySetting(bool playMusic, bool playSound, float music, float sound, List<User_CharacterData> datas)
    {
        IsPlayMusic = playMusic;
        IsPlaySound = playSound;
        Music = music;
        Sound = sound;
        user_CharacterDatas = datas;
    }

    public static bool IsLogin
    {
        get { return UserId != 0; }
    }

    public static void Clear()
    {
        UserId = 0;
        UserName = "";
        user_CharacterDatas.Clear();
        SelectedCharacterId = 0;
    }


}
public class CharacterDatas
{
    public List<CharacterData> characterdatas = new List<CharacterData>();
}
