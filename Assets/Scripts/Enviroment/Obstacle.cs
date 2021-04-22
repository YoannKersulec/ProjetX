using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public SpriteRenderer MySpriteRenderer { get; set; }

    private Color defaultColor;

    private Color fadedColor;

    void Start()
    {
        MySpriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = MySpriteRenderer.color;
        fadedColor = defaultColor;
        fadedColor.a = 0.7f;
    }

    public void FadeOut()
    {
        MySpriteRenderer.color = fadedColor;
    }

    public void FadeIn()
    {
        MySpriteRenderer.color = defaultColor;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "WallHack")
        {
            FadeOut();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "WallHack")
        {
            FadeIn();
        }

    }

}
