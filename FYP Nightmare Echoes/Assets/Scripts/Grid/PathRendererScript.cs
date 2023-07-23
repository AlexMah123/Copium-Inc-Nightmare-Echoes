using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Grid
{
    public class PathRendererScript : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void OnEnable()
        {
            lineRenderer.enabled = true;
        }

        private void OnDisable()
        {
            lineRenderer.enabled = false;
        }

        public void RenderPath(List<Vector2Int> path)
        {
            lineRenderer.positionCount = path.Count;

            for (int i = 0; i < path.Count; i++)
            {
                lineRenderer.SetPosition(i, new Vector3(path[i].x, path[i].y, 0));
            }
        }
    }
}
