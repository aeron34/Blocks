using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class first : MonoBehaviour
{
    // Start is called before the first frame update

    Rigidbody2D rgb;
    public Animator ani;
    private SpriteRenderer spr;
    public float xS = 0, xY = 15, di;
    bool jump = false;
    public int pickup;
    GameObject block = null;
    Camera camm;
    public float sh_dur = 0, sh_mag = 0;
    void Start()
    {
        camm = Camera.main;
        //Instantiate(n, transform.position, transform.rotation);
        //m_c = transform.Find("spawn_pos").gameObject.GetComponent<BoxCollider2D>();
        rgb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        // transform.position = cam.ViewportToWorldPoint(new Vector3(2 / (cam.orthographicSize * 4 * cam.aspect), 0.5f, 1));
    }


    public void none()
    {
        ani.Play("idle");
    }
    // Update is called once per frame

    void Update()
    {
        xS = 0;


        if (Input.GetKey(KeyCode.D))
        {
            xS = 10;
            di = 1;
            spr.flipX = false;
            //transform.position += m * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            xS = -10f;
            di = -1;
            spr.flipX = true;
            //transform.position += m * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            jump = true;

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Instantiate(n, t.position, t.rotation);
            if (pickup == 1)
            {
                block.GetComponent<block>().follow = true;
                GetComponent<BoxCollider2D>().offset = new Vector2(-0.006f, 0.09f);
                GetComponent<BoxCollider2D>().size = new Vector2(0.4f, 0.82f);
                pickup = 2;

            }
            else if (pickup == 2)
            {
                GetComponent<BoxCollider2D>().offset = new Vector2(-0.003f, -0.08f);
                GetComponent<BoxCollider2D>().size = new Vector2(0.33f, 0.46f);
                block.GetComponent<block>().thrown();
                pickup = 0;
            }
        }

        rgb.velocity = new Vector2(xS, rgb.velocity.y);

        if (jump)
        {
            rgb.velocity = new Vector2(rgb.velocity.x, xY);
            jump = false;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(camm.GetComponent<cam>().Shake(sh_dur,sh_mag));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9
        && pickup < 2)
        {
            pickup = 1;
            block = collision.gameObject;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (pickup != 2)
        {
            pickup = 0;
        }
    }
}