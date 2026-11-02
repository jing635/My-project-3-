using System.Collections;
using System.Collections.Generic;
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
        btncancel.onClick.AddListener(() =>
        {
            //回到登陆面板
            UIManager.Instance.HidePanel<RegisterPanel>();
            UIManager.Instance.ShowPanel<LoginPanel>();
        });

        btnsure.onClick.AddListener(() =>
        {
            //传消息给服务器  判断用户是否存在
        });
    }

   
    
}
