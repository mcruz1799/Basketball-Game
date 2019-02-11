using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreComponent : MonoBehaviour
{
    public enum PlayerType {team1, team2};
    //[SerializeField] PlayerType current_player;

    void OnTriggerEnter(Collider other){
        if (other.tag == "Basket1"){
            //TODO: update score based on distance
            GameManager.S.UpdateScore(PlayerType.team1, 1);
        }
        else if (other.tag == "Basket2"){
            //TODO: update score based on distance
            GameManager.S.UpdateScore(PlayerType.team2, 1);
        }
    }
}
