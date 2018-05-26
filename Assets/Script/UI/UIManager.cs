using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum UIType
    {
        /// <summary>
        /// 일반적인 UI.
        /// </summary>
        Normal,

        /// <summary>
        /// 팝업창에서 사용되는 Type. ui 바깥 터치를 막음.
        /// </summary>
        Modal,

        /// <summary>
        /// 팝업창에서 사용되는 Type. ui 바깥을 터치하면 창이 닫힘.
        /// </summary>
        ClosingModal,
    }

    public static readonly List<string> CloseButtonNames = new List<string>()
        {
            "closeButton",
            "ui:closeButton"
        };

    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = (UIManager)FindObjectOfType(typeof(UIManager));
                if (!instance)
                {
                    GameObject uiManagerPrefab = Resources.Load<GameObject>("Prefab/Canvas-UIManager");
                    instance = Instantiate<GameObject>(uiManagerPrefab).GetComponent<UIManager>();
                    if (GameObject.Find("EventSystem") == null)
                        Instantiate(Resources.Load("Prefab/EventSystem"));
                }

            }

            return instance;
        }
    }

    private List<UI> uiList = new List<UI>();
    private List<UI> uiModalList = new List<UI>();

	[SerializeField] private GameObject nameCanvas;
	[SerializeField] private GameObject uiCanvas;


    public static T OpenUI<T>(string filepath, UIType type = UIType.Normal) where T : UI
    {
        GameObject uiPrefab = Resources.Load<GameObject>(filepath);
        return OpenUI<T>(uiPrefab, type);
    }

    public static T OpenUI<T>(GameObject uiPrefab, UIType type = UIType.Normal) where T : UI
    {
        GameObject uiObject = Instantiate(uiPrefab);
		uiObject.transform.SetParent(Instance.uiCanvas.transform, false);

        T ui = uiObject.GetComponent<T>();
        if (ui == null)
            ui = uiObject.AddComponent<T>();

        Instance.uiList.Add(ui);

        if (type == UIType.Modal || type == UIType.ClosingModal)
        {
            Instance.uiModalList.Add(ui);
            CreateEmptyBackground(ui, type == UIType.ClosingModal);
        }

        return ui;
    }

	public static T OpenUIPhoton<T>(string filepath, UIType type = UIType.Normal) where T : UI
	{
		GameObject uiObject = PhotonNetwork.Instantiate(filepath, Vector3.zero, Quaternion.identity, 0);

        T ui = uiObject.GetComponent<T>();
        if (ui == null)
            ui = uiObject.AddComponent<T>();

        Instance.uiList.Add(ui);

        if (type == UIType.Modal || type == UIType.ClosingModal)
        {
            Instance.uiModalList.Add(ui);
            CreateEmptyBackground(ui, type == UIType.ClosingModal);
        }

        return ui;
	}

	public static void AddChildNameCanvas(GameObject gameObject)
	{
		gameObject.transform.SetParent(instance.nameCanvas.transform, false);
	}

    public static void CloseUI(UI ui)
    {
        ui.OnClose();

        Instance.uiList.Remove(ui);
        if (Instance.uiModalList.Contains(ui))
        {
            Instance.uiModalList.Remove(ui);
        }

        Destroy(ui.gameObject);
    }

	public static void CloseUIPhoton(UI ui)
    {
        ui.OnClose();

        Instance.uiList.Remove(ui);
        if (Instance.uiModalList.Contains(ui))
        {
            Instance.uiModalList.Remove(ui);
        }

        PhotonNetwork.Destroy(ui.gameObject);
    }

    /// <summary>
    /// 정말 필요한 상황에만 사용할 것. 남발하면 코드가 꼬일 여지가 큼.
    /// </summary>
    public static T GetUI<T>() where T : UI
    {
        foreach (UI ui in Instance.uiList)
        {
            if (ui is T)
                return (T)ui;
        }

        return null;
    }

    /// <summary>
    /// 보통 안드로이드 폰 back 버튼 매핑에 사용 됨.
    /// </summary>
    public static void CloseTopModal()
    {
        int count = Instance.uiModalList.Count;
        if (count > 0)
        {
            Instance.uiModalList[count - 1].Close();
        }
    }

    private static void CreateEmptyBackground(UI ui, bool isClosing)
    {
        GameObject emptyBackground = Instantiate(Resources.Load("Prefab/EmptyBackgroundButton"), ui.transform) as GameObject;
        emptyBackground.transform.SetAsFirstSibling();

        emptyBackground.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (isClosing)
                    ui.Close();
            });
    }
}