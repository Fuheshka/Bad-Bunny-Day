using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Farmer : MonoBehaviour
{
    private GameObject debugRing;
    [Header("References")]
    public Transform aimMarker;
    public PlayerController player;
    [SerializeField] private Image _shootIndicator;

    [Header("Shoot Indicator")]
    [Tooltip("Количество делений (сегментов) в спрайте индикатора.\n" +
             "Если > 1, fillAmount будет квантоваться по делениям (0..1 шагами 1/N), " +
             "и новое деление будет появляться каждые chargeTime / N секунд.")]
    [SerializeField, Min(0)] private int _shootIndicatorDivisions = 0;

    [Header("Audio Sources (ВАЖНО)")]
    [Tooltip("Для коротких звуков (Выстрел, Удар, Крик). Loop = false")]
    [SerializeField] private AudioSource _sfxSource;

    [Tooltip("Для длительных звуков (Поиск/Бормотание). Loop = true")]
    [SerializeField] private AudioSource _loopSource;

    [Header("Audio Clips & Volume")]
    [SerializeField] private AudioClip[] _audioFireClips;
    [Range(0f, 1f)] public float fireVolume = 0.5f;

    [SerializeField] private AudioClip[] _audioReactToNoiseClips;
    [Range(0f, 1f)] public float reactVolume = 0.7f;

    [SerializeField] private AudioClip[] _audioSearchingClips;
    [Range(0f, 1f)] public float searchVolume = 0.3f; // Тихое бормотание

    [Tooltip("Звук, когда перестал искать (Эх... Показалось...)")]
    [SerializeField] private AudioClip[] _audioGiveUpClips;
    [Range(0f, 1f)] public float giveUpVolume = 0.5f;

    [SerializeField] private AudioClip[] _audioDodgeClips;
    [Range(0f, 1f)] public float dodgeVolume = 0.6f;

    [SerializeField] private AudioClip[] _audioDamageRabbitClips;
    [Range(0f, 1f)] public float damageVolume = 0.8f;

    // ... (Остальные переменные без изменений) ...
    [Header("Layers")]
    public LayerMask obstacleMask = 1 << 8;
    public LayerMask playerMask = 1 << 9;
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
    private float timer = 0f;

    private enum State { Patrol, ReactToNoise, Tracking, SearchingLastSeen }
    private State currentState = State.Patrol;

    private float detectionTimer = 0f;
    private bool canFire = true;
    private bool isShooting = false;

    private Vector3 lastKnownPlayerPos;
    private float searchEndTime;

    private Coroutine moveCoroutine;
    private Coroutine patrolCoroutine;
    private Coroutine searchCoroutine;

    private bool _isSeePlayer = false;
    private bool isReady = false; // Флаг готовности
    private const float GROUND_Y = 0.05f;
    
    // Кэшированное значение громкости SFX из SettingsManager
    private float _sfxVolumeMultiplier = 1f;

    private void SetShootIndicatorFill(float normalized)
    {
        if (_shootIndicator == null) return;

        normalized = Mathf.Clamp01(normalized);

        if (_shootIndicatorDivisions > 1)
        {
            int filledSteps = Mathf.Clamp(Mathf.FloorToInt(normalized * _shootIndicatorDivisions), 0, _shootIndicatorDivisions);
            normalized = filledSteps / (float)_shootIndicatorDivisions;
        }

        _shootIndicator.fillAmount = normalized;
    }

    void Start()
    {
        // Проверка аудио
        if (_sfxSource == null) Debug.LogError("Не назначен SFX AudioSource!");
        if (_loopSource == null) Debug.LogError("Не назначен Loop AudioSource!");

        if (player != null) player.onNoise.AddListener(ReactToNoise);
        if (aimMarker != null) aimMarker.position = new Vector3(transform.position.x, GROUND_Y, transform.position.z);

        patrolCoroutine = StartCoroutine(PatrolLoop());
        InvokeRepeating(nameof(CheckLineOfSight), 0.05f, checkInterval);

        StartCoroutine(WarmupRoutine());
        
        // Загрузить начальное значение громкости
        if (SettingsManager.Instance != null)
            _sfxVolumeMultiplier = SettingsManager.Instance.EffectiveSFXVolume;
    }
    
    void OnEnable()
    {
        // Подписка на события настроек
        SettingsManager.OnSFXVolumeChanged += OnSFXVolumeSettingChanged;
        SettingsManager.OnMasterVolumeChanged += OnMasterVolumeSettingChanged;
    }
    
    void OnDisable()
    {
        // Отписка от событий
        SettingsManager.OnSFXVolumeChanged -= OnSFXVolumeSettingChanged;
        SettingsManager.OnMasterVolumeChanged -= OnMasterVolumeSettingChanged;
    }
    
    private void OnSFXVolumeSettingChanged(float value)
    {
        if (SettingsManager.Instance != null)
            _sfxVolumeMultiplier = SettingsManager.Instance.EffectiveSFXVolume;
    }
    
    private void OnMasterVolumeSettingChanged(float value)
    {
        if (SettingsManager.Instance != null)
            _sfxVolumeMultiplier = SettingsManager.Instance.EffectiveSFXVolume;
    }

    IEnumerator WarmupRoutine()
    {
        isReady = false;
        yield return new WaitForSeconds(2.0f); // Фермер тупит 2 секунды при старте
        isReady = true;
    }

    // ... (Update без изменений) ...
    private void Update()
    {
        if (player == null || !player.IsAlive())
        {
            _isSeePlayer = false;
            StopLoopingAudio(); // Если игрок умер, замолкаем
            return;
        }

        float distToAim = Vector3.Distance(GetPlayerGroundPos(), aimMarker.position);
        if (distToAim > detectionRadius) { _isSeePlayer = false; return; }

        Vector3 from = transform.position + Vector3.up * 0.5f;
        Vector3 playerCenter = player.transform.position + Vector3.up * 0.5f;
        Vector3 dir = (playerCenter - from).normalized;
        float dist = Vector3.Distance(from, playerCenter);

        if (Physics.Raycast(from, dir, out RaycastHit hit, dist, obstacleMask))
        {
            _isSeePlayer = false;
        }
        else
        {
            _isSeePlayer = true;
        }
    }

    // --- АУДИО МЕТОДЫ ---

    // 1. Для одноразовых звуков (выстрел, крик)
    private void PlaySFX(AudioClip[] clips, float volumeScale)
    {
        if (clips == null || clips.Length == 0 || _sfxSource == null) return;
        int index = Random.Range(0, clips.Length);
        _sfxSource.pitch = Random.Range(0.95f, 1.05f);
        _sfxSource.PlayOneShot(clips[index], volumeScale * _sfxVolumeMultiplier);
    }

    // 2. Для зацикленных звуков (поиск)
    // Обновленный метод запуска лупа
    private void PlayLoopingAudio(AudioClip[] clips, float volume)
    {
        if (clips == null || clips.Length == 0 || _loopSource == null) return;

        // Если мы уже играем и этот AudioSource включен - ничего не делаем
        // (чтобы не начинать звук заново каждый кадр)
        if (_loopSource.isPlaying && _loopSource.loop == true) return;

        int index = Random.Range(0, clips.Length);

        // 1. Сначала полностью останавливаем
        _loopSource.Stop();

        // 2. Назначаем клип и параметры
        _loopSource.clip = clips[index];
        _loopSource.volume = volume * _sfxVolumeMultiplier;
        _loopSource.loop = true; // ПРИНУДИТЕЛЬНО ВКЛЮЧАЕМ ЛУП
        _loopSource.pitch = 1f;  // Возвращаем нормальную скорость (если вдруг меняли)

        // 3. Запускаем
        _loopSource.Play();

        // Debug.Log($"[Audio] Запущен луп: {_loopSource.clip.name}");
    }

    // Обновленный метод остановки
    private void StopLoopingAudio()
    {
        // Останавливаем, только если он реально что-то играет
        if (_loopSource != null && _loopSource.isPlaying)
        {
            _loopSource.Stop();
            _loopSource.loop = false; // Сбрасываем галочку
            _loopSource.clip = null;  // Очищаем клип

            // Debug.Log("[Audio] Луп остановлен");
        }
    }

    // ... (CheckLineOfSight без изменений) ...
    void CheckLineOfSight()
    {
        if (!isReady) return;
        if (currentState == State.Tracking && !isShooting)
        {
            RequestMoveTo(GetPlayerGroundPos(), trackingSpeed);
        }

        if (_isSeePlayer)
        {
            // Если увидели игрока - ПЕРЕСТАЕМ БОРМОТАТЬ
            StopLoopingAudio();

            if (isShooting) return;

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
        else
        {
            if (isShooting) return;
            SetShootIndicatorFill(0f);

            detectionTimer = Mathf.Max(0f, detectionTimer - checkInterval * 3f);

            if (currentState == State.Tracking && detectionTimer <= 0f)
            {
                EnterSearchingLastSeen();
            }
        }
    }

    IEnumerator FireSequence()
    {
        // Перед выстрелом точно молчим
        StopLoopingAudio();

        canFire = false;
        isShooting = true;
        detectionTimer = 0f;

        // Сохраняем последнюю валидную позицию игрока
        Vector3 lastValidPlayerPos = GetPlayerGroundPos();

        // ФАЗА 1: НАВОДКА
        timer = 0f;
        while (timer < chargeTime)
        {
            if (player != null) // Обновляем позицию только если игрок еще существует
            {
                lastValidPlayerPos = GetPlayerGroundPos();
            }
            if (aimMarker != null) aimMarker.position = lastValidPlayerPos;

            timer += Time.deltaTime;
            float chargeProgress = (chargeTime <= 0f) ? 1f : (timer / chargeTime);
            SetShootIndicatorFill(chargeProgress);
            yield return null;
        }

        // ФАЗА 2: ЗАМОК
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        Debug.Log("<color=yellow>LOCK!</color>");
        DrawDebugRing(aimMarker.position, shotHitRadius);
        yield return new WaitForSeconds(lockTime);
        if (debugRing != null) debugRing.SetActive(false);

        // ФАЗА 3: ВЫСТРЕЛ
        if (muzzleFlash) muzzleFlash.SetActive(true);
        PlaySFX(_audioFireClips, fireVolume); // Используем PlaySFX
        SetShootIndicatorFill(0f);

        // --- ГЛАВНОЕ ИСПРАВЛЕНИЕ ТУТ ---
        // Считаем дистанцию до последней известной позиции игрока
        // Это важно, если игрок уничтожается в момент выстрела
        Vector3 finalPlayerPos = (player != null && player.IsAlive()) ? GetPlayerGroundPos() : lastValidPlayerPos;
        float distanceToPlayer = Vector3.Distance(aimMarker.position, finalPlayerPos);

        if (distanceToPlayer <= shotHitRadius)
        {
            Debug.Log($"<color=red>ПОПАДАНИЕ!</color>");
            
            // Наносим урон, ТОЛЬКО если игрок еще жив
            if (player != null && player.IsAlive()) 
            {
                player.TakeDamage(1, transform.position);
            }
            
            // Звук попадания играем в любом случае
            PlaySFX(_audioDamageRabbitClips, damageVolume);
        }
        else
        {
            Debug.Log($"<color=green>УВОРОТ!</color>");
            PlaySFX(_audioDodgeClips, dodgeVolume);
        }

        yield return new WaitForSeconds(0.15f);
        if (muzzleFlash) muzzleFlash.SetActive(false);
        isShooting = false;
        yield return new WaitForSeconds(fireCooldown);
        canFire = true;
    }

    void ReactToNoise(Vector3 pos)
    {
        if (!isReady) return; // Игнорируем шум при старте
        if (isShooting) return;

        StopLoopingAudio(); // Перестаем бормотать, если услышали шум
        PlaySFX(_audioReactToNoiseClips, reactVolume);

        Vector3 groundPos = new Vector3(pos.x, GROUND_Y, pos.z);
        EnterReactToNoise(groundPos);
    }

    void EnterReactToNoise(Vector3 pos)
    {
        currentState = State.ReactToNoise;
        StopAllStateCoroutines();
        RequestMoveTo(pos, 28f);
        StartCoroutine(DelayedPatrol(2f));
    }

    void EnterTracking()
    {
        currentState = State.Tracking;
        StopLoopingAudio(); // В погоне молчим (или можно включить музыку погони)
        StopAllStateCoroutines();
    }

    void EnterSearchingLastSeen()
    {
        currentState = State.SearchingLastSeen;

        // ЗАПУСКАЕМ БОРМОТАНИЕ
        PlayLoopingAudio(_audioSearchingClips, searchVolume);

        StopAllStateCoroutines();
        lastKnownPlayerPos = GetPlayerGroundPos();
        searchEndTime = Time.time + searchDuration;
        searchCoroutine = StartCoroutine(SearchRoutine());
    }

    Vector3 GetRandomPatrolPointRect()
    {
        float x = Random.Range(-patrolRectHalfSize.x, patrolRectHalfSize.x);
        float z = Random.Range(-patrolRectHalfSize.y, patrolRectHalfSize.y);

        return new Vector3(
            patrolRectCenter.x + x,
            GROUND_Y,
            patrolRectCenter.z + z
            );
    }


void EnterPatrol()
    {
        // Если мы переходим в патруль ПОСЛЕ поиска - значит мы не нашли игрока
        if (currentState == State.SearchingLastSeen)
        {
            // Играем звук разочарования ("Эх...", "Тьфу ты")
            // Используем PlaySFX, потому что это короткий звук
            PlaySFX(_audioGiveUpClips, giveUpVolume);
        }

        Debug.Log("Поиск окончен. Переход в патруль.");
        currentState = State.Patrol;
        
        StopLoopingAudio(); // Выключаем бесконечное бормотание
        StopAllStateCoroutines();
        
        patrolCoroutine = StartCoroutine(PatrolLoop());
    }

    // ... (Остальной код движения без изменений) ...

    IEnumerator SearchRoutine()
    {
        while (Time.time < searchEndTime && currentState == State.SearchingLastSeen)
        {
            Vector3 randomPoint = lastKnownPlayerPos + Random.insideUnitSphere * searchRadius;
            randomPoint.y = GROUND_Y;
            RequestMoveTo(randomPoint, searchMoveSpeed);
            yield return new WaitForSeconds(searchPointInterval);
        }
        EnterPatrol();
    }

    IEnumerator PatrolLoop()
    {
    while (currentState == State.Patrol)
    {
        Vector3 point = GetRandomPatrolPointRect();
        RequestMoveTo(point, patrolSpeed);

        yield return new WaitForSeconds(patrolPause + Random.Range(-0.5f, 1f));
    }
}



    IEnumerator DelayedPatrol(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentState == State.ReactToNoise) EnterPatrol();
    }

    void RequestMoveTo(Vector3 target, float speed)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveMarker(target, speed));
    }

    IEnumerator MoveMarker(Vector3 target, float speed)
    {
    // --- ограничение зоны ---
    target.x = Mathf.Clamp(
        target.x,
        patrolRectCenter.x - patrolRectHalfSize.x,
        patrolRectCenter.x + patrolRectHalfSize.x
    );

    target.z = Mathf.Clamp(
        target.z,
        patrolRectCenter.z - patrolRectHalfSize.y,
        patrolRectCenter.z + patrolRectHalfSize.y
    );

    target.y = GROUND_Y;

    Vector3 start = aimMarker.position;
    float distance = Vector3.Distance(start, target);
    if (distance < 0.1f) yield break;

    float journey = 0f;
    while (journey < 1f)
    {
        journey += Time.deltaTime * speed / distance;
        aimMarker.position = Vector3.Lerp(start, target, journey);
        yield return null;
    }
    aimMarker.position = target;
    }


    Vector3 GetPlayerGroundPos() 
    { 
        if (player == null) return transform.position; // Безопасный фолбэк
        return new Vector3(player.transform.position.x, GROUND_Y, player.transform.position.z);
    }

    void StopAllStateCoroutines()
    {
        if (moveCoroutine != null) { StopCoroutine(moveCoroutine); moveCoroutine = null; }
        if (patrolCoroutine != null) { StopCoroutine(patrolCoroutine); patrolCoroutine = null; }
        if (searchCoroutine != null) { StopCoroutine(searchCoroutine); searchCoroutine = null; }
    }

    void OnDrawGizmosSelected()
    {
        if (aimMarker != null)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawSphere(aimMarker.position, detectionRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(aimMarker.position, shotHitRadius);
          Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawWireCube(
        new Vector3(patrolRectCenter.x, GROUND_Y, patrolRectCenter.z),
        new Vector3(patrolRectHalfSize.x * 2f, 0.1f, patrolRectHalfSize.y * 2f)
);

        }
    }
    void DrawDebugRing(Vector3 pos, float radius)
    {
        if (debugRing == null)
        {
            debugRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(debugRing.GetComponent<Collider>());
            debugRing.transform.localScale = new Vector3(radius * 2, 0.1f, radius * 2);
            debugRing.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.5f);
        }
        debugRing.SetActive(true);
        debugRing.transform.position = pos;
    }

    void OnDestroy()
    {
        if (player != null) player.onNoise.RemoveListener(ReactToNoise);
    }
}