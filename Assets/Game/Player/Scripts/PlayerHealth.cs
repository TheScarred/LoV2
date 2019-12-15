using UnityEngine;
using System.Collections;
using Photon;
using UnityEngine.SceneManagement;

namespace SimpleHealthBar_SpaceshipExample
{
	public class PlayerHealth : PunBehaviour
	{
		static PlayerHealth instance;
		public static PlayerHealth Instance { get { return instance; } }
        
        //VIDA VARIABLES
        public PlayerStats jugadorsin;

        public Player player;

        public Savenick nickname;

        public SimpleHealthBar healthBar;
        public SimpleHealthBar shieldBar;

        PhotonPlayer holisoyo;

    
        [SerializeField]
        GameObject AudioListenerObject;


        void Awake ()
		{
            // If the instance variable is already assigned, then there are multiple player health scripts in the scene. Inform the user.
            if (instance != null)
                //Debug.LogError( "Agregale primero la vida baboso en los prefabs" );
               
           
            instance = GetComponent<PlayerHealth>();
        }

		void Start ()
		{

            if(photonView.isMine)
            {
                nickname = GameObject.Find("JEJE").GetComponent<Savenick>();
                player = this.gameObject.GetComponent<Player>();
                jugadorsin = this.gameObject.GetComponent<PlayerStats>();
                healthBar = GameObject.Find("Health").GetComponent<SimpleHealthBar>();

                shieldBar = GameObject.Find("Shield").GetComponent<SimpleHealthBar>();
                jugadorsin.m_HP = jugadorsin.base_HP;
                jugadorsin.m_Shield = jugadorsin.base_Shield;

                // SE van actualizando la vida y los escudos

                healthBar.UpdateBar(jugadorsin.m_HP, jugadorsin.base_HP);
                shieldBar.UpdateBar(0, jugadorsin.base_Shield);
                

            }
            
        }

        //FUNCION PARA HACER DAÑO 
		public void TakeDamage (float armourPen, float damage )
		{
            float hpDamage = damage * armourPen;
            float armourDamage = damage - hpDamage;

			// SI EL ESCUDO ES MAYOR A 0 
			if( jugadorsin.m_Shield > 0 )
			{

				// REDUCE EL ESCUDO DEL DAÑO HECHO
				jugadorsin.m_Shield -= armourDamage;

		
                //SI EL ESCUDO ES MENOR A 0 
				if( jugadorsin.m_Shield < 0 )
				{
					
                    //REDUCE LA VIDA POR CUANTO DAÑO HAYA PASADO DEL ESCUDO 
					jugadorsin.m_HP -= jugadorsin.m_Shield * -1;

					// ESCUDO A 0
					jugadorsin.m_Shield = 0;
				}

                jugadorsin.m_HP -= hpDamage;
			}
			// SI NO HAY ESCUDO ENTONCES HAZLE DAÑO
			else
				jugadorsin.m_HP -= damage;

			// SI LA VIDA ES MENOR A 0 O IGUAL
			if( jugadorsin.m_HP <= 0 )
			{
				// SIMPLEMENTE PARA ESTETICA Y QUE NO QUEDE EL -1 O NUMEROS NEGATIVOS
				jugadorsin.m_HP = 0;

				// CORRE LA FUNCION DE MORIR
				Death();
			}
			
			// UPDATE DE EL ESCUDO Y LA VIDA BARRAS
            if(photonView.isMine)
            {
                healthBar.UpdateBar(jugadorsin.m_HP, jugadorsin.base_HP);
                shieldBar.UpdateBar(jugadorsin.m_Shield, jugadorsin.base_Shield);
                
            }
			

		}

		public void Death ()
		{
            if (photonView.isMine)
            {
                gameObject.transform.DetachChildren();
                player.gameObject.SetActive(false);
                PhotonNetwork.Disconnect();
                SceneManager.LoadScene("Game Over");
            }
            //AQUI ENSEÑARIA LA ESCENA DE DEATH
            
           
        }

        public void ActivarAudioListener()
        {
            //Photon Para encontrarme 
            
            int MyID = -1;
            foreach (PhotonPlayer p in PhotonNetwork.playerList)
            {
                
                if (nickname== null)
                {

                   
                }
                else if (nickname.NickName == p.NickName&&photonView.isMine)
                {
                    MyID = p.ID;
                }
                
                else if (nickname.NickName != p.NickName) 
                AudioListenerObject.SetActive(false);

                
            }
            if (MyID != -1)//Se encontro mi ID
            {
                MyID = 1000 * MyID + 1;

                Player[] jugadores = GameObject.FindObjectsOfType<Player>();

                GameObject AudioListener1 = null;
                for (int i = 0; i < jugadores.Length; i++)
                {


                    if (jugadores[i].ID == MyID) //Este jugador tiene el ID del MVP
                    {

                        jugadores[i].GetComponent<PlayerHealth>().AudioListenerObject.SetActive(true);
                        AudioListener1 = jugadores[i].gameObject;

                    }

                    else
                    {
                        jugadores[i].GetComponent<PlayerHealth>().AudioListenerObject.SetActive(false);
                    }

                }
            }




        }


    }
}