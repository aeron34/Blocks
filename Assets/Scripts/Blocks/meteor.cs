﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class meteor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var g = collision.gameObject;
        if (g.layer == 9)
        {
            try
            {
                g.GetComponent<block>().dud = true;
                g.GetComponent<block>().touching.Clear();
                g.GetComponent<block>().die = 1;
                Destroy(gameObject);
            }
            catch(NullReferenceException e)
            {
                return;
            }
        }
        if(g.layer == 12)
        {
            StartCoroutine(g.GetComponent<Gizmo>().hurt(50f, 1));

            Destroy(gameObject);


            //Implement code to goto Gameover screen



        }
    }
}
