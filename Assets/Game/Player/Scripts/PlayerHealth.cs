/* Written by Kaz Crowe */
/* PlayerHealth.cs */
using UnityEngine;
using System.Collections;

namespace SimpleHealthBar_SpaceshipExample
{
	public class PlayerHealth : MonoBehaviour
	{
		static PlayerHealth instance;
		public static PlayerHealth Instance { get { return instance; } }
        
        //VIDA VARIABLES
        public PlayerStats jugadorsin;
		

        //ESCUDO VARIABLES 
		
		float regenShieldTimer = 0.0f;
		public float regenShieldTimerMax = 1.0f;

        Player player;



        public SimpleHealthBar healthBar;
        public SimpleHealthBar shieldBar;

            
        void Awake ()
		{
            // If the instance variable is already assigned, then there are multiple player health scripts in the scene. Inform the user.
            if (instance != null)
                //Debug.LogError( "Agregale primero la vida baboso en los prefabs" );
               
            /*healthBar = GetComponent<SimpleHealthBar>();
            shieldBar = GetComponent<SimpleHealthBar>();*/
            instance = GetComponent<PlayerHealth>();
        }
			
		

		void Start ()
		{
			// PARA ESTABLECER LA VIDA MAXIMA Y EL ESCUDO
			jugadorsin.m_HP = jugadorsin.base_HP;
			jugadorsin.m_Shield = jugadorsin.base_Shield;

			// SE van actualizando la vida y los escudos
		    healthBar.UpdateBar( jugadorsin.m_HP, jugadorsin.base_HP );
			shieldBar.UpdateBar( jugadorsin.m_Shield, jugadorsin.base_Shield );
		}

		void Update ()
		{
			
            //si el escudo es menor al maximo, y el cooldown del regenShield no esta aplicandose entonces : 
			if( jugadorsin.m_Shield < jugadorsin.base_Shield && regenShieldTimer <= 0 )
			{
				//Incrementa el escudo
				jugadorsin.m_Shield += Time.deltaTime * 5;

				// ACTUALIZA LOS VALORES DEL ESCUDO
				shieldBar.UpdateBar( jugadorsin.m_Shield, jugadorsin.base_Shield );
			}
            //AQUI TOMA EL DAÑO QUE SE REFLEJARA EN LA BARRA DE VIDA Y ESCUDO 
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                TakeDamage(10);
            }

            
			
            //SI EL REGENERAR ESCUDO ES MAYOR DE 0 ENTONCES DECREMENTA EL TIEMPO
			if( regenShieldTimer > 0 )
				regenShieldTimer -= Time.deltaTime;
		}
        
		public void HealPlayer ()
		{
			// incrementar la vida por un 25%
			jugadorsin.m_HP += ( jugadorsin.base_HP / 4 );

			
            //SI LA VIDA ACTUAL ES MAYOR QUE LA MAX, ENTONCES SE ACTUALIZA A QUE ESA SERA LA MAX, EN CASO DE CURARSE CLARO
			if( jugadorsin.m_HP > jugadorsin.base_HP )
				jugadorsin.m_HP = jugadorsin.base_HP;

			//SE ACTUALIZA LA BARRA
			healthBar.UpdateBar( jugadorsin.m_HP, jugadorsin.base_HP );
		}
        //FUNCION PARA HACER DAÑO 
		public void TakeDamage ( int damage )
		{
			
			// SI EL ESCUDO ES MAYOR A 0 
			if( jugadorsin.m_Shield > 0 )
			{

				// REDUCE EL ESCUDO DEL DAÑO HECHO
				jugadorsin.m_Shield -= damage;

		
                //SI EL ESCUDO ES MENOR A 0 
				if( jugadorsin.m_Shield < 0 )
				{
					
                    //REDUCE LA VIDA POR CUANTO DAÑO HAYA PASADO DEL ESCUDO 
					jugadorsin.m_HP -= jugadorsin.m_Shield * -1;

					// ESCUDO A 0
					jugadorsin.m_Shield = 0;
				}
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
			healthBar.UpdateBar( jugadorsin.m_HP, jugadorsin.base_HP );
			shieldBar.UpdateBar( jugadorsin.m_Shield, jugadorsin.base_Shield );

			//RESETEAMOS EL REGENSHIELD
			regenShieldTimer = regenShieldTimerMax;
		}

		public void Death ()
		{
			//AQUI ENSEÑARIA LA ESCENA DE DEATH
			
			Destroy( gameObject );
		}

		
	}
}