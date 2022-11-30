using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIText
{
    public static void SetText(string uiText)
    {
        GameObject.Find("UIText").GetComponent<Text>().text = uiText;
    }

    public static void ClearText()
    {
        GameObject.Find("UIText").GetComponent<Text>().text = "";
    }
}
