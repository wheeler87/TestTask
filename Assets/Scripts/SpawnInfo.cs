using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnInfo
{

    public Vector3 startPosition;
    public Vector3 direction;
    public float stepSize;

    public Vector3 GetSpawnPosition(int index, int maxSpawnPositions)
    {
        float offsetDistance = (maxSpawnPositions - 1) * stepSize * 0.5f;
        var offset = -direction * offsetDistance;
        var result = startPosition + offset + direction * (stepSize * index);
        return result;
    }
}
