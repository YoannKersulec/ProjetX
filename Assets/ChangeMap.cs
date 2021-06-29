using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMap : MonoBehaviour
{
    public Transform teleportTarget;
    public GameObject player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.transform.position = teleportTarget.transform.position;
    }
}
