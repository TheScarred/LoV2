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
			shieldBar.UpdateBar( 0, jugadorsin.base_Shield );
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
			healthBar.UpdateBar( jugadorsin.m_HP, jugadorsin.base_HP );
			shieldBar.UpdateBar( jugadorsin.m_Shield, jugadorsin.base_Shield );

		}

		public void Death ()
		{
            //AQUI ENSEÑARIA LA ESCENA DE DEATH
            gameObject.transform.DetachChildren();
			Destroy( gameObject );
		}

		
	}
}