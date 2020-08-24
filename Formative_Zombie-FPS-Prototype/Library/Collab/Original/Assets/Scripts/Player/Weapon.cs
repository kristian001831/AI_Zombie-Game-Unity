using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Weapon : MonoBehaviour
{
	[Header("Gun Attributes")]
	[SerializeField] private string weaponName;
	[SerializeField] private float damage = 17f;
	[SerializeField] private float fireRate = 0.1f;
	[SerializeField] private float range = 100f;
	[SerializeField] private int bulletsPerMag = 30;
	[SerializeField] private int bulletsLeft = 45;
	[SerializeField] private int startBullets = 45;
	[SerializeField] private int loadedBullets;
	[SerializeField] private int pellets = 1;
	[SerializeField] private float spread;
	[SerializeField] private float bulletEjectingSpeed = 5.0f;
	[SerializeField] private bool hasLastFire = false;
	[SerializeField] private float recoil = 0f;

	[Header("Animation Attributes")]
	[SerializeField] private Vector3 aimPos;
	[SerializeField] private float aimingSpeed = 8f;

	[Header("Sounds")]
	[SerializeField] private AudioClip drySound;
	[SerializeField] private AudioClip gunFireSound;


	[Header("External Refs")]
	[SerializeField] private GameObject hitParticle;
	[SerializeField] private GameObject bulletImpact;
	//public GameObject gunSmoke;
	[SerializeField] private GameObject emptyCase;
	//public SoundManager soundManager;
	
	[Header("Internal Refs")]
	[SerializeField] private Transform shootPoint;
	[SerializeField] private Transform muzzlePoint;
	[SerializeField] private Transform caseSpawnPoint;
	[SerializeField] private ParticleSystem muzzleflash;
	
	private Animator animator;
	private float fireTimer;
	private bool isReloading = false;
	private Vector3 originalPos;

	void Start() 
	{
		animator = GetComponent<Animator>();

		InitAmmo();

		originalPos = transform.localPosition;
	}

	void Update()
	{
		if(Input.GetButtonDown("Fire1") && !isReloading)
		{
			Fire();
		}
	
		if(Input.GetKeyDown(KeyCode.R))
		{
			StartReloading();
		}
		
		if(fireTimer < fireRate)
		{
			fireTimer += Time.deltaTime;
		}
		
		float reloadSpeed = 1;
	}

	void DrawHitRay() 
	{
		Debug.DrawRay(shootPoint.position, CalculateSpread(spread, shootPoint), Color.green, 10.0f);
	}

	Vector3 CalculateSpread(float inaccuracy, Transform trans)
	{
		if(Input.GetButton("Fire2")) inaccuracy /= 2;

		return Vector3.Lerp(trans.TransformDirection(Vector3.forward * range), Random.onUnitSphere, inaccuracy);
	}

	IEnumerator DisableFire(float time = 0.3f) //TODO useless yet
	{
		yield return new WaitForSeconds(time);
		yield break;
	}

	float GetWeaponDamage() 
	{
		return damage;
	}

	void Fire() 
	{
		if(fireTimer < fireRate) return;

		if(loadedBullets <= 0)
		{
			// When Ammo is out, make fire is not working for a moment
			StartCoroutine(DisableFire());
			//soundManager.Play(drySound);
			return;
		}
		
		RaycastHit hit;

		for(int i = 0; i < pellets; i++)
		{
			if(Physics.Raycast(shootPoint.position, CalculateSpread(spread, shootPoint), out hit, range))
			{
				HealthManager health = hit.transform.GetComponent<HealthManager>();

				if(health) 
				{
					health.ApplyDamage(GetWeaponDamage());
				}
				else 
				{
					CreateRicochet(hit.point, hit.normal);

					GameObject bulletHole = Instantiate(bulletImpact, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
					bulletHole.transform.SetParent(hit.transform);
					Destroy(bulletHole, 10f);
				}
			}
		}

		if(hasLastFire && loadedBullets <= 1)
		{
			animator.CrossFadeInFixedTime("FPSHand|FireLast", 0.01f);
		}
		else
		{
			animator.CrossFadeInFixedTime("FPSHand|Fire", 0.01f);
		}
		
		//GameObject gunSmokeEffect = Instantiate(gunSmoke, muzzlePoint.position, muzzlePoint.rotation);
		//Destroy(gunSmokeEffect, 5f);

		muzzleflash.Play();
		//soundManager.Play(gunFireSound);

		loadedBullets--;

		fireTimer = 0.0f;
	}

	void CreateRicochet(Vector3 pos, Vector3 normal)
	{
		GameObject hitParticleEffect = Instantiate(hitParticle, pos, Quaternion.FromToRotation(Vector3.up, normal));
		Destroy(hitParticleEffect, 1f);
	}

	void StartReloading()
	{
		if(isReloading || loadedBullets >= bulletsPerMag || bulletsLeft <= 0) return;
		
		isReloading = true;
		
		animator.CrossFadeInFixedTime("FPSHand|Reload", 0.01f);
	}

	void RefillAmmunitions()
	{
		int bulletsToLoad = bulletsPerMag - loadedBullets;
		int bulletsToDeduct = bulletsLeft >= bulletsToLoad ? bulletsToLoad : bulletsLeft;
		
		bulletsLeft -= bulletsToDeduct;
		loadedBullets += bulletsToDeduct;

		if(hasLastFire)
		{
			animator.SetBool("IsEmpty", false);	
		}
	}

	public void Draw()
	{
		StartCoroutine(PrepareWeapon());
	}

	public void InitAmmo()
	{
		bulletsLeft = startBullets;
		loadedBullets = bulletsPerMag;
	}

	IEnumerator PrepareWeapon() 
	{
		yield return new WaitForEndOfFrame();

		animator.Play("FPSHand|Draw");

		if(hasLastFire && loadedBullets <= 0) 
		{
			animator.SetBool("IsEmpty", true);
		}

		yield break;
	}

	public void Unload()
	{
		isReloading = false;

		gameObject.SetActive(false);
	}

	void OnCaseOut()
	{
		GameObject ejectedCase = Instantiate(emptyCase, caseSpawnPoint.position, caseSpawnPoint.rotation);
		Rigidbody caseRigidbody = ejectedCase.GetComponent<Rigidbody>();
		caseRigidbody.velocity = caseSpawnPoint.TransformDirection(-Vector3.left * bulletEjectingSpeed);
		caseRigidbody.AddTorque(Random.Range(-0.2f, 0.2f), Random.Range(0.1f, 0.2f), Random.Range(-0.2f, 0.2f));
		caseRigidbody.AddForce(0, Random.Range(2.0f, 4.0f), 0, ForceMode.Impulse);

		Destroy(ejectedCase, 10f);
	}

	void OnMagIn()
	{
		RefillAmmunitions();
	}

	void OnAmmoInsertion()
	{
		isReloading = false; // Make gun fire is possible
		bulletsLeft--;
		loadedBullets++;

		if(hasLastFire)
		{
			animator.SetBool("IsEmpty", false);	
		}
	}

	void OnFirstAmmoInsert() 
	{
		if(bulletsLeft <= 0)
		{
			animator.CrossFadeInFixedTime("FPSHand|Stand", 0.01f);
		}
		else
		{
			animator.CrossFadeInFixedTime("FPSHand|ReloadStart", 0.01f);
		}
	}

	void OnBeforeInsert()
	{
		isReloading = true;
	}

	void OnAfterInsert()
	{
		if(loadedBullets >= bulletsPerMag)
		{
			animator.CrossFadeInFixedTime("FPSHand|ReloadEnd", 0.01f);
		}
		else if(bulletsLeft <= 0) 
		{
			animator.CrossFadeInFixedTime("FPSHand|ReloadEnd", 0.01f);
		}
		else 
		{
			animator.CrossFadeInFixedTime("FPSHand|ReloadInsert", 0.01f);
		}
	}

	void OnReloadEnd()
	{
		isReloading = false;
	}
}
