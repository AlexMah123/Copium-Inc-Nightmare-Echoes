using NightmareEchoes.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class TestPathFindingMovement : MonoBehaviour
{

    private Vector3 target;
    NavMeshAgent agent;
    

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        SetTargetPos();
        SetAgentPos();
    }

    private void SetTargetPos()
    {
        if (Input.GetMouseButtonDown(0))
        {
            target = TileMapManager.Instance.getSpawnPos();
            //target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void SetAgentPos()
    {
        agent.SetDestination(new Vector3(target.x,target.y,transform.position.z));
    }
}
