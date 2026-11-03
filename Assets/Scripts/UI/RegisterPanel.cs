using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    public TMP_InputField UserName;
    public TMP_InputField PassWord;
    public Button btncancel;
    public Button btnsure;

    public override void Init()
    {
        btncancel.onClick.AddListener(OnClickCancel);
        btnsure.onClick.AddListener(OnClickSure);
        NetMgr.Instance.On<RegisterRes>(OnRegisterRes);
    }

    private void OnClickCancel()
    {
        UIManager.Instance.HidePanel<RegisterPanel>();
        UIManager.Instance.ShowPanel<LoginPanel>();
    }

    private void OnClickSure()
    {
        string user = UserName.text.Trim();
        string pwd = PassWord.text;

        if (user.Length < 6) { Tip("用户名至少 6 位"); return; }
        if (pwd.Length < 6) { Tip("密码至少 6 位"); return; }
        if (!NetMgr.Instance.IsConnected) { Tip("还没连上服务器"); return; }

        btnsure.interactable = false;
        NetMgr.Instance.Send(new RegisterReq { username = user, password = pwd });
    }

    private void OnRegisterRes(RegisterRes res)
    {
        btnsure.interactable = true;

        if (res.code == Code.Ok)
        {
            Tip("注册成功，请登录");
            UIManager.Instance.HidePanel<RegisterPanel>();
            UIManager.Instance.ShowPanel<LoginPanel>();
        }
        else
        {
            Tip(res.message);
        }
    }

    private void Tip(string info)
    {
        UIManager.Instance.ShowPanel<TipPanel>().Show(info);
    }
}