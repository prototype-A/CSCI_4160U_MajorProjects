using System;
using UnityEngine;

public static class Utils {

    public static float CalculateRectRight(float currVal, int maxVal, int rectRightAtZero, int rectRightAtFull) {
        // Calculates the RectTransform's 'right' value to properly "deplete" bars such as hp bars
        return rectRightAtZero - (currVal / maxVal) * (Math.Abs(rectRightAtFull) + Math.Abs(rectRightAtZero));
    }

    public static void SetRectLeft(RectTransform rect, float left) {
        rect.offsetMin = new Vector2(left, rect.offsetMin.y);
    }

    public static void SetRectRight(RectTransform rect, float right) {
        rect.offsetMax = new Vector2(-right, rect.offsetMax.y);
    }

    public static void SetRectTop(RectTransform rect, float top) {
        rect.offsetMax = new Vector2(rect.offsetMax.x, -top);
    }

    public static void SetRectBottom(RectTransform rect, float bottom) {
        rect.offsetMin = new Vector2(rect.offsetMin.x, bottom);
    }

    public static float GetRectLeft(RectTransform rect) {
        return rect.offsetMin.x;
    }

    public static float GetRectRight(RectTransform rect) {
        return -rect.offsetMax.x;
    }

    public static float GetRectTop(RectTransform rect) {
        return -rect.offsetMax.y;
    }

    public static float GetRectBottom(RectTransform rect) {
        return rect.offsetMin.y;
    }

}
