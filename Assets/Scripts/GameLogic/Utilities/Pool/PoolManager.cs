using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
	public static class PoolManager
	{
		#region Fields
		static List<ObjectsPool> pools;
		#endregion

		#region Methods
		public static void AddObjectsPool(ObjectsPool objectsPool)
		{
			if (objectsPool is null)
				throw new ArgumentNullException(nameof(objectsPool));

			if (pools == null)
				pools = new List<ObjectsPool>();
			pools.Add(objectsPool);
		}
		public static void RemoveObjectsPool(ObjectsPool objectsPool)
		{
			if (pools != null)
				pools.Remove(objectsPool);
        }

        public static GameObject GetGameObject(string name)
        {
            GameObject gameObject = null;
            for (int i = 0; i < pools?.Capacity; i++)
                if (pools[i].ObjectName == name)
                {
                    gameObject = pools[i].GetObject();
                    break;
                }
            if (gameObject == null)
                throw new ArgumentException("No gameObject with such name was found", name);
            return gameObject;
        }
        #endregion
    }
}