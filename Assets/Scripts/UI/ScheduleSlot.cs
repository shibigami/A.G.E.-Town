using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScheduleSlot : MonoBehaviour, IPointerEnterHandler
{
    public GameObject timeSlot;
    private float timeSliceUnit;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            gameObject.GetComponent<Button>().Select();
            gameObject.GetComponent<Button>().onClick.Invoke();
        }
    }

    public void SetTimeSliceUnit(float value)
    {
        timeSliceUnit = value;
        timeSlot.GetComponent<Text>().text = ((int)(timeSliceUnit)).ToString();
        if (timeSliceUnit - Mathf.FloorToInt(timeSliceUnit) == 0)
        {
            timeSlot.gameObject.SetActive(true);
        }
    }
}
