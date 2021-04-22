using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stat : MonoBehaviour
{

    private Image content;

    [SerializeField]
    private Text statValue;

    private float currentFill;

    private float overflow;

    [SerializeField]
    private float lerpSpeed;

    public float MyMaxValue { get; set; }

    private float currentValue;

    public bool IsFull
    {
        get { return content.fillAmount == 1; }
    }

    public float MyOverflow
    {
        get
        {
            float tmp = overflow;
            overflow = 0;
            return tmp;
        }
    }

    public float MyCurrentValue
    {
        get
        {

            return currentValue;
        }

        set
        {
            if (value > MyMaxValue)
            {
                overflow = value - MyMaxValue;
                currentValue = MyMaxValue;
            }
            else if (value < 0)
            {
                currentValue = 0;
            }
            else 
            {
                currentValue = value;
            }

            currentFill = currentValue / MyMaxValue;

            if (statValue != null)
            {
                statValue.text = currentValue + " / " + MyMaxValue;
            }

        }
    }




    void Start()
    {
        content = GetComponent<Image>();
    }

    void Update()
    {

        HandleBar();
    }

    public void Initialize(float currentValue, float maxValue)
    {
        if (content == null)
        {
            content = GetComponent<Image>();
        }

        MyMaxValue = maxValue;
        MyCurrentValue = currentValue;
        content.fillAmount = MyCurrentValue / MyMaxValue;
    }


    private void HandleBar()
    {
        if (currentFill != content.fillAmount)
        {
            content.fillAmount = Mathf.MoveTowards(content.fillAmount, currentFill, Time.deltaTime * lerpSpeed);
        }

    }

    public void Reset()
    {
        content.fillAmount = 0;
    }
}
