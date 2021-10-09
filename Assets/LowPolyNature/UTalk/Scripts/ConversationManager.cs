using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UTalk.Data;

namespace UTalk
{
    public class ConversationManager : MonoBehaviour
    {
        #region Private members

        private Button mResponseButton;

        private Conversation mCurrentConversation = null;

        private ConversationItem mCurrentConversationItem = null;

        #endregion

        #region Public members

        private static ConversationManager mInstance = null;

        public ConversationDatabase Database = null;

        public GameObject ConversationPanel = null;

        public Text Text_NPC;

        public Image Image_NPC;

        public Text Text_Player;

        public Image Image_Player;

        public Text Text_Actor_Label;

        public Button Continue_Button;

        public RectTransform ResponsePanel;

        public List<GameObject> ObjectsToHideOnStart;

        #endregion

        #region Events

        public event EventHandler<ConversationEventArgs> ConversationStart;

        public event EventHandler<ConversationEventArgs> ConversationEnd;

        public event EventHandler<ConversationItemEventArgs> ConversationItemChanged;

        #endregion

        #region Initialization

        void Awake()
        {
            if (mInstance == null)
            {
                mInstance = this;
            }
            else if (mInstance != this)
            {
                Destroy(gameObject);
            }

            // Not destroyed when reloading scene
            // Add this optional?
            //DontDestroyOnLoad(gameObject);
        }

        public static ConversationManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = (ConversationManager)FindObjectOfType(typeof(ConversationManager));
                return mInstance;
            }
        }

        void Start()
        {
            mResponseButton = Instantiate<Button>(ResponsePanel.Find("ResponseButton").GetComponent<Button>());

            if(ConversationPanel != null)
                ConversationPanel.SetActive(false);
        }

        #endregion

        private bool IsFullyConfigured
        {
            get
            {
                if (ConversationPanel == null)
                {
                    Debug.LogError("No conversation panel assigned");
                    return false;
                }

                if (Database == null)
                {
                    Debug.LogError("No database assigned");
                    return false;
                }
                return true;
            }
        }

        private bool InitConversation(string name)
        {
            mCurrentConversation = Database.GetConversation(name);

            if (mCurrentConversation == null)
            {
                Debug.LogErrorFormat("Conversation {0} could not be found", name);
                return false;
            }

            if (mCurrentConversation.Items.Count == 0)
            {
                Debug.LogErrorFormat("Conversation {0} has no items", name);
                return false;
            }

            mCurrentConversationItem = mCurrentConversation.Items[0];
            return true;
        }

        public void StartConversation(string name)
        {
            if (IsFullyConfigured && InitConversation(name))
            {
                foreach (GameObject go in ObjectsToHideOnStart)
                {
                    go.SetActive(false);
                }

                ConversationPanel.SetActive(true);

                if (ConversationStart != null)
                {
                    ConversationStart(this, new ConversationEventArgs(mCurrentConversation));
                }

                // Check for Next conversation items
                ConversationStep();


            }
        }

        public void ConversationStep()
        {
            ConversationItem item = mCurrentConversation.GetFirstConnectedItem(mCurrentConversationItem);
            if (item == null)
            {
                // No more items, stop conversation
                StopConversation();
            }
            else
            {
                Actor actor = Database.GetActor(item.ActorId);

                if (actor.ActorType == EActorType.NPC)
                {
                    mCurrentConversationItem = item;
                    OnConversationItemChanged(mCurrentConversationItem);
                    DisplayNPC(actor, item);
                }
                else
                {
                    DisplayPlayer(actor, mCurrentConversation.GetConnectedItems(mCurrentConversationItem));
                }

                mTimeRemaining = Database.ContinueConversationTime;

                if (CanContinueOrStop && mTimeRemaining > 0)
                {
                    InvokeRepeating("StartContinueTimer", 0.0f, 1.0f);
                }
            }
        }

        private float mTimeRemaining = 0.0f;

        void StartContinueTimer()
        {
            if (mTimeRemaining == 0)
            {
                StopContinueTimer();
                ConversationStep();
            }
            else
            {
                mTimeRemaining--;
            }
        }

        void StopContinueTimer()
        {
            mTimeRemaining = Database.ContinueConversationTime;
            CancelInvoke("StartContinueTimer");
        }

        private void DisplayPlayer(Actor actor, List<ConversationItem> connectedItems)
        {
            Image_Player.sprite = actor.Image;
            Text_Actor_Label.text = actor.Name;

            RemoveAllResponseButtons();

            if (connectedItems.Count > 1)
            {
                SetContinueButtonState(actor, mCurrentConversationItem, connectedItems.Count);

                Text_Player.text = "";

                foreach (var conversationItem in connectedItems)
                {
                    Button newButton = Instantiate<Button>(mResponseButton, ResponsePanel);
                    newButton.onClick.AddListener(delegate { OnResponseButtonClick(conversationItem); });

                    Text text = newButton.transform.Find("Text").GetComponent<Text>();
                    text.text = conversationItem.Text;
                }
            }
            else
            {
                mCurrentConversationItem = connectedItems[0];

                SetContinueButtonState(actor, mCurrentConversationItem, connectedItems.Count);

                Text_Player.text = mCurrentConversationItem.Text;
            }
        }

        private void DisplayNPC(Actor actor, ConversationItem item)
        {
            Text_Actor_Label.text = actor.Name + ":";

            RemoveAllResponseButtons();

            SetContinueButtonState(actor, item, 1);

            Text_NPC.text = item.Text;

            Image_NPC.sprite = actor.Image;
           
        }

        private bool CanContinueOrStop
        {
            get
            {
                // Check if continue button is active
                if (Continue_Button.gameObject.activeInHierarchy)
                    return true;

                // If not check if it is the last item and
                return !HasConnectedItem;
            }
        }

        private bool HasConnectedItem
        {
            get
            {
                return mCurrentConversation.HasConnectedItem(mCurrentConversationItem);
            }
        }

        private void SetContinueButtonState(Actor actor, ConversationItem item, int itemCount)
        {
            bool active = (actor.ActorType == EActorType.NPC);

            if(actor.ActorType == EActorType.Player)
            {
                active = itemCount == 1;
            }

            if(active)
            {
                active = mCurrentConversation.GetConnectedItems(item).Count > 0;
            }

            Continue_Button.gameObject.SetActive(active);
        }

        private void RemoveAllResponseButtons()
        {
            var buttons2Remove = ResponsePanel.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons2Remove)
            {
                btn.enabled = false;
                Destroy(btn.gameObject);
            }
        }

        public void OnResponseButtonClick(ConversationItem item)
        {
            mCurrentConversationItem = item;

            OnConversationItemChanged(mCurrentConversationItem);

            ConversationStep();
        }

        private void OnConversationItemChanged(ConversationItem item)
        {
            if(ConversationItemChanged != null)
            {
                ConversationItemChanged(this, new ConversationItemEventArgs(mCurrentConversationItem));
            }
        }

        public bool IsConversationActive
        {
            get { return mCurrentConversation != null;  }
        }

        public void StopConversation()
        {
            StopContinueTimer();

            foreach (GameObject go in ObjectsToHideOnStart)
            {
                go.SetActive(true);
            }
            ConversationPanel.SetActive(false);

            if (ConversationEnd != null && mCurrentConversation != null)
            {
                ConversationEnd(this, new ConversationEventArgs(mCurrentConversation));
            }

            mCurrentConversation = null;
        }
    }
}
