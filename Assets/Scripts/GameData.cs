// 保存"当前登录的是谁"，其他界面要用
public static class GameData
{
    public static int UserId;
    public static string UserName = "";

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