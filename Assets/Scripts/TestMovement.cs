using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    AIMovement redAi;
    AIMovement blueAi;

    void Start()  //gets the two ais on the field
    {
        foreach(AIMovement ai in FindObjectsOfType<AIMovement>())
        {
            if (ai.CompareTag("Blue"))
                blueAi = ai;
            else if (ai.CompareTag("Red"))
                redAi = ai;
        }
        blueAi.state = AIMovement.State.FROZEN;
        redAi.state = AIMovement.State.FROZEN;
    }

    void Update() //is used to setup the different behaviours by pressing a key
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            blueAi.TransformTarget = redAi.transform;
            blueAi.state = AIMovement.State.ARRIVE;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            blueAi.TransformTarget = redAi.transform;
            blueAi.state = AIMovement.State.SEEK;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            blueAi.TransformTarget = redAi.transform;
            blueAi.state = AIMovement.State.FLEE;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            blueAi.chasing = redAi;
            blueAi.state = AIMovement.State.PURSUE;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            blueAi.state = AIMovement.State.WANDER;
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            blueAi.state = AIMovement.State.FROZEN;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            redAi.TransformTarget = blueAi.transform;
            redAi.state = AIMovement.State.ARRIVE;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            redAi.TransformTarget = blueAi.transform;
            redAi.state = AIMovement.State.SEEK;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            redAi.TransformTarget = blueAi.transform;
            redAi.state = AIMovement.State.FLEE;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            redAi.chasing = blueAi;
            redAi.state = AIMovement.State.PURSUE;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            redAi.state = AIMovement.State.WANDER;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            redAi.state = AIMovement.State.FROZEN;
        }
    }
}
