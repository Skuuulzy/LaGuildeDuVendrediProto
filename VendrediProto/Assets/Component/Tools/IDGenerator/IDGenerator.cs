using UnityEngine;

namespace VComponent.Tools.IDGenerators
{
    public static class IDGenerator
    {
        private static ushort _lastGeneratedDeliveryID;

        /// <summary>
        /// Return an unique ushort for this game session.
        /// </summary>
        public static ushort RequestUniqueDeliveryID()
        {
            // Generate a new ID by incrementing the last one returned.
            ushort generatedID = _lastGeneratedDeliveryID++;

            // Making sure we never go higher than the range of a ushort.
            if (_lastGeneratedDeliveryID == 0)
            {
                Debug.LogWarning("Unable to generate any more unique id deliveries, id will be reset. Unique identifier might be compromised.");
            }
            
            return generatedID;
        }
    }
}