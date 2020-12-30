using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spider : MonoBehaviour
{
    GameObject[] b;
    System.Random r = new System.Random();
    float t = 0, bx, by = -.15f;
    public bool move = true, swap = true;
    int di = 1, n = 0, mode=1;
    Vector2[] vs;
    GameObject[] b3;
    GameObject c_b;
    // Start is called before the first frame update
    void Start()
    {
        b = new GameObject[4] {null, null, null, null};
        b3= new GameObject[3] { null, null, null };
        vs = new Vector2[4] { Vector2.right, Vector2.left, Vector2.down, Vector2.up };
    }

    // Update is called once per frame
    void Update()
    {
        bx = 0;// -0.01f;
        by = -.01f;

        if(move)
        {
            transform.position = new Vector3(transform.position.x + bx, transform.position.y + by, 0);
        }
    }

    /*
     * Use FilpX and FlipY 
     * to handle the turns
     * of the spider.
     * 
     * If spider can withstand
     * 1+ expls. then make it
     * so he falls with grav.
     * and his box a coll. turns
     * back on briefly, then
     * revert back to prev.
     * values so we can crawl
     * again.
     * 
     * Create an array 
     * with the blocks 
     * on all directions 
     * of the spider, have
     * 0 - 1 indx's be the right
     * and left blocks, 
     * and 2 - 3 be up - down.
     * 
     * make spider explode after
     * succesful swap or natural
     * block explosion.
     */
    private void newDir()
    {
        // Debug.Log();
        di = r.Next(1,5);

        bx = 0;
        by = 0;
        switch (di)
        {
            case 1:
                bx = .015f;

                break;
            case 2:
                bx = -0.015f;
                break;
            case 3:
                by = 0.015f;
                break;
            case 4:
                by = -0.015f;
                break;
        }

    }

    private void FixedUpdate()
    {

        var c = Physics2D.Raycast(transform.position,
        Vector2.right, 0.25f, LayerMask.GetMask("blocks"));

        if (c.collider != null)
        {
            c_b = c.collider.gameObject;
        }

        if(mode == 1)
        {
            for(int i = 0; i < 3; i++)
            {
                b3[i] = null;
            }
            var down = Physics2D.RaycastAll(transform.position,
            Vector2.down, 1.25f, LayerMask.GetMask("blocks"));
          

            for(int i = 0; i < down.Length; i++)
            {
               b3[i] = down[i].collider.gameObject;
            }  
            if(b3[0] != null && b3[1] != null)
            { 
                if(b3[0].GetComponent<block>().color
                == b3[1].GetComponent<block>().color)
                {
                    return;
                }
            }
            
            if(down.Length == 2)
            {
                b3[2] = Physics2D.RaycastAll(b3[1].transform.position,
                Vector2.right, 2f, LayerMask.GetMask("blocks"))[1].collider.gameObject;

                if(b3[0].GetComponent<block>().color == b3[2].GetComponent<block>().color)
                {
                    b3[2].GetComponent<block>().swap(0);
                }

                return;
            }
        }


        if (mode == 0)
        {
            var rite = Physics2D.RaycastAll(transform.position,
            Vector2.right, 3f, LayerMask.GetMask("blocks"));


            for (int i = 0; i < rite.Length; i++)
            {
                b[i] = rite[i].collider.gameObject;
            }

            if (rite.Length >= 2)
            {
                if (b[0].GetComponent<block>().v_blk != null)
                {
                    if (b[1].GetComponent<block>().color ==
                    b[0].GetComponent<block>().v_blk.GetComponent<block>().color)
                    {
                        b[1].GetComponent<block>().swap();
                        for (int i = 0; i < 4; i++)
                        {
                            b[i] = null;
                        }
                        //Destroy(GetComponent<Spider>());
                        return;
                    }
                }
                if (b[2] != null)
                {
                    if (b[0].GetComponent<block>().color ==
                    b[2].GetComponent<block>().color)
                    {
                        b[1].GetComponent<block>().swap();
                        for (int i = 0; i < 4; i++)
                        {
                            b[i] = null;
                        }
                        //Destroy(GetComponent<Spider>());
                    }
                }
            }
        }
    }

    /*
    * Use raycasts to set the 
    * direction of the spider
    */
}
