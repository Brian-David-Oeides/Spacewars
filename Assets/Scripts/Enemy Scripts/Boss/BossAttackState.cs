using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : IBossState
{
    private Boss _boss;
    private ILaserHandler _laserHandler;

    private bool isMovingToStart = true;
    private bool isSideToSideComplete = false; // Tracks side-to-side completion

    private float idleTimer;
    private int sideToSideRepetitions;
    private float elapsedTime;

    private float laserFireCooldown = 1.5f; // Time in seconds between laser spawns
    private float laserFireTimer = 0f; // Tracks elapsed time since last fire

    public BossAttackState(Boss boss)
    {  
        _boss = boss;
        _laserHandler = _boss.GetAttackLaserHandler(); // Use Attack Laser Handler
    }
   

    public void Enter(Boss boss)
    {
        // Initialize variables
        isMovingToStart = true;
        isSideToSideComplete = false;

        idleTimer = 0.5f; // Properly initialize the idle timer
        elapsedTime = 0f;
        sideToSideRepetitions = Random.Range(3, 6); // Random side-to-side repetitions

        Debug.Log("Entered BossAttackState. Preparing to move to attack position.");
    }

    public void Execute(Boss boss)
    {

        if (isMovingToStart)
        {
            MoveToAttackStart(boss);
        }
        else if (sideToSideRepetitions > 0)
        {
            PerformAttackBehavior(boss);
            // Handle laser firing
            laserFireTimer += Time.deltaTime;
            if (laserFireTimer >= laserFireCooldown)
            {
                _laserHandler.FireLaser(boss.transform, boss.playerTransform);
                laserFireTimer = 0f; // Reset the timer
            }
        }
        else if (isSideToSideComplete) // Transition only when side-to-side movement is complete
        {
            TransitionToNextState(boss);
        }
    }


    private void MoveToAttackStart(Boss boss)
    {
        // Target position for attack start
        Vector3 targetPosition = new Vector3(boss.transform.position.x, 5f, boss.transform.position.z);

        // Move boss towards target position
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, targetPosition, boss.speed * Time.deltaTime);

        if (Vector3.Distance(boss.transform.position, targetPosition) < 0.1f)
        {
            isMovingToStart = false; // Allow PerformAttackBehavior to execute
            elapsedTime = 0f;       // Reset elapsed time for oscillation
            Debug.Log("Boss reached attack position. Preparing for side-to-side movement.");
        }
    }

    private void PerformAttackBehavior(Boss boss)
    {
        // Idle briefly before starting side-to-side movement
        idleTimer -= Time.deltaTime;
        if (idleTimer > 0)
        {
            Debug.Log("Idling before side-to-side movement...");
            return; // Early return during idle phase
        }

        // Perform side-to-side movement using Mathf.Sin
        elapsedTime += Time.deltaTime * (boss.speed * 0.5f);
        float x = Mathf.Sin(elapsedTime) * 4f; // Oscillation amplitude
        
        boss.transform.position = new Vector3(x, 5f, boss.transform.position.z);

        // Check if a full oscillation (2π radians) is completed
        if (elapsedTime >= Mathf.PI * 2f)
        {
            elapsedTime = 0f; // Reset elapsedTime for the next oscillation
            sideToSideRepetitions--; // Decrement repetitions only after a full oscillation
            Debug.Log($"Full oscillation completed. Repetitions Left = {sideToSideRepetitions}");

            // Check if side-to-side repetitions are complete
            if (sideToSideRepetitions <= 0)
            {
                isSideToSideComplete = true; // Signal that the movement is complete
                Debug.Log("Side-to-side movement complete. Preparing to return to original position.");
            }
        }
    }

    private void TransitionToNextState(Boss boss)
    {
        // Smoothly return to the original position
        Vector3 targetPosition = new Vector3(0, 5f, boss.transform.position.z);
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, targetPosition, boss.speed * Time.deltaTime);

        // Once at the original position, transition to the evade state
        if (Vector3.Distance(boss.transform.position, targetPosition) < 0.1f)
        {
            Debug.Log("Boss returned to original position. Transitioning to BossEvadeState.");
            boss.SetState(new BossEvadeState(boss));
        }
    }

    public void Exit(Boss boss)
    {
        Debug.Log("Exiting BossAttackState.");
    }
}
