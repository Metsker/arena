using System;
using Assemblies.Utilities.Attributes;
using Sirenix.OdinInspector;
using Tower.Core.Entities.Classes.Alchemist.Enums;
using Unity.Netcode;

namespace Tower.Core.Entities.Classes.Alchemist.Data
{
    [Serializable]
    public struct PotionsStats : INetworkSerializable
    {
        public Toxin toxin;
        public Healing healing;
        public Bomba bomba;
        //public Wierd wierd;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref toxin);
            serializer.SerializeValue(ref healing);
            serializer.SerializeValue(ref bomba);
            //serializer.SerializeValue(ref wierd);
        }

        public int GetOverheatValue(PotionType potionType)
        {
            switch (potionType)
            {
                case PotionType.Toxin:
                    return toxin.overheat;
                case PotionType.Heal:
                    return healing.overheat;
                case PotionType.Bomba:
                    return bomba.overheat;
                default:
                    throw new ArgumentOutOfRangeException(nameof(potionType), potionType, null);
            }
        }

        [Serializable]
        public struct Toxin : INetworkSerializable
        {
            [MinValue(0.1f)] public int percentPerTick;
            [MinValue(0.1f)] public int intervalSec;
            [MinValue(0.1f)] public int duration;
            [DefaultValue(10)] public int overheat;
            
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref percentPerTick);
                serializer.SerializeValue(ref intervalSec);
                serializer.SerializeValue(ref duration);
                serializer.SerializeValue(ref overheat);
            }
        }

        [Serializable]
        public struct Healing : INetworkSerializable
        {
            [MinValue(0.1f)] public int onHitHealAmount;
            [MinValue(0.1f), DefaultValue(0.2f)] public float maxHPScaling;
            [DefaultValue(60)] public int overheat;
            
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref onHitHealAmount);
                serializer.SerializeValue(ref overheat);
            }
        }

        [Serializable]
        public struct Bomba : INetworkSerializable
        {
            [MinValue(0.1f)] public int duration;
            [MinValue(0.1f)] public float damageScaling;
            [MinValue(0.1f)] public int aoe;
            [DefaultValue(60)] public int overheat;
            
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref duration);
                serializer.SerializeValue(ref damageScaling);
                serializer.SerializeValue(ref aoe);
                serializer.SerializeValue(ref overheat);
            }
        }

        [Serializable]
        public struct Wierd : INetworkSerializable
        {
            [MinValue(0.1f)] public float positiveChance;
            [DefaultValue(100)] public int overheat;

            [MinValue(0.01f)] public float highRollChance;// = 0.01f;
            [MinValue(0.01f)] public float midRollChance;// = 0.2f;
            [MinValue(0.01f)] public float lowRollChance;// = 0.5f;
            
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref positiveChance);
                serializer.SerializeValue(ref overheat);
                serializer.SerializeValue(ref highRollChance);
                serializer.SerializeValue(ref midRollChance);
                serializer.SerializeValue(ref lowRollChance);
            }
        }
    }
}
