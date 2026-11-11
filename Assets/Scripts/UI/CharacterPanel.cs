using DG.Tweening;
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
    public Button btnCancel;
    public Image igCharacter;
    public TextMeshProUGUI textCharacterName;
    public Button btnSkill1;
    public Button btnSkill2;
    public Button btnSkill3;

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
        StartCoroutine(LoadCharacterCoroutine(GameData.CharacterDatas));

        svCharacters.inertia = false;

        // ScrollRect 会吃掉拖拽事件，挂个中转在它身上
        ScrollDragRelay relay = svCharacters.gameObject.GetComponent<ScrollDragRelay>();
        if (relay == null) relay = svCharacters.gameObject.AddComponent<ScrollDragRelay>();
        relay.OnDragBegin = () => { if (snapTween != null) snapTween.Kill(); };
        relay.OnDragEnd = SnapToNearestPage;

        btnLeft.onClick.AddListener(OnClickPrevPage);
        btnRight.onClick.AddListener(OnClickNextPage);

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

        Sprite icon = Resources.Load<Sprite>("CharacterSprite/" + data.CharacterImage);
        UpdatePanel(icon, data.CharacterName, data.Skill1, data.Skill2, data.Skill3);
    }

    // 刷新右侧显示
    private void UpdatePanel(Sprite characterSprite, string characterName, string skill1, string skill2, string skill3)
    {
        igCharacter.sprite = characterSprite;
        textCharacterName.text = characterName;
        strSkill1 = skill1;
        strSkill2 = skill2;
        strSkill3 = skill3;
    }

    // ===== 建格子 =====
    IEnumerator LoadCharacterCoroutine(List<CharacterData> characterList)
    {
        Transform contentTransform = svCharacters.content.transform;

        // 清掉上一次留下的格子（Destroy 是延迟到帧末生效，所以下面要等一下）
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        yield return null;

        int totalPages = (characterList.Count + 19) / 20;

        for (int i = 0; i < totalPages; i++)
        {
            GameObject page = Instantiate(Resources.Load<GameObject>("UI/Page"));
            page.transform.SetParent(contentTransform, false);

            for (int j = 0; j < 20; j++)
            {
                int index = i * 20 + j;
                if (index >= characterList.Count) break;

                GameObject btnObj = Instantiate(Resources.Load<GameObject>("UI/CharacterButton"));
                btnObj.transform.SetParent(page.transform, false);

                CharacterButton btn = btnObj.GetComponent<CharacterButton>();
                if (btn != null) btn.InitInfo(characterList[index]);
            }

            Canvas.ForceUpdateCanvases();
            yield return null;
        }

        // 所有格子建完之后，决定右边默认显示谁
        ShowDefaultCharacter(characterList);
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
        JumpToPage(pageIndex);
    }

    private void JumpToPage(int pageIndex)
    {
        int total = svCharacters.content.childCount;
        if (total <= 1) return;

        currentPageIndex = Mathf.Clamp(pageIndex, 0, total - 1);
        // 首次打开直接用瞬移，不要动画
        svCharacters.horizontalNormalizedPosition = (float)currentPageIndex / (total - 1);
    }
    // ===== 翻页 =====
    private void OnClickNextPage()
    {
        int total = svCharacters.content.childCount;
        if (total <= 1 || currentPageIndex >= total - 1) return;
        ScrollToPage(currentPageIndex + 1);
    }

    private void OnClickPrevPage()
    {
        if (svCharacters.content.childCount <= 1 || currentPageIndex <= 0) return;
        ScrollToPage(currentPageIndex - 1);
    }

    private void ScrollToPage(int targetPageIndex)
    {
        int total = svCharacters.content.childCount;
        if (total <= 1) return;

        currentPageIndex = Mathf.Clamp(targetPageIndex, 0, total - 1);
        float targetPos = (float)currentPageIndex / (total - 1);

        if (snapTween != null) snapTween.Kill();

        snapTween = DOTween.To(
            () => svCharacters.horizontalNormalizedPosition,
            x => svCharacters.horizontalNormalizedPosition = x,
            targetPos,
            snapDuration
        ).SetEase(snapEase);
    }

    // 松手后吸附到最近一页（由 ScrollDragRelay 调）
    private void SnapToNearestPage()
    {
        if (svCharacters.content.childCount <= 1) return;

        float currentPos = svCharacters.horizontalNormalizedPosition;
        int nearestPage = Mathf.RoundToInt(currentPos * (svCharacters.content.childCount - 1));

        float velocity = svCharacters.velocity.x;
        if (Mathf.Abs(velocity) > 800f)
        {
            nearestPage += velocity > 0 ? -1 : 1;
        }

        nearestPage = Mathf.Clamp(nearestPage, 0, svCharacters.content.childCount - 1);
        ScrollToPage(nearestPage);
    }

    private void Tip(string text)
    {
        UIManager.Instance.ShowPanel<TipPanel>().Show(text);
    }
}




