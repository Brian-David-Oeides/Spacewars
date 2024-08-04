
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // how fast is the enemy moving?
    [SerializeField]
    private float _speed = 4f;

    private Player _player;
    
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // this game object move 4 m/s
        transform.Translate(Vector3.down * _speed * Time.deltaTime );
        
        
        // if (this transforms position on Y is lest than -5)
        if (transform.position.y < -5f)
        {   // get transform's position set it to new vector that is random
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            
            Destroy(this.gameObject);
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(Random.Range(5, 10));
            }

            Destroy(this.gameObject);
        }
        
    }
}
