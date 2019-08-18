using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public enum State   //the different states the ai can be in
    {
        WANDER,
        ARRIVE,
        FLEE,
        SEEK,
        PURSUE,
        FROZEN,
        SAVING,
        NONE
    }

    private float speed = 0;
    private bool turn = false;

    //all the hyper parameters the user can change to change the behaviours

    public float lowSpeed = 0.1f;
    public float smallDistance = 0.3f;
    public int smallArc = 15;
    public int mediumArc = 30;
    public float t2t = 0.5f;
    public float stoppingRadius = 0.1f;
    public float maxMag = 7f;

    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public State state;
    [HideInInspector]
    public Vector3 target;
    [HideInInspector]
    public Transform TransformTarget = null;
    [HideInInspector]
    public AIMovement chasing;

    private Animator anim;
    private AiTeam team;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (transform.parent)
            team = transform.parent.GetComponent<AiTeam>();
        state = State.WANDER;
    }
    void KinematicArrive(bool turn = true, int speedModifier = 1)  //the arrive behaviour
    {
        Vector3 dir = target - transform.position;

        float mag = 0;
//        if (dir.magnitude > stoppingRadius)       this was removed because the ai would stop before the flag
        mag = Mathf.Min(maxMag, dir.magnitude / t2t);  //gets the desired velocity magnitude
        velocity = mag * dir.normalized / speedModifier;   //gets the velocity
        transform.position += velocity * Time.deltaTime;   //changes the position
        speed = mag / maxMag / speedModifier;     //sets the speed for the animator
        if (turn)
            TurnTowardsTarget();
    }

    void KinematicFlee(bool turn = true) //the flee behaviour
    {
        Vector3 dir = transform.position - target;

        velocity = maxMag * dir.normalized;
        transform.position += velocity * Time.deltaTime;
        speed = 1;
        if (turn)
            TurnTowardsTarget();
    }

    void TurnTowardsTarget() //Lerps the rotation towards the target or away from the target (if fleeing)
    {
        if (Vector3.Distance(target, transform.position) == 0)  //in case they are on top of each other
            return;
        if (state == State.FLEE)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.position - target), 10);
        else
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target - transform.position), 10);
    }

    private float randomNb(int precision) //gets a random number between -1 and 1 more likely to be close to 0
    {
        float nb = 0;
        for (int i = 0; i < precision; i++)
        {
            nb += Random.Range(-1f, 1f);
        }
        nb /= precision;
        return nb;
    }

    private void KinematicWander() //the wander behaviour
    {
        transform.Rotate(0, randomNb(5) * smallArc, 0);
        velocity = transform.forward * maxMag;
        transform.position += velocity * Time.deltaTime;
        speed = 1;
    }

    private void KinematicSeek() //the seek behaviour
    {
        Vector3 dir = target - transform.position;

        velocity = dir.normalized * maxMag;
        transform.position += velocity * Time.deltaTime;
        speed = maxMag;
        TurnTowardsTarget();
    }

    private void KinematicPursue() //the purue behaviour
    {
        Vector3 dir = chasing.transform.position - transform.position;
        float distance = dir.magnitude;
        float speed = maxMag;
        float prediction = distance / speed;
        if (prediction > 1.5f)
            prediction = 1.5f;

        target = chasing.transform.position;
        target += chasing.velocity * prediction;
        KinematicSeek();
    }

    void Update() //calls the behaviours according to the state
    {
        if (Time.timeScale == 0)
            return;
        CheckPursueAndSave();
        if (TransformTarget != null)
            target = TransformTarget.position;
        if (state == State.WANDER)
        {
            KinematicWander();
        }
        else if (state == State.PURSUE)
        {
            KinematicPursue();
        }
        else if (state == State.SEEK)
        {
            KinematicSeek();
        }
        else if (state == State.ARRIVE || state == State.SAVING)
        {
            if (speed < lowSpeed)
            {
                if (Vector3.Distance(transform.position, target) < smallDistance)
                {
                    KinematicArrive(false);
                }
                else if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(target - transform.position)) > smallArc)
                {
                    TurnTowardsTarget();
                }
                else
                {
                    KinematicArrive();
                }
            }
            else
            {
                float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(target - transform.position));

                if (angle > mediumArc)
                {
                    TurnTowardsTarget();
                }
                else if (angle > smallArc)
                {
                    KinematicArrive(true, 4);
                }
                else
                {
                    KinematicArrive();
                }
            }
        }
        else if (state == State.FLEE)
        {
            if (Vector3.Distance(transform.position, target) < smallDistance)
            {
                KinematicFlee(false);
            }
            else
            {
                if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(transform.position - target)) > smallArc)
                {
                    TurnTowardsTarget();
                }
                else
                {
                    KinematicFlee();
                }
            }
        }
        else
        {
            speed = 0;
        }
        anim.SetFloat("MoveSpeed", speed);
    }

    private void OnTriggerEnter(Collider other)  //when it picks up the flag goes back to its own field
    {
        target = target * -1;
    }

    private Transform GetFlag(Transform t)  //picks up the flag
    {
        for (int i = 0; i < t.transform.childCount; i++)
        {
            if (t.transform.GetChild(i).tag != "Untagged")
                return t.transform.GetChild(i);
        }
        return null;
    }

    private void CheckPursueAndSave()
    {
        if (team == null)
            return;
        if (state == State.PURSUE && Vector3.Distance(transform.position, chasing.transform.position) < 0.1f)  //if it is pursuing and hits the target
        {
            state = State.WANDER;     //puts its own state back to wandering (the aiTeam script will assign it something else if needed)
            chasing.state = State.FROZEN;  //puts the enemy state to frozen
            Transform flag;
            if ((flag = GetFlag(chasing.transform)) != null)   //if the enemy had a flag it will return it to its base
            {
                flag.transform.parent = GameObject.Find(flag.tag + "FlagBase").transform;
                flag.localPosition = Vector3.zero;
            }
            team.StopChaseHim(chasing.transform);
        }
        else if (state == State.SAVING && Vector3.Distance(transform.position, target) < 0.1f)  //if it hits a frozen ally gets them both back to wandering
        {
            state = State.WANDER;
            chasing.state = State.WANDER;
        }
    }
}
