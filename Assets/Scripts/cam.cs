using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam : MonoBehaviour
{
    public float duration, magnitude;
    public GameObject Target;
    Vector3 velocity = Vector3.zero;
    Camera camm;
    // Start is called before the first frame update
    void Start()
    {
        camm = Camera.main;
        duration = 0.1f;
        magnitude = 0.3f;
    }

    private void Update()
    {


    }
    // Update is called once per frame
    void FixedUpdate()
    {
        /*
         * DO NOT DELETE, FUNCTIONALITY IS FOR LOSS SCREEN AND SIMON + JOE PUZZLE MODE
         
        Time.timeScale = Mathf.Lerp(1.0f, 0.25f, 0.8f);
        camm.orthographicSize = Mathf.Lerp(camm.orthographicSize, 3, 0.0145f);
        transform.position = Vector3.Lerp(transform.position, 
            new Vector3(Target.transform.position.x, Target.transform.position.y, -5),
            0.1f);*/
    }
    public IEnumerator Shake()
    {
        Vector3 orignalPosition = new Vector3(-0.5f, 0, -10);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(x, y, -10f);
            elapsed += Time.deltaTime;
            yield return 0;
        }
        transform.position = orignalPosition;
        duration = 0.1f;
        magnitude = 0.3f;
    }

}
