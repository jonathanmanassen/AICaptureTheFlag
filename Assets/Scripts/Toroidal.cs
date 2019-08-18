using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Toroidal : MonoBehaviour
{
    AiTeam aiTeam;
    public float maxX = 10;
    public float maxZ = 7.5f;

    Vector3 maxXVec;
    Vector3 maxZVec;

    TextMeshProUGUI text;

    public bool TestScene = false;

    private void Awake()
    {
        maxXVec = new Vector3(maxX * 2, 0, 0);
        maxZVec = new Vector3(0, 0, maxZ * 2);
        if (!TestScene)
        {
            text = FindObjectOfType<TextMeshProUGUI>();
            aiTeam = GameObject.Find(tag + "Team").GetComponent<AiTeam>();
        }
    }

    private void OnCollisionExit(Collision collision)  //if a player leaves the plane on a board extremity, teleport it to the other end
    {
        if (collision.transform.position.x > maxX)
            collision.transform.position -= maxXVec;
        if (collision.transform.position.x < -maxX)
            collision.transform.position += maxXVec;
        if (collision.transform.position.z > maxZ)
            collision.transform.position -= maxZVec;
        if (collision.transform.position.z < -maxZ)
            collision.transform.position += maxZVec;

        if (collision.transform.CompareTag(tag) && !TestScene)
            aiTeam.StopChaseHim(collision.transform);
    }

    private Transform GetFlag(Collision collision)
    {
        for (int i = 0; i < collision.transform.childCount; i++)
        {
            if (collision.transform.GetChild(i).tag != "Untagged")
                return collision.transform.GetChild(i);
        }
        return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (TestScene)
            return;
        if (collision.transform.CompareTag(tag))  //sets the victory if an ai goes to the other collor plane while holding a flag (you cannot hold your own team flag)
        {
            Transform t = GetFlag(collision);
            if (t == null)
                return;
            t.parent = transform;
            Victory(collision.transform.tag);
        }
        else
        {
            aiTeam.ChaseHim(collision.transform);
        }
    }

    private void Victory(string tag)  //enables the victory text and freezes the game
    {
        Time.timeScale = 0;
        text.text = tag + text.text;
        text.enabled = true;
    }
}
