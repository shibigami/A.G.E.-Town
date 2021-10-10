using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text time;
    private WorldTime worldTime;

    // Start is called before the first frame update
    void Start()
    {
        worldTime = Camera.main.GetComponent<GameManager>().worldTime;
        InvokeRepeating("UpdateTime", 0.0f, 0.5f);
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectNpc();
        }

        Debug.DrawLine(Camera.main.transform.position + Camera.main.ViewportToWorldPoint(Camera.main.ScreenToViewportPoint(Input.mousePosition)),
            Camera.main.transform.position + Camera.main.ViewportToWorldPoint(Camera.main.ScreenToViewportPoint(Input.mousePosition)) + Camera.main.transform.forward * 100);
    }

    private void UpdateTime()
    {
        time.text = ConvertTime();
    }

    private string ConvertTime()
    {
        var hours = Mathf.FloorToInt(worldTime.currentTime / 60);
        var minutes = worldTime.currentTime - hours * 60;
        return hours.ToString("00") + ":" + minutes.ToString("00");
    }

    private void SelectNpc()
    {
        Ray ray = new Ray(Camera.main.transform.position + Camera.main.ViewportToWorldPoint(Camera.main.ScreenToViewportPoint(Input.mousePosition)), Camera.main.transform.forward);
        RaycastHit hit = new RaycastHit();
        Debug.Log("sending ray");
        if (Physics.Raycast(ray, out hit, 200f))
        {
            Debug.Log("rayhit: " + hit.transform.gameObject.name);
            if (hit.transform.gameObject.tag == "Citizen")
            {
                //show ui
                Debug.Log(hit.transform.gameObject.name);
            }
        }
    }
}
