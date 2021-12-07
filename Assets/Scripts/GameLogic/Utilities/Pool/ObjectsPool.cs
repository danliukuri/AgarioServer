using UnityEngine;
using System.Collections.Generic;

namespace Pool
{
	public class ObjectsPool : MonoBehaviour
	{
		#region Properties
		public string ObjectName => objectName;
		#endregion

		#region Fields
		[SerializeField] string objectName;
		[SerializeField] GameObject gameObjectPrefab;
		[SerializeField] Transform objectsParent;

		[SerializeField] List<GameObject> objects;
		#endregion

		#region Methods
		/// <summary>
		/// Finds a free object or creates a new one
		/// </summary>
		/// <returns> Found or newly created object </returns>
		public GameObject GetObject()
		{
			GameObject gameObject = null;
			for (int i = 0; i < objects?.Count; i++)
				if (!objects[i].activeSelf)
				{
					gameObject = objects[i];
					break;
				}

			if (gameObject == null)
			{
				gameObject = Instantiate(gameObjectPrefab, objectsParent);
				objects.Add(gameObject);
			}
			return gameObject;
		}

		/// <summary>
		/// Fills empty cells in the objects list with new game object prefabs
		/// </summary>
		[ContextMenu("Fill empty cells")]
		void FillEmptyCells()
		{
			for (int i = 0; i < objects.Count; i++)
				if (objects[i] == null)
				{
					objects[i] = Instantiate(gameObjectPrefab, objectsParent);
					objects[i].SetActive(false);
				}
		}

		void OnValidate()
		{
			objectName = gameObjectPrefab?.name;
		}
		void Awake()
		{
			if (objects == null)
				objects = new List<GameObject>();
			FillEmptyCells();
			PoolManager.AddObjectsPool(this);
		}
		void OnDestroy()
		{
			PoolManager.RemoveObjectsPool(this);
		}
		#endregion
	}
}