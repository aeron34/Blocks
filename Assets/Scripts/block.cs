using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class block : MonoBehaviour
{
    private GameObject p;
    private SpriteRenderer spr;
    private Rigidbody2D rgb;
    public bool follow;
    public float thr_s = 5f;
    // Start is called before the first frame update
    void Start()
    {
        p = GameObject.Find("pic");
        rgb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (follow)
        {
            transform.position = new Vector3(
                p.transform.position.x,
                p.transform.position.y + 1.5f,
                0f);

            spr.flipX = p.GetComponent<SpriteRenderer>().flipX;
            rgb.simulated = false;
        }
    }

    public void thrown()
    {
        follow = false;
        rgb.simulated = true;
        rgb.velocity = new Vector2((50 * p.GetComponent<first>().di), thr_s);
    }

    private void dis()
    {
    }

} 
