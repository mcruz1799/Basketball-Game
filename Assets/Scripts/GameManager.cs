using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    static int score_small;
    static int score_tall;
    [SerializeField] static float game_time;
    [SerializeField] static int winning_score;
    static bool overtime;

    /********************************************************
                       UI ELEMENTS TO ADD:
     * Main Menu
     * In SmallPlayer, if player collides with Basket, update score
     * Ending game for tall [with quit or restart button]
     * Ending game for small [with quit or restart button]
     * Overtime display
     * Instructions panel
     * HUD (current scores, current time left)
     ********************************************************/

    void Start(){
        StartCoroutine(GameTime());
        score_small = 0;
        score_tall = 0;
        overtime = false;
    }
    static void end_game(){
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
    public void update_score_small(int i){
        score_small += i;
        if (score_small >= winning_score || overtime){
            StopAllCoroutines();
            end_game();
        }
    }
    public void update_score_tall(int i){
        score_tall += i;
        if (score_tall >= winning_score || overtime){
            StopAllCoroutines();
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

    IEnumerator GameTime(){
        yield return new WaitForSeconds(game_time);
        end_game();
    }
}