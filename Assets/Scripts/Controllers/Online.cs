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
    public bool online = true;
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
        var dict = new Dictionary<string, string>
        {
            { "username", na["username"] }, 
            { "room", na["room"] }, 
            { "mets", x.ToString()}, 
        };

        //form "postable object" if that makes any sense
        var content = new FormUrlEncodedContent(dict);
        string a = $"http://localhost:3000/send_mets";

        var response = await h.PostAsync(a, content);
        var str = await response.Content.ReadAsStringAsync();
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


        if (content == "logged in" && na.Count == 0)
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

    private static async Task CheckRooms()
    {
        string b = na["username"];
        string a = $"http://localhost:3000/rooms?username={b}";
        var content = await h.GetStringAsync(a);

        Debug.Log(content);
        
        if(Int32.Parse(content) > -1)
        {
            room = Int32.Parse(content);
            na.Add("room", room.ToString());
            SceneManager.LoadScene("Game");
        }
    }

    private static async Task CheckMeteors()
    {
        var b = na["username"];
        string a = $"http://localhost:3000/get_mets?username={b}";
        var content = await h.GetStringAsync(a);

        if (content != "none")
        {
            FindObjectOfType<block_queue>().meteors += Int32.Parse(content);
            metes = Int32.Parse(content);
        }
    }

    public IEnumerator GetMeteors()
    {

        yield return new WaitForSeconds(3f);
        CheckMeteors();

        if (metes > 0)
        {
            StartCoroutine(FindObjectOfType<block_queue>().MeteorTime());
            metes = 0;
        }

        StartCoroutine(GetMeteors());
    }

    public IEnumerator SendUP()
    {
        if (!na.ContainsKey("room"))
        {
            yield return new WaitForSeconds(1f);
            CheckRooms();
            StartCoroutine(SendUP());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(call)
        {
          
            Debug.Log("aosjd");
            GameObject.Find("welcome").GetComponent<TextMeshProUGUI>().text = na["username"];

            GameObject.Find("welcome").SetActive(false);
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
