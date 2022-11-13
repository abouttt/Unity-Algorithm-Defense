using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent_4 : TutorialBaseEvent
{
    [SerializeField]
    private Vector3Int _gatewayPos;
    [SerializeField]
    private Vector3Int _warriorCenterPos;
    [SerializeField]
    private Vector3Int _archerCenterPos;
    [SerializeField]
    private Vector3Int _wizardCenterPos;

    private Gateway _gateway;
    private TutorialJobCenter _warriorCenter;
    private TutorialJobCenter _archerCenter;
    private TutorialJobCenter _wizardCenter;

    private UI_BaseBuildingController _gatewayUI;

    private Coroutine _spawnCitizenCoroutine;

    private void Awake()
    {
        transform.Find("Canvas").GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public override void InitEvent()
    {
        _gateway = Util.GetBuilding<Gateway>(_gatewayPos);
        _warriorCenter = Util.GetBuilding<TutorialJobCenter>(_warriorCenterPos);
        _archerCenter = Util.GetBuilding<TutorialJobCenter>(_archerCenterPos);
        _wizardCenter = Util.GetBuilding<TutorialJobCenter>(_wizardCenterPos);

        _gatewayUI = UI_BuildingMenager.GetInstance.GatewayUIController;
    }

    public override void StartEvent()
    {
        Managers.Pool.Clear();
        _gateway.Clear();
        _warriorCenter.Clear();
        _archerCenter.Clear();
        _wizardCenter.Clear();
    }

    public override void CheckEvent()
    {
        if (!_gateway.HasCitizenData() && _gatewayUI.gameObject.activeSelf)
        {
            _spawnCitizenCoroutine = null;
        }
        else
        {
            if (!_gateway.HasCitizenData() && (_spawnCitizenCoroutine == null))
            {
                _spawnCitizenCoroutine = StartCoroutine(SpawnCitizen());
            }
        }

        if ((_warriorCenter.GetCitizenDataCount() == 1) &&
            (_archerCenter.GetCitizenDataCount() == 1) &&
            (_wizardCenter.GetCitizenDataCount() == 1))
        {
            if (_warriorCenter.GetCitizenType() == Define.Citizen.Blue &&
                _archerCenter.GetCitizenType() == Define.Citizen.Red &&
                _wizardCenter.GetCitizenType() == Define.Citizen.Green)
            {
                IsSuccessEvent = true;
            }
            else
            {
                IsFailureEvent = true;
            }
        }
        else if ((_warriorCenter.GetCitizenDataCount() > 1) ||
                 (_archerCenter.GetCitizenDataCount() > 1) ||
                 (_wizardCenter.GetCitizenDataCount() > 1))
        {
            IsFailureEvent = true;
        }
    }

    private IEnumerator SpawnCitizen()
    {
        int typeIndex = (int)Define.Citizen.Red;
        while (true)
        {
            CreateCitizen((Define.Citizen)typeIndex);

            yield return new WaitForSeconds(1f);

            typeIndex++;
            if (typeIndex > (int)Define.Citizen.Blue)
            {
                break;
            }
        }
    }

    private void CreateCitizen(Define.Citizen citizenType)
    {
        var pos = TileManager.GetInstance.GetCellCenterToWorld(Define.Tilemap.Ground, Managers.Game.Setting.SpawnCellPos);
        var go = Managers.Resource.Instantiate($"{Define.CITIZEN_PREFAB_PATH}{citizenType}Citizen", pos);
        var citizen = go.GetComponent<CitizenUnitController>();
        citizen.Data.MoveType = Define.Move.Up;
        citizen.SetNextDestination(citizen.transform.position);
    }
}