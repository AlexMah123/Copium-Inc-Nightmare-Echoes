using Codice.CM.Common.Merge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Inputs
{
    public class CameraControl : MonoBehaviour
    {
        private Vector3 dragOrigin;
        private Vector3 dragDelta;
        private bool isDragging;
        [SerializeField] float minZoom;
        [SerializeField] float maxZoom;

        [SerializeField] float timer;
        float tempTimer;

        private void Start()
        {
            tempTimer = timer;
        }

        void LateUpdate()
        {
            if (Input.GetMouseButton(1)) //if holding down left click
            {
                tempTimer -= Time.deltaTime;

                dragDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;

                if(!isDragging && tempTimer < 0f)
                {
                    isDragging = true;
                    dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else
            {
                isDragging= false;
                tempTimer = timer;
            }

            if (isDragging)
            {
                Camera.main.transform.position = dragOrigin - dragDelta;
            }

            //Zoom Effect
            if(Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + Input.GetAxis("Mouse ScrollWheel") * 2, minZoom, maxZoom);
            }

        }
    }
}
