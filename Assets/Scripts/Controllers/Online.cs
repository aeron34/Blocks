using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Online : MonoBehaviour
{
    private static HttpClient h = new HttpClient();
    private static Dictionary<string, string> user_info;
    private static bool call, running_room;
    private static List<string[]> opponent_info;
    private static GameObject room_txt;

    private List<GameObject> opp_infos;
    // The game modes are: highest score wins & last man standing. self explanitory.

    public string game_mode = "highest score wins";
    private float timer = 0.0f;
    private int opp_info_init = -1;//, place;
    public bool online = true, runnable;
    private static int room=-1, metes = 0, fetch_status, place;

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
        room_txt = GameObject.Find("room_txt");
        room_txt.SetActive(false);
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
        int score = FindObjectOfType<scorer>().GetScore();
        string username = user_info["username"];
        string room = "";//ser_info["room"];

        if (user_info.ContainsKey("room"))
        {
            room = user_info["room"];
        }
        else { 
            return; 
        }
        string a = $"http://localhost:3000/getopponentsscore?username={username}&room={room}&score={score}";
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

    private async Task Login()
    {

        var u = GameObject.Find("username").GetComponent<TMP_InputField>().text;
        var p = GameObject.Find("password").GetComponent<TMP_InputField>().text;
        

        string a = $"http://localhost:3000/login?username={u}&pass={p}";
        string content = "none";

        var l = GameObject.Find("login");

        l.GetComponent<Button>().enabled = false;

        content = await h.GetStringAsync(a);

        l.GetComponent<Button>().enabled = true;


        if (content == "logged in" && user_info.Count == 0)
        {
            user_info.Add("username", u);
            user_info.Add("password", p);
            room_txt.SetActive(true);
            room_txt.GetComponent<TextMeshProUGUI>().text = "Looking for room...";
            GameObject.Find("Login Box").SetActive(false);
            runnable = true;
            StartCoroutine(CheckRooms());
        }
        
    }
    private void Restart()
    {
        GameObject.Find("Login Box").SetActive(true);
        room_txt = GameObject.Find("room_txt");
        timer = 0.0f;
        opp_info_init = -1;//, place;
        online = true;
        running_room = false;
        metes = 0;
        room = -1;
        fetch_status = 0;
    }
    public void SkipLogin()
    {
        Restart();      
        call = true;
        room_txt.SetActive(true);
        room_txt.GetComponent<TextMeshProUGUI>().text = "Looking for room...";
        GameObject.Find("Login Box").SetActive(false);
        StartCoroutine(CheckRooms());

    }
    public async Task GetOppInfo()
    {
        
        await GetOpponentsScore();

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

        foreach(GameObject g in opp_infos)
        {
            //g.transform.GetChild(0).GetComponent<Image>().color = new Color(255f, 0, 0, 183f);
        }

        for (int i = 0; i < opponent_info.Count; i++)
        {
            var obj = opp_infos.Find(a =>
            a.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text == opponent_info[i][0]);
            obj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = opponent_info[i][1];
            //obj.transform.GetChild(0).GetComponent<Image>().color = new Color(255f, 0, 0, 183f);
        }

        /*
         Use this for last man standing
        
        if(opponent_info.Count == 0)
        {
            FindObjectOfType<MainController>().LoadLoss();
        }*/
    }
    private static async Task Logout()
    {
        var content = new FormUrlEncodedContent(user_info);
        var response = await h.PostAsync("http://localhost:3000/logout", content);
        //var responseString = await response.Content.ReadAsStringAsync();

    }

    private IEnumerator CheckRooms()
    {
         
        while(!running_room)
        {
            string b = user_info["username"], room_number = "-1";


            if (user_info.ContainsKey("room"))
            {
                room_number = user_info["room"];
            }

            UnityWebRequest uwr = UnityWebRequest.Get($"http://localhost:3000/check_rooms?username={b}&room={room_number}");
            yield return uwr.SendWebRequest();

            var content = uwr.downloadHandler.text;

            var arr = content.Split(',');

            if (arr.Length == 2)
            {

                if (!user_info.ContainsKey("room"))
                {
                    room = Int32.Parse(arr[1]);
                    user_info.Add("room", room.ToString());       
                }
                running_room = true;
                room_txt.GetComponent<TextMeshProUGUI>().text = $"Starting Game...";
                SceneManager.LoadScene("Game");
                yield break;

            }

            if (!user_info.ContainsKey("room"))
            {
                room = Int32.Parse(content); 
                room_txt.GetComponent<TextMeshProUGUI>().text = $"Waiting in Room \n\n {room}";
                user_info.Add("room", room.ToString());
            }

            yield return new WaitForEndOfFrame();
        } 
    }

    private static async Task CheckMeteors()
    {
        var b = user_info["username"];
        HttpClient http = new HttpClient();
        string a = $"http://localhost:3000/get_mets?username={b}";
        var content = await http.GetStringAsync(a);

        if (content != "none")
        {
            FindObjectOfType<block_queue>().meteors += Int32.Parse(content);
            metes = Int32.Parse(content);
        }
    }

 
    public IEnumerator GetMeteors()
    {

        while (true)
        {
            CheckMeteors();

            if (metes > 0)
            {
                StartCoroutine(FindObjectOfType<block_queue>().MeteorTime());
                metes = 0;
            }
            yield return new WaitForSeconds(4f);
        }
    }


    // Update is called once per frame
    void Update()
    {

        if(call)
        { 
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
                timer = 60f;
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

            if (FindObjectOfType<block_queue>().over && fetch_status != -1)
            {
                var orderedlist = OrderContestants();
                var main_con = FindObjectOfType<MainController>();
                int score = FindObjectOfType<scorer>().GetScore();

                var g = orderedlist.First();

                place = GetPlace();
                EndGame();
                fetch_status = -1;
                SendResults();
                
                main_con.LoadLoss(score, place);

            }
        }
    }

    public List<string[]> OrderContestants()
    {
        opponent_info.RemoveAt(opponent_info.Count - 1);
        opponent_info.Add(new string[] {$"{user_info["username"]}",
        $"{FindObjectOfType<scorer>().GetScore()}" });

        return opponent_info.OrderByDescending(x => Int32.Parse(x[1])).ToList();
    }
    public static async Task SendResults()
    {
        Dictionary<string, string> body = new Dictionary<string, string>();
        body.Add("username", user_info["username"]);
        body.Add("password", user_info["password"]);
        string res = "win";
        if(place > 1)
        {
            res = "loss";
        }
        body.Add("result", res);
        var content = new FormUrlEncodedContent(body);
        string a = $"http://localhost:3000/send_result";
        var response = await h.PostAsync(a, content);

        //var res_string = await response.Content.ReadAsStringAsync();
        user_info.Remove("room");
        user_info.Remove("score");
    }

    public int GetPlace()
    {
        var list = OrderContestants();
        int place = (list.FindIndex(x => x[0] == user_info["username"]))+1;
        return place;
    }
    private static async Task DeleteFromRoom()
    {
        var content = new FormUrlEncodedContent(user_info);
        string a = $"http://localhost:3000/delete_from_room";
        var response = await h.PostAsync(a, content);
    }
   
    public static async Task EndGame()
    {

        user_info.Add("score", FindObjectOfType<scorer>().GetScore().ToString());
        var content = new FormUrlEncodedContent(user_info);
        string a = $"http://localhost:3000/end_game";
        var response = await h.PostAsync(a, content);

        var res_string = await response.Content.ReadAsStringAsync();

        var arr = res_string.Split(',');
        var texts = FindObjectOfType<MainController>().texts;
        if (place > 1)
        {
            texts[0].text = $"WINS: {arr[0]} 		LOSSES: {Int32.Parse(arr[1]) + 1}";
        }
        else
        {
            texts[0].text = $"WINS: {Int32.Parse(arr[0])+1} 		LOSSES: {arr[1]}";
        }
        texts[2].text = $"HIGHSCORE: {Int32.Parse(arr[2])}";

        if(Int32.Parse(arr[2]) < Int32.Parse(user_info["score"]))
        {
            FindObjectOfType<MainController>().new_record.SetActive(true);
        }
    }

    public Dictionary<string, string> ReturnUser()
    {
        return user_info;
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
