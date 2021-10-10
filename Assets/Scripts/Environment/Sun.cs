using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    private Light lightComponent;
    private Color color;
    private bool right;
    private WorldTime worldTime;
    private float angle;
    private float previousWorldTime;
    public GameObject container;

    // Start is called before the first frame update
    void Start()
    {
        lightComponent = GetComponent<Light>();
        color = new Color(0, 0, 0);
        right = false;

        worldTime = Camera.main.GetComponent<GameManager>().worldTime;

        angle = -180.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        updateLight();
    }

    private void updateLight()
    {
        if (worldTime == null) return;

        right = worldTime.currentTime >= 5 * 60 && worldTime.currentTime <= 17 * 60;
        float worldTimeDiff = worldTime.currentTime - previousWorldTime;

        float direction = right ? 1.5f : -1.5f;
        color += new Color(direction / 255, direction / 255, direction / 255);
        color.r = Mathf.Clamp(color.r, 0, 1);
        color.g = Mathf.Clamp(color.g, 0, 1);
        color.b = Mathf.Clamp(color.b, 0, 1);
        color.a = Mathf.Clamp(color.a, 0, 1);
        lightComponent.color = color;


        angle = (0.5f * worldTimeDiff);
        container.transform.Rotate(Vector3.up, angle);

        previousWorldTime = worldTime.currentTime;
    }
}
