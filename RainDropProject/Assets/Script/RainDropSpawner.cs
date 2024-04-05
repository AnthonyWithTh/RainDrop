using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainDropSpawner : MonoBehaviour
{
    [SerializeField] private GameObject rainDropPrefab;
    [SerializeField] private float sumWeight = 10.0f;
    [SerializeField] private float subtractionWeight = 20.0f;
    [SerializeField] private float multiplicationWeight = 30.0f;
    [SerializeField] private float divisionWeight = 40.0f;

    [SerializeField][Range(0.0f, 100.0f)] private float goldDropProbability = 10.0f;
    [SerializeField][Range(0, 100)] private int maxDropPerScreen = 10;
    [SerializeField] private List<GameObject> rainDrop = new List<GameObject>();

    [SerializeField] private AudioClip errorSound;

    public float timeBetweenSpawn = 1.0f;
    private float spawnCD = 0.0f;

    #region Singleton
    private static RainDropSpawner _instance;

    // Proprietà per accedere all'istanza del singleton
    public static RainDropSpawner InstanceRDP
    {
        get
        {
            // Se l'istanza non esiste, la crea
            if (_instance == null)
            {
                // Cerca un'istanza esistente nella scena
                _instance = FindObjectOfType<RainDropSpawner>();

                // Se non esiste, crea un nuovo GameObject con questo script
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(RainDropSpawner).Name);
                    _instance = singletonObject.AddComponent<RainDropSpawner>();
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

        //isCheckingResult = false;
    }
    #endregion

    private void OnEnable()
    {
        EventManager.Instance.Subscribe("OnInputSend", CheckList);
        EventManager.Instance.Subscribe("OnMainMenuButtonPressed", ClearScreen);
        EventManager.Instance.Subscribe("OnDropCollision", RemoveFromList);

        SpawnNewDrop();
        spawnCD = 0.0f;
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe("OnInputSend", CheckList);
        EventManager.Instance.Unsubscribe("OnMainMenuButtonPressed", ClearScreen);
        EventManager.Instance.Unsubscribe("OnDropCollision", RemoveFromList);
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnCD > (timeBetweenSpawn / Convert.ToInt32(GameManager.InstanceGM.gameDifficulty)) && rainDrop.Count < maxDropPerScreen)
        {
            spawnCD = 0.0f;
            SpawnNewDrop();
        }
        else
        {
            if (GameManager.InstanceGM.GameStatus == E_GameStatus.Play)
            {
                spawnCD += Time.deltaTime;
            }
        }
    }

    private void SpawnNewDrop()
    {
        float screenHeight = Camera.main.orthographicSize * 2.0f;
        float screenWidth = screenHeight * Camera.main.aspect;
        float spawnPosY = Camera.main.transform.position.y + (screenHeight / 2.0f);
        float spawnPosX = UnityEngine.Random.Range((-screenWidth/2.0f) + 0.5f, (screenWidth / 2.0f) - 0.5f);

        GameObject newRainDrop = Instantiate(rainDropPrefab, new Vector3(spawnPosX, spawnPosY, 0.0f), Quaternion.identity, transform);
        bool goldDrop = (UnityEngine.Random.Range(0.0f, 100.0f) < goldDropProbability);

        if (goldDrop)
        {
            for (int i = 0; i < rainDrop.Count; i++)
            { 
                if (rainDrop[i].GetComponent<Operation>().IsGoldDrop)
                {
                    goldDrop = false;
                    break;
                }
            }
            newRainDrop.GetComponent<Operation>().SetGoldDrop(goldDrop);
        }

        E_OperationType newOperationType = E_OperationType.Sum;

        float randomNumber = UnityEngine.Random.Range(0.0f, 100.0f);

        switch (GameManager.InstanceGM.gameDifficulty)
        {
            case E_GameDifficulty.Medium:
                if (randomNumber > ((100 * sumWeight) / (sumWeight + subtractionWeight)))
                {
                    newOperationType = E_OperationType.Subtraction;
                }
            break;

            case E_GameDifficulty.Hard:
                if (randomNumber > ((100 * (sumWeight + subtractionWeight)) / (sumWeight + subtractionWeight + multiplicationWeight)))
                {
                    newOperationType = E_OperationType.Multiplication;
                }
                else
                {
                    if (randomNumber > ((100 * sumWeight) / (sumWeight + subtractionWeight + multiplicationWeight)))
                    {
                        newOperationType = E_OperationType.Multiplication;
                    }
                }
                break;

            case E_GameDifficulty.Extreme:
                if (randomNumber > ((100 * (sumWeight + subtractionWeight + multiplicationWeight)) / (sumWeight + subtractionWeight + multiplicationWeight + divisionWeight)))
                {
                    newOperationType = E_OperationType.Division;
                }
                else
                {
                    if (randomNumber > ((100 * (sumWeight + subtractionWeight)) / (sumWeight + subtractionWeight + multiplicationWeight + divisionWeight)))
                    {
                        newOperationType = E_OperationType.Multiplication;
                    }
                    else
                    {
                        if (randomNumber > ((100 * (sumWeight)) / (sumWeight + subtractionWeight + multiplicationWeight + divisionWeight)))
                            {
                                newOperationType = E_OperationType.Subtraction;
                            }
                    }
                }
                break;
        }

        newRainDrop.GetComponent<Operation>().SetOperation(newOperationType);
        newRainDrop.GetComponent<Operation>().SetVelocity();

        rainDrop.Add(newRainDrop);
    }

    private void CheckList(object[] args)
    {
        bool isGoldDrop = false;

        GameObject selectedDrop = null;

        for (int i = 0; i < rainDrop.Count; i++)
        {
            if (rainDrop[i].GetComponent<Operation>().Result == (int)args[0])
            {
                selectedDrop = rainDrop[i];
                isGoldDrop = selectedDrop.GetComponent<Operation>().IsGoldDrop;
                rainDrop.RemoveAt(i);
                selectedDrop.GetComponent<Operation>().DestroyDrop(false, false, false);
                break;
            }
        }

        if (selectedDrop != null)
        {
            if (isGoldDrop)
            {
                for (int i = rainDrop.Count - 1; i >= 0; i--)
                {
                    GameObject currentDrop = rainDrop[i];
                    rainDrop.RemoveAt(i);
                    currentDrop.GetComponent<Operation>().DestroyDrop(true, false, false);
                }
            }
        }
        else
        {
            EventManager.Instance.Publish("PlaySFXByClipGlobal", errorSound);
        }

    }

    private void ClearScreen(object[] args)
    {
        for (int i = rainDrop.Count - 1; i >= 0; i--)
        {
            rainDrop[i].GetComponent<Operation>().DestroyDrop(false, true, false);
        }
        rainDrop.Clear();
    }

    private void RemoveFromList(object[] args)
    {
        rainDrop.Remove((GameObject)args[0]);
        ((GameObject)args[0]).GetComponent<Operation>().DestroyDrop(false, true, true);
    }
}
