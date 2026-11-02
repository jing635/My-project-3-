using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    public Button btnsure;
    public TextMeshProUGUI tip;



    public override void Init()
    {
        btnsure.onClick.AddListener(() =>
        {
            //隐藏面板
        });


    }

    public override void ShowMe()
    {
        base.ShowMe();
        
    }

    public void Changeinfo(string info)
    {
        tip.text = info;
    }

    
}
