using System.Collections;
using System.Collections.Generic;
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
        btnRegister.onClick.AddListener(() =>
        {
            //换到注册面板
            UIManager.Instance.HidePanel<LoginPanel>();
            UIManager.Instance.ShowPanel<RegisterPanel>();
        });

        btnLogin.onClick.AddListener(() =>
        {
            if (UserName.text < 7)
            {
                TipPanel panel = UIManager.Instance.ShowPanel<TipPanel>();
                panel.Changeinfo("用户名必须大于6位数");
            }
            else
            {
                //传消息到服务端 验证用户是否存在以及密码是否正确
                if(IsExistUser(UserName.text))
                {
                    //验证密码是否正确
                    if(IsRightPw(PassWord.text))
                    {
                        //进入游戏大厅,
                    }
                    else
                    {
                        TipPanel panel = UIManager.Instance.ShowPanel<TipPanel>();
                        panel.Changeinfo("密码错误");
                    }
                }
                else
                {
                    TipPanel panel = UIManager.Instance.ShowPanel<TipPanel>();
                    panel.Changeinfo("用户名不存在");
                }
            }
            
        });
    }
    
    private bool IsExistUser(string username)
    {
        
    }

    private bool IsRightPw(string password)
    {

    }
    
}
