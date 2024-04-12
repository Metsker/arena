// using Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers;
// using Unity.Netcode;
// using UnityEngine;
//
// namespace Arena.__Scripts.Core.Entities.Classes.Common.Data.Enemy
// {
//     public class EnemyNetworkDataContainer : HealthNetworkDataContainer
//     {
//         private const float MinSpeed = 0.5f;
//         private const float MaxSpeed = 2;
//         
//         private const float MinAttacksPerSec = 0.5f;
//         private const float MaxAttacksPerSec = 5;
//
//         public float AttacksCd => 1 / attacksPerSec.Value;
//
//         public NetworkVariable<EnemyStats> enemyStats = new ();
//
//         protected void Init(EnemyData enemyData)
//         {
//             EnemyStats stats = enemyData.EnemyStats;
//
//             InitHealth(stats.health);
//             speed.Value = stats.speed;
//             damage.Value = stats.damage;
//             contactDamage.Value = stats.contactDamage;
//             armor.Value = stats.armor;
//             attacksPerSec.Value = stats.attacksPerSec;
//         }
//
//         public void AddSpeed(float amount)
//         {
//             speed.Value += amount;
//             speed.Value = Mathf.Clamp(speed.Value, MinSpeed, MaxSpeed);
//         }
//
//         public void AddAttackSpeed(float amount)
//         {
//             attacksPerSec.Value += amount;
//             attacksPerSec.Value = Mathf.Clamp(attacksPerSec.Value, MinAttacksPerSec, MaxAttacksPerSec);
//         }
//
//         public void AddDamage(int amount) =>
//             damage.Value += amount;
//
//         public override void DealDamage(int amount)
//         {
//             int reducedAmount = amount - armor.Value;
//             base.DealDamage(reducedAmount);
//         }
//
//         public override void DealDamage(float percent)
//         {
//             int reducedAmount = PercentageDamage(percent) - armor.Value;
//             base.DealDamage(reducedAmount);
//         }
//     }
// }
