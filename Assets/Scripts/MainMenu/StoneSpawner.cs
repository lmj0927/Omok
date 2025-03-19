using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StoneSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] stonePrefab;
    CancellationTokenSource _spawnCts;


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
        
        gameObject.SetActive(false);
    }

    async UniTask SpawnStoneAsync()
    {
        while(_spawnCts != null && !_spawnCts.Token.IsCancellationRequested)
        {
            await UniTask.Delay(20);
            SpawnStone(Random.Range(0, stonePrefab.Length), new Vector3(Random.Range(-1f, 1f), transform.position.y, Random.Range(-1f, 1f)));
        }
    }

    public void SpawnStone(int index, Vector3 position)
    {
        Instantiate(stonePrefab[index], position, Random.rotation, transform);
    }
}
