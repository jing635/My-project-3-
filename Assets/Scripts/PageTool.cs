using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public static class PageTool 
{
    private static float snapDuration = 0.3f;
    private static Ease snapEase = Ease.OutCubic;
    private static Tween snapTween;

    public static void JumpToPage(ScrollRect sv, int currentPageIndex, int pageIndex)
    {
        int total = sv.content.childCount;
        if (total <= 1) return;

        currentPageIndex = Mathf.Clamp(pageIndex, 0, total - 1);
        // 首次打开直接用瞬移，不要动画
        sv.horizontalNormalizedPosition = (float)currentPageIndex / (total - 1);
    }

    public static void OnClickNextPage(ScrollRect sv, int currentPageIndex)
    {
        int total = sv.content.childCount;
        if (total <= 1 || currentPageIndex >= total - 1) return;
        ScrollToPage(sv,currentPageIndex,currentPageIndex + 1);
    }

    public static void OnClickPrevPage(ScrollRect sv, int currentPageIndex)
    {
        if (sv.content.childCount <= 1 || currentPageIndex <= 0) return;
        ScrollToPage(sv,currentPageIndex,currentPageIndex - 1);
    }

    public static void ScrollToPage(ScrollRect sv, int currentPageIndex, int targetPageIndex)
    {
        int total = sv.content.childCount;
        if (total <= 1) return;

        currentPageIndex = Mathf.Clamp(targetPageIndex, 0, total - 1);
        float targetPos = (float)currentPageIndex / (total - 1);

        if (snapTween != null) snapTween.Kill();

        snapTween = DOTween.To(
            () => sv.horizontalNormalizedPosition,
            x => sv.horizontalNormalizedPosition = x,
            targetPos,
            snapDuration
        ).SetEase(snapEase);
    }

    public static void SnapToNearestPage(ScrollRect sv, int currentPageIndex)
    {
        if (sv.content.childCount <= 1) return;

        float currentPos = sv.horizontalNormalizedPosition;
        int nearestPage = Mathf.RoundToInt(currentPos * (sv.content.childCount - 1));

        float velocity = sv.velocity.x;
        if (Mathf.Abs(velocity) > 800f)
        {
            nearestPage += velocity > 0 ? -1 : 1;
        }

        nearestPage = Mathf.Clamp(nearestPage, 0, sv.content.childCount - 1);
        ScrollToPage(sv,currentPageIndex,nearestPage);
    }

}
