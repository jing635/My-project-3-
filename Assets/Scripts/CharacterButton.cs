using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public Button selfButton;
    public TextMeshProUGUI characterName;
    private CharacterData data;


    // Start is called before the first frame update
    void Start()
    {
        selfButton.onClick.AddListener(() =>
        {
            //通知面板改变右侧数据
            CharacterPanel panel = UIManager.Instance.GetPanel<CharacterPanel>();
            if (panel == null) return;
            panel.SelectCharacter(data);
        });
    }

    //更新自己的方法
    public void InitInfo(CharacterData character)
    {
        data = character;
        characterName.text = character.CharacterName;
        selfButton.GetComponent<Image>().sprite =Resources.Load<Sprite>("CharacterSprite/" + character.CharacterImage);
    }
    
}
