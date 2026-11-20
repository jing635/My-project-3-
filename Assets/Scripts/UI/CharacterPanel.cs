using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CharacterPanel : BasePanel
{
    public ScrollRect svCharacters;
    public Button btnLeft;
    public Button btnRight;
    public Button btnCancel;
    public Image igCharacter;
    public TextMeshProUGUI textCharacterName;
    public Button btnSkill1;
    public Button btnSkill2;
    public Button btnSkill3;
    public Slider sdShards;
    public TextMeshProUGUI textShards;
    public TextMeshProUGUI textlev;

    // 三个技能文本，点角色时刷新
    private string strSkill1 = "请先选择一个角色";
    private string strSkill2 = "请先选择一个角色";
    private string strSkill3 = "请先选择一个角色";

    // 这次打开期间，右边正在显示谁
    private CharacterData currentCharacter;

    private int currentPageIndex = 0;
    public float snapDuration = 0.3f;
    public Ease snapEase = Ease.OutCubic;
    private Tween snapTween;

    public override void Init()
    {
        StartCoroutine(LoadCharacterCoroutine(GameData.characterDatas.characterdatas));

        svCharacters.inertia = false;

        // ScrollRect 会吃掉拖拽事件，挂个中转在它身上
        ScrollDragRelay relay = svCharacters.gameObject.GetComponent<ScrollDragRelay>();
        if (relay == null) relay = svCharacters.gameObject.AddComponent<ScrollDragRelay>();
        relay.OnDragBegin = () => { if (snapTween != null) snapTween.Kill(); };
        relay.OnDragEnd =() => PageTool.SnapToNearestPage(svCharacters,currentPageIndex);

        btnLeft.onClick.AddListener(()=>
        {
            PageTool.OnClickPrevPage(svCharacters,currentPageIndex);
        });

        btnRight.onClick.AddListener(()=>
        {
            PageTool.OnClickNextPage(svCharacters,currentPageIndex);
        });

        btnCancel.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayMusic("click");
            UIManager.Instance.HidePanel<CharacterPanel>();
            UIManager.Instance.ShowPanel<GameLobbyPanel>();
        });

        btnSkill1.onClick.AddListener(() => Tip(strSkill1));
        btnSkill2.onClick.AddListener(() => Tip(strSkill2));
        btnSkill3.onClick.AddListener(() => Tip(strSkill3));
    }

    // ===== 选中一个角色：这是唯一改"当前角色"的入口 =====
    public void SelectCharacter(CharacterData data)
    {
        if (data == null) return;

        currentCharacter = data;                        // 本次打开期间用
        GameData.SelectedCharacterId = data.CharacterId; // 记到全局，下次进来用


        User_CharacterData owned = GameData.user_CharacterDatas.Find(u => u.CharacterName == data.CharacterName);
        Sprite icon = ABMgr.Instance().LoadRes<Sprite>("character", data.CharacterImage);

        int level  = owned != null ? owned.CharacterLevel  : 0;
        int shards = owned != null ? owned.CharacterShards : 0;

        UpdatePanel(icon, data.CharacterName, level, shards, data.CharacterShardsCount, data.Skill1, data.Skill2, data.Skill3);
    }

    // 刷新右侧显示
    private void UpdatePanel(Sprite characterSprite, string characterName, int characterLevel, int characterShards, int characterShardsCount, string skill1, string skill2, string skill3)
    {
        igCharacter.sprite = characterSprite;
        textCharacterName.text = characterName;
        textlev.text = "lev: " + characterLevel;
        textShards.text = characterShards + "/" + characterShardsCount;
        sdShards.maxValue = characterShardsCount;
        sdShards.value = characterShards;
        strSkill1 = skill1;
        strSkill2 = skill2;
        strSkill3 = skill3;
    }

    // ===== 建格子 =====
    IEnumerator LoadCharacterCoroutine(List<CharacterData> characterList)
    {
        Transform contentTransform = svCharacters.content.transform;
        GameObject buttonPrefab = Resources.Load<GameObject>("UI/CharacterButton");
        GameObject pagePrefab = Resources.Load<GameObject>("UI/CharacterPage");

        var CharacterListSorted = ShowUser_CharactersFirst(GameData.user_CharacterDatas, characterList);
        // 清掉上一次留下的格子
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        yield return null;

        int totalPages = (characterList.Count + 19) / 20;

        for (int i = 0; i < totalPages; i++)
        {
            GameObject page = Instantiate(pagePrefab);
            page.transform.SetParent(contentTransform, false);

            for (int j = 0; j < 20; j++)
            {
                int index = i * 20 + j;
                if (index >= characterList.Count) break;

                GameObject btnObj = Instantiate(buttonPrefab);
                btnObj.transform.SetParent(page.transform, false);

                CharacterButton btn = btnObj.GetComponent<CharacterButton>();
                if (btn != null) btn.InitInfo(CharacterListSorted[index]);

                bool isOwned = GameData.user_CharacterDatas.Any(uc => uc.CharacterName == CharacterListSorted[index].CharacterName);
                Image img = btnObj.GetComponent<Image>();
                if (img != null)
                {
                    img.color = isOwned ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
                }
            }

            Canvas.ForceUpdateCanvases();
            yield return null;
        }

        // 所有格子建完之后，决定右边默认显示谁
        ShowDefaultCharacter(characterList);
    }

    //把自己拥有的角色显示在前面
    private List<CharacterData> ShowUser_CharactersFirst(List<User_CharacterData> user_Characters, List<CharacterData> characterList)
    {
        return characterList 
            .OrderByDescending(c => user_Characters.Any(uc => uc.CharacterName == c.CharacterName))
            .ThenBy(c => c.CharacterId)
            .ToList();
    }

    // 打开面板时决定右边显示哪一个
    private void ShowDefaultCharacter(List<CharacterData> characterList)
    {
        if (characterList.Count == 0) return;

        CharacterData target = null;

        // 1. 上次选过的那个还在，就继续显示它
        if (GameData.SelectedCharacterId != 0)
        {
            for (int i = 0; i < characterList.Count; i++)
            {
                if (characterList[i].CharacterId == GameData.SelectedCharacterId)
                {
                    target = characterList[i];
                    break;
                }
            }
        }

        // 2. 没选过 / 选的那个已经被删了 → 用列表第一个（按 id 升序就是 id=1）
        if (target == null) target = characterList[0];

        SelectCharacter(target);

        int pageIndex = characterList.IndexOf(target) / 20;
        PageTool.JumpToPage(svCharacters,currentPageIndex,pageIndex);
    }
    
    private void Tip(string text)
    {
        UIManager.Instance.ShowPanel<TipPanel>().Show(text);
    }
}




