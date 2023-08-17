using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Inputs
{
    public class CameraControl : MonoBehaviour
    {
        public static CameraControl Instance;

        [Header("Cameras")]
        public Camera gameCamera;

        [Header("Camera Zoom")]
        [SerializeField] float defaultZoom;
        [SerializeField] float minZoom;
        [SerializeField] float maxZoom;
        [SerializeField] float zoomMulitplier = 1;

        [Header("Camera Panning")]
        public Vector3 offset = new Vector3(0, 0, -10f);
        public float smoothTime = 0.25f;
        public Vector3 velocity = Vector3.zero;
        Vector3 targetPosition;
        GameObject targetUnit;
        public bool isPanning;

        [Header("Camera Boundaries")]
        [SerializeField] float boundaryX;
        [SerializeField] float boundaryY;

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

        private void Start()
        {
            gameCamera = Camera.main;
        }

        private void Update()
        {
            IfCameraDrag();
            IfCameraZoom();
            IfCameraZoomReset();

            //pans to the targetUnit till it reaches it and stops
            if (isPanning && targetUnit != null)
            {
                targetPosition = targetUnit.transform.position + offset;
                gameCamera.transform.position = Vector3.SmoothDamp(
                    gameCamera.transform.position, targetPosition, ref velocity, smoothTime);

                if (Vector3.Distance(gameCamera.transform.position, targetPosition) < 0.05f)
                {
                    isPanning = false;
                }
            }
        }

        public void UpdateCameraPan(GameObject currentUnit)
        {
            if (currentUnit != null)
            {
                targetUnit = currentUnit;
                isPanning = true;
            }
        }

        void IfCameraDrag()
        {
            if (Input.GetMouseButton(1))
            {
                dragDelta = gameCamera.ScreenToWorldPoint(Input.mousePosition) - gameCamera.transform.position;

                if (!isDragging)
                {
                    isDragging = true;
                    dragOrigin = gameCamera.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else
            {
                isDragging = false;
            }

            if (isDragging)
            {
                gameCamera.transform.position = dragOrigin - dragDelta;
                gameCamera.transform.position = new Vector3(
                    Mathf.Clamp(gameCamera.transform.position.x, -boundaryX, boundaryX),
                    Mathf.Clamp(gameCamera.transform.position.y, -boundaryY, boundaryY),
                    gameCamera.transform.position.z);
            }
        }

        void IfCameraZoom()
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                gameCamera.orthographicSize = Mathf.Clamp(gameCamera.orthographicSize + Input.GetAxis("Mouse ScrollWheel") * zoomMulitplier, minZoom, maxZoom);
            }
        }

        void IfCameraZoomReset()
        {
            if (Input.GetMouseButtonDown(2))
            {
                gameCamera.orthographicSize = defaultZoom;
                gameCamera.transform.position = Vector3.zero + offset;
            }
        }
    }
}
