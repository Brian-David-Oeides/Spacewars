using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLaser : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed = 8f;
    private float destroyBoundary = -5.8f; // Y-axis boundary for destruction

    public void Initialize(Vector3 target, float destroyBoundary)
    {
        targetPosition = target;
        this.destroyBoundary = destroyBoundary; // Set the destruction boundary
    }

    private void Update()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        Debug.Log($"Laser moving toward: {targetPosition} from {transform.position}");
        if (transform.position.y <= destroyBoundary)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage(1);
            }
            Destroy(this.gameObject);
        }
    }
}
