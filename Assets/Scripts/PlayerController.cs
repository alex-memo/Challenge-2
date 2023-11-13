using System.Collections;
using UnityEngine;
/// <summary>
/// @alex-memo 2023
/// </summary>
public class PlayerController : MonoBehaviour
{
	public static PlayerController Instance;
	private Animator anim;
	private bool isAttacking;

	public Transform attackPoint;
	private float attackRange = .75f;
	private LayerMask enemyMask;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		anim = GetComponent<Animator>();
		enemyMask = LayerMask.GetMask("Enemy");
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttacking)
		{
			StartCoroutine(attack());
		}
	}
	private IEnumerator attack()
	{
		isAttacking = true;
		anim.SetTrigger("Attack");
		yield return new WaitForSeconds(.5f);
		dealDamage();
		yield return new WaitForSeconds(.5f);
		isAttacking = false;
	}
	/// <summary>
	/// @alex-memo 2020
	/// </summary>
	private void dealDamage()
	{
		Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyMask);
		int[] hittedEnemies = new int[hitEnemies.Length];
		int i = 0;
		foreach (Collider enemy in hitEnemies)
		{
			int id = enemy.gameObject.GetInstanceID();
			int j = 0; bool isHit = true;
			while (j < hittedEnemies.Length)
			{
				if (hittedEnemies[j] == id)
				{
					isHit = false;
				}

				j++;
			}
			hittedEnemies[i] = enemy.gameObject.GetInstanceID();
			i++;
			if (isHit == true)
			{
				enemy.GetComponent<MaguuKenki>().TakeDamage(5);
			}
		}
	}
}
