using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner1 : MonoBehaviour
{
    [SerializeField] private List<GameObject> levelPrefab = new List<GameObject>();
    private float spawnPos = 0;
    private float levelLength = 40;
    private int startLevels = 20;

    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i <= startLevels; i++)
        {
            SpawnLevel(Random.Range(0, levelPrefab.Count));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //SpawnLevel(Random.Range(0, levelPrefab.Count));
    }

    private void SpawnLevel(int tileIndex)
    {
        // создаём объект, который достаем из списка, в точке spawnPos
        Instantiate(levelPrefab[tileIndex], transform.forward * spawnPos, transform.rotation);
        //прибавляем к точке следующего спауна длину только, что добавленного элемента 
        spawnPos += levelLength;
    }
}
