using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SmallPlayer))]
public class ScoreComponent : MonoBehaviour {
  public enum PlayerType { team1, team2 };

  private SmallPlayer scorer;

  private void Awake() {
    scorer = GetComponent<SmallPlayer>();
  }

  private void OnTriggerEnter(Collider other) {
    if (scorer.Below != null) {
      if (other.tag == "Basket1") {
        //TODO: update score based on distance
        GameManager.S.UpdateScore(PlayerType.team1, 1);
      } else if (other.tag == "Basket2") {
        //TODO: update score based on distance
        GameManager.S.UpdateScore(PlayerType.team2, 1);
      }
    }
  }
}
