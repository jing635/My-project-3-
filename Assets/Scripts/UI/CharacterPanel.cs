using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CharacterPanel : BasePanel, IEndDragHandler, IBeginDragHandler
{
    public ScrollRect svCharacters;
    public Button btnLeft;
    public Button btnRight;
    public Button btnCancel;
    public Image igCharacter;
    public TextMeshProUGUI textCharacterName;
    public Button btnSkill1;
    public TextMeshProUGUI textskill1;
    public Button btnSkill2;
    public TextMeshProUGUI textskill2;
    public Button btnSkill3;
    public TextMeshProUGUI textskill3;
    private int currentPageIndex = 0;
    public float snapDuration = 0.3f;
    public Ease snapEase = Ease.OutCubic;
    private Tween snapTween;

    public override void Init()
    {
        //拿到gamedata里的数据
        List<CharacterData> characterList = GameData.CharacterDatas;

        StartCoroutine(LoadCharacterCoroutine(characterList));

        svCharacters.inertia = false;

        btnLeft.onClick.AddListener(() =>
        {
            //换到上一页
            OnClickPrevPage();
        });

        btnRight.onClick.AddListener(() =>
        {
            //换到下一页
            OnClickNextPage();
        });

        btnCancel.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayMusic("click");
            UIManager.Instance.HidePanel<CharacterPanel>();
            UIManager.Instance.ShowPanel<GameLobbyPanel>();
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

    private void Update()
    {
        //更新当前页数
        if (svCharacters.content.childCount <= 1) return;
        float currentPos = svCharacters.horizontalNormalizedPosition;
        currentPageIndex = Mathf.RoundToInt(currentPos * (svCharacters.content.childCount - 1));

    }


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

        currentPageIndex = targetPageIndex;
        float targetPos = (float)targetPageIndex / (total - 1);

        snapTween?.Kill(); // 杀掉旧动画

        snapTween = DOTween.To(
            () => svCharacters.horizontalNormalizedPosition,
            x => svCharacters.horizontalNormalizedPosition = x,
            targetPos,
            snapDuration
        ).SetEase(snapEase);
    }

    private void Tip(string text)
    {
        UIManager.Instance.ShowPanel<TipPanel>().Show(text);
    }

    //更新右侧数据的方法
    public void UpdataPanel(Sprite charactersprite, string characterName, string skill1, string skill2, string skill3)
    {
        igCharacter.sprite = charactersprite;
        textCharacterName.text = characterName;
        textskill1.text = skill1;
        textskill2.text = skill2;
        textskill3.text = skill3;
    }

    IEnumerator LoadCharacterCoroutine(List<CharacterData> characterList)
    {
        Transform contentTransform = svCharacters.content.transform;
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        yield return null;
        int totalPages = (characterList.Count + 19) / 20;
        List<GameObject> list = new List<GameObject>(totalPages);
        for (int i = 0; i < totalPages; i++)
        {
            list[i] = GameObject.Instantiate(Resources.Load<GameObject>("UI/Page"));
            list[i].transform.SetParent(svCharacters.content.transform, false);
            for (int j = 0; j < 20; j++)
            {
                int currentButton = i * 20 + j;
                if ((currentButton) == characterList.Count)
                {
                    break;
                }
                GameObject btnObj = Instantiate(Resources.Load<GameObject>("UI/CharacterButton"));
                btnObj.transform.SetParent(list[i].transform, false);

                btncharacter btnCharacter = btnObj.GetComponent<btncharacter>();
                btnCharacter.InitInfo(characterList[currentButton]);
            }
            Canvas.ForceUpdateCanvases();
            yield return null;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (svCharacters.content.childCount <= 1) return;

        // 1. 获取当前滚动到的位置
        float currentPos = svCharacters.horizontalNormalizedPosition;

        // 2. 算最接近的页码
        float pageIndexFloat = currentPos * (svCharacters.content.childCount - 1);
        int nearestPage = Mathf.RoundToInt(pageIndexFloat);

        // 3. 速度判定（如果快速滑动，多翻一页）
        float velocity = svCharacters.velocity.x;
        if (Mathf.Abs(velocity) > 800f)
        {
            // 注意：ScrollRect 的 velocity 方向与内容的移动方向相反
            nearestPage += velocity > 0 ? -1 : 1;
        }

        // 4. 限制不能超出范围
        nearestPage = Mathf.Clamp(nearestPage, 0, svCharacters.content.childCount - 1);

        // 5. 执行吸附动画
        ScrollToPage(nearestPage);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 打断正在进行的翻页动画
        snapTween?.Kill();
    }
}



    

