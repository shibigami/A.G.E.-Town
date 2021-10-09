using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OneTimePlayerTrigger : MonoBehaviour
{
    public UnityEvent PlayerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            PlayerEnter.Invoke();

            Destroy(gameObject);
        }
    }
}
