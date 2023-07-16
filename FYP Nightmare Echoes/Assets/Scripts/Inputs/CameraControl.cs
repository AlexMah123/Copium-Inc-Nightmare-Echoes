using Codice.CM.Common.Merge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//created by Alex
namespace NightmareEchoes.Inputs
{
    public class CameraControl : MonoBehaviour
    {
        

        [Header("Camera Zoom")]
        [SerializeField] float defaultZoom;
        [SerializeField] float minZoom;
        [SerializeField] float maxZoom;
        [SerializeField] float zoomMulitplier = 1;

        [Header("Camera Boundaries")]
        [SerializeField] float boundaryX;
        [SerializeField] float boundaryY;

        private Vector3 dragOrigin;
        private Vector3 dragDelta;
        private bool isDragging;

        private void Start()
        {

        }

        void LateUpdate()
        {
            if (Input.GetMouseButton(1)) //if holding down left click
            {

                dragDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;

                if(!isDragging)
                {
                    isDragging = true;
                    dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else
            {
                isDragging= false;
            }

            if (isDragging)
            {
                Camera.main.transform.position = dragOrigin - dragDelta;
                Camera.main.transform.position = new Vector3(
                    Mathf.Clamp(Camera.main.transform.position.x, -boundaryX, boundaryX),
                    Mathf.Clamp(Camera.main.transform.position.y, -boundaryY, boundaryY),
                    Camera.main.transform.position.z);
            }

            //Zoom Effect
            if(Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + Input.GetAxis("Mouse ScrollWheel") * zoomMulitplier, minZoom, maxZoom);
            }

            if(Input.GetMouseButtonDown(2))
            {
                Camera.main.orthographicSize = defaultZoom;
            }
        }
    }
}
