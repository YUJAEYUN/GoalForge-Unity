using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuGenerator : MonoBehaviour
{
    [MenuItem("GoalForge/Generate Main Menu")]
    public static void GenerateMainMenu()
    {
        // 1. Create Canvas
        GameObject canvasObj = new GameObject("MainMenuCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 2. Create Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = Color.white;
        Sprite bgSprite = LoadSprite("Assets/Art/UI/MainMenuBG.png");
        if (bgSprite != null) bgImage.sprite = bgSprite;
        else bgImage.color = new Color(0.1f, 0.1f, 0.3f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // 3. Create Title
        // 3. Create Title (Logo)
        Sprite logoSprite = LoadSprite("Assets/Art/UI/GameLogo.png");
        if (logoSprite != null)
        {
            GameObject logoObj = new GameObject("LogoImage");
            logoObj.transform.SetParent(canvasObj.transform, false);
            Image logoImage = logoObj.AddComponent<Image>();
            logoImage.sprite = logoSprite;
            logoImage.preserveAspect = true;
            RectTransform logoRect = logoObj.GetComponent<RectTransform>();
            logoRect.anchoredPosition = new Vector2(0, 200);
            logoRect.sizeDelta = new Vector2(600, 200);
        }
        else
        {
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(canvasObj.transform, false);
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "GoalForge";
            titleText.fontSize = 80;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, 200);
            titleRect.sizeDelta = new Vector2(600, 100);
        }

        // 4. Create Buttons
        Sprite btnSprite = LoadSprite("Assets/Art/UI/UIButton.png");

        CreateButton(canvasObj.transform, "StartButton", "START GAME", new Vector2(0, 50), btnSprite, () => {
             // Logic to be handled by BtnCtrl
        });
        
        CreateButton(canvasObj.transform, "ManualButton", "HOW TO PLAY", new Vector2(0, -50), btnSprite, () => {
             // Logic to be handled by BtnCtrl
        });

        CreateButton(canvasObj.transform, "ExitButton", "EXIT", new Vector2(0, -150), btnSprite, () => {
             // Logic to be handled by BtnCtrl
        });

        // 5. Add BtnCtrl
        BtnCtrl btnCtrl = canvasObj.AddComponent<BtnCtrl>();
        
        // Link Buttons to BtnCtrl methods
        LinkButton(canvasObj, "StartButton", btnCtrl, "OnClickStartGame");
        LinkButton(canvasObj, "ManualButton", btnCtrl, "OnClickManual");
        LinkButton(canvasObj, "ExitButton", btnCtrl, "OnClickExit");

        // 6. Create EventSystem
        CreateEventSystem();

        Debug.Log("✅ Main Menu Generated!");
    }

    [MenuItem("GoalForge/Generate Manual UI")]
    public static void GenerateManualUI()
    {
        // 1. Create Canvas
        GameObject canvasObj = new GameObject("ManualCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 2. Create Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = Color.white;
        Sprite bgSprite = LoadSprite("Assets/Art/UI/MainMenuBG.png");
        if (bgSprite != null) bgImage.sprite = bgSprite;
        else bgImage.color = new Color(0.1f, 0.1f, 0.1f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // 3. Create Panel
        GameObject panelObj = new GameObject("InstructionPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(800, 600);

        // 4. Create Text
        GameObject textObj = new GameObject("Instructions");
        textObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI instructions = textObj.AddComponent<TextMeshProUGUI>();
        instructions.text = "HOW TO PLAY\n\n1. Use Arrow Keys to Move\n2. Spacebar to Shoot\n3. Score Goals!\n\nGood Luck!";
        instructions.fontSize = 32;
        instructions.alignment = TextAlignmentOptions.Center;
        instructions.color = Color.white;
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(50, 50);
        textRect.offsetMax = new Vector2(-50, -50);

        // 5. Create Back Button
        Sprite btnSprite = LoadSprite("Assets/Art/UI/UIButton.png");
        CreateButton(canvasObj.transform, "BackButton", "BACK", new Vector2(0, -350), btnSprite, () => { });

        // 6. Add BtnCtrl
        BtnCtrl btnCtrl = canvasObj.AddComponent<BtnCtrl>();
        LinkButton(canvasObj, "BackButton", btnCtrl, "OnClickTitle");

        // 7. Create EventSystem
        CreateEventSystem();

        Debug.Log("✅ Manual UI Generated!");
    }

    private static void CreateEventSystem()
    {
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    private static void CreateButton(Transform parent, string name, string label, Vector2 position, Sprite sprite, System.Action onClickAction)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        
        Image btnImage = btnObj.AddComponent<Image>();
        if (sprite != null) btnImage.sprite = sprite;
        btnImage.color = Color.white;

        Button btn = btnObj.AddComponent<Button>();
        
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchoredPosition = position;
        btnRect.sizeDelta = new Vector2(200, 60);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI btnText = textObj.AddComponent<TextMeshProUGUI>();
        btnText.text = label;
        btnText.fontSize = 24;
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.color = Color.black;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }

    private static void LinkButton(GameObject canvas, string btnName, BtnCtrl controller, string methodName)
    {
        Transform btnTrans = canvas.transform.Find(btnName);
        if (btnTrans != null)
        {
            Button btn = btnTrans.GetComponent<Button>();
            if (btn != null)
            {
                // Correct way to add persistent listener in Editor:
                var methodInfo = typeof(BtnCtrl).GetMethod(methodName);
                if (methodInfo != null)
                {
                    var action = System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), controller, methodInfo) as UnityEngine.Events.UnityAction;
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, action);
                }
                else
                {
                    Debug.LogError($"Method {methodName} not found in BtnCtrl");
                }
            }
        }
    }


    private static Sprite LoadSprite(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null && importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
