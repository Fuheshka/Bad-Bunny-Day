using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Farmer : MonoBehaviour
{
	private enum State
	{
		Patrol = 0,
		ReactToNoise = 1,
		Tracking = 2,
		SearchingLastSeen = 3
	}

	private GameObject debugRing;

	[Header("References")]
	public Transform aimMarker;

	public PlayerController player;

	[SerializeField]
	private Image _fullShootIndicator;

	[SerializeField]
	private Image _firstShootIndicator;

	[SerializeField]
	private Image _shootIndicators;

	[Header("Audio Sources (ВАЖНО)")]
	[Tooltip("Для коротких звуков (Выстрел, Удар, Крик). Loop = false")]
	[SerializeField]
	private AudioSource _sfxSource;

	[Tooltip("Для длительных звуков (Поиск/Бормотание). Loop = true")]
	[SerializeField]
	private AudioSource _loopSource;

	[Header("Audio Clips & Volume")]
	[SerializeField]
	private AudioClip[] _audioFireClips;

	[Range(0f, 1f)]
	public float fireVolume = 0.5f;

	[SerializeField]
	private AudioClip[] _audioReactToNoiseClips;

	[Range(0f, 1f)]
	public float reactVolume = 0.7f;

	[SerializeField]
	private AudioClip[] _audioSearchingClips;

	[Range(0f, 1f)]
	public float searchVolume = 0.3f;

	[Tooltip("Звук, когда перестал искать (Эх... Показалось...)")]
	[SerializeField]
	private AudioClip[] _audioGiveUpClips;

	[Range(0f, 1f)]
	public float giveUpVolume = 0.5f;

	[SerializeField]
	private AudioClip[] _audioDodgeClips;

	[Range(0f, 1f)]
	public float dodgeVolume = 0.6f;

	[SerializeField]
	private AudioClip[] _audioDamageRabbitClips;

	[Range(0f, 1f)]
	public float damageVolume = 0.8f;

	[Header("Layers")]
	public LayerMask obstacleMask = 256;

	public LayerMask playerMask = 512;

	public LayerMask groundMask = 1;

	[Header("Patrol Rectangle Area")]
	public Vector3 patrolRectCenter = Vector3.zero;

	public Vector2 patrolRectHalfSize = new Vector2(20f, 15f);

	[Header("Patrol")]
	public Vector3 patrolAreaCenter = Vector3.zero;

	public float patrolAreaRadius = 45f;

	public float patrolSpeed = 3f;

	public float patrolPause = 2f;

	[Header("Detection")]
	public float detectionTimeNeeded = 1.5f;

	public float checkInterval = 0.1f;

	public float detectionRadius = 8f;

	[Header("Search")]
	public float searchDuration = 5f;

	public float searchRadius = 6f;

	public float searchMoveSpeed = 4f;

	public float searchPointInterval = 1.2f;

	[Header("Tracking & Attack")]
	public float trackingSpeed = 9f;

	public float fireCooldown = 2f;

	public GameObject muzzleFlash;

	[Header("Accuracy & Dodge")]
	public float chargeTime = 0.5f;

	public float lockTime = 0.3f;

	public float shotHitRadius = 2.46f;

	private float timer;

	private State currentState;

	private float detectionTimer;

	private bool canFire = true;

	private bool isShooting;

	private Vector3 lastKnownPlayerPos;

	private float searchEndTime;

	private Coroutine moveCoroutine;

	private Coroutine patrolCoroutine;

	private Coroutine searchCoroutine;

	private bool _isSeePlayer;

	private bool isReady;

	private const float GROUND_Y = -0.046f;

	private float _sfxVolumeMultiplier = 1f;

	private void Start()
	{
		if (_sfxSource == null)
		{
			Debug.LogError("Не назначен SFX AudioSource!");
		}
		if (_loopSource == null)
		{
			Debug.LogError("Не назначен Loop AudioSource!");
		}
		if (player != null)
		{
			player.onNoise.AddListener(ReactToNoise);
		}
		if (aimMarker != null)
		{
			aimMarker.position = new Vector3(base.transform.position.x, -0.046f, base.transform.position.z);
		}
		patrolCoroutine = StartCoroutine(PatrolLoop());
		InvokeRepeating("CheckLineOfSight", 0.05f, checkInterval);
		StartCoroutine(WarmupRoutine());
		if (SettingsManager.Instance != null)
		{
			_sfxVolumeMultiplier = SettingsManager.Instance.EffectiveSFXVolume;
		}
	}

	private void OnEnable()
	{
		SettingsManager.OnSFXVolumeChanged += OnSFXVolumeSettingChanged;
		SettingsManager.OnMasterVolumeChanged += OnMasterVolumeSettingChanged;
	}

	private void OnDisable()
	{
		SettingsManager.OnSFXVolumeChanged -= OnSFXVolumeSettingChanged;
		SettingsManager.OnMasterVolumeChanged -= OnMasterVolumeSettingChanged;
	}

	private void OnSFXVolumeSettingChanged(float value)
	{
		if (SettingsManager.Instance != null)
		{
			_sfxVolumeMultiplier = SettingsManager.Instance.EffectiveSFXVolume;
		}
	}

	private void OnMasterVolumeSettingChanged(float value)
	{
		if (SettingsManager.Instance != null)
		{
			_sfxVolumeMultiplier = SettingsManager.Instance.EffectiveSFXVolume;
		}
	}

	private IEnumerator WarmupRoutine()
	{
		isReady = false;
		yield return new WaitForSeconds(2f);
		isReady = true;
	}

	private void Update()
	{
		if (player == null || !player.IsAlive())
		{
			_isSeePlayer = false;
			StopLoopingAudio();
			return;
		}
		if (Vector3.Distance(GetPlayerGroundPos(), aimMarker.position) > detectionRadius)
		{
			_isSeePlayer = false;
			return;
		}
		Vector3 vector = base.transform.position + Vector3.up * 0.5f;
		Vector3 vector2 = player.transform.position + Vector3.up * 0.5f;
		Vector3 normalized = (vector2 - vector).normalized;
		float maxDistance = Vector3.Distance(vector, vector2);
		if (Physics.Raycast(vector, normalized, out var _, maxDistance, obstacleMask))
		{
			_isSeePlayer = false;
		}
		else
		{
			_isSeePlayer = true;
		}
	}

	private void PlaySFX(AudioClip[] clips, float volumeScale)
	{
		if (clips != null && clips.Length != 0 && !(_sfxSource == null))
		{
			int num = Random.Range(0, clips.Length);
			_sfxSource.pitch = Random.Range(0.95f, 1.05f);
			_sfxSource.PlayOneShot(clips[num], volumeScale * _sfxVolumeMultiplier);
		}
	}

	private void PlayLoopingAudio(AudioClip[] clips, float volume)
	{
		if (clips != null && clips.Length != 0 && !(_loopSource == null) && (!_loopSource.isPlaying || !_loopSource.loop))
		{
			int num = Random.Range(0, clips.Length);
			_loopSource.Stop();
			_loopSource.clip = clips[num];
			_loopSource.volume = volume * _sfxVolumeMultiplier;
			_loopSource.loop = true;
			_loopSource.pitch = 1f;
			_loopSource.Play();
		}
	}

	private void StopLoopingAudio()
	{
		if (_loopSource != null && _loopSource.isPlaying)
		{
			_loopSource.Stop();
			_loopSource.loop = false;
			_loopSource.clip = null;
		}
	}

	private void CheckLineOfSight()
	{
		if (!isReady)
		{
			return;
		}
		if (currentState == State.Tracking && !isShooting)
		{
			RequestMoveTo(GetPlayerGroundPos(), trackingSpeed);
		}
		if (_isSeePlayer)
		{
			StopLoopingAudio();
			if (!isShooting)
			{
				detectionTimer += checkInterval;
				lastKnownPlayerPos = GetPlayerGroundPos();
				if (currentState != State.Tracking)
				{
					EnterTracking();
					RequestMoveTo(lastKnownPlayerPos, trackingSpeed);
				}
				if (detectionTimer >= detectionTimeNeeded && canFire)
				{
					StartCoroutine(FireSequence());
				}
			}
		}
		else if (!isShooting)
		{
			ChangeShootTimer(0f);
			detectionTimer = Mathf.Max(0f, detectionTimer - checkInterval * 3f);
			if (currentState == State.Tracking && detectionTimer <= 0f)
			{
				EnterSearchingLastSeen();
			}
		}
	}

	private IEnumerator FireSequence()
	{
		StopLoopingAudio();
		canFire = false;
		isShooting = true;
		detectionTimer = 0f;
		Vector3 lastValidPlayerPos = GetPlayerGroundPos();
		timer = 0f;
		while (timer < chargeTime)
		{
			if (player != null)
			{
				lastValidPlayerPos = GetPlayerGroundPos();
			}
			if (aimMarker != null)
			{
				aimMarker.position = lastValidPlayerPos;
			}
			timer += Time.deltaTime;
			ChangeShootTimer(timer);
			yield return null;
		}
		if (moveCoroutine != null)
		{
			StopCoroutine(moveCoroutine);
		}
		Debug.Log("<color=yellow>LOCK!</color>");
		DrawDebugRing(aimMarker.position, shotHitRadius);
		yield return new WaitForSeconds(lockTime);
		if (debugRing != null)
		{
			debugRing.SetActive(value: false);
		}
		if ((bool)muzzleFlash)
		{
			muzzleFlash.SetActive(value: true);
		}
		PlaySFX(_audioFireClips, fireVolume);
		ChangeShootTimer(0f);
		Vector3 b = ((player != null && player.IsAlive()) ? GetPlayerGroundPos() : lastValidPlayerPos);
		if (Vector3.Distance(aimMarker.position, b) <= shotHitRadius)
		{
			Debug.Log("<color=red>ПОПАДАНИЕ!</color>");
			if (player != null && player.IsAlive())
			{
				player.TakeDamage(1, base.transform.position);
			}
			PlaySFX(_audioDamageRabbitClips, damageVolume);
		}
		else
		{
			Debug.Log("<color=green>УВОРОТ!</color>");
			PlaySFX(_audioDodgeClips, dodgeVolume);
		}
		yield return new WaitForSeconds(0.15f);
		if ((bool)muzzleFlash)
		{
			muzzleFlash.SetActive(value: false);
		}
		isShooting = false;
		yield return new WaitForSeconds(fireCooldown);
		canFire = true;
	}

	private void ChangeShootTimer(float currentTimer)
	{
		if (currentTimer < 0.3f)
		{
			if (currentTimer < 0.1f)
			{
				if (currentTimer == 0f)
				{
					_fullShootIndicator.enabled = false;
					_firstShootIndicator.enabled = false;
					_shootIndicators.enabled = false;
				}
				else
				{
					_firstShootIndicator.enabled = true;
					_shootIndicators.fillAmount = 0.25f;
				}
			}
			else if (currentTimer < 0.2f)
			{
				_shootIndicators.enabled = true;
				_shootIndicators.fillAmount = 0.5f;
			}
			else
			{
				_shootIndicators.fillAmount = 0.75f;
			}
		}
		else if (!(currentTimer < 0.4f))
		{
			if (currentTimer == 5f)
			{
				_fullShootIndicator.enabled = true;
				_firstShootIndicator.enabled = false;
				_shootIndicators.enabled = false;
			}
		}
		else
		{
			_shootIndicators.fillAmount = 1f;
		}
	}

	private void ReactToNoise(Vector3 pos)
	{
		if (isReady && !isShooting)
		{
			StopLoopingAudio();
			PlaySFX(_audioReactToNoiseClips, reactVolume);
			Vector3 pos2 = new Vector3(pos.x, -0.046f, pos.z);
			EnterReactToNoise(pos2);
		}
	}

	private void EnterReactToNoise(Vector3 pos)
	{
		currentState = State.ReactToNoise;
		StopAllStateCoroutines();
		RequestMoveTo(pos, 28f);
		StartCoroutine(DelayedPatrol(2f));
	}

	private void EnterTracking()
	{
		currentState = State.Tracking;
		StopLoopingAudio();
		StopAllStateCoroutines();
	}

	private void EnterSearchingLastSeen()
	{
		currentState = State.SearchingLastSeen;
		PlayLoopingAudio(_audioSearchingClips, searchVolume);
		StopAllStateCoroutines();
		lastKnownPlayerPos = GetPlayerGroundPos();
		searchEndTime = Time.time + searchDuration;
		searchCoroutine = StartCoroutine(SearchRoutine());
	}

	private Vector3 GetRandomPatrolPointRect()
	{
		float num = Random.Range(0f - patrolRectHalfSize.x, patrolRectHalfSize.x);
		float num2 = Random.Range(0f - patrolRectHalfSize.y, patrolRectHalfSize.y);
		return new Vector3(patrolRectCenter.x + num, -0.046f, patrolRectCenter.z + num2);
	}

	private void EnterPatrol()
	{
		if (currentState == State.SearchingLastSeen)
		{
			PlaySFX(_audioGiveUpClips, giveUpVolume);
		}
		Debug.Log("Поиск окончен. Переход в патруль.");
		currentState = State.Patrol;
		StopLoopingAudio();
		StopAllStateCoroutines();
		patrolCoroutine = StartCoroutine(PatrolLoop());
	}

	private IEnumerator SearchRoutine()
	{
		while (Time.time < searchEndTime && currentState == State.SearchingLastSeen)
		{
			Vector3 target = lastKnownPlayerPos + Random.insideUnitSphere * searchRadius;
			target.y = -0.046f;
			RequestMoveTo(target, searchMoveSpeed);
			yield return new WaitForSeconds(searchPointInterval);
		}
		EnterPatrol();
	}

	private IEnumerator PatrolLoop()
	{
		while (currentState == State.Patrol)
		{
			Vector3 randomPatrolPointRect = GetRandomPatrolPointRect();
			RequestMoveTo(randomPatrolPointRect, patrolSpeed);
			yield return new WaitForSeconds(patrolPause + Random.Range(-0.5f, 1f));
		}
	}

	private IEnumerator DelayedPatrol(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (currentState == State.ReactToNoise)
		{
			EnterPatrol();
		}
	}

	private void RequestMoveTo(Vector3 target, float speed)
	{
		if (moveCoroutine != null)
		{
			StopCoroutine(moveCoroutine);
		}
		moveCoroutine = StartCoroutine(MoveMarker(target, speed));
	}

	private IEnumerator MoveMarker(Vector3 target, float speed)
	{
		target.x = Mathf.Clamp(target.x, patrolRectCenter.x - patrolRectHalfSize.x, patrolRectCenter.x + patrolRectHalfSize.x);
		target.z = Mathf.Clamp(target.z, patrolRectCenter.z - patrolRectHalfSize.y, patrolRectCenter.z + patrolRectHalfSize.y);
		target.y = -0.046f;
		Vector3 start = aimMarker.position;
		float distance = Vector3.Distance(start, target);
		if (!(distance < 0.1f))
		{
			float journey = 0f;
			while (journey < 1f)
			{
				journey += Time.deltaTime * speed / distance;
				aimMarker.position = Vector3.Lerp(start, target, journey);
				yield return null;
			}
			aimMarker.position = target;
		}
	}

	private Vector3 GetPlayerGroundPos()
	{
		if (player == null)
		{
			return base.transform.position;
		}
		return new Vector3(player.transform.position.x, -0.046f, player.transform.position.z);
	}

	private void StopAllStateCoroutines()
	{
		if (moveCoroutine != null)
		{
			StopCoroutine(moveCoroutine);
			moveCoroutine = null;
		}
		if (patrolCoroutine != null)
		{
			StopCoroutine(patrolCoroutine);
			patrolCoroutine = null;
		}
		if (searchCoroutine != null)
		{
			StopCoroutine(searchCoroutine);
			searchCoroutine = null;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (aimMarker != null)
		{
			Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
			Gizmos.DrawSphere(aimMarker.position, detectionRadius);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(aimMarker.position, shotHitRadius);
			Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
			Gizmos.DrawWireCube(new Vector3(patrolRectCenter.x, -0.046f, patrolRectCenter.z), new Vector3(patrolRectHalfSize.x * 2f, 0.1f, patrolRectHalfSize.y * 2f));
		}
	}

	private void DrawDebugRing(Vector3 pos, float radius)
	{
		if (debugRing == null)
		{
			debugRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			Object.Destroy(debugRing.GetComponent<Collider>());
			debugRing.transform.localScale = new Vector3(radius * 2f, 0.1f, radius * 2f);
			debugRing.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f, 0.5f);
		}
		debugRing.SetActive(value: true);
		debugRing.transform.position = pos;
	}

	private void OnDestroy()
	{
		if (player != null)
		{
			player.onNoise.RemoveListener(ReactToNoise);
		}
	}
}
