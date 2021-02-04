using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
public class Online : MonoBehaviour
{
    static readonly HttpClient h = new HttpClient();
    private static Dictionary<string, string> na;

    void Awake()
    {
        if(FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Get()
    {
        Login();
    }

    private static async Task Login()
    {
        var u = GameObject.Find("username").GetComponent<TMP_InputField>().text;
        var p = GameObject.Find("password").GetComponent<TMP_InputField>().text;

        string a = $"http://localhost:3000/login?username={u}&pass={p}";

        var content = await h.GetStringAsync(a);

        
        Debug.Log(content);

        if(content != "nope")
        {
            na = new Dictionary<string, string> { };
            na.Add("username", content);
            
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            Debug.Log(na["username"]);
        }
    }

    private void OnApplicationQuit()
    {
        try
        {    
            Debug.Log(na["username"]);
        }
        catch(NullReferenceException e)
        {
            Debug.Log("no");
        }
    }
}
