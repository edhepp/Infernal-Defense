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
    //Todo: Create a singleton and dont destroy on load
    //Todo: do something with Options like volume control (Music, Sound FX)
    public delegate void MenuButtons();
    public static event MenuButtons ContinueEvent;
    public static event MenuButtons OptionsEvent;
    public static event MenuButtons StartEvent;
    public static event MenuButtons RestartEvent;
    public static event MenuButtons QuitEvent;

    public delegate void GameState();
    public static event GameState GameOverState; // move to game manager
    public static event GameState PausedState; // move to input manager

    //Todo: access labels (score and lives) threw singalton instance

    [SerializeField]
    private UIDocument UIComponent = null;
    [SerializeField]
    private VisualTreeAsset MainMenu_Asset = null;

    //Quit, restart, score, lives, start, options, Continue
    private Button _continue_B = null;
    private Button _options_B = null;
    private Button _start_B = null;
    private Button _restart_B = null;
    private Button _quit_B = null;

    private Label _gameOver_L = null;
    private Label _lives_L = null;
    private Label _score_L = null;

    private bool _goodUIRefferences = true;
    void Start()
    {
        //todo: Listen for event from Input manager esc for Pausing game
        //todo: Listen for event from Game manager for Game over
        VerifyUIRef();
        // Run code if all refferences check out as good
        if (_goodUIRefferences)
        {
            UIComponent.visualTreeAsset = MainMenu_Asset;
            //Main menu setup
            SetUIElements();
            MainMenu();
            RegisterAllCallbacks();
        }
        else return;
    }
    private void RegisterAllCallbacks()
    {
        _continue_B.RegisterCallback<ClickEvent>(_ => ContinueButton());
        _restart_B.RegisterCallback<ClickEvent>(_ => RestartButton());
        _options_B.RegisterCallback<ClickEvent>(_ => OptionsEvent?.Invoke());
        _start_B.RegisterCallback<ClickEvent>(_ => StartButton());
        _quit_B.RegisterCallback<ClickEvent>(_ => QuitButton());
    }
    private void UnRegisterAllCallbacks()
    {
        _continue_B.UnregisterCallback<ClickEvent>(_ => ContinueEvent?.Invoke());
        _restart_B.UnregisterCallback<ClickEvent>(_ => RestartEvent?.Invoke());
        _options_B.UnregisterCallback<ClickEvent>(_ => OptionsEvent?.Invoke());
        _start_B.UnregisterCallback<ClickEvent>(_ => StartEvent?.Invoke());
        _quit_B.UnregisterCallback<ClickEvent>(_ => QuitEvent?.Invoke());
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
            MainMenu_Asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/Managers/UI/MainMenu_UI.uxml");
            if(MainMenu_Asset)
                Debug.Log("AssetTree MainMenu: set threw code");
            else
                Debug.Log("AssetTree MainMenu not found Assets/Prefabs/Managers/UI/MainMenu_UI.uxml");
        }
        _goodUIRefferences = UIComponent && MainMenu_Asset;
    }
    private VisualElement root = null;
    private void SetUIElements()
    {
        root = UIComponent.rootVisualElement;

        _continue_B = root.Q<Button>("ContinueButton");
            Debug.Log(_continue_B != null ? "_continue_B Set" : "_continue Not found");
        _options_B = root.Q<Button>("OptionsButton");
            Debug.Log(_options_B != null ? "_options_B Set" : "_options_B Not found");
        _start_B = root.Q<Button>("StartButton");
            Debug.Log(_start_B != null ? "_start_B Set" : "_start_B Not found");
        _restart_B = root.Q<Button>("RestartButton");
            Debug.Log(_restart_B != null ? "_restart_B Set" : "_reset_B Not found");
        _quit_B = root.Q<Button>("QuitButton");
            Debug.Log(_quit_B != null ? "_quit_B Set" : "_quit_B Not found");

        _gameOver_L = root.Q<Label>("GameOverLabel");
            Debug.Log(_gameOver_L != null ? "_gameOver_L Set" : "_gameOver_L Not found");
        _lives_L = root.Q<Label>("ScoreLabel");
            Debug.Log(_lives_L != null ? "_lives_L Set" : "_lives_L Not found");
        _score_L = root.Q<Label>("LivesLabel");
            Debug.Log(_score_L != null ? "_score_L Set" : "_score_L Not found");
    }
    private void ContinueButton()
    {
        ContinueEvent?.Invoke();
        GameplayUI();
    }
    private void RestartButton()
    {
        RestartEvent?.Invoke();
        GameplayUI();
    }
    private void StartButton()
    {
        StartEvent?.Invoke();
        GameplayUI();
    }
    private void QuitButton()
    {
        QuitEvent?.Invoke();
        MainMenu();
    }
    private void MainMenu()
    {
        // Continue false, Options true, Start true, Quit true, restart false
        _start_B.style.display = DisplayStyle.Flex;
        _options_B.style.display = DisplayStyle.Flex;
        _quit_B.style.display = DisplayStyle.Flex;

        _continue_B.style.display = DisplayStyle.None;
        _restart_B.style.display = DisplayStyle.None;
        _gameOver_L.style.display = DisplayStyle.None;
        _lives_L.style.display = DisplayStyle.None;
        _score_L.style.display = DisplayStyle.None;
        // GameOver false, Lives false, score false.
    }
    private void GamePlayMenu()
    {
        _continue_B.style.display = DisplayStyle.Flex;
        _options_B.style.display = DisplayStyle.Flex;
        _quit_B.style.display = DisplayStyle.Flex;

        //_lives_L.style.display = DisplayStyle.None;
        //_score_L.style.display = DisplayStyle.None;

        _start_B.style.display = DisplayStyle.None;
        _restart_B.style.display = DisplayStyle.None;
        _gameOver_L.style.display = DisplayStyle.None;
    }
    private void GameplayUI()
    {
        _lives_L.style.display = DisplayStyle.Flex;
        _score_L.style.display = DisplayStyle.Flex;

        _start_B.style.display = DisplayStyle.None;
        _options_B.style.display = DisplayStyle.None;
        _quit_B.style.display = DisplayStyle.None;
        _continue_B.style.display = DisplayStyle.None;
        _restart_B.style.display = DisplayStyle.None;
        _gameOver_L.style.display = DisplayStyle.None;
    }
    private void GameOver()
    {
        _restart_B.style.display = DisplayStyle.Flex;
        _quit_B.style.display = DisplayStyle.Flex;
        _gameOver_L.style.display = DisplayStyle.Flex;

        _start_B.style.display = DisplayStyle.None;
        _options_B.style.display = DisplayStyle.None;
        _continue_B.style.display = DisplayStyle.None;
        _lives_L.style.display = DisplayStyle.None;
        _score_L.style.display = DisplayStyle.None;
    }
    private void OnDestroy()
    {
        UnRegisterAllCallbacks();
    }
}
