﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreComponent : MonoBehaviour
{
    public enum PlayerType {small, tall};
    [SerializeField] PlayerType current_player;
    void update_score_local(int i){
        GameManager.update_score(current_player, i);
    }

    void OnTriggerEnter(Collider other){
        if (other.tag == "Basket"){
            //TODO: update score based on distance
            update_score_local(1);
        }
    }
}
