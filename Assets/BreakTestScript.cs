using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakTestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log(transform.childCount);
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(true);
                Debug.Log(child.name);
                SpriteRenderer s = child.GetComponent<SpriteRenderer>();
                child.GetComponent<Rigidbody2D>().velocity = (child.position - transform.position) * 3;
                
            }
            transform.GetComponent<SpriteRenderer>().enabled = false;
            //gameObject.SetActive(false);
        }
    }
}
