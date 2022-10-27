using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBuilding : MonoBehaviour
{
    public bool HasUI { get; protected set; }

    [SerializeField]
    protected float _releaseTime;

    private void Start()
    {
        Init();
    }

    public abstract void EnterTheBuilding(CitizenController citizen);

    protected abstract void Init();

    protected CitizenController DequeueCitizen(Queue<CitizenData> citizenOrderQueue)
    {
        CitizenData citizenData = citizenOrderQueue.Dequeue();

        GameObject go = null;
        if (citizenData.JobType == Define.Job.None)
        {
            go = Managers.Resource.Instantiate($"{Define.CITIZEN_PATH}{citizenData.CitizenType}Citizen");
        }
        else
        {
            go = Managers.Resource.Instantiate(
                $"{Define.CITIZEN_PATH}" +
                $"{citizenData.CitizenType}Citizen_{citizenData.JobType}");
        }

        var citizen = go.GetComponent<CitizenController>();
        citizen.Data = citizenData;

        return citizen;
    }

    protected void SetUnitPosition(GameObject go, Define.Move moveType)
    {
        var pos = TileManager.GetInstance.GetWorldToCellCenterToWorld(Define.Tilemap.Ground, transform.position);

        switch (moveType)
        {
            case Define.Move.Right:
                pos += new Vector3(0.51f, 0, 0);
                break;
            case Define.Move.Left:
                pos += new Vector3(-0.51f, 0, 0);
                break;
            case Define.Move.Up:
                pos += new Vector3(0, 0.51f, 0);
                break;
            case Define.Move.Down:
                pos += new Vector3(0, -0.51f, 0);
                break;
        }

        go.transform.position = pos;
    }

    protected bool HasRoadNextPosition(Define.Move moveType)
    {
        var nextPos = TileManager.GetInstance.GetWorldToCell(Define.Tilemap.Ground, transform.position);

        switch (moveType)
        {
            case Define.Move.None:
                return false;
            case Define.Move.Right:
                nextPos += Vector3Int.right;
                break;
            case Define.Move.Left:
                nextPos += Vector3Int.left;
                break;
            case Define.Move.Up:
                nextPos += Vector3Int.up;
                break;
            case Define.Move.Down:
                nextPos += Vector3Int.down;
                break;
        }

        return Util.GetRoad(Define.Tilemap.Road, nextPos) ? true : false;
    }
}
