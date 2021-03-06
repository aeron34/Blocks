﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class column : MonoBehaviour
{
    public List<GameObject> blocks;
    public GameObject block, spawn, up_indi, ply=null;
    bool b_drp;
    public bool casc = true;
    // Start is called before the first frame update
    void Start()
    {
       // blocks = new List<GameObject>();
       // block = Resources.Load<GameObject>("block");
        up_indi = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(b_drp && ply != null && !GameObject.Find("up_indi").GetComponent<AudioSource>().isPlaying)
        {
            GameObject.Find("up_indi").GetComponent<AudioSource>().PlayOneShot(GameObject.Find("up_indi").GetComponent<AudioSource>().clip);
            up_indi.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    private IEnumerator realDrop(string col = "")
    {

        yield return new WaitForSeconds(1f);
        var n_b = Instantiate(block, transform.position, transform.rotation);
        n_b.transform.position = new Vector3(transform.position.x, 12.75f, 0);
        n_b.GetComponent<Rigidbody2D>().simulated = true;
        n_b.GetComponent<block>().color = col;
        n_b.GetComponent<block>().colm = gameObject;
        up_indi.GetComponent<SpriteRenderer>().enabled = false;
        
        b_drp = false;
    }

    public IEnumerator Meteor(GameObject a)
    {
        b_drp = true;
        //Add an anim that makes the column 
        //red regardless of if ply is in or not.
        up_indi.GetComponent<SpriteRenderer>().enabled = true;

        yield return new WaitForSeconds(.5f);
        var n_b = Instantiate(a, transform.position, transform.rotation);
        n_b.transform.position = new Vector3(transform.position.x, 25, 0);
        up_indi.GetComponent<SpriteRenderer>().enabled = false;

        b_drp = false;
    }
    public void MakeTrainingBlock(string color="", int offset_y = 0)
    {
        float y = -4.15f + (2*offset_y) * .85f;
        var new_block = Instantiate(block, transform.position, transform.rotation);
        new_block.transform.position = new Vector3(transform.position.x, y, 0);
        new_block.GetComponent<Rigidbody2D>().simulated = true;
        new_block.GetComponent<block>().color = color.Trim(' ');
        new_block.GetComponent<block>().transform.localScale = new Vector3(.85f, .85f, 1);// = color;
        new_block.GetComponent<block>().colm = gameObject;

    }

    public void DestoryBlocks()
    {
        foreach (GameObject n in blocks)
        {
            Destroy(n);
        }
        blocks.Clear();
    }
    public void drop(string col="")
    {
        b_drp = true;
        StartCoroutine(realDrop(col));
        // blocks.Add(n_b);
    }
    public void Takeoff(GameObject a)
    {
        if (casc) 
        { 
            foreach (GameObject n in blocks)
            {
                if (n != null && n.transform.position.y > a.transform.position.y)
                {
                    try
                    {
                        n.GetComponent<block>().sChk = true;
                    } catch (NullReferenceException e)
                    {
                        continue;
                    }
                }
            } 
        }


        blocks.Remove(a);
        casc = true;
        //blocks.Clear();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            ply = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            ply = null;

            if (b_drp && GameObject.Find("up_indi").GetComponent<AudioSource>().isPlaying)
            {
                GameObject.Find("up_indi").GetComponent<AudioSource>().Stop();
            }
        }
    }
}
