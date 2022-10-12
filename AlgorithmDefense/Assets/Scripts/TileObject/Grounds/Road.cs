using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Road : MonoBehaviour
{
    [field: SerializeField]
    public Define.Road RoadType { get; private set; }
    public bool IsStartRoad = false;
    public int GroupNumber = 0;
    public int Index = 0;

    public void Refresh(Vector3Int pos)
    {
        Rule(pos);

        if (Index > 0)
        {
            var backCellPos = RoadBuilder.GetInstance.RoadGroupDic[GroupNumber][Index - 1];
            var go = Managers.Tile.GetTilemap(Define.Tilemap.WillRoad).GetInstantiatedObject(backCellPos);
            if (go)
            {
                go.GetComponent<Road>().Rule(backCellPos);
            }
        }

        if (Index < RoadBuilder.GetInstance.RoadGroupDic[GroupNumber].Count - 1)
        {
            var frontCellPos = RoadBuilder.GetInstance.RoadGroupDic[GroupNumber][Index + 1];
            var go = Managers.Tile.GetTilemap(Define.Tilemap.WillRoad).GetInstantiatedObject(frontCellPos);
            if (go)
            {
                go.GetComponent<Road>().Rule(frontCellPos);
            }
        }
    }

    private void Rule(Vector3Int pos)
    {
        TileBase nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_B");

        if (IsStartRoad)
        {
            Vector3Int gapPos = Vector3Int.zero;
            if (Index > 0)
            {
                var backCellPos = RoadBuilder.GetInstance.RoadGroupDic[GroupNumber][Index - 1];
                gapPos = pos - backCellPos;
            }

            if (Index < RoadBuilder.GetInstance.RoadGroupDic[GroupNumber].Count - 1)
            {
                var frontCellPos = RoadBuilder.GetInstance.RoadGroupDic[GroupNumber][Index + 1];
                gapPos = pos - frontCellPos;
            }

            if (gapPos == Vector3.down)
            {
                nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_UD");
            }
            else if (gapPos == Vector3.right)
            {
                nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_CUL");
            }
            else if (gapPos == Vector3.left)
            {
                nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_CUR");
            }
        }
        else
        {
            bool hasBackTile = false;
            bool hasFrontTile = false;

            Vector3Int backGapCellPos = Vector3Int.zero;
            Vector3Int frontGapCellPos = Vector3Int.zero;

            if (Index > 0)
            {
                var backCellPos = RoadBuilder.GetInstance.RoadGroupDic[GroupNumber][Index - 1];
                backGapCellPos = pos - backCellPos;
                hasBackTile = true;
            }

            if (Index < RoadBuilder.GetInstance.RoadGroupDic[GroupNumber].Count - 1)
            {
                var frontCellPos = RoadBuilder.GetInstance.RoadGroupDic[GroupNumber][Index + 1];
                frontGapCellPos = pos - frontCellPos;
                hasFrontTile = true;
            }

            if (hasBackTile && !hasFrontTile)
            {
                if (backGapCellPos == Vector3Int.up)
                {
                    nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_BD");
                }
                else if (backGapCellPos == Vector3Int.down)
                {
                    nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_BU");
                }
                else if (backGapCellPos == Vector3Int.left)
                {
                    nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_BR");
                }
                else if (backGapCellPos == Vector3Int.right)
                {
                    nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_BL");
                }
            }

            if (!hasBackTile && hasFrontTile)
            {
                if (frontGapCellPos == Vector3Int.up)
                {
                    nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_BD");
                }
                else if (frontGapCellPos == Vector3Int.down)
                {
                    nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_BU");
                }
                else if (frontGapCellPos == Vector3Int.left)
                {
                    nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_BR");
                }
                else if (frontGapCellPos == Vector3Int.right)
                {
                    nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_BL");
                }
            }

            if (hasBackTile && hasFrontTile)
            {
                var absBackGapCellPos = Util.GetAbsVector3Int(backGapCellPos);
                var absFrontGapCellPos = Util.GetAbsVector3Int(frontGapCellPos);

                if (absBackGapCellPos == Vector3Int.up &&
                    absFrontGapCellPos == Vector3Int.up)
                {
                    nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_UD");
                }
                else if (absBackGapCellPos == Vector3Int.right &&
                         absFrontGapCellPos == Vector3Int.right)
                {
                    nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_LR");
                }
                else if ((backGapCellPos == Vector3Int.right && frontGapCellPos == Vector3Int.up) ||
                         (backGapCellPos == Vector3Int.up && frontGapCellPos == Vector3Int.right))
                {
                    nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_CUL");
                }
                else if ((backGapCellPos == Vector3Int.left && frontGapCellPos == Vector3Int.up) ||
                         (backGapCellPos == Vector3Int.up && frontGapCellPos == Vector3Int.left))
                {
                    nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_CUR");
                }
                else if ((backGapCellPos == Vector3Int.right && frontGapCellPos == Vector3Int.down) ||
                         (backGapCellPos == Vector3Int.down && frontGapCellPos == Vector3Int.right))
                {
                    nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_CDL");
                }
                else if ((backGapCellPos == Vector3Int.left && frontGapCellPos == Vector3Int.down) ||
                        (backGapCellPos == Vector3Int.down && frontGapCellPos == Vector3Int.left))
                {
                    nextTile = Managers.Resource.Load<Tile>($"{Define.ROAD_TILE_PATH}Road_CDR");
                }
            }
        }

        if (nextTile)
        {
            Managers.Tile.SetTile(Define.Tilemap.WillRoad, pos, nextTile);
            var go = Managers.Tile.GetTilemap(Define.Tilemap.WillRoad).GetInstantiatedObject(pos);
            var road = go.GetComponent<Road>();
            road.GroupNumber = GroupNumber;
            road.Index = Index;
            road.IsStartRoad = IsStartRoad;
        }
    }
}
