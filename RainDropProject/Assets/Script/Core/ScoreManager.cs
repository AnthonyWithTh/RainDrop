using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int gameScore = 0;
    [SerializeField] private int initialPlayerLife = 5;
    private int lifeInGame;
    //[SerializeField] private int Combo = 0;
    //[SerializeField] private int ComboMultiplier = 1;

    private static ScoreManager _instance;

    // Proprietà per accedere all'istanza del singleton
    public static ScoreManager InstanceSM
    {
        get
        {
            // Se l'istanza non esiste, la crea
            if (_instance == null)
            {
                // Cerca un'istanza esistente nella scena
                _instance = FindObjectOfType<ScoreManager>();

                // Se non esiste, crea un nuovo GameObject con questo script
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(GameManager).Name);
                    _instance = singletonObject.AddComponent<ScoreManager>();
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

    private void OnEnable()
    {
        EventManager.Instance.Subscribe("OnStartButtonPressed", ResetScore);
        EventManager.Instance.Subscribe("OnStartButtonPressed", ResetLife);
        EventManager.Instance.Subscribe("OnOperationSuccesfull", AddScore);
        EventManager.Instance.Subscribe("OnDropCollision", RemoveLife);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe("OnStartButtonPressed", ResetScore);
        EventManager.Instance.Unsubscribe("OnStartButtonPressed", ResetLife);
        EventManager.Instance.Unsubscribe("OnOperationSuccesfull", AddScore);
        EventManager.Instance.Unsubscribe("OnDropCollision", RemoveLife);
    }

    public void AddScore(object[] args)
    {
        gameScore += (int)args[0];// (1 * ComboMultiplier);
        EventManager.Instance.Publish("OnAddScore", gameScore);
    }

    private void ResetScore(object[] args)
    {
        gameScore = 0;
        EventManager.Instance.Publish("OnAddScore", gameScore);
    }

    private void ResetLife(object[] args)
    {
        lifeInGame = initialPlayerLife;
        EventManager.Instance.Publish("OnPlayerLifeSet", lifeInGame);
    }

    private void RemoveLife(object[] args)
    {
        lifeInGame--;
        if (lifeInGame <= 0)
        {
            EventManager.Instance.Publish("OnLivesEnded",E_GameStatus.GameOver);
        }
        else
        {
            EventManager.Instance.Publish("OnLifeRemoved", lifeInGame);
        }
    }

    //public void AddCombo()
    //{
    //    Combo++;
    //    ComboMultiplier *= (Combo / 100);
    //}

}
