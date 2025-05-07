using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<Transform> levelParts;
    private List<Transform> currentLevelParts;
    [SerializeField] private Transform lastLevelPart;
    [SerializeField] private SnapPoint nextSnapPoint;
    [Space]
    [SerializeField] private float generationCooldown;
    private float cooldownTimer;

    private bool generationOver;

	private void Start()
	{
        currentLevelParts = new List<Transform>(levelParts);

		GenerateNextLevelPart();
	}

	private void Update()
	{
        if (generationOver)
            return;


		cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0)
        {   
            if (currentLevelParts.Count > 0)
            {
                cooldownTimer = generationCooldown;
                GenerateNextLevelPart();
            }
            else if (!generationOver)
            {
                FinishGeneration();
            }
        }
	}

	private void FinishGeneration()
	{
		generationOver = true;

        Transform levelPart = Instantiate(lastLevelPart);
        LevelPart levelPartScript = levelPart.GetComponent<LevelPart>();

        levelPartScript.SnapAndAlignPartTo(nextSnapPoint);
	}

	[ContextMenu("Create next level part")]

    private void GenerateNextLevelPart()
    {
        Transform newPart = Instantiate(ChooseRandomPart());
        LevelPart levelPartScript = newPart.GetComponent<LevelPart>();

        levelPartScript.SnapAndAlignPartTo(nextSnapPoint);

        if (levelPartScript.IntersectionDetected())
            Debug.LogWarning("Intersection between level parts detected");

        nextSnapPoint = levelPartScript.GetExitPoint();
    }
    private Transform ChooseRandomPart()
    {
        int randomIndex = Random.Range(0, currentLevelParts.Count);

        Transform chosenPart = currentLevelParts[randomIndex];

        currentLevelParts.RemoveAt(randomIndex);

        return chosenPart;
    }
}
