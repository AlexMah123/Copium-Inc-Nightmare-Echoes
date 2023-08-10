using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Inputs
{
    public class CameraControl : MonoBehaviour
    {
        public static CameraControl Instance;

        [Header("Camera Zoom")]
        [SerializeField] float defaultZoom;
        [SerializeField] float minZoom;
        [SerializeField] float maxZoom;
        [SerializeField] float zoomMulitplier = 1;

        [Header("Camera Boundaries")]
        public Vector3 offset = new Vector3(0, 0, -10f);
        public float smoothTime = 0.25f;
        public Vector3 velocity = Vector3.zero;
        [SerializeField] float boundaryX;
        [SerializeField] float boundaryY;

        public Vector3 targetPosition;
        private Vector3 dragOrigin;
        private Vector3 dragDelta;
        private bool isDragging;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Update()
        {
            IfCameraDrag();
            IfCameraZoom();
            IfCameraZoomReset();
        }

        public bool UpdateCameraPan(GameObject currentUnit)
        {
            if(currentUnit != null)
            {
                targetPosition = currentUnit.transform.position + Instance.offset;
                transform.position = Vector3.SmoothDamp(
                    transform.position, targetPosition, ref Instance.velocity, Instance.smoothTime);

                if (Vector3.Distance(Instance.gameObject.transform.position, Instance.targetPosition) < 0.05f)
                {
                    return false;
                }
            }

            return true;
        }

        void IfCameraDrag()
        {
            if (Input.GetMouseButton(1)) //if holding down left click
            {

                dragDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;

                if (!isDragging)
                {
                    isDragging = true;
                    dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Camera.main.transform.position = dragOrigin - dragDelta;
                Camera.main.transform.position = new Vector3(
                    Mathf.Clamp(Camera.main.transform.position.x, -boundaryX, boundaryX),
                    Mathf.Clamp(Camera.main.transform.position.y, -boundaryY, boundaryY),
                    Camera.main.transform.position.z);
            }
        }

        void IfCameraZoom()
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + Input.GetAxis("Mouse ScrollWheel") * zoomMulitplier, minZoom, maxZoom);
            }
        }

        void IfCameraZoomReset()
        {
            if (Input.GetMouseButtonDown(2))
            {
                Camera.main.orthographicSize = defaultZoom;
            }
        }
    }
}
