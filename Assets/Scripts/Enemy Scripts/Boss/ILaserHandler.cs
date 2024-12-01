using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILaserHandler 
{
    void FireLaser(Transform bossTransform, Transform playerTransform);
}
