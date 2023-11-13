using UnityEngine;

public class MaskScript : MonoBehaviour
{
	private Rigidbody rb;
	
	private void Awake()
	{
		var _lookat=PlayerController.Instance.transform.position;
		_lookat.y=transform.position.y;
		transform.LookAt(_lookat);
		rb= GetComponent<Rigidbody>();
		rb.AddForce(transform.forward * 500);
		Destroy(gameObject, 3f);
	}
}
