using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spider : MonoBehaviour
{

    public GameObject[] blks;
    public List<string> start;
    bool strt = true;
    public GameObject blk;
    private void Start()
    {
        blks = new GameObject[3] { null, null, null };
        start = new List<string>();//[3] { null, null, null };
        StartCoroutine(Timer());
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(1f);
        strt = false;
    }
    private void FixedUpdate()
    {
        var cast = Physics2D.RaycastAll(new Vector2(transform.position.x - 2.5f,
        transform.position.y), Vector2.right, 4f, LayerMask.GetMask("blocks"));

        if (strt)
        {
            if (cast[0].collider != null)
            {
                for (int i = 0; i < cast.Length; i++)
                {
                    start.Add(cast[i].collider.gameObject.GetComponent<block>().color);
                }
                strt = false;
            }
            return;
        }
        else
        {
            if (cast.Length > 0)
            {
                for (int i = 0; i < cast.Length; i++)
                {
                    if (i < 3)
                    {
                        blks[i] = cast[i].collider.gameObject;
                    }
                }
            }
        }
        if (blk.GetComponent<block>().die > 0)
        {
            Destroy(gameObject);
        }
    }
    private IEnumerator Timer()
    {
        var seconds = 0;
        while(seconds < 6)
        {
            seconds += 1;
            yield return new WaitForSeconds(1f);
        }

        Destroy(gameObject);
    }

    private void Update()
    {
      
        transform.position = new Vector3(transform.position.x,
        transform.position.y - 0.005f, 0);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!strt)
        {
            blk = collision.gameObject;
            if (blk.layer == 9)
            {
              
                if (transform.position.y > blk.transform.position.y)
                {
                    for (int i = 0; i < start.ToArray().Length; i++)
                    {
                        blks[i].GetComponent<block>().color = start[i];
                        blks[i].GetComponent<block>().MyColor();
                    }
                }
            }
        }
    }
}
