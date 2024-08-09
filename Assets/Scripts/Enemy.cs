
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;

    private Player _player;
    // reference to Animator component
    private Animator _explosionAnimation;  
    
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        
        if (_player == null)
        {
            Debug.LogError("The player is NULL.");
        }
        // assign the component to the animation
        _explosionAnimation = GetComponent<Animator>();
        
        if (_explosionAnimation == null)
        {
            Debug.LogError("The player is NULL.");
        }
        
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

            // use SetTrigger anim
            _explosionAnimation.SetTrigger("OnEnemyDeath");
            _speed = 0;
            Destroy(this.gameObject, 2.8f); //add time delay parameter for anim
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(Random.Range(5, 10));
            }

            // use SetTrigger anim
            _explosionAnimation.SetTrigger("OnEnemyDeath");
            _speed = 0;
            Destroy(this.gameObject, 2.8f); 
        }
        
    }
}
