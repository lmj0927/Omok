using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] stonePrefab;
    
    Coroutine _spawnRoutine;

    public void StartSpawn()
    {
        if(_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
        }
        _spawnRoutine = StartCoroutine(SpawnStoneRoutine());
    }

    public void StopSpawn()
    {
        StopCoroutine(_spawnRoutine);
    }


    IEnumerator SpawnStoneRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
            
            SpawnStone(Random.Range(0, stonePrefab.Length), new Vector3(Random.Range(-1f, 1f), transform.position.y, Random.Range(-1f, 1f)));
        }
    }


    public void SpawnStone(int index, Vector3 position)
    {
        Instantiate(stonePrefab[index], position, Random.rotation, transform);
    }
}
