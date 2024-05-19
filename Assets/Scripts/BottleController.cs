using System.Collections;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    [SerializeField] private Color[] bottleColors;
    [SerializeField] private SpriteRenderer bottleMaskSR;
    [SerializeField] private AnimationCurve scaleAndRotationMultiplierCurve;
    [SerializeField] private AnimationCurve fillAmountCurve;
    [SerializeField] private AnimationCurve rotationSpeedMultiplier;
    [SerializeField] private float timeToRotate = 1f;
    [SerializeField] private float[] fillAmounts;
    [SerializeField] private float[] rotationValues;
    [SerializeField][Range(0,4)] private int numberOfColorsInBottle = 4;
    [SerializeField] private Transform leftRotationPoint;
    [SerializeField] private Transform rightRotationPoint;
    
    private LineRenderer _lineRenderer;
    private Color _topColor;
    private int _numberOfTopColorLayers = 1;
    private BottleController _bottleControllerRef;
    private int _numberOfColorsToTransfer = 0;
    private int _rotationIndex = 0;
    private float _directionMultiplier = 1f;
    private Transform _chosenRotationPoint;
    private Vector3 _originalPosition;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private bool _isTransferring = false;
    
    public Color TopColor
    {
        get => _topColor;
        set => _topColor = value;
    }

    public BottleController BottleControllerRef
    {
        get => _bottleControllerRef;
        set => _bottleControllerRef = value;
    }

    public int NumberOfColorsInBottle
    {
        get => numberOfColorsInBottle;
        set => numberOfColorsInBottle = value;
    }

    public Color[] BottleColors
    {
        get => bottleColors;
        set => bottleColors = value;
    }

    public bool IsTransferring
    {
        get => _isTransferring;
        set => _isTransferring = value;
    }

    private void Start()
    {
        bottleMaskSR.material.SetFloat("_FA",fillAmounts[numberOfColorsInBottle]);
        _originalPosition = transform.position;
        _lineRenderer = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();
        UpdateOnShaderColor();
    }

    void UpdateOnShaderColor()
    {
        bottleMaskSR.material.SetColor("_Color01",bottleColors[0]);
        bottleMaskSR.material.SetColor("_Color02",bottleColors[1]);
        bottleMaskSR.material.SetColor("_Color03",bottleColors[2]);
        bottleMaskSR.material.SetColor("_Color04",bottleColors[3]);
        UpdateTopColorValues();
    }
    
    public void StartColorTransfer()
    {
        ChooseRotationPointAndDirection();
        _numberOfColorsToTransfer =
            Mathf.Min(_numberOfTopColorLayers, 4 - _bottleControllerRef.numberOfColorsInBottle);

        for (int index = 0; index < _numberOfColorsToTransfer; index++)
        {
            _bottleControllerRef.bottleColors[_bottleControllerRef.numberOfColorsInBottle + index] = _topColor;
        }
        
        _bottleControllerRef.UpdateOnShaderColor();
        
        CalculateRotationIndex(4 -_bottleControllerRef.numberOfColorsInBottle);

        transform.GetComponent<SpriteRenderer>().sortingOrder += 2;
        bottleMaskSR.sortingOrder += 2;
        StartCoroutine(MoveBottle());
    }

    IEnumerator RotateBottle()
    {
        float t = 0;
        float lerpValue;
        float angleValue;

        float lastAngleValue = 0;
        
        while (t < timeToRotate)
        {
            lerpValue = t / timeToRotate;
            angleValue = Mathf.Lerp(0f, _directionMultiplier * rotationValues[_rotationIndex], lerpValue);
            transform.RotateAround(_chosenRotationPoint.position,Vector3.forward, lastAngleValue - angleValue);
            bottleMaskSR.material.SetFloat("_SARM",scaleAndRotationMultiplierCurve.Evaluate(angleValue));
            if (fillAmounts[numberOfColorsInBottle] > fillAmountCurve.Evaluate(angleValue) + 0.005f)
            {
                if (_lineRenderer.enabled == false)
                {
                    _lineRenderer.startColor = _topColor;
                    _lineRenderer.endColor = _topColor;
                    _lineRenderer.SetPosition(0,_chosenRotationPoint.position);
                    _lineRenderer.SetPosition(1,_chosenRotationPoint.position - Vector3.up * 1.15f);

                    _lineRenderer.enabled = true;
                    SoundManager.Instance.WaterSound();
                }
                bottleMaskSR.material.SetFloat("_FA",fillAmountCurve
                    .Evaluate((angleValue)));
                
                _bottleControllerRef.FillUp(fillAmountCurve.Evaluate(lastAngleValue) - fillAmountCurve.Evaluate(angleValue));
            }
            t += Time.deltaTime * rotationSpeedMultiplier.Evaluate(angleValue);
            lastAngleValue = angleValue;
            yield return new WaitForEndOfFrame();
        }
        angleValue = _directionMultiplier * rotationValues[_rotationIndex];
        bottleMaskSR.material.SetFloat("_SARM",scaleAndRotationMultiplierCurve.Evaluate(angleValue));
        bottleMaskSR.material.SetFloat("_FA",fillAmountCurve
            .Evaluate((angleValue)));

        numberOfColorsInBottle -= _numberOfColorsToTransfer;
        _bottleControllerRef.numberOfColorsInBottle += _numberOfColorsToTransfer;
        
        SoundManager.Instance.StopPlaying();
        _lineRenderer.enabled = false;

        StartCoroutine(RotateBottleBack());
    }

    IEnumerator RotateBottleBack()
    {
        float t = 0;
        float lerpValue;
        float angleValue;
        float lastAngleValue = _directionMultiplier * rotationValues[_rotationIndex];
        while (t < timeToRotate)
        {
            lerpValue = t / timeToRotate;
            angleValue = Mathf.Lerp(_directionMultiplier * rotationValues[_rotationIndex], 0f, lerpValue);
            transform.RotateAround(_chosenRotationPoint.position,Vector3.forward, lastAngleValue - angleValue);
            bottleMaskSR.material.SetFloat("_SARM",scaleAndRotationMultiplierCurve.Evaluate(angleValue));
            lastAngleValue = angleValue;
            t += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        UpdateTopColorValues();
        angleValue = 0f;
        transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMaskSR.material.SetFloat("_SARM",scaleAndRotationMultiplierCurve.Evaluate(angleValue));
        
        StartCoroutine(MoveBottleBack());
    }

    IEnumerator MoveBottle()
    {
        _startPosition = transform.position;
        if (_chosenRotationPoint == leftRotationPoint)
        {
            _endPosition = _bottleControllerRef.rightRotationPoint.position;
        }
        else
        {
            _endPosition = _bottleControllerRef.leftRotationPoint.position;
        }
        _isTransferring = true;
        float t = 0;
        while (t <= 1)
        {
            transform.position = Vector3.Lerp(_startPosition, _endPosition, t);

            t += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }

        transform.position = _endPosition;
        StartCoroutine(RotateBottle());
    }
    
    IEnumerator MoveBottleBack()
    {
        _startPosition = transform.position;
        _endPosition = _originalPosition;

        float t = 0;
        while (t <= 1)
        {
            transform.position = Vector3.Lerp(_startPosition, _endPosition, t);

            t += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }

        transform.position = _endPosition;
        LevelManager.Instance.CheckLevelComplete();
        transform.GetComponent<SpriteRenderer>().sortingOrder -= 2;
        bottleMaskSR.sortingOrder -= 2;
        _isTransferring = false;
    }

    public void UpdateTopColorValues()
    {
        if (numberOfColorsInBottle != 0)
        {
            _numberOfTopColorLayers = 1;
            _topColor = bottleColors[numberOfColorsInBottle - 1];

            if (numberOfColorsInBottle == 4)
            {
                if (bottleColors[3].Equals(bottleColors[2]))
                {
                    _numberOfTopColorLayers = 2;
                    if (bottleColors[2].Equals(bottleColors[1]))
                    {
                        _numberOfTopColorLayers = 3;
                        if (bottleColors[1].Equals(bottleColors[0]))
                        {
                            _numberOfTopColorLayers = 4;
                        }
                    }
                }
            }
            else if (numberOfColorsInBottle == 3)
            {
                if (bottleColors[2].Equals(bottleColors[1]))
                {
                    _numberOfTopColorLayers = 2;
                    if (bottleColors[1].Equals(bottleColors[0]))
                    {
                        _numberOfTopColorLayers = 3;
                    }
                }
            }
            else if (numberOfColorsInBottle == 2)
            {
                if (bottleColors[1].Equals(bottleColors[0]))
                {
                    _numberOfTopColorLayers = 2;
                }
            }
            _rotationIndex = 3 - (numberOfColorsInBottle - _numberOfTopColorLayers);
        }
    }

    public bool FillBottleCheck(Color colorToCheck)
    {
        if (numberOfColorsInBottle == 0) return true;
        else
        {
            if (numberOfColorsInBottle == 4) return false;
            else
            {
                if (_topColor.Equals(colorToCheck)) return true;
                return false;
            }
        }
    }

    public bool CheckBottleColorIsFull()
    {
        int count = 0;
        if (numberOfColorsInBottle == 4)
        {
            for (int index = bottleColors.Length - 1; index >= 0; index--)
            {
                if (bottleColors[index].Equals(bottleColors[index]))
                {
                    count++;
                }
            }
        }

        if (count == numberOfColorsInBottle)
        {
            return true;
        }

        return false;
    }

    void CalculateRotationIndex(int numberOfEmptySpaceInSecondBottle)
    {
        _rotationIndex = 3 - (numberOfColorsInBottle -
                              Mathf.Min(numberOfEmptySpaceInSecondBottle, _numberOfTopColorLayers));
    }

    void FillUp(float fillAmountToAdd)
    {
        bottleMaskSR.material.SetFloat("_FA",bottleMaskSR.material.GetFloat("_FA") + fillAmountToAdd);
    }

    void ChooseRotationPointAndDirection()
    {
        if (transform.position.x > _bottleControllerRef.transform.position.x)
        {
            _chosenRotationPoint = rightRotationPoint;
            _directionMultiplier = -1f;
        }
        else
        {
            _chosenRotationPoint = leftRotationPoint;  
            _directionMultiplier = 1f;
        }
    }
}
