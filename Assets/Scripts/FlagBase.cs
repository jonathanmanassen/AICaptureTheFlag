using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagBase : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag(tag) && transform.childCount > 0)  //an ai will pick up the flag if it is hit and of the right team
        {
            transform.GetChild(0).parent = collision.transform;
        }
    }
}
