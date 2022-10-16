using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoldMine : BaseBuilding
{
    public int GoldIncrease;

    private GoldAnimation _goldUI;

    public override void EnterTheBuilding(CitizenController citizen)
    {
        //Managers.Game.Gold += GoldIncrease;
        _goldUI.GoldSaving(GoldIncrease);
        Managers.Resource.Destroy(citizen.gameObject);
    }

    protected override IEnumerator ReleaseCitizen()
    {
        yield return null;
    }

    protected override void Init()
    {
        HasUI = false;
        _goldUI = FindObjectOfType<GoldAnimation>();
    }
}
