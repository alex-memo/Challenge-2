using UnityEngine;
/// <summary>
/// @alex-memo 2023
/// </summary>
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private void Awake()
    {
        if(Instance!=null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void OnAttack()
    {

    }
    private void attack()
    {

    }
}
