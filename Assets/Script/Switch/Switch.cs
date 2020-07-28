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
        switchTarget = switchTargetObject.GetComponent<Switchable>();
        if (switchTarget == null)
            Debug.Log("Switch Target is Null");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
