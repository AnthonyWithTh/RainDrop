using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollider : MonoBehaviour
{
    [SerializeField] private float horizontalSpeed = 1.0f;
    [SerializeField] private float waterAmplitude = 1.0f;
    [SerializeField] private float waterIncreaseHeight = 1.0f;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe("OnMainMenuButtonPressed", ResetInitialPosition);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe("OnMainMenuButtonPressed", ResetInitialPosition);
    }
    // Start is called before the first frame update
    void Start()
    {
        SetInitialPosition();
    }

    private void ResetInitialPosition(object[] args)
    {
        SetInitialPosition();
    }

    private void SetInitialPosition()
    {
        gameObject.transform.position = new Vector3(0, -Camera.main.orthographicSize + waterIncreaseHeight, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.InstanceGM.GameStatus == E_GameStatus.Play || GameManager.InstanceGM.GameStatus == E_GameStatus.MainMenu)
        {
            if(gameObject.transform.position.x > waterAmplitude || gameObject.transform.position.x < -waterAmplitude)
            {
                horizontalSpeed *= -1.0f;
            }

            gameObject.transform.position = new Vector3(gameObject.transform.position.x + (horizontalSpeed * Time.deltaTime), gameObject.transform.position.y, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "rainDrop")
        {
            gameObject.transform.position = new Vector3(0, gameObject.transform.position.y + waterIncreaseHeight, 0);
            EventManager.Instance.Publish("OnDropCollision", collision.gameObject);
        }
    }
}
