using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

	[SerializeField] private List<Transform> _spawnPosition;
	private static Map _instance;

	private void Start()
	{
		_instance = (Map)FindObjectOfType(typeof(Map));
	}

	public static Map ThisMap
	{
		get {
			if (_instance == null)
			{
				_instance = (Map)FindObjectOfType(typeof(Map));
			}

			return _instance;
		}
	}

	public static Transform GetRandomSpawnPosition()
	{
		return _instance._spawnPosition[Random.Range(0, _instance._spawnPosition.Count - 1)];
	}
}
