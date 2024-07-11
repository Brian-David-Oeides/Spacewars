using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // speed attribute = 8 meters per second
    [SerializeField]
    private float _speed = 8.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //this game object needs to move up in real-time at 8 meters per second
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        // if laser position on the Y is greater than 7 
        if (this.transform.position.y > 7f)
        {
            // {destroy this game object}
            Destroy(this.gameObject, 5f);
        }
    }
}
