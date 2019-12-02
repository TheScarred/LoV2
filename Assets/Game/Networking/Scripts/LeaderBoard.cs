using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.ObjectModel;

public class LeaderBoard : MonoBehaviour
{
    public Image[] lugares;
    public Text[] nombre;
    public Text[] score;

    void Score(List<LB> posiciones)
    {
        for(int i = 0; i < lugares.Length; i++)
        {
            if (posiciones.Count < lugares.Length)
            {
                nombre[i].text = posiciones[i].name;
                score[i].text = posiciones[i].score.ToString();
            }
        }
    }
    struct LB
    {
        public string name;
        public int score;
    }
}
