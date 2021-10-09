using UnityEngine;
using UnityEngine.UI;

public class QuestAcceptUI : MonoBehaviour
{
    public Text QuestText;

    public Text QuestDescriptionText;

    private QuestGiver mQuestGiver;

    public Button AcceptButton;

    public Button CloseButton;

    public void Start()
    {
        AcceptButton.onClick.AddListener(Accept);
        CloseButton.onClick.AddListener(Close);
    }

    public void Accept()
    {
        mQuestGiver.Accept();

        Close();
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void Open(QuestGiver questGiver)
    {
        mQuestGiver = questGiver;

        QuestText.text = "Quest: " + mQuestGiver.Quest.Name;
        QuestDescriptionText.text = mQuestGiver.Quest.Description;

        this.gameObject.SetActive(true);
    }
}
