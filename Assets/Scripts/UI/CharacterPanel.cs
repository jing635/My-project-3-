using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CharacterPanel : BasePanel
{
    public ScrollRect svCharacters;
    public Button btnLeft;
    public Button btnRight;
    public Image igCharacter;
    public TextMeshProUGUI textCharacterName;
    public Button btnSkill1;
    public TextMeshProUGUI textskill1;
    public Button btnSkill2;
    public TextMeshProUGUI textskill2;
    public Button btnSkill3;
    public TextMeshProUGUI textskill3;
    private List<Character> characterList;


    public override void Init()
    {
        //拿到gamedata里的数据
        characterList = GameData.CharacterDatas;

        StartCoroutine(LoadCharacterCoroutine(characterList));

        btnLeft.onClick.AddListener(() =>
        {
            //换到上一页
        });

        btnRight.onClick.AddListener(() =>
        {
            //换到下一页
        });

        btnSkill1.onClick.AddListener(() =>
        {
            //显示一技能介绍
            Tip(textskill1.text);
        });

        btnSkill2.onClick.AddListener(() =>
        {
            //显示二技能介绍
            Tip(textskill2.text);
        });

        btnSkill3.onClick.AddListener(() =>
        {
            //显示三技能介绍
            Tip(textskill3.text);
        });
    }

    //显示技能介绍
    private void Tip(string text)
    {
        UIManager.Instance.ShowPanel<TipPanel>().Show(text);
    }

    //更新右侧数据的方法
    public void UpdataPanel(Image characterImage, string characterName, string skill1, string skill2, string skill3)
    {
        igCharacter = characterImage;
        textCharacterName = characterName;
        textskill1 = skill1;
        textskill2 = skill2;
        textskill3 = skill3;
    }

    IEnumerator LoadCharacterCoroutine(List<CharacterData> characterList)
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            GameObject btnObj = Instantiate(Resources.Load<GameObject>("Ui/CharacterButton"));
            btnObj.transform.SetParent(svCharacters.content, false);

            btncharacter btnCharacter = btnObj.GetComponent<btncharacter>();
            btnCharacter.InitInfo(characterList[i]);
            if (i % 5 == 0)
            {
                yield return null;
            }
        }
    }
}
