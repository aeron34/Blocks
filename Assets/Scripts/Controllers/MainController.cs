using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{

    public bool training;
    private string char_name;
    GameObject char_select, main_menu;
    public GameObject Gizmo, Boxer, weap_box, ultra_bar;
    void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        main_menu = GameObject.Find("Main Menu");
        char_select = GameObject.Find("Char Select");
        main_menu.SetActive(true);
        char_select.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += LoadChar;
    }
    public void LoadGizmo()
    {
        LobbyMenu();
        char_name = "Gizmo";
    }   
    public void LoadBoxer()
    {
        LobbyMenu();
        char_name = "Boxer";
    }

    private void LoadChar(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Game")
        { 
        if (char_name == "Gizmo")
        {
            var canvas = GameObject.Find("Canvas");
            var g = Instantiate(Gizmo);
            g.name = "pic";
            Instantiate(weap_box, canvas.transform).name = "weap_box";
        }
        if (char_name == "Boxer")
        {
            var canvas = GameObject.Find("Canvas");
            var g = Instantiate(Boxer);
            g.name = "pic";
            Instantiate(ultra_bar, canvas.transform).name = "ultra_bar";
        }
    }
    }

    #region View and Scene Loaders

    public void LobbyMenu()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void MainMenu()
    {
        main_menu.SetActive(true);
        char_select.SetActive(false);
    }
    public void CharSelect()
    {
        main_menu.SetActive(false);
        char_select.SetActive(true);
    }
    
    #endregion

    void Update()
    {
        
    }
}
