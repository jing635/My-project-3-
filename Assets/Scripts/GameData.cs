// 保存"当前登录的是谁"，其他界面要用
public static class GameData
{
    public static int UserId;
    public static string UserName = "";
    public static bool IsPlayMusic = true;
    public static bool IsPlaySound = true;
    public static float Music = 0.7f;
    public static float Sound = 0.7f;

    public static void ApplySetting(bool playMusic, bool playSound, int music, int sound)
    {
        IsPlayMusic = playMusic;
        IsPlaySound = playSound;
        Music = music;
        Sound = sound;
    }

    public static bool IsLogin
    {
        get { return UserId != 0; }
    }

    public static void Clear()
    {
        UserId = 0;
        UserName = "";
    }
}