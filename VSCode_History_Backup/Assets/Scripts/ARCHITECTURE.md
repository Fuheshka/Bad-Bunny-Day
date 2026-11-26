# Архитектура проекта CDB (Chungus Bunny Day)

## Обзор

Проект организован по модульному принципу с разделением ответственности. Используются паттерны:
- **Singleton** - для менеджеров (GameStateManager, AudioManager, ScoreManager)
- **Component** - для разделения логики игрока на отдельные компоненты
- **ScriptableObject** - для данных конфигурации (Data Objects)
- **Observer** - для событий и уведомлений

## Структура папок

```
Assets/Scripts/
├── Audio/                      # Система звука
│   ├── AudioManager.cs         # Синглтон управления звуком
│   ├── SoundData.cs           # ScriptableObject для звуков
│   └── SoundEmitter.cs        # Компонент воспроизведения звука
│
├── Core/                       # Базовая инфраструктура
│   ├── Base/                   # Базовые классы
│   │   ├── Singleton.cs       # Базовый синглтон
│   │   └── PersistentSingleton.cs
│   │
│   ├── Events/                 # Система событий
│   │   ├── GameEvent.cs       # ScriptableObject события
│   │   └── GameEventListener.cs
│   │
│   ├── GameStates/             # Состояния игры
│   │   ├── GameState.cs       # Enum состояний
│   │   └── GameStateManager.cs # Управление состояниями
│   │
│   └── Interfaces/             # Интерфейсы
│       ├── IDamageable.cs     # Получение урона
│       ├── IHealable.cs       # Лечение
│       ├── IInteractable.cs   # Взаимодействие
│       └── IHighlightable.cs  # Подсветка
│
├── Enemy/                      # Система врагов
│   ├── Data/                   # Данные врагов
│   │   ├── EnemyPatrolData.cs
│   │   ├── EnemyDetectionData.cs
│   │   └── EnemyCombatData.cs
│   │
│   ├── Components/             # Компоненты врага
│   │   ├── EnemyDetection.cs  # Обнаружение игрока
│   │   └── EnemySounds.cs     # Звуки врага
│   │
│   └── EnemyState.cs          # Enum состояний AI
│
├── Food/                       # Система еды
│   ├── EdibleObject.cs        # Съедобный объект
│   └── FoodSpawner.cs         # Спавнер еды
│
├── Player/                     # Система игрока
│   ├── Data/                   # Данные конфигурации
│   │   ├── PlayerMovementData.cs
│   │   ├── PlayerCombatData.cs
│   │   ├── PlayerStaminaData.cs
│   │   └── PlayerWeightData.cs
│   │
│   ├── Components/             # Компоненты игрока
│   │   ├── PlayerHealth.cs    # Здоровье
│   │   ├── PlayerStamina.cs   # Выносливость
│   │   ├── PlayerWeight.cs    # Система веса
│   │   ├── PlayerMovement.cs  # Движение
│   │   └── PlayerInteraction.cs # Взаимодействие
│   │
│   ├── PlayerController.cs    # Основной контроллер (legacy)
│   └── ProceduralAnimator.cs  # Процедурные анимации
│
├── Settings/                   # Настройки
│   ├── SettingsManager.cs     # Менеджер настроек
│   └── SettingsMenu.cs        # UI настроек
│
└── UI/                         # Интерфейс
    ├── Base/                   # Базовые классы UI
    │   ├── UIPanel.cs         # Базовая панель
    │   └── UIProgressBar.cs   # Прогресс-бар
    │
    ├── Panels/                 # UI панели
    │   ├── PausePanel.cs
    │   ├── GameOverPanel.cs
    │   └── LevelCompletePanel.cs
    │
    └── [существующие UI файлы]
```

## Ключевые классы

### Core

#### Singleton<T>
Базовый класс для синглтонов:
```csharp
public class MyManager : Singleton<MyManager> { }
```

#### GameStateManager
Управляет состояниями игры (Playing, Paused, GameOver, LevelComplete):
```csharp
GameStateManager.Instance.Pause();
GameStateManager.Instance.TriggerGameOver();
```

### Player

#### Компонентная архитектура
Вместо монолитного PlayerController, логика разделена:
- `PlayerHealth` - здоровье, урон, смерть
- `PlayerStamina` - выносливость
- `PlayerWeight` - система сытости/веса
- `PlayerMovement` - перемещение, прыжки, рывки
- `PlayerInteraction` - взаимодействие с объектами

#### Data Objects
Параметры вынесены в ScriptableObjects для удобной настройки:
```csharp
[CreateAssetMenu(fileName = "PlayerMovementData", menuName = "CDB/Player/Movement Data")]
public class PlayerMovementData : ScriptableObject { }
```

### Audio

#### AudioManager
Централизованное управление звуками:
```csharp
AudioManager.Instance.PlaySound(soundData);
AudioManager.Instance.PlayMusic(clip);
```

#### SoundData
ScriptableObject для настройки звуков:
- Массив клипов (случайный выбор)
- Громкость, вариация питча
- 3D настройки

### UI

#### UIPanel
Базовый класс для панелей:
```csharp
public class MyPanel : UIPanel
{
    protected override void OnShow() { }
    protected override void OnHide() { }
}
```

## Интерфейсы

### IDamageable
```csharp
public interface IDamageable
{
    void TakeDamage(int damage, Vector3? knockbackSource = null);
    bool IsAlive();
    int CurrentHealth { get; }
    int MaxHealth { get; }
}
```

### IInteractable
```csharp
public interface IInteractable
{
    void OnInteractStart();
    void OnInteractEnd();
    void OnInteractComplete();
    float InteractionTime { get; }
    bool CanInteract { get; }
}
```

## Использование

### Создание нового врага
1. Создать ScriptableObjects с данными в `Enemy/Data/`
2. Использовать компоненты из `Enemy/Components/`
3. При необходимости наследовать от базовых классов

### Добавление нового типа еды
1. Создать префаб с компонентом `EdibleObject`
2. Настроить параметры (nutritionValue, eatTime, scoreValue)
3. Добавить в массив префабов `FoodSpawner`

### Создание новой UI панели
1. Наследовать от `UIPanel`
2. Переопределить `OnShow()` и `OnHide()`
3. Подписаться на события `GameStateManager`

## Миграция со старого кода

Старый код (PlayerController, GameManager, etc.) оставлен для обратной совместимости.
Постепенно заменяйте использование старых классов на новые компоненты.

## TODO
- [ ] Полностью мигрировать PlayerController на компонентную архитектуру
- [ ] Рефакторинг Farmer.cs на компоненты
- [ ] Добавить систему сохранений
- [ ] Добавить систему достижений
