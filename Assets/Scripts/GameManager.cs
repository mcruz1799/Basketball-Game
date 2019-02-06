using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    static int score_small;
    static int score_tall;
    [SerializeField] static float game_time;
    [SerializeField] static int winning_score;
    [SerializeField] static Text game_time_text;
    [SerializeField] static Text small_score_text;
    [SerializeField] static Text tall_score_text;
    static bool overtime;
    //this is pointless, only needed to call StopCoroutine
    static GameManager gm;

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
        tall_score_text = GameObject.Find("HUDCanvas/team_tall/tall_pts/Pts").GetComponent<Text>();
        small_score_text = GameObject.Find("HUDCanvas/team_small/small_pts/Pts").GetComponent<Text>();
        game_time_text = GameObject.Find("HUDCanvas/game_time/time").GetComponent<Text>();
        game_time = 30.0f;
        start_game();
        Debug.Log("Starting Game with " + game_time + "Seconds");
    }

    static void start_game(){
        setGameTime(game_time);
        gm.StartCoroutine(GameTime());
        gm.StartCoroutine(UpdateTime());
        score_small = 0;
        score_tall = 0;
        overtime = false;
    }

    static void end_game(){
        gm.StopCoroutine(GameTime());
        overtime = false;
        if (score_tall > score_small){
            //TODO: ending seq where tall wins, eventually quit
            QuitGame();
        }
        else if (score_small > score_tall) {
            //TODO: ending seq where small wins, eventually quit
            QuitGame();
        }
        else{
            overtime = true;
            //display a screen saying WHOEVER SCORES NEXT WINS
        }
    }
    //scoring, to be called from player script
    public static void update_score(ScoreComponent.PlayerType p, int i){
        if (p == ScoreComponent.PlayerType.small) {
            score_small += i;
            small_score_text.text = score_small.ToString();
        }
        else {
            score_tall += i;
            tall_score_text.text = score_tall.ToString();
        }
        if (score_small >= winning_score || score_tall >= winning_score 
            || overtime)
        {
            end_game();
        }
    }

    public static void QuitGame(){
        Application.Quit();
    }

    public static void RestartGame(){
        //TODO: change to load scene of main menu
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
        while (game_time > 0)
        {
            game_time -= 1;
            yield return new WaitForSeconds(1);
            setGameTime(game_time);
        }
    }

    private static void setGameTime(float time)
    {
        float minutes = Mathf.Floor(game_time / 60);
        float seconds = game_time % 60;
        game_time_text.text = minutes + " : " + seconds;
    }
}