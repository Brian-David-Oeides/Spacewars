using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    // Start is called before the first frame update
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // move down speed of 3 (visible in the Inspector)
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        // destroy this when below game view
        if (this.transform.position.y < -4.7f)
        {
            Destroy(this.gameObject);
        }
    }

    // OnTrigger collision
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // communicate with Player script, get a reference to the component, assign the referent to the component
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.TripleShotActive();
            }

            Destroy(this.gameObject);
        }
    }
    // only collectible by the Player tag
}
