using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryCollider : MonoBehaviour
{
    // public Vector2[] MiddleOfEachCut;
    public int currentCut;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "InteractableObject")
        {
            
        }
    }
}
