using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {

	public int bulletsPerMag = 30;
	public float range = 100f;
	public int bulletsLeft;
	public int currentBullets;
	public float fireRate = 0.1f;
	public Transform shootPoint;
	public ParticleSystem muzzleFlash;
	private float fireTimer;
	private Animator anim;
	private AudioSource _AudioSource;
	private bool isReloading;
	public AudioClip shootSound;
	public GameObject hitParticles;
	public GameObject bulletImpact;
	public enum ShootMode { Auto , Semi}
	public ShootMode shootingMode;
	private bool shootInput;
	public float damage = 20f;
	private Vector3 originalPosition;
	public Vector3 aimPosition;
	public float aodSpeed;
	private bool isAiming;
	// Use this for initialization
	void Start () {
		currentBullets = bulletsPerMag;
		anim = GetComponent<Animator> ();
		_AudioSource = GetComponent<AudioSource> ();
		originalPosition = transform.localPosition;

	}
	
	// Update is called once per frame
	void Update () {
		switch (shootingMode) {

		case ShootMode.Auto:
			shootInput = Input.GetButton ("Fire1");
			break;
		case ShootMode.Semi:
			shootInput = Input.GetButtonDown ("Fire1");
			break;


	}
		AimDownSights ();
		if(shootInput){
			if (currentBullets > 0)
				Fire ();
			else if(bulletsLeft > 0)
				DoReload ();
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			if (currentBullets < bulletsPerMag && bulletsLeft > 0)
				DoReload ();
		}
		if (fireTimer < fireRate) {
			fireTimer += Time.deltaTime;
		}
	}
	void FixedUpdate()
	{
		AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo (0);
		if (info.IsName ("Fire"))
			anim.SetBool ("Fire", false);
		isReloading = info.IsName ("Reload");
		anim.SetBool ("Aim", isAiming);
	}

	private void Fire ()
	{
		if (fireTimer < fireRate || currentBullets<=0 || isReloading) return;
			
		RaycastHit hit;
		if (Physics.Raycast (shootPoint.position, shootPoint.transform.forward, out hit,range)) {
			Debug.Log(hit.transform.name + " fired!");
			GameObject hitParticleEffect = Instantiate (hitParticles, hit.point, Quaternion.FromToRotation (Vector3.up, hit.normal)); 
			GameObject bulletHole = Instantiate (bulletImpact, hit.point, Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			Destroy (hitParticleEffect, 1f);
			Destroy (bulletHole, 5f);
			if (hit.transform.GetComponent<EnemyHealth> ()) 
			{
//				hit.transform.GetComponent<EnemyHealth> ().ApplyDamage (damage);
			}

		}
		anim.CrossFadeInFixedTime ("Fire",.1f);
		muzzleFlash.Play ();
		PlayShootSound ();
		//anim.SetBool ("Fire",true);
		fireTimer = 0.0f;
		currentBullets--;
	
	}

	private void PlayShootSound()
	{
		_AudioSource.PlayOneShot (shootSound);
		//_AudioSource.clip = shootSound;
		//_AudioSource.Play ();
	}
	public void Reload()
	{
		if (bulletsLeft <= 0)
			return;
		int bulletsToLoad = bulletsPerMag - currentBullets;
		int bulletsToDeduct = (bulletsLeft >= bulletsToLoad) ? bulletsToLoad : bulletsLeft;
		bulletsLeft -= bulletsToDeduct;
		currentBullets += bulletsToDeduct;
	}
	private void DoReload()
	{
		//AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo (0);
		if (isReloading )
			return;
		anim.CrossFadeInFixedTime ("Reload" , 0.01f);
	}
	private void AimDownSights()
	{
		if (Input.GetButton ("Fire2") && !isReloading) {
			transform.localPosition = Vector3.Lerp (transform.localPosition, aimPosition, Time.deltaTime * aodSpeed);	
			isAiming = true;
		} else {
			transform.localPosition = Vector3.Lerp (transform.localPosition, originalPosition, Time.deltaTime * aodSpeed);
			isAiming = false;
		}
	}
}
