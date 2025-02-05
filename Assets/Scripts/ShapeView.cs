using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ShapeView : MonoBehaviour
{

    private const string MaterialPropertyNameColor = "_Color";

    public Action<ShapeView> OnShapeViewTap;

    public ColorInfo ColorInfo { get; private set; }

    private Vector3 _spawnPosition;
    private Vector3 _showPosition;
    private Vector3 _invalidSelectionPosition;

    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _materialPropertyBlock;

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();

        _materialPropertyBlock = new MaterialPropertyBlock();
        _materialPropertyBlock.SetColor(MaterialPropertyNameColor, Color.white);
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }


    public void Initialize(Vector3 position, float spawnDistance, float invalidSelectionMovementDistance)
    {
        _showPosition = position;
        _spawnPosition = position + Vector3.up * spawnDistance;
        _invalidSelectionPosition = position + Vector3.up * invalidSelectionMovementDistance;
        transform.position = _spawnPosition;
    }

    public void SetColorInfo(ColorInfo colorInfo)
    {
        ColorInfo = colorInfo;

        _materialPropertyBlock.SetColor(MaterialPropertyNameColor, colorInfo.color);
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    public async UniTask ShowAsync(float delay, float duration)
    {
        await MoveAsync(delay, duration, _showPosition, Ease.OutBounce);
    }

    public async UniTask HideAsync(float delay, float duration)
    {
        await MoveAsync(delay, duration, _spawnPosition);
    }

    public async UniTask ShowInvalidSelection(float invalidSelectionMovementDuration)
    {
        await MoveAsync(0, invalidSelectionMovementDuration, _invalidSelectionPosition);
        await MoveAsync(0, invalidSelectionMovementDuration, _showPosition, Ease.OutBounce);
    }

    private async UniTask MoveAsync(float delay, float duration, Vector3 position, Ease? ease = null)
    {
        bool isComplete = false;
        var tween = transform.DOMove(position, duration);
        tween.SetDelay(delay);
        tween.onComplete += () =>
        {
            isComplete = true;
        };

        if (ease != null)
        {
            tween.SetEase(ease.Value);
        }

        await UniTask.WaitWhile(() => !isComplete);
    }

    private void OnMouseDown()
    {
        OnShapeViewTap?.Invoke(this);
    }
}
