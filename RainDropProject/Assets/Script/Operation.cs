using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Operation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI OperationText;

    [SerializeField] private int firstValue = 0;
    [SerializeField] private int secondValue = 0;
    [SerializeField] private int result = 0;
    [SerializeField] private bool isGoldDrop = false;
    [SerializeField] private Sprite goldDrop;
    [SerializeField] private AudioClip popSound;
    [SerializeField] private AudioClip collideSound;
    [SerializeField] private GameObject particleStar;
    private Vector2 speed = Vector2.zero;

    public int Result { get { return result; } private set { } }
    public bool IsGoldDrop { get { return isGoldDrop; } private set { } }

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe("OnPauseGameButtonPressed", PauseDrop);
        EventManager.Instance.Subscribe("OnResumeGameButtonPressed", UnPauseDrop);
        EventManager.Instance.Subscribe("OnLivesEnded", PauseDrop);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe("OnPauseGameButtonPressed", PauseDrop);
        EventManager.Instance.Unsubscribe("OnResumeGameButtonPressed", UnPauseDrop);
        EventManager.Instance.Unsubscribe("OnLivesEnded", PauseDrop);
    }

    public void DestroyDrop(bool _byGoldDrop, bool _isWiped, bool _hasCollide)
    {
        if (!_isWiped)
        {
            if (!_byGoldDrop)
            {
                EventManager.Instance.Publish("PlaySFXByClipGlobal", popSound);
            }
            Instantiate(particleStar, transform.position, Quaternion.identity);
            EventManager.Instance.Publish("OnOperationSuccesfull", result);
        }

        if(_hasCollide)
        {
            EventManager.Instance.Publish("PlaySFXByClipGlobal", collideSound);
        }
        Destroy(gameObject);
    }

    public void SetGoldDrop(bool _isGoldDrop)
    {
        isGoldDrop = _isGoldDrop;

        if (isGoldDrop)
        {
            sr.sprite = goldDrop;
        }
    }

    public void SetOperation(E_OperationType operationType)
    { 
        switch (operationType)
        {
            case E_OperationType.Sum:
                firstValue = Random.Range(0, 10 * Convert.ToInt32(GameManager.InstanceGM.gameDifficulty));
                secondValue = Random.Range(0, ((10 * Convert.ToInt32(GameManager.InstanceGM.gameDifficulty)) - firstValue));
                result = firstValue + secondValue;
                OperationText.text = firstValue + " + " + secondValue;
                break;

            case E_OperationType.Subtraction:
                firstValue = Random.Range(0, 10 * Convert.ToInt32(GameManager.InstanceGM.gameDifficulty));
                secondValue = Random.Range(0, firstValue);
                result = firstValue - secondValue;
                OperationText.text = firstValue + " - " + secondValue;
                break;

            case E_OperationType.Multiplication:
                firstValue = Random.Range(0, 10);
                secondValue = Random.Range(0, 10);
                result = firstValue * secondValue;
                OperationText.text = firstValue + " x " + secondValue;
                break;

            case E_OperationType.Division:
                secondValue = Random.Range(1, 10);
                firstValue = Random.Range(0, 10) * secondValue;
                result = firstValue / secondValue;
                OperationText.text = firstValue + " / " + secondValue;
                break;
        }
    }

    public void SetVelocity()
    {
        speed = new Vector2(0.0f, -0.5f - Convert.ToInt32(GameManager.InstanceGM.gameDifficulty) / 5.0f);
        rb.velocity = speed;
    }

    private void PauseDrop(object[] args)
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        OperationText.gameObject.SetActive(false);
    }

    private void UnPauseDrop(object[] args)
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        rb.velocity = speed;
        OperationText.gameObject.SetActive(true);
    }
}
