using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class first : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject n;
    public Transform t;
    Rigidbody2D rgb;
    public Animator ani;
    float xS = 0, xY = 15;
    bool jump = false;
    public int side, sx;
    Camera cam;
    Color blue = new Color(0f, 255f, 0f);
    void Start()
    {
        //Instantiate(n, transform.position, transform.rotation);
        //m_c = transform.Find("spawn_pos").gameObject.GetComponent<BoxCollider2D>();
        rgb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        cam = Camera.main;
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

        if (!(Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A)))
        {
            if (side == 0)
            {
                if (Input.GetKey(KeyCode.D))
                {
                    xS = 10;

                    //transform.position += m * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    xS = -10f;
                    //transform.position += m * Time.deltaTime;
                }
            }

            if (side == -1 && Input.GetKey(KeyCode.D))
            {
                xS = 10;
            }

            if (side == 1 && Input.GetKey(KeyCode.A))
            {
                xS = -10f;
                //transform.position += m * Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            jump = true;

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(n, t.position, t.rotation);
        }

        rgb.velocity = new Vector2(xS, rgb.velocity.y);
        if (jump)
        {
            rgb.velocity = new Vector2(rgb.velocity.x, xY);
            jump = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*in other words(side = 1 / -1) when it's pressed up against 
        a plat or another it's pressed up on another block pressed 
        up against a plat.
        */

        Check(collision);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Check(collision);
    }
    
    private void Check(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {

            if (collision.gameObject.transform.position.x < transform.position.x)
            {
                side = -1;
            }

            if (collision.gameObject.transform.position.x > transform.position.x)
            {
                side = 1;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        side = 0;
        sx = 0;
    }
}
