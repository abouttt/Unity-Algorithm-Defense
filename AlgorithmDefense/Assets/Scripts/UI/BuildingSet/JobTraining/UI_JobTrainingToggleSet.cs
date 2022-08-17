using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_JobTrainingToggleSet : MonoBehaviour
{
    [field: SerializeField]
    public Define.Move MoveType { get; private set; }

    [field: SerializeField]
    public Define.Job JobType { get; private set; }
}
