using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XboxCtrlrInput;
using static GameState;

public enum GameState { MainMenu, PlayerSelection, Tipoff, InPlay, Overtime, GameOver }

public class GameManager : MonoBehaviour {
  public static GameManager S { get; private set; }

  public GameState State { get; private set; }

#pragma warning disable 0649
  [SerializeField] private Ball _ball;

  [SerializeField] private SmallPlayer _smallPlayer1;
  [SerializeField] private TallPlayer _tallPlayer1;
  [SerializeField] private SmallPlayer _smallPlayer2;
  [SerializeField] private TallPlayer _tallPlayer2;

  [SerializeField] private GameObject basket1;
  [SerializeField] private GameObject basket2;

  [SerializeField] private int gameLength;

  [SerializeField] private GameObject sp1_spawn;
  [SerializeField] private GameObject sp2_spawn;
  [SerializeField] private GameObject tp1_spawn;
  [SerializeField] private GameObject tp2_spawn;

  //Sound Effects
  [SerializeField] private AudioClip scoreSound;
  [SerializeField] private AudioClip endBuzzer;

  [SerializeField] public GameObject arrow;
#pragma warning restore 0649

  public IBall Ball { get; private set; }
  private Vector3 ballInitialPosition;

  public float Xboundary { get; } = 7.0f;
  public float Zboundary { get; } = 14.0f;

  public IPlayer SmallPlayer1 { get; private set; }
  public IPlayer TallPlayer1 { get; private set; }
  public IPlayer SmallPlayer2 { get; private set; }
  public IPlayer TallPlayer2 { get; private set; }

  //Exposed for MainGameGUI to look at
  public int ScoreTeam1 { get; private set; }
  public int ScoreTeam2 { get; private set; }

  public float CurrentTime { get; private set; }
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
    if (S == null) {
      S = this;
      DontDestroyOnLoad(this);
    } else {
      Debug.LogWarning("Duplicate GameManager detected and destroyed.");
      Destroy(gameObject);
    }

    if (basket1 == null || basket2 == null) {
      Debug.LogError("Baskets need to be initialized via inspector in GameManager!");
    }

    Ball = _ball;
    ballInitialPosition = _ball.transform.position;

    SmallPlayer1 = _smallPlayer1;
    TallPlayer1 = _tallPlayer1;
    SmallPlayer2 = _smallPlayer2;
    TallPlayer2 = _tallPlayer2;

    //Start from main menu
    State = MainMenu;
  }


  public void StartPlayerSelection() {
    State = PlayerSelection;
  }

  public void EndPlayerSelection() {
    if (State != PlayerSelection) {
      Debug.LogError("GameManager.EndPlayerSelection() called outside of the PlayerSelection phase");
      return;
    }

    StartGame();
  }

  public bool PositionIsOutOfBounds(Vector3 position) {
    return Mathf.Abs(position.x) > S.Xboundary || Mathf.Abs(position.z) > S.Zboundary;
  }

  private void StartGame() {
    Ball.SetParent(null);
    Ball.SetPosition(ballInitialPosition);

    ScoreTeam1 = 0;
    ScoreTeam2 = 0;
    CurrentTime = gameLength;

    State = Tipoff;

    //Start the game's timer
    StartCoroutine(GameTimeRoutine());
  }

  private void EndGame() {
    S.StopAllCoroutines();

    if (ScoreTeam2 != ScoreTeam1) {
      State = GameOver;
    } else {
      State = Overtime;
      MainGameGUI.S.ShowOvertimeScreen(3f);
    }
    SoundManager.Instance.Play(endBuzzer);
  }

  public GameObject BasketFromTeam(ScoreComponent.PlayerType team) {
    switch (team) {
      case ScoreComponent.PlayerType.team1:
        return basket1;
      case ScoreComponent.PlayerType.team2:
        return basket2;
      default:
        string errorMessage = "Undefined enum value";
        Debug.LogError(errorMessage);
        throw new System.Exception(errorMessage);
    }
  }

  public GameObject OpponentBasketFromTeam(ScoreComponent.PlayerType team) {
    switch (team) {
      case ScoreComponent.PlayerType.team1:
        return basket2;
      case ScoreComponent.PlayerType.team2:
        return basket1;
      default:
        string errorMessage = "Undefined enum value";
        Debug.LogError(errorMessage);
        throw new System.Exception(errorMessage);
    }
  }

  //Scoring, to be called from player script
  public void UpdateScore(ScoreComponent.PlayerType p, int i) {
    Ball.SetParent(null);
    Ball.SetPosition(ballInitialPosition);
     SoundManager.Instance.Play(scoreSound);
    if (p == ScoreComponent.PlayerType.team1) {
      ScoreTeam1 += i;
      SmallPlayer1.HoldBall(Ball);
    } else {
      ScoreTeam2 += i;
      SmallPlayer2.HoldBall(Ball);
    }
    MainGameGUI.S.UpdateScores();

    if (State == Overtime) {
      EndGame();
    }
    ResetAfterScore();
  }

  public void QuitGame() {
    Application.Quit();
  }

  public void RestartGame() {
    //TODO: change to load scene of main menu
    ScoreTeam1 = 0;
    ScoreTeam2 = 0;
    State = Tipoff;
    //somehow reset player positions?
    SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
  }

  private IEnumerator GameTimeRoutine() {
    CurrentTime = gameLength;
    while (CurrentTime > 0) {
      CurrentTime -= 1;
      yield return new WaitForSeconds(1);
      MainGameGUI.S.UpdateTime();
    }

    Debug.Log("Game Time Up");
    CurrentTime = 0f;
    MainGameGUI.S.UpdateTime();
    EndGame();
  }

  public void ResetAfterScore() {

    _smallPlayer1.transform.position = new Vector3(0,0,0);
    _smallPlayer2.transform.position = new Vector3(0, 0, 0);
    _tallPlayer1.transform.position = new Vector3(0, 0, 0);
    _tallPlayer2.transform.position = new Vector3(0, 0, 0);
    _tallPlayer1.ThrowSmallPlayer();
    _tallPlayer2.ThrowSmallPlayer();
    _smallPlayer1.transform.position = sp1_spawn.transform.position;
    _smallPlayer2.transform.position = sp2_spawn.transform.position;
    _tallPlayer1.transform.position = tp1_spawn.transform.position;
    _tallPlayer2.transform.position = tp2_spawn.transform.position;
    //S.tipoff = true;
    ///S.tipoffScreen.SetActive(true);
   }

  //Players can check for tip-off priority.
  public bool CheckTipOff(XboxController controller) {
    Debug.Log("Checking TipOff...");
    if (State == Tipoff) {
      State = InPlay;
      switch (controller) {
        case XboxController.First: //small1
          SmallPlayer1.HoldBall(Ball);
          return true;

        case XboxController.Second: //tall1
          TallPlayer1.HoldBall(Ball);
          return true;

        case XboxController.Third: //small2
          SmallPlayer2.HoldBall(Ball);
          return true;

        case XboxController.Fourth: //tall2
          TallPlayer2.HoldBall(Ball);
          return true;

        default:
          Debug.LogError("Illegal XboxController enum-value detected in GameManager.CheckTipoff");
          return false;
      }

    } else return false;
  }

  //TODO: Get rid of this, just have callers use "MainGameGUI.S.UpdatePossessionIndicator(team)"
  public void NotifyOfBallOwnership(ScoreComponent.PlayerType? team) {
    MainGameGUI.S.UpdatePossessionIndicator(team);
  }
}