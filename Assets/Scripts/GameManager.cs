﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XboxCtrlrInput;

public class GameManager : MonoBehaviour {
  public static GameManager S;

#pragma warning disable 0649
  [SerializeField] private Ball _ball;

  [SerializeField] private SmallPlayer _smallPlayer1;
  [SerializeField] private TallPlayer _tallPlayer1;
  [SerializeField] private SmallPlayer _smallPlayer2;
  [SerializeField] private TallPlayer _tallPlayer2;

  [SerializeField] private int gameLength;
  [SerializeField] private int winningScore;

  [SerializeField] private GameObject winningScreen;
  [SerializeField] private Text winningText;
  [SerializeField] private GameObject tipoffScreen;
  [SerializeField] private GameObject overtimeScreen;
#pragma warning restore 0649

  public IBall Ball { get; private set; }
  public Vector3 ball_pos;

  public IPlayer SmallPlayer1 { get; private set; }
  public IPlayer TallPlayer1 { get; private set; }
  public IPlayer SmallPlayer2 { get; private set; }
  public IPlayer TallPlayer2 { get; private set; }

  private int score_team1;
  private int score_team2;
  private int winning_score;

  private float game_time;
  private float curr_time;

  private Text gameTimeText;
  private Text team1ScoreText;
  private Text team2ScoreText;
  private bool tipoff;

  private bool overtime;
  //this is pointless, only needed to call StopCoroutine

  /********************************************************
                     UI ELEMENTS TO ADD:
   * Main Menu
   * Ending game for tall [with quit or restart button]
   * Ending game for small [with quit or restart button]
   * Overtime display
   * Instructions panel
   * HUD (current scores, current time left)
   ********************************************************/

  private void Awake() {
    S = this;

    Ball = _ball;
    ball_pos = new Vector3(7, 0.8f, 10.5f);

    SmallPlayer1 = _smallPlayer1;
    TallPlayer1 = _tallPlayer1;
    SmallPlayer2 = _smallPlayer2;
    TallPlayer2 = _tallPlayer2;

    team1ScoreText = GameObject.Find("HUDCanvas/Team1/team1_pts/Pts").GetComponent<Text>();
    team2ScoreText = GameObject.Find("HUDCanvas/Team2/team2_pts/Pts").GetComponent<Text>();
    gameTimeText = GameObject.Find("HUDCanvas/game_time/time").GetComponent<Text>();

    StartGame();
  }

  private void StartGame() {
    Debug.Log("Starting Game with " + game_time + " Seconds");

    winningScreen.SetActive(false);
    overtimeScreen.SetActive(false);

    Ball.SetPosition(ball_pos);
    game_time = gameLength;
    winning_score = winningScore;
    score_team1 = 0;
    score_team2 = 0;
    overtime = false;
    curr_time = game_time;
    SetGameTime(curr_time);
    tipoff = true;
    tipoffScreen.SetActive(true);
    StartCoroutine(UpdateTime()); //start updating the game
    StartCoroutine(GameTime()); //start counting the game
  }

  private void EndGame() {
    //TODO: disable player movements
    S.StopAllCoroutines();
    overtimeScreen.SetActive(false);
    overtime = false;
    if (score_team2 > score_team1) {
      S.winningText.text = "Congratulations to \nTeam 2!";
      S.winningScreen.SetActive(true);
    } else if (score_team1 > score_team2) {
      S.winningText.text = "Congratulations to \nTeam 1!";
      S.winningScreen.SetActive(true);
    } else {
      overtime = true;
      S.StartCoroutine(OvertimeGame());
    }
  }

  //scoring, to be called from player script
  public void UpdateScore(ScoreComponent.PlayerType p, int i) {
    if (p == ScoreComponent.PlayerType.team1) {
      score_team1 += i;
      team1ScoreText.text = score_team1.ToString();
    } else {
      score_team2 += i;
      team2ScoreText.text = score_team2.ToString();
    }
    if (score_team1 >= winning_score || score_team2 >= winning_score
        || overtime) {
      EndGame();
    }
    Ball.SetPosition(ball_pos);
  }

  public void QuitGame() {
    Application.Quit();
  }

  public void RestartGame() {
    //TODO: change to load scene of main menu
    score_team1 = 0;
    score_team2 = 0;
    curr_time = game_time;
    overtime = false;
    //somehow reset player positions?
    SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
  }

  private IEnumerator GameTime() {
    yield return new WaitForSeconds(game_time);
    Debug.Log("Game Time Up");
    SetGameTime(0);
    EndGame();
  }

  private IEnumerator UpdateTime() {
    while (curr_time > 0) {
      curr_time -= 1;
      yield return new WaitForSeconds(1);
      SetGameTime(curr_time);
    }
  }

  private IEnumerator OvertimeGame() {
    overtimeScreen.SetActive(true);
    yield return new WaitForSeconds(3);
    overtimeScreen.SetActive(false);
  }

  private void SetGameTime(float time) {
    float minutes = Mathf.Floor(time / 60);
    float seconds = time % 60;
    string min_str = minutes.ToString();
    string sec_str = seconds.ToString();
    if (minutes < 10) min_str = "0" + min_str;
    if (seconds < 10) sec_str = "0" + sec_str;
    gameTimeText.text = min_str + ":" + sec_str;
  }

  //Players can check for tip-off priority.
  public void CheckTipOff(XboxController controller) {
    Debug.Log("Checking TipOff...");
    if (S.tipoff) {
      S.tipoff = false;
      switch (controller) {
        case XboxController.First: //small1
          SmallPlayer1.HoldBall(Ball);
          break;

        case XboxController.Second: //tall1
          TallPlayer1.HoldBall(Ball);
          break;

        case XboxController.Third: //small2
          SmallPlayer2.HoldBall(Ball);
          break;

        case XboxController.Fourth: //tall2
          TallPlayer2.HoldBall(Ball);
          break;
      }
      S.tipoffScreen.SetActive(false);
    }
  }
}