using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using static Define;


public class UI_CitizenCountController : UI_BaseBuildingController
{

    [SerializeField]
    private ToggleGroup[] _toggleGroups;
    [SerializeField]
    private Text countText;

    public Define.Move MoveType;

    public int CitizenCount;


    //���� ����� GateWay prtfab(Clone)
    private GameObject ThisWithCount;

    public void SetDirection(GameObject obj)
    {
        //��� ��� �ݱ�
        AllOffToggles();

        //��� ����
        SetupGatewayInfo();

        //ī��Ʈ ����
        countText.text = CitizenCount.ToString();

        //���� ����� GateWay ����
        ThisWithCount = obj;

    }

    public void AllOffToggles()
    {
        //���� �����ִ� ��� ��� �ݱ�
        for (int i = 0; i < _toggleGroups.Length; i++)
        {
            _toggleGroups[i].SetAllTogglesOff();

        }
    }

    private void SetupGatewayInfo()
    {

        if (MoveType != Define.Move.None)
        {
            var toggle = findToggle(MoveType);
            toggle.isOn = true;
        }

    }

    private Toggle findToggle(Define.Move moveType)
    {
        foreach (var toggles in _toggleGroups)
        {
            return toggles.GetComponentsInChildren<UI_CitizenCountToggleSet>()
                          .First(toggle => toggle.MoveType == moveType)
                          .GetComponent<Toggle>();
        }

        return null;
    }



    public void OKButtonClick()
    {
        //Ʈ���� �׷쿡�� ã��
        foreach (var toggles in _toggleGroups)
        {
            //���� Ʈ���� ��������
            var toggle = toggles.GetFirstActiveToggle();
            //���� ���ȴٸ�
            if (toggle != null)
            {
                //Ʈ���� ���� ������
                var info = toggle.GetComponent<UI_CitizenCountToggleSet>();
                //Ʈ���� ���� ������ �־���
                MoveType = info.MoveType;

            }
        }

        // ����� GateWay ������ ������Ʈ

        //��� ��� �ݱ�(�ܻ� ������)
        AllOffToggles();

        //UI �ݱ�
        UI_BuildingMenager.GetInstance.CloseUIController();

    }

    public void DestructionButtonClick()

    {
        //�ǹ� ����
        Managers.Tile.SetTile(Define.Tilemap.Building, MouseController.GetInstance.MouseCellPos, null);
        Managers.Tile.SetTile(Define.Tilemap.Road, MouseController.GetInstance.MouseCellPos, null);

        //UI �ݱ�
        UI_BuildingMenager.GetInstance.CloseUIController();

    }

    public void CountPlusButtonClick()
    {
        CitizenCount++;

        countText.text = CitizenCount.ToString();

    }
    public void CountMinusButtonClick()
    {
        if (CitizenCount != 0)
        {
            CitizenCount--;

            countText.text = CitizenCount.ToString();
        }

    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }
}