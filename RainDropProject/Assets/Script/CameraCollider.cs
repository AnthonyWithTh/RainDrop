using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{
    private PolygonCollider2D pc;

    private void Awake()
    {
        pc = GetComponent<PolygonCollider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        ResizeCameraCollider(1.0f);
    }

    private void ResizeCameraCollider(float height)
    {
        if (pc != null)
        {
            float halfScreenHeight = Camera.main.orthographicSize;
            float halfScreenWidth = (halfScreenHeight * Camera.main.aspect);

            Vector2[] newPoints = new Vector2[]
            {
                new Vector2(halfScreenWidth, (-halfScreenHeight)),
                new Vector2(halfScreenWidth, -halfScreenHeight + height),
                new Vector2(-halfScreenWidth, -halfScreenHeight + height),
                new Vector2(-halfScreenWidth, (-halfScreenHeight))
            };

            pc.points = newPoints;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    Debug.Log("Hitted");
    //    if (other.transform.parent.gameObject.CompareTag("rainDrop"))
    //    {
    //        GameObject rainDropHitted = other.transform.parent.gameObject;
    //        RainDropSpawner.InstanceRDP.RemoveRainDropFromList(rainDropHitted);
    //        Destroy(rainDropHitted);
    //    }
    //}
}
