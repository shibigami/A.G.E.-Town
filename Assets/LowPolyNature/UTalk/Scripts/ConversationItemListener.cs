using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UTalk
{
    public class ConversationItemListener : MonoBehaviour {

        public UnityEvent OnConversationItemChange;

        public UnityEvent OnConversationItemChangeDelayed;

        public string Tag;

        public float Delay;

        // Use this for initialization
        void Start() {
            ConversationManager.Instance.ConversationItemChanged += Instance_ConversationItemChanged;
        }

        private void Instance_ConversationItemChanged(object sender, ConversationItemEventArgs e)
        {
            if(e.ConversationItem.Tag == Tag)
            {
                OnConversationItemChange.Invoke();

                if(Delay != 0)
                    StartCoroutine(InvokeDelayedItemChange());
            }
        }

        IEnumerator InvokeDelayedItemChange()
        {
            yield return new WaitForSeconds(Delay);

            if(OnConversationItemChangeDelayed != null)
                OnConversationItemChangeDelayed.Invoke();
        }
    }
}
