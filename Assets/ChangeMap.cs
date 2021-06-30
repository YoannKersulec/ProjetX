using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMap : MonoBehaviour
{
    public Transform teleportTarget;
    public GameObject player;
    public GameObject on;
    public GameObject off;
    public GameObject UI;
    public GameObject Menu;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.transform.position = teleportTarget.transform.position;
        Player.MyInstance.MyCurrentTile.position = teleportTarget.transform.position;
        off.SetActive(false);
        UI.SetActive(false);
        StartCoroutine(waiter());
    }
    IEnumerator waiter()
    {
        yield return new WaitForSeconds(4);
        UI.SetActive(true);
        Menu.SetActive(false);
        off.SetActive(true);

    }
}
