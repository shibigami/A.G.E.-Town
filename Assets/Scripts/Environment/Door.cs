using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform building { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        building = transform.parent;
    }
}
