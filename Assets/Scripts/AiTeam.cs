using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiTeam : MonoBehaviour
{
    List<AIMovement> ais;
    Vector3 flagPosition;

    List<Transform> chase;

    public GameObject ai;
    public AiTeam otherAITeam;

    [HideInInspector]
    public int randomNb = 0;

    private void Awake()
    {
        if (otherAITeam != null)
        {
            randomNb = Random.Range(2, 10);     //in order to set the random number of players in the game
            otherAITeam.randomNb = randomNb;    //and give the information to the other team (so they have the same size)
        }
    }

    void Start()
    {
        for (int i = 0; i < randomNb; i++) //instantiates all the players at a random point in the field
        {
            Instantiate(ai, new Vector3(Random.Range(-5f, 5f), 0, Random.Range(7.5f, -7.5f)), Quaternion.identity, transform);
        }


        flagPosition = transform.position * -1;
        chase = new List<Transform>();
        ais = new List<AIMovement>();
        foreach (AIMovement ai in GetComponentsInChildren<AIMovement>())  //put all the ais in a list
        {
            ais.Add(ai);
        }
    }

    void Update()
    {
        updateAIStatesNb();
        if (GetFlagChaser() == null && NonFrozenAINb > 0)  //assigns one ai to chase the flag if no one else currently is and someone can (this is the top priority task)
        {
            SetupFlagChaser();
        }
        while (WanderingAINb + SavingAINb > 0 && chase.Count > PursuingAINb) //the second priority task is to pursue people entering our side of the field
        {
            StartPursue(chase[0]);
            updateAIStatesNb();
        }
        while (WanderingAINb > 0 && FrozenAINb > 0) // the third priority task is to save a teammate that has been frozen
        {
            if (StartSave() == false)
                break;
            updateAIStatesNb();
        }
    }


    #region ai state counts

    [HideInInspector]
    public int NonFrozenAINb;

    [HideInInspector]
    public int PursuingAINb;

    [HideInInspector]
    public int WanderingAINb;

    [HideInInspector]
    public int SavingAINb;

    [HideInInspector]
    public int FrozenAINb;

    void updateAIStatesNb()  //updates all the AINb variables
    {
        foreach (AIMovement ai in ais)
        {
            if (ai.state != AIMovement.State.FROZEN)
                NonFrozenAINb++;
            else
                FrozenAINb++;
            if (ai.state == AIMovement.State.PURSUE)
                PursuingAINb++;
            else if (ai.state == AIMovement.State.WANDER)
                WanderingAINb++;
            else if (ai.state == AIMovement.State.SAVING)
                SavingAINb++;
        }
    }

    #endregion

    private void SetupFlagChaser()   //assigns the flag getting task to someone
    {
        List<AIMovement> tmp = new List<AIMovement>();

        foreach (AIMovement ai in ais)
        {
            if (ai.state != AIMovement.State.FROZEN)
                tmp.Add(ai);
        }
        int nb = Random.Range(0, tmp.Count);
        tmp[nb].state = AIMovement.State.ARRIVE;
        tmp[nb].target = flagPosition;
    }

    private AIMovement GetFlagChaser()    //gets the current ai chasing the flag (or null)
    {
        foreach (AIMovement ai in ais)
        {
            if (ai.state == AIMovement.State.ARRIVE)
                return ai;
        }
        return null;
    }

    public void ChaseHim(Transform transform)   //add an ai to chase (this is called with a plane collider in toroidal when someone enters the enemy field)
    {
        chase.Add(transform);
    }

    public void StopChaseHim(Transform transform)  //stops chasing an ai (this is called with a plane collider in toroidal when someone leaves the enemy field)
    {
        chase.Remove(transform);
    }

    private void StartPursue(Transform transform)  //sets up the closest ai (in a state of lesser priority) to chase the enemy
    {
        float minDist = Mathf.Infinity;
        AIMovement tmp = null;

        foreach (AIMovement ai in ais)
        {
            if (ai.state == AIMovement.State.WANDER || ai.state == AIMovement.State.SAVING)
            {
                float dist = Vector3.Distance(transform.position, ai.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    tmp = ai;
                }
            }
        }
        if (tmp == null)
            return;
        tmp.state = AIMovement.State.PURSUE;
        tmp.chasing = transform.GetComponent<AIMovement>();
    }

    private bool StartSave()  //sets up the closest ai (in a state of lesser priority) to save its ally
    {
        AIMovement save = null;
        foreach (AIMovement ai in ais)
        {
            if (ai.state == AIMovement.State.FROZEN)
            {
                save = ai;
                break;
            }
        }
        if (save == null)
            return false;
        float minDist = Mathf.Infinity;
        AIMovement tmp = null;

        foreach (AIMovement ai in ais)
        {
            if (ai.state == AIMovement.State.WANDER)
            {
                float dist = Vector3.Distance(save.transform.position, ai.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    tmp = ai;
                }
            }
        }
        if (tmp == null)
            return false;
        tmp.state = AIMovement.State.SAVING;
        tmp.target = save.transform.position;
        tmp.chasing = save;
        return true;
    }
}
