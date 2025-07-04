using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public bool canHit = false;


    private void OnTriggerEnter(Collider other)
    {
        if (!canHit) return;

        Enemy enemy = other.GetComponentInParent<Enemy>(); // works if collider is on child
        if (enemy != null && !enemy.isDead)
        {
            int damage = EquipSystem.Instance.GetWeaponDamage();
            int penetration = EquipSystem.Instance.GetWeaponPenetration();

            enemy.TakeDamage(damage, penetration);
            Debug.Log($"Sword hit {enemy.enemyName} for {damage} dmg | Pen: {penetration}");
        }
    }

    public void EnableHit() => canHit = true;
    public void DisableHit() => canHit = false;
}
