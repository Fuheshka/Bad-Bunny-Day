# 🎮 STAGE 1 - ПОШАГОВОЕ РЕШЕНИЕ ПРОБЛЕМ УПРАВЛЕНИЯ

## Если не работает управление — следуй этому плану ⬇️

---

## ШАГ 0: Включи диагностику

1. **Создай пустой GameObject в сцене:**
   - Right-click в Hierarchy → Create Empty
   - Назови его `Diagnostic`

2. **Добавь компонент диагностики:**
   - Выбери Diagnostic
   - Add Component → Stage1Diagnostic (из папки Debug)

3. **Нажми Play** в редакторе

4. **Открой Console:**
   - Window → General → Console
   - Там будет подробный отчёт что работает, а что нет

---

## ШАГ 1: Проверь физику персонажа

### Найди Rabbit в сцене:
- В Hierarchy найди объект с моделью зайца
- Обычно это `Rabbit`, `Bunny`, `Character` или похожее название

### Убедись, есть ли Animator:
- Выбери этот объект
- В Inspector смотри список компонентов
- Должен быть компонент `Animator` (если модель FBX или prefab)
- ✓ Если есть → переходи к следующему
- ✗ Если нет → `Add Component → Animator`

### Добавь Rigidbody:
- Выбери rabbit
- **Add Component → Physics → Rigidbody**

**ТОЧНЫЕ НАСТРОЙКИ RIGIDBODY:**

```
╔═══════════════════════════════════════╗
║ RIGIDBODY SETTINGS                    ║
╠═══════════════════════════════════════╣
║ Mass: 1                               ║
║ Linear Damping (Drag): 0.3            ║
║ Angular Damping: 0.05                 ║
║ Use Gravity: ✓ (должна быть галочка!) ║
║ Collision Detection: Discrete         ║
║                                       ║
║ CONSTRAINTS (Freeze):                 ║
║   ✓ Freeze Rotation X                 ║
║   ✓ Freeze Rotation Z                 ║
║   (НЕ трогай Y!)                      ║
╚═══════════════════════════════════════╝
```

### Добавь Capsule Collider:
- **Add Component → Physics → Capsule Collider**

**ТОЧНЫЕ НАСТРОЙКИ CAPSULE COLLIDER:**

```
╔═══════════════════════════════════════╗
║ CAPSULE COLLIDER SETTINGS             ║
╠═══════════════════════════════════════╣
║ Center: (0, 1, 0)                     ║
║ Radius: 0.5                           ║
║ Height: 2                             ║
║ Direction: Y-Axis                     ║
║ Is Trigger: ✗ (НЕ отмечай!)           ║
╚═══════════════════════════════════════╝
```

**Результат в Inspector должен выглядеть так:**
```
Inspector (Rabbit):
├── Transform
├── Animator ✓
├── Rigidbody ✓
├── Capsule Collider ✓
└── (другие компоненты)
```

**Нажми Play → смотри Console:**
- Должен появиться лог: `✓ [GameInstaller] Player movement bound`
- ✗ Если всё ещё нет → идём на ШАГ 2

---

## ШАГ 2: Проверь камеру

### Найди Main Camera:
- В Hierarchy найди объект `Main Camera`

### Проверь Tag:
- Выбери Main Camera
- В Inspector → вверху жми **Tag** dropdown
- Выбери `MainCamera` (точно с такой капитализацией!)
- Если нет в списке → создай новый Tag называется `MainCamera`

**Результат в Inspector:**
```
─────────────────────────
Main Camera
Tag: MainCamera ✓
Layer: Default
─────────────────────────
```

**Нажми Play → смотри Console:**
- Должен появиться лог: `✓ [GameInstaller] Camera follower bound`

---

## ШАГ 3: Проверь Input System

### Найди PlayerInput компонент:
- В Hierarchy ищи объект с компонентом `PlayerInput`
- Если не найдёшь → создай:
  - Right-click в Hierarchy → Create Empty
  - Назови `PlayerInput` (точно так!)
  - **Add Component → Player Input**

### Настрой PlayerInput компонент:
- Выбери объект с PlayerInput
- В Inspector найди компонент "Player Input"

**ТОЧНЫЕ НАСТРОЙКИ:**

```
╔═══════════════════════════════════════╗
║ PLAYER INPUT SETTINGS                 ║
╠═══════════════════════════════════════╣
║ Actions: InputSystem_Actions          ║
║ Default Control Scheme: Keyboard      ║
║ (убедись что дропдаун имеет значение) ║
║                                       ║
║ UI Input Module: (оставь пустым)      ║
║ Camera: (оставь пустым)               ║
╚═══════════════════════════════════════╝
```

### Проверь Actions в InputSystem:

1. В папке Assets найди файл `InputSystem_Actions.inputactions`
2. Двойным кликом открой его в редакторе
3. Должны быть:
   - Action Map: `Player`
   - Action: `Move` (тип: Value, Control Type: Vector 2)
   - Action: `Sprint` (тип: Button)

