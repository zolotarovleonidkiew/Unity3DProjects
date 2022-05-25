using Assets.Scripts;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Menu on Start/Failed/Pause
/// </summary>
public class LoseEndGameMenu : MonoBehaviour
{
    #region Fields
    [SerializeField] private Text TitleWindow;
    [SerializeField] private Image _deadPicture1;
    [SerializeField] private bool _OlafWasDead = false;
    [SerializeField] private Image _deadPicture2;
    [SerializeField] private bool _UlrichWasDead = false;
    [SerializeField] private Image _deadPicture3;
    [SerializeField] private bool _BaealogWasDead = false;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private MenuStateEnum _menuType;

    [SerializeField] private GameObject PanelMenu;
    [SerializeField] private GameObject PanelSettings;

    [SerializeField] private Button _saveSettingsButton;
    [SerializeField] private Button _closeSettingsButton;

    [SerializeField] private LevelsEnum _currentLevel;
    #endregion

    #region Properties

    public Action DisableHeroes;

    public MenuStateEnum MenuType
    {
        get { return _menuType; }
        set { _menuType = value; }
    }

    public bool OlafWasDead
    {
        set { _OlafWasDead = value; }
    }

    public bool UlrichWasDead
    {
        set { _UlrichWasDead = value; }
    }

    public bool BaealogWasDead
    {
        set { _BaealogWasDead = value; }
    }

    #endregion

    public void Show()
    {
        _settingsButton.onClick.AddListener(ShowSettings);
        _closeButton.onClick.AddListener(Exit);

        gameObject.SetActive(true);

        UpdateForm();
    }

    public void Hide()
    {
        DisableHeroes.Invoke();

        gameObject.SetActive(false);
    }

    public void Exit()
    {
        Hide();
    }

    public void Restart()
    {
        if (_menuType != MenuStateEnum.PauseGame)
        {
            SceneManager.LoadScene(_currentLevel.ToString(), LoadSceneMode.Single);
        }
    }

    public void ShowSettings()
    {
        ShowSettingsPanel();
    }

    private void UpdateForm()
    {
        if (_menuType == MenuStateEnum.StartGame)
        {
            TitleWindow.text = "Wellcome!";
            GameObject.Find("btnRestart").GetComponentInChildren<Text>().text = "START";
            _restartButton.onClick.AddListener(Hide);

            _deadPicture1.enabled = false;
            _deadPicture2.enabled = false;
            _deadPicture3.enabled = false;
        }
        else if (_menuType == MenuStateEnum.FailedGame)
        {
            TitleWindow.text = "YOU LOSE !";
            GameObject.Find("btnRestart").GetComponentInChildren<Text>().text = "reSTART";
            _restartButton.onClick.AddListener(Restart);

            UpdateDeadIcons();
        }
        else if (_menuType == MenuStateEnum.WinTheGame)
        {
            TitleWindow.text = "YOU ARE WINNER !!!";
            _restartButton.onClick.AddListener(Restart);
            UpdateDeadIcons();
        }
        else
        {
            TitleWindow.text = "Pause";
            _restartButton.onClick.AddListener(Restart);

            UpdateDeadIcons();
        }

    }


    private void UpdateDeadIcons()
    {
        _deadPicture1.enabled = _OlafWasDead;
        _deadPicture2.enabled = _UlrichWasDead;
        _deadPicture3.enabled = _BaealogWasDead;
    }


    private void ShowSettingsPanel()
    {
        PanelMenu.SetActive(false);
        PanelSettings.SetActive(true);

        _closeSettingsButton.onClick.AddListener(CloseSettingsPanel);
        _saveSettingsButton.onClick.AddListener(SaveSettings);
    }

    private void CloseSettingsPanel()
    {
        PanelMenu.SetActive(true);
        PanelSettings.SetActive(false);
    }

    private void SaveSettings()
    {
        // TO DO
    }
}
