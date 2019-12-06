using MEC;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Custom.Indicators
{

    public class OffscreenIndicator : MonoBehaviour
    {
        public Camera activeCamera;
        public List<Indicator> targetIndicators;
        public GameObject indicatorPrefab;
        public float checkTime = 0.1f;
        public Vector2 offset;

        private Transform _transform;
        // Start is called before the first frame update
        void Start()
        {
            _transform = transform;
            InstantiateIndicators();
            Timing.RunCoroutine(UpdateIndicators().CancelWith(gameObject));
        }

        public void AddTarget(GameObject targetObject)
        {
            /*Indicator identificador = new Indicator();
            identificador.target = targetObject.transform;
            if (identificador.indicatorUI == null)
            {
                identificador.indicatorUI = Instantiate(indicatorPrefab).transform;
                identificador.indicatorUI.SetParent(_transform);
            }

            var rectTransform = identificador.indicatorUI.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = identificador.indicatorUI.gameObject.AddComponent<RectTransform>();
            }
            identificador.rectTransform = rectTransform;

            targetIndicators.Add(identificador);
            */

            targetIndicators.Add(new Indicator()
            {
                target = targetObject.transform
            });
            InstantiateIndicators();

        }

        private void InstantiateIndicators()
        {
            foreach (var targetIndicator in targetIndicators)
            {
                if(targetIndicator.indicatorUI == null)
                {
                    targetIndicator.indicatorUI = Instantiate(indicatorPrefab).transform;
                    targetIndicator.indicatorUI.SetParent(_transform);
                }

                var rectTransform = targetIndicator.indicatorUI.GetComponent<RectTransform>();
                if(rectTransform == null)
                {
                    rectTransform = targetIndicator.indicatorUI.gameObject.AddComponent<RectTransform>();
                }
                targetIndicator.rectTransform = rectTransform;
            }

            
        }
        private void UpdatePosition(Indicator targetIndicator)
        {
            GameObject MVP = GameObject.FindGameObjectWithTag("MVP"); 
            
            if(MVP==null)
            {
                return;
            }


            var rect = targetIndicator.rectTransform.rect;

            var indicatorPosition = activeCamera.WorldToScreenPoint(MVP.transform.position);

            if(indicatorPosition.z < 0)
            {
                indicatorPosition.y = -indicatorPosition.y;
                indicatorPosition.x = -indicatorPosition.x;
            }
            var newPosition = new Vector3(indicatorPosition.x, indicatorPosition.y, indicatorPosition.z);
            // asignar coordenadas para mostrar los objetos dentro de la pantalla
            indicatorPosition.x = Mathf.Clamp(indicatorPosition.x, rect.width / 2, Screen.width - rect.width) + offset.x;
            indicatorPosition.y = Mathf.Clamp(indicatorPosition.y, rect.height / 2, Screen.height - rect.height)+offset.y;
            indicatorPosition.z = 0;

            //update la posicion y rotacion
            targetIndicator.indicatorUI.up = (newPosition - indicatorPosition).normalized;
            targetIndicator.indicatorUI.position = indicatorPosition;
        }

        private IEnumerator<float> UpdateIndicators()
        {
            while (true)
            {
                foreach(var targetIndicator in targetIndicators)
                {
                    UpdatePosition(targetIndicator);
                }
                yield return Timing.WaitForSeconds(checkTime);

            }
        }
        // Update is called once per frame
       
    }
    [System.Serializable]
    public class Indicator
    {
        public Transform target;

        public Transform indicatorUI;

        public RectTransform rectTransform;

    }
}
