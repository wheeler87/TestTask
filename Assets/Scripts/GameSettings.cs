using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(GameSettings), menuName = "Game/" + nameof(GameSettings))]
public class GameSettings : ScriptableObject
{
    public List<ColorInfo> colors;
    public List<ShapeView> shapes;
    public int spawnShapesCount;
    public SpawnInfo spawnInfo;

    public float shapesSpawnDistance;
    public float shapeShowDelayStep;
    public float shapeShowDuration;
    public float invalidSelectionMovementDistance;
    public float invalidSelectionMovementDuration;
}
