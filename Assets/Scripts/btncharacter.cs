using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class btncharacter : MonoBehaviour
{
    public Button btnSelf;
    public TextMeshProUGUI characterName;
    private string strSkill1;
    private string strSkill2;
    private string strSkill3;


    // Start is called before the first frame update
    void Start()
    {
        btnSelf.onClick.AddListener(() =>
        {
            //通知面板改变右侧数据
            CharacterPanel panel = UIManager.Instance.GetPanel<CharacterPanel>();
            panel.UpdataPanel(btnSelf.GetComponent<Image>().sprite,characterName.text,strSkill1,strSkill2,strSkill3);
        });
    }

    //更新自己的方法
    public void InitInfo(CharacterData data)
    {
        characterName.text = data.CharacterName;
        btnSelf.GetComponent<Image>().sprite = Resources.Load<Sprite>("CharacterSprite/" + data.CharacterImage);
        strSkill1 = data.Skill1;
        strSkill2 = data.Skill2;
        strSkill3 = data.Skill3;
    }
    
}
