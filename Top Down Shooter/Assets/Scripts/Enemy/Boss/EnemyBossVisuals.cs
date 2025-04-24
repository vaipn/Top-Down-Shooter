using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossVisuals : MonoBehaviour
{
    private EnemyBoss enemy;

    [SerializeField] private GameObject[] batteries;
	[SerializeField] private float initialBatteryScaleY = 0.2f;

	private float dischargeSpeed;
	private float rechargeSpeed;

	private bool isRecharging;

	private void Awake()
	{
		enemy = GetComponent<EnemyBoss>();
	}
	private void Start()
	{
		ResetBatteries();
	}

	private void Update()
	{
		UpdateBatteriesScale();
	}

	private void UpdateBatteriesScale()
	{
		if (batteries.Length <= 0)
			return;

		foreach (GameObject battery in batteries)
		{
			if (battery.activeSelf)
			{
				float scaleChange = (isRecharging ? rechargeSpeed : -dischargeSpeed) * Time.deltaTime;
				float newScaleY = Mathf.Clamp(battery.transform.localScale.y + scaleChange, 0, initialBatteryScaleY);

				battery.transform.localScale = new Vector3(0.15f, newScaleY, 0.15f);

				if (battery.transform.localScale.y <= 0)
					battery.SetActive(false);
			}
		}
	}

	public void ResetBatteries()
	{
		isRecharging = true;

		rechargeSpeed = initialBatteryScaleY / enemy.abilityCooldown;
		dischargeSpeed = initialBatteryScaleY / (enemy.flameThrowDuration * 0.75f);

		foreach (GameObject battery in batteries)
			battery.SetActive(true);
	}

	public void DischargeBatteries() => isRecharging = false;
}
