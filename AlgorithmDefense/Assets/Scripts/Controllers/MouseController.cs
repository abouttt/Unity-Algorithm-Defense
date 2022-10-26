using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class MouseController : MonoBehaviour
{
    private static MouseController s_instance;
    public static MouseController GetInstance { get { Init(); return s_instance; } }

    public Vector3 MouseWorldPos { get; private set; }
    public Vector3Int MouseCellPos { get; private set; }

    private Camera _camera;
    private TileBase _cursorTile;
    private Vector3Int _prevMouseCellPos;

    private void Start()
    {
        Init();

        _camera = Camera.main;
        _cursorTile = Managers.Resource.Load<TileBase>("Tiles/Grounds/Cursor");
    }

    private void Update()
    {
        MouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        MouseCellPos = TileManager.GetInstance.GetGrid().WorldToCell(MouseWorldPos);

        if (MouseCellPos != _prevMouseCellPos)
        {
            TileManager.GetInstance.SetTile(Define.Tilemap.Temp, _prevMouseCellPos, null);
            TileManager.GetInstance.SetTile(Define.Tilemap.Temp, MouseCellPos, _cursorTile);
            _prevMouseCellPos = MouseCellPos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (RoadBuilder.GetInstance.IsWillRoadBuild())
            {
                return;
            }

            var building = Util.GetBuilding<BaseBuilding>(MouseCellPos);
            if (building)
            {
                if (building.HasUI)
                {
                    var name = building.name.Replace("(Clone)", string.Empty);
                    UI_BuildingMenager.GetInstance.CloseUIController();
                    UI_BuildingMenager.GetInstance.ShowUIController((Define.Building)Enum.Parse(typeof(Define.Building), name), building);
                }
                else
                {
                    var jobCenter = building as JobCenter;
                    if (jobCenter)
                    {
                        jobCenter.ChangeOutputDir();
                    }
                }

                //RoadBuilder.GetInstance.IsBuilding = false;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (RoadBuilder.GetInstance.IsBuilding)
            {
                return;
            }

            RoadBuilder.GetInstance.IsBuilding = true;
        }

        /* if (Input.GetMouseButtonUp(0))
         {
             if (EventSystem.current.IsPointerOverGameObject())
             {
                 return;
             }

             if (RoadBuilder.GetInstance.IsBuilding)
             {
                 return;
             }

             var go = TileManager.GetInstance.GetTilemap(Define.Tilemap.Building).GetInstantiatedObject(MouseCellPos);
             if (go)
             {
                 var building = go.GetComponent<BaseBuilding>();
                 if (building.HasUI)
                 {
                     //생성된 클론으로 건물 정보 찾기
                     var name = building.name.Replace("(Clone)", string.Empty);

                     //일딴 켜져있는 해당 건물UI 닫기(같은 이름의 건물 클릭 오류 방지)
                     UI_BuildingMenager.GetInstance.CloseUIController();

                     //클릭한 건물 이름과 오브젝트 전달
                     UI_BuildingMenager.GetInstance.ShowUIController((Define.Building)Enum.Parse(typeof(Define.Building), name), building);
                 }

                 var jobCenter = go.GetComponent<JobCenter>();
                 if (jobCenter)
                 {
                     jobCenter.ChangeOutputDir();
                 }
             }
         }*/

        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (TileObjectBuilder.GetInstance.IsBuilding)
            {
                return;
            }

            // 길 삭제.
            var go = TileManager.GetInstance.GetTilemap(Define.Tilemap.Road).GetInstantiatedObject(MouseCellPos);
            if (go)
            {
                RoadBuilder.GetInstance.RemoveRoads(go.GetComponent<Road>().GroupNumber);
            }

            // 건물 삭제.
            /* var building = TileManager.GetInstance.GetTile(Define.Tilemap.Building, MouseCellPos);
             if (building)
             {
                 TileManager.GetInstance.SetTile(Define.Tilemap.Building, MouseCellPos, null);
             }*/
        }
    }

    private static void Init()
    {
        if (s_instance == null)
        {
            var go = GameObject.Find("@MouseController");
            if (!go)
            {
                go = Util.CreateGameObject<MouseController>("@MouseController");
            }

            s_instance = go.GetComponent<MouseController>();
        }
    }
}
