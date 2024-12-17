using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HYJ_Spawn : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] bool point1;
    [SerializeField] bool point2;
    [SerializeField] bool point3;
    [SerializeField] bool point4;
    [SerializeField] bool point5;
    [SerializeField] bool point6;
    void Start()
    {
        if (point1)
        {
            Instantiate(player,new Vector3(-2.5f,2f,0f),Quaternion.identity);
        }
        if (point2)
        {
            Instantiate(player,new Vector3(-2f,0,0),Quaternion.identity);
        }
        if (point3)
        {
            Instantiate(player, new Vector3(-0.5f, 2, 0), Quaternion.identity);
        }
        if (point4)
        {
            Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity);
        }
        if (point5)
        {
            Instantiate(player, new Vector3(1.5f, 2, 0), Quaternion.identity);
        }
        if (point6)
        {
            Instantiate(player, new Vector3(2, 0, 0), Quaternion.identity);
        }
    }

    void Update()
    {
        
    }
}
