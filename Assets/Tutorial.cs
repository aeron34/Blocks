﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public Sprite[] sprs = new Sprite[10];
    int current_image = 0;
    SpriteRenderer spr_renderer;
    public TextMeshProUGUI tut_text;
    public string character = "Gizmo";
    // Start is called before the first frame update
    void Start()
    {
        spr_renderer = GetComponent<SpriteRenderer>();
        tut_text = GameObject.Find("tutorial").GetComponent<TextMeshProUGUI>();
        tut_text.enabled = false;
        if(character == "Boxer")
        {
            spr_renderer.sprite = sprs[9];
            for (int i = 1; i < 10; i++)
            {
                sprs[i] = null;
            }
        }
        else
        {
            Destroy(GameObject.Find("next"));
        }
    }

    public void SetText(string tesxt)
    {
        GameObject.Find("tutorial").GetComponent<TextMeshProUGUI>().text = tesxt;
    }

    public void NextImage()
    {
        current_image++;
        spr_renderer.sprite = sprs[current_image];
        spr_renderer.enabled = true;
        if (sprs[current_image] != null)
        {
            tut_text.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            spr_renderer.enabled = false;
            tut_text.enabled = true;
        }
    }
    
}
