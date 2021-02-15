using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cursor : MonoBehaviour
{

    public GameObject[] blks;
    // Start is called before the first frame update
    void Start()
    {
        blks = new GameObject[2] { null, null };   
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(
        Input.mousePosition.x, Input.mousePosition.y, 0f));

        transform.position = new Vector3(pos.x + .1f, pos.y - .1f, 0);

    }
}
