using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject rainDropSpawner;
    [SerializeField] private float timeForChangeDifficulty = 120.0f;
    private float timeInPlay = 0.0f;

    [SerializeField] private E_GameDifficulty GameDifficulty;
    public E_GameDifficulty gameDifficulty
    {
        get
        {
            return GameDifficulty;
        }
        private set { }
    }


    [SerializeField] private E_GameStatus _gameStatus;
    public E_GameStatus GameStatus
    {
        get
        {
            return _gameStatus;
        }
        private set
        {
            EventManager.Instance.Publish("OnGameStatusChange", value);
        }
    }

    #region Singleton
    private static GameManager _instance;

    // Proprietà per accedere all'istanza del singleton
    public static GameManager InstanceGM
    {
        get
        {
            // Se l'istanza non esiste, la crea
            if (_instance == null)
            {
                // Cerca un'istanza esistente nella scena
                _instance = FindObjectOfType<GameManager>();

                // Se non esiste, crea un nuovo GameObject con questo script
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(GameManager).Name);
                    _instance = singletonObject.AddComponent<GameManager>();
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
            GameStatus = _gameStatus;
        }
    }
    #endregion


    private void OnEnable()
    {
        EventManager.Instance.Subscribe("OnStartButtonPressed", SetInitialDifficulty, ChangeGameMode);
        EventManager.Instance.Subscribe("OnPauseGameButtonPressed", ChangeGameMode);
        EventManager.Instance.Subscribe("OnResumeGameButtonPressed", ChangeGameMode);
        EventManager.Instance.Subscribe("OnMainMenuButtonPressed", ChangeGameMode);
        EventManager.Instance.Subscribe("OnLivesEnded", ChangeGameMode);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe("OnStartButtonPressed", SetInitialDifficulty, ChangeGameMode);
        EventManager.Instance.Unsubscribe("OnPauseGameButtonPressed", ChangeGameMode);
        EventManager.Instance.Unsubscribe("OnResumeGameButtonPressed", ChangeGameMode);
        EventManager.Instance.Unsubscribe("OnLivesEnded", ChangeGameMode);
    }

    private void Update()
    {
        if (GameStatus == E_GameStatus.Play && GameDifficulty != E_GameDifficulty.Extreme)
        {
            timeInPlay += Time.deltaTime;
            if (timeInPlay > timeForChangeDifficulty)
            {
                timeInPlay = 0.0f;
                IncreaseDifficulty();
            }
        }
        else
        {
            timeInPlay = 0.0f;
        }
    }



    public void IncreaseDifficulty()
    {
        if (GameDifficulty != E_GameDifficulty.Extreme)
        {
            GameDifficulty++;
        }
    }

    private void SetInitialDifficulty(object[] args)
    {
        GameDifficulty = (E_GameDifficulty)args[1];
    }

    private void ChangeGameMode(object[] args)
    {
        _gameStatus = (E_GameStatus)args[0];
        GameStatus = (E_GameStatus)args[0];

        switch (_gameStatus)
        {
            case E_GameStatus.MainMenu:
                rainDropSpawner.SetActive(false);
                break;
            case E_GameStatus.Play:
                rainDropSpawner.SetActive(true);
                break;
        }
    }
}
