using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    public Button btnsure;
    public TextMeshProUGUI tip;

    public override void Init()
    {
        btnsure.onClick.AddListener(delegate
        {
            UIManager.Instance.HidePanel<TipPanel>();
        });
    }

    public void Show(string info)
    {
        tip.text = info;
    }
}