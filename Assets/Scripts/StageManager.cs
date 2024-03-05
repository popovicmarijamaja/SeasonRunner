using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class StageManager : NetworkBehaviour
{
    public Transform leftPos;
    public Transform rightPos;
    public Transform centrePos;
    public Transform environmentSpawnPos;
    public SpawnManager spawnManager;
    public TextMeshProUGUI scoreText;
    public Slider HealthSlider;
    public BoxCollider CollectibleCollider;

    public int StageID;

    private void Awake()
    {
        GameObject[] stages = GameObject.FindGameObjectsWithTag("stage");
        StageID = stages.Length;
        print("stageov id " + StageID);
    }
}
