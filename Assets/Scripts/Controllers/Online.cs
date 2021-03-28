using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Online : MonoBehaviour
{
    private static Dictionary<string, string> user_info;
    private static bool call, running_room;
    private static List<string[]> opponent_info;
    private List<GameObject> room_usernames;
    private static GameObject room_txt, room_box, friend_box;
    public GameObject username_text;
    private List<GameObject> opp_infos;
    // The game modes are: highest score wins & last man standing. self explanitory.

    public string game_mode = "highest score wins";
    private float timer = 0.0f;
    private int opp_info_init = -1;//, place;
    public bool online = true, runnable, team_battle;
    private string  room = "-1";
    private static int metes = 0, fetch_status, place, wins, losses;

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
        room_usernames = new List<GameObject>();
        Restart();
    }

    private IEnumerator Testpost()
    {
        WWWForm form = new WWWForm();
        form.AddField("myField", "myData");
        form.AddField("Game Name", "Mario Kart");

        UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/testing", form);
        yield return www.SendWebRequest();
        var content = www.downloadHandler.text;

        Debug.Log(content);
    }

    public void Get()
    {
        StartCoroutine(Login());   
    }

    public void SendMeteorsCaller(int x)
    {
        StartCoroutine(SendMeteors(x));
    }

    private void process_opp_info(string content)
    {
      
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

    private IEnumerator SendMeteors(int x)
    {
        bool sent = false;

        while (!sent)//string u = na["username"];
        { 

            var form = CreateField(new string[] { "username", "room" });
            form.AddField("mets", x.ToString());
            var uwr = CreatePostReq("send_mets", form);

            yield return uwr.SendWebRequest();

            var responseString = uwr.downloadHandler.text;

            if (responseString != "error")
            {
                sent = true;
            }
        }
    }

    private IEnumerator Login()
    {

        var u = GameObject.Find("username").GetComponent<TMP_InputField>().text;
        var p = GameObject.Find("password").GetComponent<TMP_InputField>().text;

        var l = GameObject.Find("login");

        l.GetComponent<Button>().enabled = false;

        string url = $"http://localhost:3000/login?username={u}&pass={p}";
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();

        var content = uwr.downloadHandler.text;

        l.GetComponent<Button>().enabled = true;


        if (content == "logged in" && user_info.Count == 0)
        {
            user_info.Add("username", u);
            user_info.Add("password", p);
            GameObject.Find("Login Box").SetActive(false);
            runnable = true;
            if (FindObjectOfType<MainController>().peer2peer == false)
            {
                room_txt.SetActive(true);
                room_txt.GetComponent<TextMeshProUGUI>().text = "Looking for room...";
                StartCoroutine(CheckRooms());
            }
            else
            {
                team_battle = false;
                friend_box.SetActive(true);
            }
        }
    }

    public void AddP2PRoom()
    {
        string txt = GameObject.Find("room").GetComponent<TMP_InputField>().text;
        user_info.Add("room", txt.Replace(" ", ""));
        StartCoroutine(CheckRooms());
    }

    private void Restart()
    {
        GameObject.Find("Login Box").SetActive(true);
        room_txt = GameObject.Find("room_txt");
        room_box = GameObject.Find("Room Box");
        friend_box = GameObject.Find("Friend Box");
        GameObject.Find("create room").GetComponent<Button>().onClick.AddListener(AddP2PRoom);
        friend_box.SetActive(false);

        room_txt.SetActive(false);
        room_box.SetActive(false);
        timer = 0.0f;
        opp_info_init = -1;//, place;
        online = true;
        running_room = false;
        metes = 0;
        room = "-1";
        user_info.Remove("room");
        room_usernames.Clear();
        fetch_status = 0;
        team_battle = FindObjectOfType<MainController>().teams;
        
    }
    public void SkipLogin()
    {
        Restart();      
        call = true;
        room_txt.SetActive(true);
        room_box.SetActive(true);
        // GameObject.Find("Login Box").SetActive(false);
        if (FindObjectOfType<MainController>().peer2peer == false)
        {
            room_txt.GetComponent<TextMeshProUGUI>().text = "Looking for room...";
            GameObject.Find("Login Box").SetActive(false);

            StartCoroutine(CheckRooms());
        }
        else
        {
            GameObject.Find("Login Box").SetActive(false);
            room_txt.SetActive(false);
            team_battle = false;
            friend_box.SetActive(true);
        }
    }
     
    public IEnumerator GetOppInfo()
    {
        if (user_info.ContainsKey("room"))
        {
            int score = FindObjectOfType<scorer>().GetScore();
            string username = user_info["username"];
            string room = user_info["room"];

            string url = $"http://localhost:3000/getopponentsscore?username={username}&room={room}&score={score}";

            UnityWebRequest uwr = UnityWebRequest.Get(url);
            yield return uwr.SendWebRequest();

            process_opp_info(uwr.downloadHandler.text);

            if (opp_info_init == -1)
            {

                opp_info_init = 1;

                opp_infos = new List<GameObject>();
                var prefab = GameObject.FindObjectOfType<block_queue>().opp_info_container;
                var canvas = GameObject.Find("Canvas");
                float x = -1175, y = -450;
                int m = 0;

                for (int i = 0; i < opponent_info.Count - 1; i++)
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

            foreach (GameObject g in opp_infos)
            {
                //g.transform.GetChild(0).GetComponent<Image>().color = new Color(255f, 0, 0, 183f);
            }

            for (int i = 0; i < opp_infos.Count; i++)
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
    }

    private WWWForm CreateField(string[] keys)
    {
        WWWForm form = new WWWForm();

        for (int i = 0; i < keys.Length; i++)
        {
            form.AddField($"{keys[i]}", $"{user_info[keys[i]]}");
        }
       
        return form;
    }
    
    private IEnumerator Logout()
    {
        WWWForm form = CreateField(new string[] {"username", "password"});

        var uwr = CreatePostReq("logout", form);
        yield return uwr.SendWebRequest();
        //var responseString = await response.Content.ReadAsStringAsync();
    }

    private IEnumerator CheckRooms()
    {
        if (FindObjectOfType<MainController>().teams)
        {
            team_battle = true;
        }

        while (!running_room)
        {
            string b = user_info["username"], room_number = "-1";
            if(team_battle)
            {
                room_number = "team-1";
            }

            if (user_info.ContainsKey("room"))
            {

                room_number = $"{user_info["room"]}";                
                room_txt.GetComponent<TextMeshProUGUI>().text = $"Waiting in Room \n\n {user_info["room"]}";
                room_txt.SetActive(true);
            }
            string url = $"http://localhost:3000/check_rooms?username={b}&room={room_number}";
            UnityWebRequest uwr = UnityWebRequest.Get(url);
            yield return uwr.SendWebRequest();

            var content = uwr.downloadHandler.text;

            var arr = content.Split(',');

            if (arr.Length == 2)
            {

                if (!user_info.ContainsKey("room"))
                {
                    
                    room = arr[1];
                    user_info.Add("room", room.ToString());       
                }
                running_room = true;
                room_txt.GetComponent<TextMeshProUGUI>().text = $"Starting Game...";
                SceneManager.LoadScene("Game");
                yield break;

            }

            if (!user_info.ContainsKey("room"))
            {
                room = content; 
                room_txt.GetComponent<TextMeshProUGUI>().text = $"Waiting in Room \n\n {room}";
                user_info.Add("room", room);
            }

            //Get a list of usernames for the people that are in the room
            //We can assign room_number to the userinfo because we can't 
            //reach this line until the above has finished.
            
            room_box.SetActive(true);

            if (user_info.ContainsKey("room"))
            {
                UnityWebRequest get_users = UnityWebRequest.Get($"http://localhost:3000/get_users_in_room?room={room_number}");
                yield return get_users.SendWebRequest();

                var get_response = get_users.downloadHandler.text;

                var users_array = get_response.Split(',');
                GameObject.Find("Player Count").GetComponent<TextMeshProUGUI>().text = $"Players In Room: {users_array.Length}";

                foreach (GameObject g in room_usernames)
                {

                    Destroy(g);

                }
                room_usernames.Clear();
                for (int i = 0; i < users_array.Length; i++)
                {
                    float offset_y = 121 - (room_usernames.ToArray().Length * 41);
                    var username = Instantiate(username_text, room_box.transform);
                    room_usernames.Add(username);
                    string obj_text = $"{users_array[i]}";

                    if (team_battle)
                    {
                        char team = 'A';
                        if (i >= 4)
                        {
                            team = 'B';
                        }
                        obj_text = $"{users_array[i]}       TEAM {team}";
                    }
                    username.GetComponent<TextMeshProUGUI>().text = obj_text;
                    username.transform.localPosition = new Vector3(
                        username.transform.localPosition.x,
                        offset_y,
                        username.transform.localPosition.z);

                }
            }

            yield return new WaitForSecondsRealtime(2.5f);
        } 
    }

    private IEnumerator CheckMeteors()
    {
        var b = user_info["username"];
        var url = $"http://localhost:3000/get_mets?username={b}";
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();

        var content = uwr.downloadHandler.text;

        if (content != "none")
        {
            FindObjectOfType<block_queue>().meteors += Int32.Parse(content);
            metes = Int32.Parse(content);
        }
    }

 
    public IEnumerator GetMeteors()
    {

        while (true && SceneManager.GetActiveScene().name == "Game")
        {
            StartCoroutine(CheckMeteors());

            if (metes > 0)
            {
                StartCoroutine(FindObjectOfType<block_queue>().MeteorTime());
                metes = 0;
            }
            yield return new WaitForSecondsRealtime(3);
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
                StartCoroutine(GetOppInfo());
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
                fetch_status = -1;

                if (team_battle)
                {
                    place = -1;
                }
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
    public IEnumerator SendResults(bool team_battle = false)
    {

        var form = CreateField(new string[] { "username", "password", "room" });
        
        string res = "win";
        if (!team_battle)
        {
            if (place > 1)
            {
                res = "loss";
            }

            form.AddField("result", res);
        }

        var uwr = CreatePostReq("send_result", form);
        yield return uwr.SendWebRequest();

        var res_string = uwr.downloadHandler.text;

        if(team_battle)
        {
            var result_text = GameObject.Find("result").GetComponent<TextMeshProUGUI>();
            var winloss = FindObjectOfType<MainController>().texts[0];
            result_text.text = $"YOU {res_string}";
            winloss.text = $"WINS: {wins + 1} 		LOSSES: {losses}";

            if (res_string == "loss")
            {
                winloss.text = $"WINS: {wins} 		LOSSES: {losses + 1}";
                result_text.text = $"YOU LOSE";
            }
        }
        user_info.Remove("room");
        user_info.Remove("score");
    }

    public int GetPlace()
    {
        var list = OrderContestants();
        int place = (list.FindIndex(x => x[0] == user_info["username"]))+1;
        return place;
    }

    private UnityWebRequest CreatePostReq(string endpoint = "", WWWForm form= null)
    {
        UnityWebRequest uwr = UnityWebRequest.Post($"http://localhost:3000/{endpoint}", form);

        return uwr;
    }
    private IEnumerator DeleteFromRoom()
    {
        var form = CreateField(new string[] {"username", "password"});
        var uwr = CreatePostReq("delete_from_room", form);
        yield return uwr.SendWebRequest();        
    }

    public IEnumerator EndGame(bool team_battle = false, int score=0)
    {
        var form = CreateField(new string[] { "username", "password"});

        form.AddField("score", score.ToString());
        
        var uwr = CreatePostReq("end_game", form);
        yield return uwr.SendWebRequest();

        var res_string = uwr.downloadHandler.text;


        var arr = res_string.Split(',');
        FindObjectOfType<MainController>().SetTexts();
        var texts = FindObjectOfType<MainController>().texts;

        wins = Int32.Parse(arr[0]);
        losses = Int32.Parse(arr[1]);

        if(!team_battle)
        { 
            if (place > 1)
            {
                texts[0].text = $"WINS: {wins} 		LOSSES: {losses + 1}";
            }
            else
            {
                texts[0].text = $"WINS: {wins + 1} 		LOSSES: {losses}";
            }
        }
        texts[2].text = $"HIGHSCORE: {Int32.Parse(arr[2])}";

        if (Int32.Parse(arr[2]) < score) 
        {
            FindObjectOfType<MainController>().new_record.SetActive(true);
        }
    }

    public Dictionary<string, string> ReturnUser()
    {
        if(user_info.ContainsKey("username"))
        {
            return user_info;
        }
        return new Dictionary<string, string>();
    }

    private void OnApplicationQuit()
    {
        try
        {
            if (user_info.ContainsKey("username"))
            {
                if (user_info["username"] != "")
                {
                    StartCoroutine(Logout());
                    if (!running_room)
                    {
                        StartCoroutine(DeleteFromRoom());
                    }
                }
            }
        }
        catch(NullReferenceException e)
        {

        }
    }
}
