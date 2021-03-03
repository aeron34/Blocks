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
    private static Dictionary<string, string> user_info;
    private static bool call, running_room;
    private static List<string[]> opponent_info;

    private List<GameObject> opp_infos;
    // The game modes are: highest score wins & last man standing. self explanitory.
    public string game_mode = "highest score wins";
    private float timer = 0.0f;
    private int opp_info_init = -1;

    public bool online = true;
    private static int room, metes = 0, fetch_status;

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
        user_info = new Dictionary<string, string> { };
        game_mode = "highest score wins";
    }

    public void Get()
    {
        Login();   
    }

    public void SendMeteorsCaller(int x)
    {
        SendMeteors(x);
    }

    private static async Task GetOpponentsScore()
    {
       
        string username = user_info["username"];
        string room = "";//ser_info["room"];

        if (user_info.ContainsKey("room"))
        {
            room = user_info["room"];
        }
        else { 
            return; 
        }
        string a = $"http://localhost:3000/getopponentsscore?username={username}&room={room}";
        string content = "none";

        content = await h.GetStringAsync(a);

        opponent_info = new List<string[]>();
        var arr = content.Split('|');

        for (int i = 0; i < arr.Length; i++)
        {
            var sub_arr = arr[i].Split(',');
            opponent_info.Add(sub_arr);
        }

       // await FindObjectOfType<block_queue>().util.DisplayOpponentInfo(opponent_info);
        Debug.Log("Called");
        fetch_status = 2;
    }
    private static async Task SendMeteors(int x)
    {
        //string u = na["username"];
        var dict = new Dictionary<string, string>
        {
            { "username", user_info["username"] }, 
            { "room", user_info["room"] }, 
            { "mets", x.ToString()}, 
        };

        //form "postable object" if that makes any sense
        var content = new FormUrlEncodedContent(dict);
        string a = $"http://localhost:3000/send_mets";

        var response = await h.PostAsync(a, content);
        var responseString = await response.Content.ReadAsStringAsync();

        if (responseString == "error")
        {
            SendMeteors(x);
            return;
        }
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


        if (content == "logged in" && user_info.Count == 0)
        {
            user_info.Add("username", u);
            user_info.Add("password", p);
            GameObject.Find("Login Box").SetActive(false);
            call = true;
        }
        
    }

    public async Task GetOppInfo()
    {
        await GetOpponentsScore();
        Debug.Log(opp_info_init);
        if (opp_info_init == -1)
        {
            opp_info_init = 1;

            opp_infos = new List<GameObject>();
            var prefab = GameObject.FindObjectOfType<block_queue>().opp_info_container;
            var canvas = GameObject.Find("Canvas");
            float x = -1175, y = -450;
            int m = 0;

            for (int i = 0; i < opponent_info.Count-1; i++)
            {
                if (i % 5 == 0)
                {
                    m = 0;
                }
                var g = GameObject.Instantiate(prefab, canvas.transform);
                g.transform.localPosition = new Vector3((x + (500 * m)), y - ((int)(i / 5) * 180),
                g.transform.localPosition.z);
                g.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = opponent_info[i][0];
                g.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text
                    = $"SCORE: {opponent_info[i][1]}";
                m++;
                opp_infos.Add(g);
            }
        }

        for (int i = 0; i < opponent_info.Count; i++)
        {
            var obj = opp_infos.Find(a =>
           a.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text == opponent_info[i][0]);

            obj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = opponent_info[i][1];
        }
    }
    private static async Task Logout()
    {
        var content = new FormUrlEncodedContent(user_info);
        var response = await h.PostAsync("http://localhost:3000/logout", content);
        //var responseString = await response.Content.ReadAsStringAsync();

    }

    private static async Task CheckRooms()
    {
        if (!running_room)
        {
            string b = user_info["username"], room_number = "-1";

            if (user_info.ContainsKey("room"))
            {
                room_number = user_info["room"];
            }


            string a = $"http://localhost:3000/check_rooms?username={b}&room={room_number}";
            var content = await h.GetStringAsync(a);

            var arr = content.Split(',');

            if (arr.Length == 2)
            {

                if (!user_info.ContainsKey("room"))
                {
                    room = Int32.Parse(arr[1]);
                    user_info.Add("room", room.ToString());       
                    Debug.Log($"ROOM: {room}");

                }
                running_room = true;

                SceneManager.LoadScene("Game");

            }

            if (!user_info.ContainsKey("room"))
            {
                room = Int32.Parse(content); 
                Debug.Log($"ROOM: {room}");

                user_info.Add("room", room.ToString());
            }
        }
      
    }

    private static async Task CheckMeteors()
    {
        var b = user_info["username"];
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

    public IEnumerator CheckRoomsCaller()
    {
        /*
         * If user_info contains a "room" key 
         * then that means your good to go
         * bcuz you can only have a room key if
         * the room is full.
         */
        if (!running_room)
        {
            yield return new WaitForSeconds(1f);
            CheckRooms();
            StartCoroutine(CheckRoomsCaller());
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if(call)
        {
            GameObject.Find("welcome").GetComponent<TextMeshProUGUI>().text = user_info["username"];
            GameObject.Find("welcome").SetActive(false);
            StartCoroutine(CheckRoomsCaller());
            call = false;
        }

        if(SceneManager.GetActiveScene().name == "Game")
        {
            if(fetch_status == 0)
            {
                GetOppInfo();
                fetch_status = 1;
            }
            /*
             FETCH_STATUS, 0 means time for update,
             1 means pending arrival, 
                2 means arrived from which we 
            count down a timer and when it's up
            we reset to 0.
             */
            if(fetch_status == 1)
            {
                timer = 30f;
            }
            if(fetch_status == 2)
            {
                if (timer > 0f)
                {
                    timer -= 1f;
                }
                else
                {
                    fetch_status = 0;
                }
            }
        }
    }

    private static async Task DeleteFromRoom()
    {
        var content = new FormUrlEncodedContent(user_info);
        string a = $"http://localhost:3000/delete_from_room";

        var response = await h.PostAsync(a, content);
    }

    private void OnApplicationQuit()
    {
        try
        {
            if (user_info.ContainsKey("username"))
            {
                if (user_info["username"] != "")
                {
                    Logout();
                    if (!running_room)
                    {
                        DeleteFromRoom();
                    }
                }
            }
        }
        catch(NullReferenceException e)
        {

        }
    }
}
