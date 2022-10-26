using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class Define
{
    public static readonly string CONTENTS_PATH = "Prefabs/Contents/";
    public static readonly string GROUND_PREFAB_PATH = "Prefabs/Ground/";
    public static readonly string CITIZEN_PATH = "Prefabs/Units/Citizens/";
    public static readonly string BATTILE_UNIT_PATH = "Prefabs/Units/BattleUnits/";
    public static readonly string MONSTER_UNIT_PATH = "Prefabs/Units/MonsterUnits/";
    public static readonly string BUILDING_PREFAB_PATH = "Prefabs/TileObject/Buildings/";
    public static readonly string ROAD_PREFAB_PATH = "Prefabs/TileObject/Roads/";
    public static readonly string ROAD_TILE_PATH = "Tiles/Roads/";
    public static readonly string RULE_TILE_PATH = "Tiles/RuleTiles/";
    public static readonly string BUILDING_TILE_PATH = "Tiles/Buildings/";
    public static readonly string ITEM_TILE_PATH = "Tiles/Items/";
    public static readonly string PROJECTILE_PREFAB_PATH = "Prefabs/Projectile/";
    public static readonly string STREAM_SAVE_DATA_PATH = $"{Application.streamingAssetsPath}/SaveDatas/";

    public static int[] DX = { 0, 1, 0, -1 };
    public static int[] DY = { 1, 0, -1, 0 };

    public enum Tilemap
    {
        Ground,
        Road,
        WillRoad,
        Building,
        Rampart,
        Temp,
    }

    public enum Road
    {
        B,
        BD,
        BL,
        BR,
        BU,
        CDL,
        CDR,
        CUL,
        CUR,
        LR,
        TD,
        TL,
        TR,
        TU,
        UD,
        Cross,
    }

    public enum Building
    {
        Gateway,
        WarriorCenter,
        ArcherCenter,
        WizardCenter,
        GoldMine,
        CastleGate,
        MonsterGate,
    }

    public enum Job
    {
        None,
        Warrior,
        Archer,
        Wizard,
    }

    public enum Citizen
    {
        None,
        Red,
        Green,
        Blue,
    }

    public enum Move
    {
        None,
        Up,
        Right,
        Down,
        Left,
    }

    public enum Magic
    {
        None,
        Thunderstroke,
        Madness,
        Heal,
    }

    public enum Data
    {
        Tilemap,
        Citizen,
        Camp,
        Gateway,
        GatewayWithCount,
        JobTraining,
        MagicFactory,
        OreMine,
        Sawmill,
        CitizenSpawner,
        Runtime,
        Ore,
        Wood,
        Magic,
        BattleUnit,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }
}
