// 保存"当前登录的是谁"，其他界面要用
using System.Collections.Generic;

public static class GameData
{
    public static int UserId;
    public static string UserName = "";
    public static bool IsPlayMusic = true;
    public static bool IsPlaySound = true;
    public static float Music = 0.7f;
    public static float Sound = 0.7f;
    public static List<CharacterData> CharacterDatas = new List<CharacterData>();
    public static int SelectedCharacterId = 0;


    public static void ApplySetting(bool playMusic, bool playSound, float music, float sound, List<CharacterData> datas)
    {
        IsPlayMusic = playMusic;
        IsPlaySound = playSound;
        Music = music;
        Sound = sound;
        CharacterDatas = datas;
    }

    public static bool IsLogin
    {
        get { return UserId != 0; }
    }

    public static void Clear()
    {
        UserId = 0;
        UserName = "";
        CharacterDatas.Clear();
        SelectedCharacterId = 0;
    }

}
public class CharacterData
{
    public int CharacterId;
    public string CharacterName = "";
    public string CharacterImage = "";
    public string Skill1 = "";
    public string Skill2 = "";
    public string Skill3 = "";
}