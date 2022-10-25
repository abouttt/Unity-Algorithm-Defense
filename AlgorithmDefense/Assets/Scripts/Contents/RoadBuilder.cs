using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class RoadBuilder : MonoBehaviour
{
    private static RoadBuilder s_instance;
    public static RoadBuilder GetInstance { get { Init(); return s_instance; } }

    [HideInInspector]
    public bool IsBuilding;
    public Dictionary<int, List<Vector3Int>> RoadGroupDic = new();

    private Tile _roadTile;
    private Vector3Int _prevPos;
    private int _groupCount = 1;

    private Vector3Int _firstPos;
    private Vector3Int _lastPos;
    private Vector3Int? _startRoadPos;


    private void Start()
    {
        _roadTile = Managers.Resource.Load<TileBase>($"{Define.ROAD_TILE_PATH}Road_B") as Tile;
    }

    private void Update()
    {
        if (IsBuilding)
        {
            // 설치할 위치 지정.
            if (Input.GetMouseButton(0))
            {
                BuildWillRoads(MouseController.GetInstance.MouseCellPos);
            }

            // 설치.
            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0))
            {
                BuildRoads();
                IsBuilding = false;
            }
        }
    }

    public void RemoveRoads(int groupNumber)
    {
        if (!RoadGroupDic.ContainsKey(groupNumber))
        {
            Debug.Log($"RoadBuilder - RemoveRoads() : No contains group number({groupNumber}).");
            return;
        }

        // 시민이 있는지 검사
        foreach (var pos in RoadGroupDic[groupNumber])
        {
            var road = Util.GetRoad(Define.Tilemap.Road, pos);
            if (road && road.IsOnCitizen)
            {
                return;
            }
        }

        foreach (var pos in RoadGroupDic[groupNumber])
        {
            TileManager.GetInstance.SetTile(Define.Tilemap.Road, pos, null);

            if (_startRoadPos.HasValue && _startRoadPos.Value == pos)
            {
                TileManager.GetInstance.SetTile(Define.Tilemap.Road, _startRoadPos.Value, Define.Road.BD);
                Util.GetRoad(Define.Tilemap.Road, _startRoadPos.Value).IsStartRoad = true;
                _startRoadPos = null;
            }
        }

        RoadGroupDic.Remove(groupNumber);
    }

    private void BuildWillRoads(Vector3Int pos)
    {
        // 같은 위치라면 진행하지 않는다.
        if (_prevPos == pos)
        {
            return;
        }

        _prevPos = pos;

        // 범위 체크.
        if (!IsInOfRange(pos))
        {
            return;
        }

        // 성벽이 있다면 진행하지 않는다.
        if (TileManager.GetInstance.GetTile(Define.Tilemap.Rampart, pos))
        {
            return;
        }

        // 그룹이 없다면 만든다.
        if (!RoadGroupDic.ContainsKey(_groupCount))
        {
            RoadGroupDic.Add(_groupCount, new());
        }

        // 이전에 예약한 위치라면 취소한다.
        if (TileManager.GetInstance.GetTile(Define.Tilemap.WillRoad, pos))
        {
            RevertWillRoad(pos);
            return;
        }

        // 시작길이 아니며 길이 있다면 진행하지 않는다.
        if (TileManager.GetInstance.GetTile(Define.Tilemap.Road, pos))
        {
            if (!_startRoadPos.HasValue && IsStartRoad(pos))
            {
                _startRoadPos = pos;
            }
            else
            {
                return;
            }
        }

        // 이전에 예약한 위치 옆이 아닐 경우 진행하지 않는다.
        if (!IsNextToPrevPos(pos))
        {
            return;
        }

        // 이전에 예약한 위치가 시작길이라면 진행하지 않는다.
        if (IsStartRoadPrevPos())
        {
            return;
        }

        // 마지막 위치에 건물이 있다면 진행하지 않는다.
        if (TileManager.GetInstance.GetTilemap(Define.Tilemap.Building).GetInstantiatedObject(_lastPos))
        {
            return;
        }

        RoadGroupDic[_groupCount].Add(pos);
        TileManager.GetInstance.SetTile(Define.Tilemap.WillRoad, pos, Define.Road.B);

        var willRoad = Util.GetRoad(Define.Tilemap.WillRoad, pos);
        willRoad.GroupNumber = _groupCount;
        willRoad.Index = GetGroupLastIndex();
        if (_startRoadPos.HasValue && (_startRoadPos.Value == pos))
        {
            willRoad.IsStartRoad = true;
        }
        willRoad.Refresh(pos);

        // 예약한 위치가 마지막 건물 또는 시작길인지 판별하기 위한 저장.
        if (RoadGroupDic[_groupCount].Count == 1)
        {
            _firstPos = pos;
        }
    }

    private void BuildRoads()
    {
        // 아무것도 예약되어 있지 않다면 진행하지 않는다.
        if (RoadGroupDic[_groupCount].Count == 0)
        {
            return;
        }

        _lastPos = GetGroupLastPos();

        if (IsConnectedBuilding())
        {
            Road willRoad = null;
            Road road = null;

            foreach (var pos in RoadGroupDic[_groupCount])
            {
                willRoad = Util.GetRoad(Define.Tilemap.WillRoad, pos);
                Define.Road roadType = willRoad.RoadType;
                int willRoadGroupNumber = willRoad.GroupNumber;
                int willRoadIndex = willRoad.Index;
                bool isStartRoad = willRoad.IsStartRoad;

                TileManager.GetInstance.SetTile(Define.Tilemap.WillRoad, pos, null);
                TileManager.GetInstance.SetTile(Define.Tilemap.Road, pos, roadType);

                road = Util.GetRoad(Define.Tilemap.Road, pos);
                road.GroupNumber = willRoadGroupNumber;
                road.Index = willRoadIndex;
                road.IsStartRoad = isStartRoad;
            }

            RemoveFirstAndLastRoad();

            _groupCount++;
        }
        else
        {
            foreach (var pos in RoadGroupDic[_groupCount])
            {
                TileManager.GetInstance.SetTile(Define.Tilemap.WillRoad, pos, null);
                if (_startRoadPos.HasValue && (_startRoadPos == pos))
                {
                    _startRoadPos = null;
                }
            }

            RoadGroupDic.Remove(_groupCount);
        }

        _firstPos = Vector3Int.zero;
        _lastPos = Vector3Int.zero;
    }

    private void RevertWillRoad(Vector3Int pos)
    {
        var nextPos = GetGroupLastPos();
        RoadGroupDic[_groupCount].RemoveAt(GetGroupLastIndex());

        if ((RoadGroupDic[_groupCount].Count >= 1) && (pos == GetGroupLastPos()))
        {
            TileManager.GetInstance.SetTile(Define.Tilemap.WillRoad, nextPos, null);
            Util.GetRoad(Define.Tilemap.WillRoad, pos).Refresh(pos);

            if (_startRoadPos.HasValue && (_startRoadPos.Value == nextPos))
            {
                _startRoadPos = null;
            }

            _lastPos = GetGroupLastPos();
        }
        else
        {
            RoadGroupDic[_groupCount].Add(nextPos);
        }
    }

    private bool IsNextToPrevPos(Vector3Int pos)
    {
        if (RoadGroupDic[_groupCount].Count == 0)
        {
            return true;
        }

        var prevPos = GetGroupLastPos();
        for (int i = 0; i < 4; i++)
        {
            int nx = prevPos.x + Define.DX[i];
            int ny = prevPos.y + Define.DY[i];

            if (pos.x == nx && pos.y == ny)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsStartRoadPrevPos()
    {
        if (RoadGroupDic[_groupCount].Count == 0)
        {
            return false;
        }

        // 첫번째 예약위치가 시작길이라면 예외.
        if (_startRoadPos.HasValue && (_startRoadPos.Value == _firstPos))
        {
            return false;
        }

        return _startRoadPos.HasValue && (_startRoadPos.Value == GetGroupLastPos());
    }

    private bool IsInOfRange(Vector3Int pos)
    {
        if (pos.x <= Managers.Game.Setting.StartPosition.x ||
            pos.y <= Managers.Game.Setting.StartPosition.y ||
            pos.x > Managers.Game.Setting.StartPosition.x + Managers.Game.Setting.RampartWidth - 1 ||
            pos.y > Managers.Game.Setting.StartPosition.y + Managers.Game.Setting.RampartHeight - 1)
        {
            return false;
        }

        return true;
    }

    private bool IsConnectedBuilding()
    {
        var firstBuilding = TileManager.GetInstance.GetTilemap(Define.Tilemap.Building).GetInstantiatedObject(_firstPos);
        var secondBuilding = TileManager.GetInstance.GetTilemap(Define.Tilemap.Building).GetInstantiatedObject(_lastPos);

        if (firstBuilding && secondBuilding)
        {
            return true;
        }

        if (firstBuilding && _startRoadPos.HasValue && (_startRoadPos.Value == _lastPos))
        {
            return true;
        }

        if (secondBuilding && _startRoadPos.HasValue && (_startRoadPos.Value == _firstPos))
        {
            return true;
        }

        return false;
    }

    private void RemoveFirstAndLastRoad()
    {
        var firstBuilding = TileManager.GetInstance.GetTile(Define.Tilemap.Building, _firstPos);
        var secondBuilding = TileManager.GetInstance.GetTile(Define.Tilemap.Building, _lastPos);

        if (firstBuilding && _startRoadPos != _firstPos)
        {
            TileManager.GetInstance.SetTile(Define.Tilemap.Road, _firstPos, null);
        }

        if (secondBuilding && _startRoadPos != _lastPos)
        {
            TileManager.GetInstance.SetTile(Define.Tilemap.Road, _lastPos, null);
        }
    }

    private bool IsStartRoad(Vector3Int pos)
    {
        var road = Util.GetRoad(Define.Tilemap.Road, pos);
        if (road)
        {
            return road.IsStartRoad;
        }

        return false;
    }

    private Vector3Int GetGroupLastPos() => RoadGroupDic[_groupCount][RoadGroupDic[_groupCount].Count - 1];

    private int GetGroupLastIndex() => RoadGroupDic[_groupCount].Count - 1;

    private static void Init()
    {
        if (s_instance == null)
        {
            var go = GameObject.Find("@RoadBuilder");
            if (!go)
            {
                go = Util.CreateGameObject<RoadBuilder>("@RoadBuilder");
            }

            s_instance = go.GetComponent<RoadBuilder>();
        }
    }
}
