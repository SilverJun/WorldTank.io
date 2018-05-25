using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class UIHelper
{
    public static void AddButtonListener(Button button, Action action)
    {
        button.onClick.AddListener(() => action());
    }

    public static void AddButtonListener(GameObject buttonObject, Action action)
    {
        AddButtonListener(buttonObject.GetComponent<Button>(), action);
    }

    public static void SetText(GameObject textObject, string text)
    {
        textObject.GetComponent<Text>().text = text;
    }

    public static void AutoClose(UI ui, Func<bool> condition)
    {
        ui.StartCoroutine(_autoClose(ui, condition));
    }

    private static IEnumerator _autoClose(UI ui, Func<bool> cond)
    {
        yield return new WaitWhile(cond);
        ui.Close();
    }
}