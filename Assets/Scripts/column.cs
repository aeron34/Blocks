using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class column : MonoBehaviour
{
    public List<GameObject> blocks;
    bool run = false;
    //float t = 0;
    // Start is called before the first frame update
    void Start()
    {
        blocks = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

   private void OnTriggerEnter2D(Collider2D collision)
    {
 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            var scr = collision.gameObject.GetComponent<block>();
         
            blocks.Remove(collision.gameObject);
            foreach (GameObject n in blocks)
            {
                n.GetComponent<block>().locked = false;
                n.GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            }
        }
    }
}
