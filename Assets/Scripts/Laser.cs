using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    private bool _isEnemyLaser = false;

    void Update()
    {
        if (_isEnemyLaser == false)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    void MoveUp()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.up);

        if (this.transform.position.y > 8.0f)
        {   
            if (this.transform.parent != null)
            {   
                Destroy(this.transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    void MoveDown()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.down);

        if (this.transform.position.y < -8.0f)
        {   
            if (this.transform.parent != null)
            {   
                Destroy(this.transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyLaser == true)
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
        }
    }
}
