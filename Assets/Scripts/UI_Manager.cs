using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;

public class UI_Manager : MonoBehaviour
{
    [SerializeField]
    private UIDocument UIComponent = null;
    [SerializeField]
    private VisualTreeAsset MainMenu_Asset = null;
    [SerializeField]
    private VisualTreeAsset GamePlay_UI_Asset = null;
    [SerializeField]
    private VisualTreeAsset GamePlay_Menu_Asset = null;

    private VisualElement MainMenuRoot = null;
    private VisualElement GameplayMenuRoot = null;
    private VisualElement UIGameplayRoot = null;

    //Quit, restart, score, lives, start, options, Continue

    private bool _goodUIRefferences = true;
    void Start()
    {
        VerifyUIRef();
        // Run code if all refferences check out as good
        if (_goodUIRefferences)
        {
            SetUIRoots();
            //Main menue setup
            //UIComponent.rootVisualElement.Add(MainMenuRoot);
        }
        else return;
    }
    private void VerifyUIRef()
    {
        if (UIComponent == null)
        {
            UIComponent = GetComponent<UIDocument>();
            if(UIComponent)
                Debug.Log("UIDocument component not found: Set threw code");
            else
                Debug.Log("UIDocument Component not found");

        }
        if(MainMenu_Asset == null)
        {
            MainMenu_Asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/Managers/UI/Main_Menu.uxml");
            if(MainMenu_Asset)
                Debug.Log("AssetTree MainMenu not found: set threw code");
            else
                Debug.Log("AssetTree MainMenu not found Assets/Prefabs/Managers/UI/Main_Menu.uxml");
        }
        if(GamePlay_UI_Asset == null)
        {
            GamePlay_Menu_Asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/Managers/UI/GamePlay_Menu.uxml");
            if (GamePlay_UI_Asset)
                Debug.Log("AssetTree Gameplay_UI_root gameplay not found: set threw code");
            else
                Debug.Log("AssetTree Gameplay_UI not found Assets/Prefabs/Managers/UI/GamePlay_Menu.uxml");
        }
        if(GamePlay_UI_Asset == null)
        {
            GamePlay_UI_Asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/Managers/UI/GamePlay_UI.uxml");
            if (GamePlay_UI_Asset)
                Debug.Log("AssetTree GamePlay_UI not founs: set threw code");
            else
                Debug.Log("AssetTree Gameplay_UI not found Assets/Prefabs/Managers/UI/GamePlay_UI.uxml");
        }
        _goodUIRefferences = UIComponent & MainMenu_Asset & GamePlay_Menu_Asset & GamePlay_UI_Asset;
    }
    private void SetUIRoots()
    {
        MainMenuRoot = MainMenu_Asset.CloneTree();
        UIGameplayRoot = GamePlay_UI_Asset.CloneTree();
        GameplayMenuRoot = GamePlay_Menu_Asset.CloneTree();
    }
    private void RemoveAndReplaceVisualTree(VisualTreeAsset replaceWith)
    {

    }
    private void GameOver()
    {
        // remove Gameplay UI
        // Add a Game over Screen
    }
    private void GameStarted()
    {
        // add Gameplay UI when game is started 
        // remove menu
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
