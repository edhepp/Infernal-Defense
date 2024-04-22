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

    //Quit, restart, score, lives, start, options, Continue
    private Button _continue_B = null;
    private Button _options_B = null;
    private Button _start_B = null;
    private Button _reset_B = null;
    private Button _quit_B = null;

    private Label _gameOver_L = null;
    private Label _lives_L = null;
    private Label _score_L = null;

    private bool _goodUIRefferences = true;
    void Start()
    {
        VerifyUIRef();
        // Run code if all refferences check out as good
        if (_goodUIRefferences)
        {
            UIComponent.visualTreeAsset = MainMenu_Asset;
            //Main menu setup
            SetUIElements();
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
            MainMenu_Asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Prefabs/Managers/UI/MainMenu_UI.uxml");
            if(MainMenu_Asset)
                Debug.Log("AssetTree MainMenu: set threw code");
            else
                Debug.Log("AssetTree MainMenu not found Assets/Prefabs/Managers/UI/MainMenu_UI.uxml");
        }
        _goodUIRefferences = UIComponent & MainMenu_Asset;
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
        _reset_B = root.Q<Button>("ResetButton");
            Debug.Log(_reset_B != null ? "_reset_B Set" : "_reset_B Not found");
        _quit_B = root.Q<Button>("QuitButton");
            Debug.Log(_quit_B != null ? "_quit_B Set" : "_quit_B Not found");

        _gameOver_L = root.Q<Label>("GameOverLabel");
            Debug.Log(_gameOver_L != null ? "_gameOver_L Set" : "_gameOver_L Not found");
        _lives_L = root.Q<Label>("ScoreLabel");
            Debug.Log(_lives_L != null ? "_lives_L Set" : "_lives_L Not found");
        _score_L = root.Q<Label>("LivesLabel");
            Debug.Log(_score_L != null ? "_score_L Set" : "_score_L Not found");

        
    }
    private void MainMenu()
    {
        // Start and Quit Changes
    }
    private void GamePlayMenu()
    {

    }
    private void GameplayUI()
    {

    }
    private void GameOver()
    {
        // remove Gameplay UI
        // Add a Game over Screen
    }
}
