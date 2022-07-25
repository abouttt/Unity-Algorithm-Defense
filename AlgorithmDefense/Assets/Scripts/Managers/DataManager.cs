using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;

public class DataManager
{
    [Serializable]
    public class TilemapData
    {
        public Vector3Int Pos;
        public string TileName;
    }

    private Dictionary<Define.Tilemap, List<TilemapData>> _tilemapDatas = new Dictionary<Define.Tilemap, List<TilemapData>>();

    public void Init()
    {
        _tilemapDatas.Add(Define.Tilemap.Road, new List<TilemapData>());
        _tilemapDatas.Add(Define.Tilemap.Building, new List<TilemapData>());
    }

    public void SaveTilemaps()
    {
        foreach (var tilemapData in _tilemapDatas)
        {
            var tilemap = Managers.Tile.GetTilemap(tilemapData.Key);
            tilemap.CompressBounds();

            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                var localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                if (tilemap.HasTile(localPlace))
                {
                    _tilemapDatas[tilemapData.Key].Add(new TilemapData
                    {
                        Pos = pos,
                        TileName = tilemap.GetTile(localPlace).name
                    });
                }
            }
        }

        string data = JsonConvert.SerializeObject(_tilemapDatas);
        PlayerPrefs.SetString(Define.Data.TilemapData.ToString(), data);
        PlayerPrefs.Save();
    }

    public void LoadTilemaps()
    {
        if (PlayerPrefs.HasKey(Define.Data.TilemapData.ToString()))
        {
            string dataStr = PlayerPrefs.GetString(Define.Data.TilemapData.ToString());
            _tilemapDatas = JsonConvert.DeserializeObject<Dictionary<Define.Tilemap, List<TilemapData>>>(dataStr);

            foreach (var tilemapData in _tilemapDatas)
            {
                for (int i = 0; i < tilemapData.Value.Count; i++)
                {
                    var data = tilemapData.Value[i];
                    if (data.TileName.Equals("RoadRuleTile"))
                    {
                        var tile = Managers.Resource.Load<RuleTile>($"{Define.RULE_TILE_PATH}{data.TileName}");
                        TileObjectBuilder.GetInstance.Build(tile, data.Pos);
                    }
                    else
                    {
                        var tile = Managers.Resource.Load<TileBase>($"{Define.BUILDING_TILE_PATH}{data.TileName}");
                        TileObjectBuilder.GetInstance.Build(tile, data.Pos);
                    }
                }
            }
        }
    }

    public void DeleteTilemaps()
    {
        if (PlayerPrefs.HasKey(Define.Data.TilemapData.ToString()))
        {
            PlayerPrefs.DeleteKey(Define.Data.TilemapData.ToString());
        }
    }

    public void Clear() => PlayerPrefs.DeleteAll();
}