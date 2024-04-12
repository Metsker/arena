using System;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Enums;
using Sirenix.OdinInspector;
using Unity.Netcode;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Data
{
    [Serializable]
    public struct PotionBeltStats : INetworkSerializable
    {
        public Toxin toxin;
        public Healing healing;
        public Bomba bomba;
        public Wierd wierd;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref toxin);
            serializer.SerializeValue(ref healing);
            serializer.SerializeValue(ref bomba);
            serializer.SerializeValue(ref wierd);
        }

        public float GetCdTime(PotionType potionType)
        {
            switch (potionType)
            {
                case PotionType.Toxin:
                    return toxin.cd;
                case PotionType.Heal:
                    return healing.cd;
                case PotionType.Bomba:
                    return bomba.cd;
                case PotionType.Wierd:
                    return wierd.cd;
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
            [MinValue(0.1f)] public float cd;
            
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref percentPerTick);
                serializer.SerializeValue(ref intervalSec);
                serializer.SerializeValue(ref duration);
                serializer.SerializeValue(ref cd);
            }
        }

        [Serializable]
        public struct Healing : INetworkSerializable
        {
            [MinValue(0.1f)] public int onHitHealAmount;
            [MinValue(0.1f)] public int puddleHealPerSec;
            [MinValue(0.1f)] public int puddleSize;
            [MinValue(0.1f)] public int puddleDuration;
            [MinValue(0.1f)] public int cd;
            
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref onHitHealAmount);
                serializer.SerializeValue(ref puddleHealPerSec);
                serializer.SerializeValue(ref puddleSize);
                serializer.SerializeValue(ref puddleDuration);
                serializer.SerializeValue(ref cd);
            }
        }

        [Serializable]
        public struct Bomba : INetworkSerializable
        {
            [MinValue(0.1f)] public int duration;
            [MinValue(0.1f)] public int damage;
            [MinValue(0.1f)] public int aoe;
            [MinValue(0.1f)] public int cd;
            
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref duration);
                serializer.SerializeValue(ref damage);
                serializer.SerializeValue(ref aoe);
                serializer.SerializeValue(ref cd);
            }
        }

        [Serializable]
        public struct Wierd : INetworkSerializable
        {
            [MinValue(0.1f)] public float positiveChance;
            [MinValue(0.1f)] public int cd;

            [MinValue(0.01f)] public float highRollChance;// = 0.01f;
            [MinValue(0.01f)] public float midRollChance;// = 0.2f;
            [MinValue(0.01f)] public float lowRollChance;// = 0.5f;
            
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref positiveChance);
                serializer.SerializeValue(ref cd);
                serializer.SerializeValue(ref highRollChance);
                serializer.SerializeValue(ref midRollChance);
                serializer.SerializeValue(ref lowRollChance);
            }
        }
    }
}
