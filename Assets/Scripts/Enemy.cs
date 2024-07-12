﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // how fast is the enemy moving?
    [SerializeField]
    private float _speed = 4f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // this game object move 4 m/s
        transform.Translate(Vector3.down * _speed * Time.deltaTime );
        
        float randomX = Random.Range(-8f, 8f);
        // if (this transforms position on Y is lest than -5)
        if (transform.position.y < -5f)
        {   // get transform's position set it to new vector that is random
            transform.position = new Vector3(randomX, 7, 0);
        }
    }
}
