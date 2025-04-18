using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitRoomHandling : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        GenerationManager gm = FindAnyObjectByType<GenerationManager>();

        gm.WinGame();
    }
}
