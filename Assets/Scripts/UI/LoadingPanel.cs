using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadingPanel : BasePanel
{
    public UnityEngine.UI.Image CharacterImage1;
    public UnityEngine.UI.Image CharacterImage2;
    public UnityEngine.UI.Image CharacterImage3;
    public UnityEngine.UI.Image CharacterImage4;
    public TextMeshProUGUI CharacterName1;
    public TextMeshProUGUI CharacterName2; 
    public TextMeshProUGUI CharacterName3;
    public TextMeshProUGUI CharacterName4;



    public override void Init()
    {
        NetMgr.Instance.Battle.On<ChooseCharacterRes>(OnChooseCharacterRes);
    }

    private void OnChooseCharacterRes(ChooseCharacterRes res)
    {
        CharacterImage1.sprite = ABMgr.Instance().LoadRes<Sprite>("character",GameData.characterDatas.characterdatas.Find(c => c.CharacterName == res.CharacterName1).CharacterImage);
        CharacterImage2.sprite = ABMgr.Instance().LoadRes<Sprite>("character",GameData.characterDatas.characterdatas.Find(c => c.CharacterName == res.CharacterName2).CharacterImage);
        CharacterImage3.sprite = ABMgr.Instance().LoadRes<Sprite>("character",GameData.characterDatas.characterdatas.Find(c => c.CharacterName == res.CharacterName3).CharacterImage);
        CharacterImage4.sprite = ABMgr.Instance().LoadRes<Sprite>("character",GameData.characterDatas.characterdatas.Find(c => c.CharacterName == res.CharacterName4).CharacterImage);

        CharacterName1.text = res.CharacterName1;
        CharacterName2.text = res.CharacterName2;
        CharacterName3.text = res.CharacterName3;
        CharacterName4.text = res.CharacterName4;
    }
    
    private IEnumerable LoadBattleScene()
    {
        SceneManager.LoadSceneAsync("BattleScene");
        yield return null;
    }
}
