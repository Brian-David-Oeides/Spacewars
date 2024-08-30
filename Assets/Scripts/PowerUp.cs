using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    
    [SerializeField] // 0 = Triple Shot, 1 = Speed, 2 = Shield, 3 = Ammo
    private int _powerUpID;
    [SerializeField]
    private AudioClip _clip;

  
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (this.transform.position.y < -4.7f)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_clip, transform.position); 

            switch(_powerUpID)
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
                case 3:
                    player.AmmoActive();
                    break;
                // add case 4
                    // player HealingActive
                    // break; 
                default:
                    Debug.Log("Default Value");
                    break;
            }

            Destroy(this.gameObject);
        }
    }
}
