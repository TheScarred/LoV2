using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using System.Text;
using SimpleHealthBar_SpaceshipExample;
public class PlayerStats : PunBehaviour
{
    public float base_HP;
    public float base_Shield = 50;
    public float base_DamageMeele = 20;   
    public float base_DamageRange = 20;
    public float base_MeleeSpeed = 2f;
    public float base_speed = 2f;    
    public float base_ShootingSpeed = 2f;

    //Modifiers
    public float m_Speed;
    public float m_Shield;
    public float m_HP;
    public float m_ShootingSpeed;
    public float m_MeeleSpeed;
    public float m_DamageRange;
    public float m_DamageMelee;


    //SCOREBOARD
    GameObject scoreboard;
    int playerCount;
    public int Score;

    //Show Score in UI
    public Text UI_Score;

    //Show Health Bar to Other Players
    public Image HP_bar;
    public Image Armor_bar;

    //Connect to PlayerHealth Script
    public PlayerHealth player_health;

    void Start()
    {
        ResetStats();

        if (photonView.isMine)
        {
            HP_bar.gameObject.SetActive(false);
            Armor_bar.gameObject.SetActive(false);
            scoreboard = GameObject.Find("Canvas").transform.Find("Scoreboard").gameObject;

            UI_Score = GameObject.Find("Canvas").transform.Find("Score").GetComponent<Text>();
        }
    }

    public void ResetStats()
    {

        
       

        m_Speed = base_speed;
        m_HP = base_HP;
        m_Shield = base_Shield;
        m_DamageMelee = base_DamageMeele;
        m_DamageRange = base_DamageRange;
        m_MeeleSpeed = base_MeleeSpeed;
        m_ShootingSpeed = base_ShootingSpeed;
    }

    public void ReceiveDamage(int damage)
    {

        player_health.TakeDamage(damage);
        float fillmount;
        fillmount = HP_bar.fillAmount = (m_HP / base_HP);

    }

    public void KilledTarget(int points)
    {
        Score += points;
        Debug.Log("Killed Target! Points: " + Score);
       
       
        UI_Score.text = "Score: " + Score;

        
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(m_HP);
            //stream.SendNext(Score);
            //stream.SendNext(HP_bar);
            //stream.SendNext(Armor_bar);
        }
        else
        {
            m_HP = (float)stream.ReceiveNext();
            //Score = (int)stream.ReceiveNext();



        }
    }
    public void UpdateScoreboard() // ESTA FUNCION SE LLAMA EN EL PLAYER.CS en el update cuando se activa el scoreboard

    {
        

        // checar el contador de jugadores que hay
        playerCount = PhotonNetwork.playerList.Length;
        // obtener nombres de los jugadores
        var playerList = new StringBuilder();
        //mostrando la lista con sus respectivos scores
        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            
            playerList.Append("Nick Jugador: " + p.NickName + " Score: " + p.GetScore() + "\n");

        }
        string output = "Numero de jugadores: " + playerCount.ToString() + "\n" + playerList.ToString();
       if(photonView.isMine)
        scoreboard.transform.Find("Text").GetComponent<Text>().text = output;

        
    }

}
