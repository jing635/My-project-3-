using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCharacterButton : MonoBehaviour
{
     public Button selfButton;
    private CharacterData data;

    void Start()
    {
        selfButton.onClick.AddListener(()=>
        {
            //更新左边面板
        });
    }

    public void InitInfo(CharacterData character)
    {
        data = character;
        selfButton.GetComponent<Image>().sprite = ABMgr.Instance().LoadRes<Sprite>("character", character.CharacterImage);
    }
}
