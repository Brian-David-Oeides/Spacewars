using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeLaser : MonoBehaviour
{
    private Vector3 arcTarget;
    private float speed = 8f;
    private float destroyY;

    public void Initialize(Vector3 arcTargetPosition, float destroyBoundary)
    {
        arcTarget = arcTargetPosition;
        destroyY = destroyBoundary;
    }

    private void Update()
    {
        Vector3 direction = (arcTarget - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (transform.position.y <= destroyY)
        {
            Destroy(gameObject);
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
            Destroy(gameObject);
        }
    }
}
