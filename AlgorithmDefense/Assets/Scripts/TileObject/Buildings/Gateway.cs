using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gateway : BaseBuilding
{
    public Dictionary<Define.Citizen, Define.Move> DirectionCondition { get; private set; }

    private Queue<CitizenData> _redOrderQueue = new();
    private Queue<CitizenData> _greenOrderQueue = new();
    private Queue<CitizenData> _blueOrderQueue = new();

    private bool _isRedReleasing;
    private bool _isGreenReleasing;
    private bool _isBlueReleasing;

    public override void EnterTheBuilding(CitizenController citizen)
    {
        switch (citizen.Data.CitizenType)
        {
            case Define.Citizen.Red:
                _redOrderQueue.Enqueue(citizen.Data);
                if (!_isRedReleasing)
                {
                    _isRedReleasing = true;
                    StartCoroutine(ReleaseRedCitizen());
                }
                break;
            case Define.Citizen.Green:
                _greenOrderQueue.Enqueue(citizen.Data);
                if (!_isGreenReleasing)
                {
                    _isGreenReleasing = true;
                    StartCoroutine(ReleaseGreenCitizen());
                }
                break;
            case Define.Citizen.Blue:
                _blueOrderQueue.Enqueue(citizen.Data);
                if (!_isBlueReleasing)
                {
                    _isBlueReleasing = true;
                    StartCoroutine(ReleaseBlueCitizen());
                }
                break;
        }

        Managers.Resource.Destroy(citizen.gameObject);
    }

    private IEnumerator ReleaseRedCitizen()
    {
        while (true)
        {
            if (_redOrderQueue.Count == 0)
            {
                _isRedReleasing = false;
                yield break;
            }

            yield return new WaitForSeconds(_releaseTime);

            if (!HasRoadNextPosition(DirectionCondition[Define.Citizen.Red]))
            {
                continue;
            }

            Release(_redOrderQueue);
        }
    }

    private IEnumerator ReleaseGreenCitizen()
    {
        while (true)
        {
            if (_greenOrderQueue.Count == 0)
            {
                _isGreenReleasing = false;
                yield break;
            }

            yield return new WaitForSeconds(_releaseTime);

            if (!HasRoadNextPosition(DirectionCondition[Define.Citizen.Green]))
            {
                continue;
            }

            Release(_greenOrderQueue);
        }
    }

    private IEnumerator ReleaseBlueCitizen()
    {
        while (true)
        {
            if (_blueOrderQueue.Count == 0)
            {
                _isBlueReleasing = false;
                yield break;
            }

            yield return new WaitForSeconds(_releaseTime);

            if (!HasRoadNextPosition(DirectionCondition[Define.Citizen.Blue]))
            {
                continue;
            }

            Release(_blueOrderQueue);
        }
    }


    private void Release(Queue<CitizenData> citizenOrderQueue)
    {
        var citizen = DequeueCitizen(citizenOrderQueue);
        citizen.Data.MoveType = DirectionCondition[citizen.Data.CitizenType];
        SetCitizenPosition(citizen);
        citizen.SetNextDestination(transform.position);
    }

    protected override void Init()
    {
        if (DirectionCondition == null)
        {
            DirectionCondition = new Dictionary<Define.Citizen, Define.Move>()
            {
                { Define.Citizen.Red, Define.Move.None },
                { Define.Citizen.Green, Define.Move.None },
                { Define.Citizen.Blue, Define.Move.None },
            };
        }

        HasUI = true;
    }
}