**Если их нет → создай:**
- Жми `+` → Add Action
- Задай имя `Move` → Control Type: Vector2
- Жми `+` → Add Action
- Задай имя `Sprint` → Control Type: Button

4. **Сохрани файл** (Ctrl+S / Cmd+S)

### Регенерируй Input System код:

1. В Project окне найди `InputSystem_Actions.inputactions`
2. **Right-click → Generate C# Class**
3. Жди, пока Unity перезагрузится
4. ✓ Если видишь новый файл `InputSystem_Actions.cs` → хорошо!

### Перепривяжи Actions в PlayerInput:

- Выбери объект с PlayerInput компонентом
- В Inspector, поле `Actions`:
  - Жми на выпадающий список
  - Выбери `InputSystem_Actions`

**Нажми Play → смотри Console:**
```
[PlayerInputHandler] Found PlayerInput component
```

---

## ШАГ 4: Настрой Ground/Препятствия

### Создай Ground (земля):
- Right-click в Hierarchy → 3D Object → Plane
- Назови `Ground`
- Set Position: (0, 0, 0)
- Set Scale: (10, 1, 10)
- **Add Component → Box Collider** (если нет)

### Создай несколько препятствий:
- Right-click → 3D Object → Cube
- Назови `Obstacle_1`
- Set Position: (3, 1, 0)
- Повтори 2-3 раза с разными позициями

---

## ШАГ 5: Финальная проверка Zenject

### Убедись Scene Context существует:
- В Hierarchy ищи объект `Scene Context`
- Если нет → Right-click → 3D Object → Scene Context
  - (это специальный объект для Zenject)

### Убедись GameInstaller привязан:
- Выбери Scene Context
- В Inspector смотри компоненты
- Должен быть компонент `GameInstaller` (наш скрипт)
- Если нет → **Add Component → GameInstaller**

---

## ✅ ФИНАЛЬНЫЙ ТЕСТ

**Если ты прошёл все 5 шагов:**

1. **Нажми Play** в редакторе

2. **Смотри Console** — должны быть логи:
   ```
   ════════════════════════════════════════
   [GameInstaller] Starting bindings...
   ════════════════════════════════════════
   ✓ [GameInstaller] Input system bound
   ✓ [GameInstaller] Found rabbit: Rabbit
   ✓ [GameInstaller] Player movement bound
   ✓ [GameInstaller] Camera follower bound
   ════════════════════════════════════════
   [GameInstaller] Bindings complete!
   ════════════════════════════════════════
   [PlayerInputHandler] Found PlayerInput component
   ```

3. **Тест управления:**
   - Нажми **W** → Rabbit должен двигаться вперёд
   - Нажми **A** → двигаться влево
   - Нажми **S** → двигаться назад
   - Нажми **D** → двигаться вправо
   - Удерживай **Shift** → 2x скорость движения
   - Камера должна следить сзади-слева

4. **Если всё работает:**
   - Отлично! 🎉 Переходи на Этап 2
   - Если нет → смотри логи ошибок и вернись на нужный шаг

---

## 🔧 Если логи показывают ошибку

**Ошибка: "No Animator found"**
- Решение: Шаг 1 → Проверь Animator компонент на rabbit

**Ошибка: "Rabbit has no Rigidbody"**
- Решение: Шаг 1 → Добавь Rigidbody

**Ошибка: "Main camera not found"**
- Решение: Шаг 2 → Проверь Tag "MainCamera"

**Ошибка: "PlayerInput component not found"**
- Решение: Шаг 3 → Создай объект с PlayerInput

**Ошибка: "ArgumentException: There is no InputAction 'Move'"**
- Решение: Шаг 3 → Регенерируй Input System код

**Ошибка: "NullReferenceException"**
- Решение: Запусти диагностику (ШАГ 0) и посмотри что нарушается

---

## 💡 Если rabbit падает сквозь пол

1. Проверь Ground имеет BoxCollider
2. Проверь Rigidbody Use Gravity = ✓
3. Проверь Rigidbody Constraints НЕ содержит "Freeze Position Y"

---

## 🆘 Всё ещё не работает?

**Попробуй ядерный вариант:**

1. **Перекомпилируй:**
   - Assets → Reimport All
   - Жди 30 секунд

2. **Перезагрузи Unity:**
   - File → Close Scene
   - File → Open Recent → SampleScene
   - Или закрой Unity полностью и открой заново

3. **Очисти кэш:**
   - Закрой Unity
   - Удали папку `Library/` из проекта
   - Открой Unity снова
   - Жди, пока реимпортирует всё (это может занять 5 минут)

4. **Проверь версию Input System:**
   - Window → TextMesh Pro → Import
   - Потом: Ctrl+R (перекомпилировать скрипты)

