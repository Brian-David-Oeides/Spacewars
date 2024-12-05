using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeLaser : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 direction;
    private float speed = 8f;
    private float sineAmplitude = 1f;
    private float sineFrequency = 2f;
    private float time = 0f;
    private bool isTracking = true;

    public void Initialize(Vector3 playerPosition)
    {
        targetPosition = playerPosition;
        direction = (targetPosition - transform.position).normalized;
    }

    private void Update()
    {
        if (isTracking)
        {
            MoveToTarget();
        }
        else
        {
            MoveInSineWave();
        }

        if (Mathf.Abs(transform.position.y) > 10f || Mathf.Abs(transform.position.x) > 10f)
            Destroy(gameObject);
    }

    private void MoveToTarget()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            isTracking = false;
    }

    private void MoveInSineWave()
    {
        float sineOffset = Mathf.Sin(time * sineFrequency) * sineAmplitude;
        transform.position += new Vector3(sineOffset, -speed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Handle collision with the player
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>()?.Damage(1);
            Destroy(gameObject);
        }
    }
}
