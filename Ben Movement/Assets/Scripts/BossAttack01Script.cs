using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack01Script : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private BossController boss;
    public SphereCollider col;
    // Start is called before the first frame update
    void Start()
    {
        boss = GetComponentInParent<BossController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HurtPlayer(int amount)
    {
        for(int i = 0; i < Physics.OverlapBox(transform.position + Vector3.up, Vector3.one * 2.0f).Length - 1; i++)
        {
            if (Physics.OverlapBox(transform.position + Vector3.up, Vector3.one * 2.0f)[i].CompareTag("Player"))
            {
                playerHealth = boss.currentTarget.GetComponent<PlayerHealth>();
                playerHealth.Damage(amount);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + Vector3.up, Vector3.one * 2.0f);
    }
}
