using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assemblies.Network.Messages.Network
{
    public abstract class NetworkMessageBase : IDisposable
    {
        protected readonly string uniqueName;

        private readonly ulong _networkObjectId;
        
        private static readonly Dictionary<ulong, HashSet<string>> Messages = new ();

        protected NetworkMessageBase(ulong networkObjectId, string messageName)
        {
            _networkObjectId = networkObjectId;
            
            ValidateName(messageName);
            
            uniqueName = messageName + networkObjectId;
        }
        
        public virtual void Dispose()
        {
            Messages[_networkObjectId].Remove(uniqueName);
            
            if (Messages[_networkObjectId].Count == 0)
                Messages.Remove(_networkObjectId);
        }

        private void ValidateName(string messageName)
        {
            if (Messages.ContainsKey(_networkObjectId))
            {
                if (Messages[_networkObjectId].Contains(messageName))
                {
                    Debug.LogError($"Message with name {messageName} already exists for object with id {_networkObjectId}");
                    return;
                }
                
                Messages[_networkObjectId].Add(messageName);
            }
            else
            {
                Messages.Add(_networkObjectId, new HashSet<string>
                {
                    messageName
                });
            }
        }
    }
}
