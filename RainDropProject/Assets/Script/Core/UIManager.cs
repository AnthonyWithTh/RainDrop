using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UIManager : MonoBehaviour
{
    private GameObject activeMenu;

    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private AudioClip confirmSound;
    [SerializeField] private AudioClip gameOverSound;

    //MainMenuUI
    [Header("MainMenuUI")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private TMP_Dropdown dropdown;

    //SettingUI
    [Header("SettingUI")]
    [SerializeField] private GameObject settingUI;
    [SerializeField] private Slider audioSFXSlider;
    [SerializeField] private Slider audioMusicSlider;


    //GameUI
    [Header("GameUI")]
    [SerializeField] private GameObject gameUI;
    [SerializeField] private TextMeshProUGUI gameScoreText;
    //[SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject lifeContainer;
    [SerializeField] private GameObject lifeImage;
    [SerializeField] private List<GameObject> lives = new List<GameObject>();

    //GameOver
    [Header("GameOverUI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;

    //Pause
    [Header("PauseUI")]
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private TextMeshProUGUI pauseScoreText;

    #region Singleton
    private static UIManager _instance;

    // Proprietà per accedere all'istanza del singleton
    public static UIManager InstanceGM
    {
        get
        {
            // Se l'istanza non esiste, la crea
            if (_instance == null)
            {
                // Cerca un'istanza esistente nella scena
                _instance = FindObjectOfType<UIManager>();

                // Se non esiste, crea un nuovo GameObject con questo script
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(GameManager).Name);
                    _instance = singletonObject.AddComponent<UIManager>();
                }
            }
            return _instance;
        }
    }

    // Assicura che l'istanza venga distrutta correttamente se necessario
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    // Assicura che l'istanza del singleton non venga distrutta al cambio di scena
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    private void OnEnable()
    {
        EventManager.Instance.Subscribe("OnAddScore", ScoreAdded);
        EventManager.Instance.Subscribe("OnGameStatusChange", ChangeUI);
        EventManager.Instance.Subscribe("OnPlayerLifeSet", ResetLifeContainer);
        EventManager.Instance.Subscribe("OnLifeRemoved", UpdateLifeUI);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe("OnAddScore", ScoreAdded);
        EventManager.Instance.Unsubscribe("OnGameStatusChange", ChangeUI);
        EventManager.Instance.Unsubscribe("OnPlayerLifeSet", ResetLifeContainer);
        EventManager.Instance.Unsubscribe("OnLifeRemoved", UpdateLifeUI);
    }


    private void Start()
    {
        EventManager.Instance.Publish("OnSFXSliderChanged", audioSFXSlider.value);
        EventManager.Instance.Publish("OnMusicSliderChanged", audioMusicSlider.value);
    }

    private void ChangeUI(object[] args)
    {
        UpdateUI((E_GameStatus)args[0]);
    }

    private void UpdateUI(E_GameStatus gameStatus)
    {
        switch (gameStatus)
        {
            case E_GameStatus.MainMenu:
                mainMenuUI.SetActive(true);
                gameUI.SetActive(false);
                pauseUI.SetActive(false);
                settingUI.SetActive(false);
                gameOverUI.SetActive(false);

                activeMenu = mainMenuUI;

                dropdown.ClearOptions();
                dropdown.AddOptions(new List<string>(Enum.GetNames(typeof(E_GameDifficulty))));
                break;
            case E_GameStatus.Play:
                mainMenuUI.SetActive(false);
                gameUI.SetActive(true);
                pauseUI.SetActive(false);
                settingUI.SetActive(false);
                gameOverUI.SetActive(false);

                activeMenu = gameUI;

                inputField.ActivateInputField();
                break;
            case E_GameStatus.Pause:
                mainMenuUI.SetActive(false);
                gameUI.SetActive(false);
                pauseUI.SetActive(true);
                settingUI.SetActive(false);
                gameOverUI.SetActive(false);

                activeMenu = pauseUI;
                break;
            case E_GameStatus.GameOver:
                mainMenuUI.SetActive(false);
                gameUI.SetActive(false);
                pauseUI.SetActive(false);
                settingUI.SetActive(false);
                gameOverUI.SetActive(true);
                EventManager.Instance.Publish("PlaySFXByClipGlobal", gameOverSound);

                activeMenu = gameOverUI;
                break;
        }
    }

    private void ResetLifeContainer(object[] args)
    {
        for (int i = lives.Count - 1; i >= 0; i--)
        {
            Destroy(lives[i]);
        }
        lives.Clear();

        int _nOfLife = (int)args[0];
        if (lives.Count != _nOfLife)
        {
            for (int i = 0; i < _nOfLife; i++)
            {
                GameObject newLife = Instantiate(lifeImage, lifeContainer.transform);
                newLife.SetActive(true);
                lives.Add(newLife);
            }
        }
    }

    private void UpdateLifeUI(object[] args)
    {
        int _nOfLife = (int)args[0];
        if (_nOfLife <= 0)
        {
            _nOfLife = 0;
        }
        else if (_nOfLife > lives.Count)
        {
            _nOfLife = lives.Count - 1;
        }
        lives[_nOfLife].SetActive(false);
    }

    private void ScoreAdded(object[] args)
    {
        UpdateText((int)args[0]);//, (int)args[1]);
    }

    private void UpdateText(int score)//, int combo)
    {
        gameScoreText.text = "Score: " + score.ToString();
        pauseScoreText.text = "Your Score is: " + score.ToString();
        gameOverScoreText.text = "Your Score is: " + score.ToString();
    }

    public void StartGame()
    {
        EventManager.Instance.Publish("OnStartButtonPressed", E_GameStatus.Play, (dropdown.value + 1));
        EventManager.Instance.Publish("PlaySFXByClipGlobal", confirmSound);
    }

    public void PauseGame()
    {
        EventManager.Instance.Publish("OnPauseGameButtonPressed", E_GameStatus.Pause);
        EventManager.Instance.Publish("PlaySFXByClipGlobal", buttonSound);
    }

    public void ResumeGame()
    {
        EventManager.Instance.Publish("OnResumeGameButtonPressed", E_GameStatus.Play);
        EventManager.Instance.Publish("PlaySFXByClipGlobal", buttonSound);
    }

    public void BackToMainMenu()
    {
        EventManager.Instance.Publish("OnMainMenuButtonPressed", E_GameStatus.MainMenu);
        EventManager.Instance.Publish("PlaySFXByClipGlobal", buttonSound);
    }

    public void ReadStringInput(string _input)
    {
        if (!string.IsNullOrEmpty(_input))
        {
            EventManager.Instance.Publish("OnInputSend", int.Parse(_input));
        }
        inputField.text = "";
        inputField.ActivateInputField();
    }

    public void ShowSetting()
    {
        EventManager.Instance.Publish("PlaySFXByClipGlobal", buttonSound);
        activeMenu.SetActive(false);
        settingUI.SetActive(true);
    }

    public void SetSFXAudio(float value)
    {
        EventManager.Instance.Publish("OnSFXSliderChanged", value);
    }

    public void SetMusicAudio(float value)
    {
        EventManager.Instance.Publish("OnMusicSliderChanged", value);
    }

    public void BackToPreviousMenu()
    {
        EventManager.Instance.Publish("PlaySFXByClipGlobal", buttonSound);
        settingUI.SetActive(false);
        activeMenu.SetActive(true);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
