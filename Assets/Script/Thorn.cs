using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorn : MonoBehaviour
{
    [SerializeField]
    float attackPower = 1f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Enter Player");
            collision.transform.GetComponent<Damageabel>().Hit(attackPower);
        }
    }
}
