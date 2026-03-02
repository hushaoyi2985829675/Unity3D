using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rd;
    [SerializeField]
    private float speed = 10;
    void Start()
    {
        rd = GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        rd.velocity = new Vector3(horizontal * speed, 0, vertical * speed);
    }
}
