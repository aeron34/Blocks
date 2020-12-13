using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Text txt;
    private System.Random ran = new System.Random();
    public int min = 0, max = 1000, gs = 0, c = 400;
    // Start is called before the first frame update
    void Start()
    {
        txt.text = "Yo";
    }

    public void Up()
    {
        min = gs;
        gs = Guess();
        txt.text = gs.ToString();
    }
    public void Down()
    {
        max = gs;
        gs = Guess();
        txt.text = gs.ToString();
    }

    public int Guess()
    {
        int g = ran.Next(min, max);
        return g;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Up();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Down();
        }
    }
}
