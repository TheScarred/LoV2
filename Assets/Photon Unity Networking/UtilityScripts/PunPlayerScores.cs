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

        player.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
    }

    public static void OrdenarScore(this PhotonPlayer player,PhotonPlayer[] p, ref string mvp)
    {
        int current = player.GetScore();
        int i, j;
        if(mvp=="")
        {
            mvp = player.NickName;
        }
        for (i = 0; i < p.Length; i++)
        {
            for (j = i + 1; j < p.Length; j++)
            {
                if (p[j].GetScore() > p[i].GetScore())
                {
                    mvp = p[j].NickName;
                    var aux = p[i];
                    p[i] = p[j];
                    p[j] = aux;
                    
                }
            }
            
        }
        
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