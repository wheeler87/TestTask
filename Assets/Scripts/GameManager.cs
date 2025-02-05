using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameSettings gameSettings;
    public GameHUDView gameHUDView;

    private List<ColorInfo> _colorOptions = new List<ColorInfo>();
    private ColorInfo _validColorInfo;
    private List<ShapeView> _shapeViews = new List<ShapeView>();
    private List<UniTask> _uniTaskList = new List<UniTask>();

    private bool _interactionEnabled = false;

    private void Awake()
    {
        SpawnShapes();
        SelectColors();
        ShowShapes(_uniTaskList).Forget();

        _interactionEnabled = true;
    }

    private void SelectColors()
    {
        _colorOptions.Clear();
        _colorOptions.AddRange(gameSettings.colors);

        while ((_colorOptions.Count > gameSettings.spawnShapesCount) && (_colorOptions.Count > 0))
        {
            int index = Random.Range(0, _colorOptions.Count);
            _colorOptions.RemoveAt(index);
        }

        int validColorIndex = Random.Range(0, _colorOptions.Count);
        _validColorInfo = _colorOptions[validColorIndex];

        gameHUDView.SetValidColorInfo(_validColorInfo);

        for (int i = 0; i < _shapeViews.Count; i++)
        {
            var shapeView = _shapeViews[i];
            var colorInfo = _colorOptions[i];
            shapeView.SetColorInfo(colorInfo);
        }
    }

    private void SpawnShapes()
    {
        int shapeViewIndexOffset = Random.Range(0, gameSettings.shapes.Count);
        for (int i = 0; i < gameSettings.spawnShapesCount; i++)
        {
            int shapeViewIndex = (shapeViewIndexOffset + i) % gameSettings.shapes.Count;
            var shapeViewPrefab = gameSettings.shapes[shapeViewIndex];
            var shapeView = Instantiate(shapeViewPrefab);
            shapeView.OnShapeViewTap += OnShapeViewTap;
            _shapeViews.Add(shapeView);

            var spawnPosition = gameSettings.spawnInfo.GetSpawnPosition(i, gameSettings.spawnShapesCount);
            shapeView.Initialize(spawnPosition, gameSettings.shapesSpawnDistance, gameSettings.invalidSelectionMovementDistance);
        }
    }

    private async UniTask ShowShapes(List<UniTask> uniTaskList = null)
    {
        if (uniTaskList == null)
        {
            uniTaskList = new List<UniTask>();
        }
        else
        {
            uniTaskList.Clear();
        }

        for (int i = 0; i < _shapeViews.Count; i++)
        {
            var shapeView = _shapeViews[i];
            float delay = i * gameSettings.shapeShowDelayStep;
            var showTask = shapeView.ShowAsync(delay, gameSettings.shapeShowDuration);
            uniTaskList.Add(showTask);
        }

        await UniTask.WhenAll(uniTaskList);
    }

    private async UniTask HideShapes(int validShapeIndex, List<UniTask> uniTaskList = null)
    {
        if (uniTaskList == null)
        {
            uniTaskList = new List<UniTask>();
        }
        else
        {
            uniTaskList.Clear();
        }

        for (int i = 0; i < _shapeViews.Count; i++)
        {
            var shapeView = _shapeViews[i];
            int distance = Mathf.Abs(validShapeIndex - i);
            float delay = distance * gameSettings.shapeShowDelayStep;
            var showTask = shapeView.HideAsync(delay, gameSettings.shapeShowDuration);
            uniTaskList.Add(showTask);
        }

        await UniTask.WhenAll(uniTaskList);
    }

    private async void OnShapeViewTap(ShapeView shapeView)
    {
        if (!_interactionEnabled)
        {
            return;
        }

        _interactionEnabled = false;

        if (shapeView.ColorInfo == _validColorInfo)
        {
            int validShapeIndex = _shapeViews.IndexOf(shapeView);
            await HideShapes(validShapeIndex, _uniTaskList);
            SelectColors();
            await ShowShapes(_uniTaskList);
        }
        else
        {
            await shapeView.ShowInvalidSelection(gameSettings.invalidSelectionMovementDuration);
        }

        _interactionEnabled = true;
    }

    private void OnDrawGizmos()
    {
        float sphereRadius = 0.5f;
        for (int i = 0; i < gameSettings.spawnShapesCount; i++)
        {
            var spawnPosition = gameSettings.spawnInfo.GetSpawnPosition(i, gameSettings.spawnShapesCount);
            var spherePosition = spawnPosition + Vector3.up * sphereRadius;

            Gizmos.DrawWireSphere(spherePosition, sphereRadius);
        }
    }
}
