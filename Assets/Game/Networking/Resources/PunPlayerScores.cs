using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PunPlayerScores : MonoBehaviour
{
    public const string PlayerScoreProp = "score";
}

public static class ScoreExtensions
{
    public static void SetScore(this PhotonPlayer player, int newScore)
    {
        Hashtable score = new Hashtable();  // using PUN's implementation of Hashtable
        score[PunPlayerScores.PlayerScoreProp] = newScore;

        player.SetCustomProperties(score); // this locally sets the score and will sync it in-game asap.
    }

    public static void OrdenarScore(this PhotonPlayer player, PhotonPlayer[] p, ref string mvp, ref int yo)
    {
        int current = player.GetScore();
        int i, mejorPuntaje = 0, mejorPuntajeIndex = 0, currentindex = 1;
        
        PhotonPlayer aux;
        for (i = 0; i < p.Length; i++)
        {
            if (mejorPuntaje < p[i].GetScore())
            {
                mejorPuntaje = p[i].GetScore();
                mejorPuntajeIndex = i;

            }
            if (current < p[i].GetScore())
            {
                
                currentindex += 1;
            }

        }
        

        /*for (int k = 0; k< p.Length; k++)
        {
            for(int j =0; j < p.Length-1; j++)
            {
                if(p[j].GetScore() > p[j+1].GetScore())
                {
                    aux = p[j + 1];
                    p[j + 1] = p[j];
                    p[j] = aux ;
                }
                else if(current < p[j+1].GetScore())
                {
                    //currentindex = j;
                }
            }
        }*/

        //currentindex += 1;
        yo = currentindex;
        //yop = p[yo].NickName;
        mvp = p[mejorPuntajeIndex].NickName;
    }
   

    public static void AddScore(this PhotonPlayer player, int scoreToAddToCurrent)
    {
        int current = player.GetScore();
        current = current + scoreToAddToCurrent;

        Hashtable score = new Hashtable();  // using PUN's implementation of Hashtable
        score[PunPlayerScores.PlayerScoreProp] = current;

        player.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
    }

    public static int GetScore(this PhotonPlayer player)
    {
        object score;
        if (player.CustomProperties.TryGetValue(PunPlayerScores.PlayerScoreProp, out score))
        {
            return (int) score;
        }

        return 0;
    }
}