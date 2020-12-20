using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class block_queue : MonoBehaviour
{

    public Transform g;
    float s = 0.5f, drp_tm;
    public GameObject n;
    // Start is called before the first frame update
    void Start()
    {
        var a = Instantiate(g, gameObject.transform);
        n = a.gameObject;        
       //rect.anchoredPosition = new Vector2(-41f);

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
           StartCoroutine(move(2));
        }
        drop();
    }

    public void drop()
    {
        var b = n.transform.Find("bar").gameObject;

        if(b != null)
        { 
            if(drp_tm < 1)
            {
                drp_tm += 0.05f*.0025f;
            }else
            {
                drp_tm = 0;
            }
            
            b.GetComponent<RectTransform>().localScale = new Vector2(drp_tm, .1f);
        }
    }

    public IEnumerator move(int b_d=1)
    {
        var rect = n.GetComponent<RectTransform>();
        var x = rect.anchoredPosition.x;
        float dist = Math.Abs(x) + (100f * b_d);

        while (Math.Abs(x) < dist)
        {
           
            float sx = Math.Abs(((dist - Mathf.Abs(x)) * 0.05f) + Mathf.Abs(x));
            if (Math.Abs(x) >= dist - 5)
            {
                sx = dist;
            }
            rect.anchoredPosition = new Vector2(-Math.Abs(sx), -50f);
            x = rect.anchoredPosition.x;
            yield return 0;
           // break;
        }          

        StopCoroutine("move");
    }
}
