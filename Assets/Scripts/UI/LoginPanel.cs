using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    public TMP_InputField UserName;
    public TMP_InputField PassWord;
    public Button btnLogin;
    public Button btnRegister;

    public override void Init()
    {
        AudioManager.Instance.PlayMusic("bk1");
        btnLogin.onClick.AddListener(OnClickLogin);
        btnRegister.onClick.AddListener(OnClickRegister);

        // 告诉 NetMgr：收到"登录结果"就调 OnLoginRes
        NetMgr.Instance.On<LoginRes>(OnLoginRes);
    }

    private void OnClickLogin()
    {
        string user = UserName.text.Trim();
        string pwd = PassWord.text;

        if (user.Length < 6) { Tip("用户名至少 6 位"); return; }
        if (pwd.Length == 0) { Tip("请输入密码"); return; }
        if (!NetMgr.Instance.IsConnected) { Tip("还没连上服务器"); return; }

        btnLogin.interactable = false;      // 等结果期间不能重复点
        

        // 只负责发，不在这里判断对错
        NetMgr.Instance.Send(new LoginReq { username = user, password = pwd });
    }

    // 服务器回消息时自动调到这里
    private void OnLoginRes(LoginRes res)
    {
        btnLogin.interactable = true;

        if (res.code == Code.Ok)
        {
            GameData.UserId = res.userId;
            GameData.UserName = UserName.text.Trim();

            GameData.ApplySetting(res.isPlayMusic, res.isPlaySound, res.music, res.sound, res.characterdatas);
            ApplyAudio();

            TipPanel panel = Tip("正在登录....");
            Destroy(panel, 3f);

            AudioManager.Instance.StopMusic("bk1");
            AudioManager.Instance.PlayMusic("bk2");
            UIManager.Instance.HidePanel<LoginPanel>();
            UIManager.Instance.ShowPanel<GameLobbyPanel>();
        }
        else
        {
            Tip(res.message);       // 服务端说啥就显示啥
        }
    }

    private void ApplyAudio()
    {
        AudioManager.Instance.SetMusicVolume(GameData.IsPlayMusic ? GameData.Music : 0f);
        AudioManager.Instance.SetSoundVolume(GameData.IsPlaySound ? GameData.Sound : 0f);
    }

    private void OnClickRegister()
    {
        AudioManager.Instance.StopMusic("bk1");
        UIManager.Instance.HidePanel<LoginPanel>();
        UIManager.Instance.ShowPanel<RegisterPanel>();
    }
    private TipPanel Tip(string text)
    {
        TipPanel panel = UIManager.Instance.ShowPanel<TipPanel>();
        panel.Show(text);
        return panel;
    }
}