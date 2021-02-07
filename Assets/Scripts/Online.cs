using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.SceneManagement;

public class Online : MonoBehaviour
{
    static readonly HttpClient h = new HttpClient();
    private static Dictionary<string, string> na;
    private static bool call;

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
        
        try
        {
            if(na["username"] != null)
            {
                u = na["username"];
            }
        }
        catch (NullReferenceException e)
        {

        }

        string a = $"http://localhost:3000/login?username={u}&pass={p}";



        var content = await h.GetStringAsync(a);

        if (content != "nope")
        {
            na = new Dictionary<string, string> { };
            na.Add("username", u);
            na.Add("password", p);
            call = true;
        }
    }

    private static async Task Logout()
    {
        var content = new FormUrlEncodedContent(na);
        var response = await h.PostAsync("http://localhost:3000/logout", content);
        //var responseString = await response.Content.ReadAsStringAsync();

    }
    private static async Task CheckRooms()
    {
        string b = na["username"];
        string a = $"http://localhost:3000/rooms?username={b}";
        var content = await h.GetStringAsync(a);


        if(content == "run")
        {
            SceneManager.LoadScene(1);
        }
    }

    public IEnumerator SendUP()
    {
        yield return new WaitForSeconds(1f);
        CheckRooms();
        StartCoroutine(SendUP());
    }
    // Update is called once per frame
    void Update()
    {
        if(call)
        {
            StartCoroutine(SendUP());
            call = false;
        }
    }

    private void OnApplicationQuit()
    {

        try
        {    
            if(na["username"] != "")
            {
                Logout();
            }
        }
        catch(NullReferenceException e)
        {
            return;
        }
    }
}
