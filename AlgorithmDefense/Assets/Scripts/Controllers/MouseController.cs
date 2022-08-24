using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        Init();

        _camera = Camera.main;
    }

    private void Update()
    {
        MouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        MouseCellPos = Managers.Tile.GetGrid().WorldToCell(MouseWorldPos);

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (TileObjectBuilder.GetInstance.IsBuilding)
            {
                return;
            }

            var go = Managers.Tile.GetTilemap(Define.Tilemap.Building).GetInstantiatedObject(MouseCellPos);
            if (go)
            {
                var building = go.GetComponent<BaseBuilding>();
                if (building.HasUI)
                {
                    //생성된 클론으로 건물 정보 찾기
                    var name = building.name.Replace("(Clone)", String.Empty);
                    Debug.Log("빌딩이름: " + name);

                    //일딴 켜져있는 해당 건물UI 닫기(같은 이름의 건물 클릭 오류 방지)
                    UI_BuildingMenager.GetInstance.CloseUIController();

                    //클릭한 건물 이름과 오브젝트 전달
                    UI_BuildingMenager.GetInstance.ShowUIController((Define.Building)Enum.Parse(typeof(Define.Building), name), building);
                }
            }

            var item = Managers.Tile.GetTile(Define.Tilemap.Item, MouseCellPos);
            if (item)
            {
                if (item.name.Equals(Define.Item.Ore.ToString()))
                {
                    var tile = Managers.Resource.Load<Tile>($"{Define.BUILDING_TILE_PATH}{Define.Building.OreMine}");
                    TileObjectBuilder.GetInstance.Build(tile, MouseCellPos);
                    Managers.Tile.SetTile(Define.Tilemap.Item, MouseCellPos, null);
                    Managers.Game.HasOreMine = true;
                }
                else if(item.name.Equals(Define.Item.Wood.ToString()))
                {
                    var tile = Managers.Resource.Load<Tile>($"{Define.BUILDING_TILE_PATH}{Define.Building.Sawmill}");
                    TileObjectBuilder.GetInstance.Build(tile, MouseCellPos);
                    Managers.Tile.SetTile(Define.Tilemap.Item, MouseCellPos, null);
                    Managers.Game.HasSawmill = true;
                }
            }
        }

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

            var building = Managers.Tile.GetTile(Define.Tilemap.Building, MouseCellPos);
            if (building)
            {
                Managers.Tile.SetTile(Define.Tilemap.Building, MouseCellPos, null);
            }

            var road = Managers.Tile.GetTile(Define.Tilemap.Road, MouseCellPos);
            if (road)
            {
                Managers.Tile.SetTile(Define.Tilemap.Road, MouseCellPos, null);
            }
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
