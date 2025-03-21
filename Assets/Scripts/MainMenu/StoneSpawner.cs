using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class StoneSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] stonePrefab;
    CancellationTokenSource _spawnCts;
    
    private Queue<GameObject> _spawnedStone = new Queue<GameObject>();

    public void StartSpawn()
    {
        gameObject.SetActive(true);

        _spawnCts = new CancellationTokenSource();
        SpawnStoneAsync().Forget();
    }

    public void StopSpawn()
    {
        if(_spawnCts != null){
            _spawnCts.Cancel();
            _spawnCts = null;
        }
    }

    public void DisableSpawner()
    {
        StopSpawn();
        
        foreach (var stone in _spawnedStone)
        {
            stone.gameObject.SetActive(false);
        }
        
        gameObject.SetActive(false);
    }

    async UniTask SpawnStoneAsync()
    {
        try 
        {
            while(!_spawnCts.Token.IsCancellationRequested)
            {
                await UniTask.Delay(20, cancellationToken: _spawnCts.Token);
                SpawnStone(Random.Range(0, stonePrefab.Length), new Vector3(Random.Range(-1f, 1f), transform.position.y, Random.Range(-1f, 1f)));
            }
        }
        catch (OperationCanceledException) 
        {
            Debug.Log("SpawnStoneAsync Cancelled");
        }
    }

    private void SpawnStone(int index, Vector3 position)
    {
        if (_spawnedStone.Count != 0)
        {
            var first = _spawnedStone.Dequeue();
            first.transform.position = position;
            first.transform.rotation = Random.rotation;
            first.SetActive(true);

            return;
        }
        
        var stoneObject = Instantiate(stonePrefab[index], position, Random.rotation, transform);
        if (stoneObject.TryGetComponent<Stone>(out var stone))
        {
            stone.OnStop += ReturnStone;
            stone.SetKinematic(false);
        }
        
        _spawnedStone.Enqueue(stoneObject);
    }

    private void ReturnStone(GameObject stoneObject)
    {
        stoneObject.SetActive(false);
        _spawnedStone.Enqueue(stoneObject);
    }
}
