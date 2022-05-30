using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBuilding : MonoBehaviour
{
    public bool CanSelect { get; protected set; }

    public abstract void EnterTheBuilding(CitizenController citizen);

    public abstract void ShowUIController();
}
