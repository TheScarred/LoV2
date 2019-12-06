using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using System.Text;
using SimpleHealthBar_SpaceshipExample;
public class PlayerStats : PunBehaviour
{
    public float base_HP = 50;
    public float base_Shield = 100;
    public float base_DamageMeele = 20;
    public float base_DamageRange = 20;
    public float base_MeleeSpeed = 2f;
    public float base_speed = 2f;
    public float base_ShootingSpeed = 2f;
    public int base_AmmoCap = 30;

    //Modifiers
    public float m_Speed;
    public float m_Shield;
    public float m_HP;
    public float m_DamageRange;
    public float m_DamageMelee;
    public int m_Ammo;

    //MVP
    public GameObject MVP;
    string mvp;

    //SCOREBOARD
    public GameObject scoreboard;
    int playerCount;
    int Score;

    //Show Score in UI
    public Text UI_Score;

    //Show Health Bar to Other Players
    public Image HP_bar;
    public Image Armor_bar;

    //Particles
    public Player player;

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
        mvp = "";
        m_Speed = base_speed;
        m_HP = base_HP;
        m_Shield = 0;
        m_DamageMelee = base_DamageMeele;
        m_DamageRange = base_DamageRange;
        m_Ammo = base_AmmoCap / 3;
    }

    public void ReceiveDamage(float armourPen, float damage)
    {
        ParticleManager.GetInstance().ActivateParticle(this.transform, player.particleHit);
        player_health.TakeDamage(armourPen, damage);
        float fillmount;
        fillmount = HP_bar.fillAmount = (m_HP / base_HP);
    }

    public void KilledTarget(int points)
    {
        Score += points;
        UI_Score.text = "Score: " + Score;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(m_HP);
            stream.SendNext(Score);
            stream.SendNext(m_Shield);
            //stream.SendNext(HP_bar);
            //stream.SendNext(Armor_bar);
        }
        else
        {
            m_HP = (float)stream.ReceiveNext();
            Score = (int)stream.ReceiveNext();
            m_Shield = (float)stream.ReceiveNext();

        }
    }
    public void UpdateScoreboard() // ESTA FUNCION SE LLAMA EN EL PLAYER.CS en el update cuando se activa el scoreboard
    {/*
        // checar el contador de jugadores que hay
        playerCount = PhotonNetwork.playerList.Length;
        // obtener nombres de los jugadores
        var playerList = new StringBuilder();
        //mostrando la lista con sus respectivos scores

        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {

            p.OrdenarScore(PhotonNetwork.playerList, ref mvp);

            playerList.Append("Nick Jugador: " + p.NickName + " Score: " + p.GetScore() + "\n");
            if (mvp == p.NickName)
            {
              //  MVP.SetActive(true);
            }
            else
            {
              //  MVP.SetActive(false);
            }
            /*else if (mvp != p.NickName)//&& photonView.isMine )
                Debug.Log(this.photonView.name);
                this.MVP.SetActive(false);*/
    }

        //string output = "Numero de jugadores: " + playerCount.ToString() + "\n" + playerList.ToString();
       // if (photonView.isMine)

        //scoreboard.transform.Find("Text").GetComponent<Text>().text = output;
}
