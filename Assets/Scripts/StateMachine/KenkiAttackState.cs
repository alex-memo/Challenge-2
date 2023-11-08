using System.Collections;
using UnityEngine;
/// <summary>
/// @alex-memo 2023
/// </summary>
public class KenkiAttackState : KenkiStateBase
{
    public override void BuildTransitions()
    {
        AddTransition(KenkiStateTransition.STOP_ATTACK, KenkiStateID.DEAD);
    }
    public override void Enter()
    {
        StartCoroutine(StartBehaviour());
    }
    public IEnumerator StartBehaviour()
    {
        Parent.Agent.ResetPath();
        Parent.IsInvincible = false;
        //push mask to player
        yield return sendMaskToPlayer();
        Parent.Agent.SetDestination(PlayerController.Instance.transform.position);
        //run to player and when distance is close enough do circular attack
        yield return circularAttack();
        //3 hit combo
        //push mask forward
        //charge sword  and do circular attack
        yield return new WaitForSeconds(3);
        //3 hit combo
        //mask forward
        //3 hit combo
        //when at 75% of max hp then pull player to enemy
        //after 2 seconds stab ground dealing aoe dmg
        //charge to circular but now with ghost
        //dash back away from player and leave clone in the last position
        //dash to player
        //if player away targeted mask that closes on player and deals dmg

        //if its under 75% hp ghost will replicate the same attack
    }
    private IEnumerator sendMaskToPlayer()
    {
        //anim
        //throw mask
        yield return new WaitForSeconds(2);
    }
    private IEnumerator circularAttack()
    {
        //anim
        //charge sword
        //do circular attack
        yield return new WaitForSeconds(2);
    }
    public override void Update()
    {
        
    }

    public override void OnTriggerEnter(Collider _coll)
    {
        
    }

    public override void FixedUpdate()
    {
       
    }

    public override void Exit()
    {
        
    }
}
