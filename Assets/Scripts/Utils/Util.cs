using System;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        var component = go.GetComponent<T>();
        if (!component)
        {
            component = go.AddComponent<T>();
        }

        return component;
    }

    public static GameObject CreateGameObject<T>(string name, Transform parent = null) where T : UnityEngine.Component
    {
        var go = CreateGameObject(name, parent);
        go.AddComponent<T>();
        return go;
    }

    public static GameObject CreateGameObject(string name, Transform parent = null)
    {
        var go = new GameObject { name = name };
        go.transform.SetParent(parent);
        return go;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        var transform = FindChild<Transform>(go, name, recursive);
        if (!transform)
        {
            return null;
        }

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (!go)
        {
            return null;
        }

        if (recursive)
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name.Equals(name))
                {
                    return component;
                }
            }
        }
        else
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name.Equals(name))
                {
                    var component = transform.GetComponent<T>();
                    if (component)
                    {
                        return component;
                    }
                }
            }
        }

        return null;
    }

    public static Vector3Int GetAbsVector3Int(Vector3Int value)
    {
        int x = Mathf.Abs(value.x);
        int y = Mathf.Abs(value.y);
        int z = Mathf.Abs(value.z);
        return new Vector3Int(x, y, z);
    }

    public static Road GetRoad(Define.Tilemap tilemap, Vector3Int pos)
    {
        var go = TileManager.GetInstance.GetTilemap(tilemap).GetInstantiatedObject(pos);
        if (go)
        {
            return go.GetComponent<Road>();
        }

        return null;
    }

    public static T GetBuilding<T>(Vector3Int pos) where T : UnityEngine.Component
    {
        var go = TileManager.GetInstance.GetTilemap(Define.Tilemap.Building).GetInstantiatedObject(pos);
        if (go)
        {
            return go.GetComponent<T>();
        }

        return null;
    }

    public static bool IsEndAnimation(Animator animator)
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.99f)
        {
            return true;
        }

        return false;
    }

    public static bool IsEndAnimation(Animator animator, string stateName)
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName(stateName) && info.normalizedTime >= 0.99f)
        {
            return true;
        }

        return false;
    }

    public static BattleUnitController CreateMonster(Define.Job job, Vector3 spawnPos)
    {
        var go = Managers.Resource.Instantiate($"{Define.MONSTER_UNIT_PREFAB_PATH}Goblin_{job}");
        var monster = go.GetComponent<BattleUnitController>();
        monster.transform.position = spawnPos;
        monster.Data.MoveType = Define.Move.Down;
        monster.Data.CurrentHp = monster.Data.MaxHp;
        return monster;
    }
}
