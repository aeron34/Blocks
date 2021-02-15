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
    public bool online;
    private static int room, metes = 0;

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
        online = true;
        na = new Dictionary<string, string> { };

    }

    public void Get()
    {
        Login();   
    }

    public void SendMeteors(int x)
    {
        SendMets(x);
    }

    private static async Task SendMets(int x)
    {
        //string u = na["username"];

        string a = $"http://localhost:3000/send_mets?username=Son&meteors={x}";

        var content = await h.GetStringAsync(a);

       // Debug.Log(content);
    }

    private static async Task Login()
    {

        var u = GameObject.Find("username").GetComponent<TMP_InputField>().text;
        var p = GameObject.Find("password").GetComponent<TMP_InputField>().text;
        

        string a = $"http://localhost:3000/login?username={u}&pass={p}";
        string content = "none";

        var l = GameObject.Find("login");

        l.GetComponent<Button>().enabled = false;

        content = await h.GetStringAsync(a);
        Debug.Log(content);

        l.GetComponent<Button>().enabled = true;


        if (content == "done" && na.Count == 0)
        {
            na.Add("username", u);
            na.Add("password", p);
            GameObject.Find("Login Box").SetActive(false);
            call = true;
        }
        
    }

    private static async Task Logout()
    {
        var content = new FormUrlEncodedContent(na);
        var response = await h.PostAsync("http://localhost:3000/logout", content);
        //var responseString = await response.Content.ReadAsStringAsync();

    }

    /*  Online.cs, and this is an aysnc function that 
        get calls every 1.5 seconds.
        If the condition that updates the database 
        is statisfied, NODE JS/the server returns
        the text "run" and this funct. is no longer
        called.
    */
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

    private static async Task CheckMeteors()
    {
        var b = na["username"];
        string a = $"http://localhost:3000/get_mets?username={b}";
        var content = await h.GetStringAsync(a);

        if (content != "none")
        {
            FindObjectOfType<block_queue>().metes += Int32.Parse(content);
            metes = Int32.Parse(content);
        }
    }

    public IEnumerator GetMeteors()
    {

        yield return new WaitForSeconds(3f);
        CheckMeteors();

        Debug.Log($"ON: {metes}");

        if (metes > 0)
        {
            StartCoroutine(FindObjectOfType<block_queue>().MeteorTime());
            metes = 0;
        }

        StartCoroutine(GetMeteors());
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
          
            Debug.Log("aosjd");
            GameObject.Find("welcome").GetComponent<TextMeshProUGUI>().text = na["username"];

            GameObject.Find("welcome").SetActive(false);  StartCoroutine(SendUP());
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
