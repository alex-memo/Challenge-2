using UnityEngine;
using Overtime.FSM;
/// <summary>
/// @alex-memo 2023
/// </summary>
public abstract class KenkiStateBase : State<MaguuKenki, KenkiStateID, KenkiStateTransition>
{

}
public enum KenkiStateID
{
    WANDERING,
    ATTACK,
    DEAD
}
public enum KenkiStateTransition
{
    START_WANDER,
    STOP_WANDER,
    START_ATTACK,
    STOP_ATTACK,
    START_DEAD,
    STOP_DEAD,
}
