using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    
    // ID for power ups
    [SerializeField] // 0 = Triple Shot, 1 = Speed, 2 = Shield
    private int powerUpID; 

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
            Player player = other.transform.GetComponent<Player>();

            switch(powerUpID)
            {
                case 0:
                    player.TripleShotActive();
                    break;
                case 1:
                    player.SpeedBoostActive(); 
                    break;
                case 2:
                    player.ShieldActive();
                    break;
                default:
                    Debug.Log("Default Value");
                    break;
            }

            Destroy(this.gameObject);
        }
    }
}
