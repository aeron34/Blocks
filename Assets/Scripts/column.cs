using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class column : MonoBehaviour
{
    public List<GameObject> blocks;
    public GameObject block, spawn, up_indi, ply=null;
    bool b_drp;
    // Start is called before the first frame update
    void Start()
    {
       // blocks = new List<GameObject>();
        block = Resources.Load<GameObject>("block");
        up_indi = GameObject.Find("up_indi");
    }

    // Update is called once per frame
    void Update()
    {
        if(b_drp && ply != null && !up_indi.GetComponent<AudioSource>().isPlaying)
        {
            up_indi.GetComponent<AudioSource>().PlayOneShot(up_indi.GetComponent<AudioSource>().clip);
            up_indi.transform.position = new Vector3(transform.position.x, transform.position.y + 4f,
                 transform.position.z);

        }
    }

    private IEnumerator indi()
    {
        up_indi.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(2f); 
        up_indi.GetComponent<SpriteRenderer>().enabled = false;
    }

    private IEnumerator realDrop(string col = "")
    {
        if (ply != null)
        {
            up_indi.transform.position = new Vector3(transform.position.x, transform.position.y + 4f,
            transform.position.z);
        }
        yield return new WaitForSeconds(1f);
        var n_b = Instantiate(block, transform.position, transform.rotation);
        n_b.transform.position = new Vector3(transform.position.x, 12.75f, 0);
        n_b.GetComponent<Rigidbody2D>().simulated = true;
        n_b.GetComponent<block>().color = col;
        n_b.GetComponent<block>().colm = gameObject;
        up_indi.transform.position = new Vector3(40f, transform.position.y + 4f,
        transform.position.z);
        
        b_drp = false;
    }

    public void drop(string col="")
    {
        b_drp = true;
        StartCoroutine(realDrop(col));
        // blocks.Add(n_b);
    }
    public void Takeoff(GameObject a)
    {
        foreach (GameObject n in blocks)
        {
            if (n != null && n.transform.position.y > a.transform.position.y)
            {
                n.GetComponent<block>().sChk = true;
            }
        }
        blocks.Remove(a);
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

            if (b_drp && up_indi.GetComponent<AudioSource>().isPlaying)
            {
                up_indi.GetComponent<AudioSource>().Stop();
            }
        }
    }
}
