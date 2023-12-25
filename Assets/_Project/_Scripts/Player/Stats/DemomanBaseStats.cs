using UnityEngine;
namespace Arena._Project._Scripts.Player.Stats
{
    //TODO: Consider to move to json?
    //TODO: Implement stats provider with the proper scaling
    
    [CreateAssetMenu(fileName = "DemomanBaseStats", menuName = "Stats/Classes/DemomanBaseStats")]
    public class DemomanBaseStats : ScriptableObject
    {
        [field: SerializeField]
        public float Cooldown { get; private set; }
    }
}
