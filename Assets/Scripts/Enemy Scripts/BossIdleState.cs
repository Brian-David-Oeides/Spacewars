using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdleState : IBossState
{
    private float idleTimer = 1f;
    private bool movedToCenter = false;
    private int lemniscateRepetitions;
    private float elapsedTime;

    public void Enter(Boss boss)
    {
        movedToCenter = false;
        idleTimer = 1f;
        lemniscateRepetitions = Random.Range(2, 5); // Random repetitions for lemniscate
        elapsedTime = 0f;
    }

    public void Execute(Boss boss)
    {
        if (!movedToCenter)
        {
            MoveToCenter(boss);
        }
        else
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                PerformLemniscateMovement(boss);
            }
        }
    }

    private void MoveToCenter(Boss boss)
    {
        Vector3 targetPosition = new Vector3(0, 1.5f, 0);
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, targetPosition, boss.speed * Time.deltaTime);

        if (Vector3.Distance(boss.transform.position, targetPosition) < 0.1f)
        {
            movedToCenter = true;
        }
    }

    private void PerformLemniscateMovement(Boss boss)
    {
        // Parameters for the lemniscate
        float horizontalScale = 3f; // Controls the width of the figure-eight
        float verticalScale = 1f;   // Controls the height of the figure-eight
        float centerY = 1.5f;       // Fixed y-axis position (center)

        // Update time
        elapsedTime += Time.deltaTime * boss.speed;

        // Parametric equations for lemniscate
        float x = horizontalScale * Mathf.Sin(elapsedTime);
        float y = verticalScale * Mathf.Sin(2 * elapsedTime) + centerY;

        // Move the boss
        boss.transform.localPosition = new Vector3(x, y, 0);

        // Check if the lemniscate movement is complete
        if (elapsedTime >= Mathf.PI * 2 * lemniscateRepetitions)
        {
            boss.SetState(new BossAttackState());
        }
    }

    public void Exit(Boss boss)
    {
        // Reset elapsed time to avoid cumulative errors
        elapsedTime = 0f;
    }
}
