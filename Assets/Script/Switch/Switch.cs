using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] public GameObject switchTargetObject;
    [SerializeField] Switchable switchTarget;
    Animator animator;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        if (switchTargetObject == null)
        {
            transform.GetComponent<Switch>().enabled = false;
            Debug.Log("Switch Target is Null");

        }
        else
        {
            switchTarget = switchTargetObject.GetComponent<Switchable>();
            Debug.Log("Switchable : " + switchTarget.ToString());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
    //    if (collision.transform.CompareTag("Throw"))
    //    {
    //        switchTarget.SwitchOff();
    //        animator.SetBool("Activation", false);
    //    }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Switch Colliding : " + collision.ToString());
        if (collision.CompareTag("Throw"))
        {
            switchTarget.SwitchOn();
            animator.SetBool("Activation", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Throw"))
        {
            switchTarget.SwitchOff();
            animator.SetBool("Activation", false);
        }
    }
}
