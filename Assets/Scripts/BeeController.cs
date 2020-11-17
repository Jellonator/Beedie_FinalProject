using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeController : MonoBehaviour
{
    public float speed;

    enum BeeState {
        Shooting
    }

    private BeeState m_State = BeeState.Shooting;
    private Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(Vector3 direction) {
        transform.LookAt(transform.position + direction, Vector3.up);
        m_State = BeeState.Shooting;
        GetComponent<Rigidbody>().velocity = direction.normalized * speed;
    }

    void OnCollisionEnter(Collision col)
    {
        m_Rigidbody.velocity = Vector3.zero;
    }
}
