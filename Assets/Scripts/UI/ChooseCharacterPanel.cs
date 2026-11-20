using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCharacterPanel : BasePanel
{
    public ScrollRect svChooseCharacter;
    public Button btnLast;
    public Button btnNext;
    public Button btnSure;
    private GameObject FristCharacter;
    private GameObject SecondCharacter;
    private int currentPageIndex;

    public override void Init()
    {
        LoadUser_Character(ShowUser_Characters(GameData.user_CharacterDatas,GameData.characterDatas.characterdatas));
        btnLast.onClick.AddListener(()=>
        {
            PageTool.OnClickPrevPage(svChooseCharacter,currentPageIndex);
        });
        btnNext.onClick.AddListener(()=>
        {
            PageTool.OnClickNextPage(svChooseCharacter,currentPageIndex);
        });
        btnSure.onClick.AddListener(()=>
        {
            if (FristCharacter == null || SecondCharacter == null)
            {
                return;
            }
            //换到加载界面，异步加载战斗场景
            UIManager.Instance.HidePanel<ChooseCharacterPanel>();
            NetMgr.Instance.Battle.Send(new ChooseCharacterReq() { CharacterName1 = FristCharacter.transform.Find("CharacterName").GetComponent<Text>().text, 
                                                                    CharacterName2 = SecondCharacter.transform.Find("CharacterName").GetComponent<Text>().text });
            //UIManager.Instance.ShowPanel<LoadingPanel>();
        });
    }

    private void LoadUser_Character(List<CharacterData> datas)
    {
        Transform tf = svChooseCharacter.content.transform;
        GameObject ChooseCharacterButton = Resources.Load<GameObject>("UI/ChooseCharacterButton");
        GameObject ChooseCharacterPage = Resources.Load<GameObject>("UI/ChooseCharacterPage");

        foreach (Transform child in tf)
        {
            Destroy(child.gameObject);
        }

        int totalPages = (datas.Count + 14) / 15;

        for (int i = 0; i < totalPages; i++)
        {
            GameObject page = Instantiate(ChooseCharacterPage);
            page.transform.SetParent(tf, false);

            for (int j = 0; j < 20; j++)
            {
                int index = i * 20 + j;
                if (index >= datas.Count) break;

                GameObject btnObj = Instantiate(ChooseCharacterButton);
                btnObj.transform.SetParent(page.transform, false);

                ChooseCharacterButton btn = btnObj.GetComponent<ChooseCharacterButton>();
                if (btn != null) btn.InitInfo(datas[j + i * 15]);
            }
        }
    }

    private List<CharacterData> ShowUser_Characters(List<User_CharacterData> user_Characters, List<CharacterData> characterList)
    {
        return characterList 
            .OrderByDescending(c => user_Characters.Any(uc => uc.CharacterName == c.CharacterName))
            .ThenBy(c => c.CharacterId)
            .ToList();
    }

    public void UpdataPanel(CharacterData Character)
    {
        if (FristCharacter == null)
        {
            FristCharacter = GameObject.Find("FristCharacter");
            FristCharacter.transform.Find("CharacterImage").GetComponent<Image>().sprite = ABMgr.Instance().LoadRes<Sprite>("character", Character.CharacterImage);
            FristCharacter.transform.Find("CharacterName").GetComponent<Text>().text = Character.CharacterName;
        }
        else if (SecondCharacter == null)
        {
            SecondCharacter = GameObject.Find("SecondCharacter");
            SecondCharacter.transform.Find("CharacterImage").GetComponent<Image>().sprite = ABMgr.Instance().LoadRes<Sprite>("character", Character.CharacterImage);
            SecondCharacter.transform.Find("CharacterName").GetComponent<Text>().text = Character.CharacterName;
        }
    }
}
