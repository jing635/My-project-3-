using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="Character1" , menuName ="Character/Character1")]
public class Character : ScriptableObject
{
    public string strcharacter;
    //角色图片
    public Sprite spcharacter;
    //角色技能描述
    public string strcharacterskill;
}
