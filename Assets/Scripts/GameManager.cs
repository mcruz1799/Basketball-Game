﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XboxCtrlrInput;

public class GameManager : MonoBehaviour {
    public XboxController controller;
    public GameObject ball;
    public Transform player1;
    public Transform player2;
    public Transform player3;
    public Transform player4;
    static int score_team1;
    static int score_team2;
    [SerializeField] int game_time_ins;
    [SerializeField] int winning_score_ins;
    static float game_time;
    static float curr_time;
    static int winning_score;
    static Text game_time_text;
    static Text team1_score_text;
    static Text team2_score_text;
    static bool overtime;
    //this is pointless, only needed to call StopCoroutine
    static GameManager gm;
    [SerializeField] GameObject winning_screen;

    /********************************************************
                       UI ELEMENTS TO ADD:
     * Main Menu
     * Ending game for tall [with quit or restart button]
     * Ending game for small [with quit or restart button]
     * Overtime display
     * Instructions panel
     * HUD (current scores, current time left)
     ********************************************************/

    void Start(){
        gm = this;
        team1_score_text = GameObject.Find("HUDCanvas/Team1/team1_pts/Pts").GetComponent<Text>();
        team2_score_text = GameObject.Find("HUDCanvas/Team2/team2_pts/Pts").GetComponent<Text>();
        game_time_text = GameObject.Find("HUDCanvas/game_time/time").GetComponent<Text>();
        game_time = 30.0f;
        start_game();
        Debug.Log("Starting Game with " + game_time + " Seconds");
        winning_screen.SetActive(false);
    }

    static void start_game(){
        gm.StartCoroutine(TipOff());
        score_team1 = 0;
        score_team2 = 0;
        overtime = false;
        game_time = gm.game_time_ins;
        winning_score = gm.winning_score_ins;
        curr_time = game_time;
        setGameTime(curr_time);
    }

    static void end_game(){
        gm.StopCoroutine(GameTime());
        overtime = false;
        if (score_team2 > score_team1){
            gm.winning_screen.GetComponent<Text>().text = 
                "Congratulations to Team 2!";
            gm.winning_screen.SetActive(true);
        }
        else if (score_team1 > score_team2) {
            gm.winning_screen.GetComponent<Text>().text = 
                "Congratulations to Team 1!";
            gm.winning_screen.SetActive(true);
        }
        else{
            overtime = true;
            //TODO: display a screen saying WHOEVER SCORES NEXT WINS
        }
    }
    //scoring, to be called from player script
    public static void update_score(ScoreComponent.PlayerType p, int i){
        if (p == ScoreComponent.PlayerType.team1) {
            score_team1 += i;
            team1_score_text.text = score_team1.ToString();
        }
        else {
            score_team2 += i;
            team2_score_text.text = score_team2.ToString();
        }
        if (score_team1 >= winning_score || score_team2 >= winning_score 
            || overtime)
        {
            end_game();
        }
    }

    public void QuitGame(){
        quit_game();
    }

    static void quit_game(){
        Application.Quit();
    }

    public void RestartGame(){
        restart_game();
    }

    static void restart_game(){
        //TODO: change to load scene of main menu
        score_team1 = 0;
        score_team2 = 0;
        curr_time = 0;
        //somehow reset player positions?
        SceneManager.LoadScene(SceneManager.GetActiveScene().name,
            LoadSceneMode.Single);
    }

    static IEnumerator GameTime(){
        yield return new WaitForSeconds(game_time);
        Debug.Log("Game Time Up");
        end_game();
    }

    static IEnumerator UpdateTime()
    {
        while (curr_time > 0)
        {
            curr_time -= 1;
            yield return new WaitForSeconds(1);
            setGameTime(curr_time);
        }
    }

    IEnumerator TipOff(){
        while (true){
            if (XCI.GetButtonDown(XboxButton.A)){
                switch(controller)
                {
                    case XboxController.First:
                        ball.GetComponent<Ball>().SetParent(player1);
                        break;
                    case XboxController.Second:
                        ball.GetComponent<Ball>().SetParent(player2);
                        break;
                    case XboxController.Third:
                        ball.GetComponent<Ball>().SetParent(player3);
                        break;
                    case XboxController.Fourth:
                        ball.GetComponent<Ball>().SetParent(player4);
                        break;
                }
            }
        }
        gm.StartCoroutine(GameTime()); //start counting the game
        gm.StartCoroutine(UpdateTime()); //start updating the game
        yield return new WaitForSeconds(0);
    }

    private static void setGameTime(float time)
    {
        float minutes = Mathf.Floor(curr_time / 60);
        float seconds = curr_time % 60;
        game_time_text.text = minutes + " : " + seconds;
    }
}