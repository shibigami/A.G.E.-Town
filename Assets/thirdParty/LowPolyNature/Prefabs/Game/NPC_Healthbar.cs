using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC_Healthbar : MonoBehaviour
{
    private Text mText;
    private Image mHealthImg;

    // Start is called before the first frame update
    void Start()
    {
        mText = transform.Find("Text").GetComponent<Text>();
        mHealthImg = transform.Find("HealthImg").GetComponent<Image>();
    }

    public void SetHealth(int value)
    {
        mText.text = value.ToString();
        mHealthImg.rectTransform.sizeDelta = new Vector2(value, 20);
    }
}
