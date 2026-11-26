# STAGE 1 - Подробная диагностика управления

## 🔍 Диагностический процесс

### Шаг 1: Проверка логов консоли (критично!)

1. Открой **Window → General → Console** в Unity
2. **Нажми Play** и посмотри на логи
3. Ищи сообщения `[GameInstaller]` и `[PlayerInputHandler]`

**Ожидаемые логи (если всё работает):**
```
════════════════════════════════════════
[GameInstaller] Starting bindings...
════════════════════════════════════════
✓ [GameInstaller] Input system bound
✓ [GameInstaller] Found rabbit: YourRabbitName
✓ [GameInstaller] Player movement bound
✓ [GameInstaller] Camera follower bound
════════════════════════════════════════
[GameInstaller] Bindings complete!
════════════════════════════════════════
[PlayerInputHandler] Found PlayerInput component
```

---

## 🚨 Проблема #1: "Rabbit has no Rigidbody" или "No Animator found"

**Логи показывают:**
```
✗ [GameInstaller] Rabbit has no Rigidbody!
✗ [GameInstaller] No Animator found in scene!
```

**Решение:**

1. **Найди Rabbit в Hierarchy** (обычно это модель в формате FBX или префаб)
   - Выбери его в иерархии

2. **Проверь, есть ли Animator компонент:**
   - Смотри в Inspector → Components список
   - Если нет Animator → `Add Component → Animator`
   - Если есть → проверь его вкл (галочка слева)

3. **Добавь Rigidbody (если его нет):**
   - Выбери rabbit в Hierarchy
   - `Add Component → Physics → Rigidbody`
   - Настройки:
     ```
     Mass: 1
     Drag: 0.3
     Angular Drag: 0.05
     Gravity: ✓ (включено)
     Constraints:
       ✓ Freeze Rotation X
       ✓ Freeze Rotation Z
     ```

4. **Добавь Capsule Collider:**
   - `Add Component → Physics → Capsule Collider`
   - Настройки:
     ```
     Center: (0, 1, 0)
     Radius: 0.5
     Height: 2
     Direction: Y-Axis
     ```

5. **Нажми Play** → проверь логи снова

---

## 🚨 Проблема #2: "Main camera not found!"

**Логи показывают:**
```
✗ [GameInstaller] Main camera not found!
```

**Решение:**

1. **Найди Main Camera в Hierarchy**
2. **Проверь Tag:**
   - Выбери Main Camera
   - В Inspector → **Tag** dropdown → выбери **"MainCamera"**
   - (Если его нет в списке → Create Tag → добавь "MainCamera")
3. **Нажми Play** → проверь логи

---

## 🚨 Проблема #3: "PlayerInput component not found"

**Логи консоли:**
```
[PlayerInputHandler] PlayerInput component not found in scene!
```

**Решение:**

1. **Проверь, есть ли PlayerInput в сцене:**
   - Ищи в Hierarchy объект с компонентом PlayerInput
   - Если не видишь → создай:
     - `Right-click в Hierarchy → Create Empty`
     - Назови его `PlayerInput`
     - `Add Component → Player Input` (из UI Input System)

2. **Настрой PlayerInput компонент:**
   - Выбери PlayerInput объект
   - В Inspector найди компонент "Player Input"
   - `Actions` field → перетащи или выбери `InputSystem_Actions`
   - Отметь галочки нужные действия (Move, Sprint)

3. **Убедись, что Input Actions сгенерированы:**
   - В проекте найди файл `InputSystem_Actions.inputactions`
   - **Right-click → Generate C# Class**
   - Unity перезагрузится
   - Нажми Play → проверь логи

---

## 🚨 Проблема #4: InputSystem_Actions не существует

**Логи:**
```
ArgumentException: There is no InputAction 'Move'
```

**Решение:**

1. **Найди InputSystem_Actions.inputactions файл:**
   - `Assets → InputSystem_Actions.inputactions`
   - Открой его двойным кликом

2. **Проверь, есть ли Action Map "Player":**
   - Если нет → + → Add Action Map → "Player"

3. **Проверь Actions (должны быть):**
   - `Move` - тип Vector2
   - `Sprint` - тип Button
   - Если нет → + → Add Action

4. **Генерируй C# код:**
   - Файл → **Generate C# Class**

5. **Замени ссылку в PlayerInput:**
   - Выбери PlayerInput компонент в сцене
   - `Actions` field → перетащи регенерированный `InputSystem_Actions` asset

---

## ✅ Проверка системы по частям

Если логи показывают ошибки, проверь поэтапно:

### Часть 1: Сцена настроена?
```
✓ Rabbit имеет Animator компонент
✓ Rabbit имеет Rigidbody
✓ Rabbit имеет Capsule Collider
✓ Main Camera есть с Tag "MainCamera"
✓ Ground/Plane существует с BoxCollider
```

### Часть 2: Input System готов?
```
✓ InputSystem_Actions файл существует
✓ C# код был сгенерирован (или пересгенерирован)
✓ PlayerInput компонент в сцене
✓ PlayerInput указывает на InputSystem_Actions
✓ Actions содержат "Move" и "Sprint"
```

### Часть 3: Zenject готов?
```
✓ Scene Context существует в сцене
✓ GameInstaller скрипт прикреплен к Scene Context
✓ Компилировано без ошибок (Debug → Compile C# проверить)
```

### Часть 4: Логи консоли?
```
✓ GameInstaller выводит все ✓ сообщения
✓ Нет ошибок типа NullReferenceException
✓ PlayerInputHandler находит PlayerInput компонент
```

---

## 🎮 Если всё работает

**Логи должны быть:**
```
✓ [GameInstaller] Input system bound
✓ [GameInstaller] Found rabbit: ...
✓ [GameInstaller] Player movement bound
✓ [GameInstaller] Camera follower bound
[PlayerInputHandler] Found PlayerInput component
```

**Тест управления:**
- WASD → rabbit должен двигаться
- Shift → 2x скорость (рывок)
- Камера следует сзади-слева

---

## 💡 Дополнительные советы

### Если rabbit не двигается, но ошибок нет:

1. **Проверь Physics:**
   - Window → Physics → Physics Debug Visualizer
   - Включи и посмотри на vectors скорости

2. **Проверь Input читается:**
   - Добавь временно в консоль:
     ```csharp
     // В PlayerMovement.cs, добавь в Tick():
     if (_inputState.MoveDirection.magnitude > 0.1f)
         Debug.Log($"Moving: {_inputState.MoveDirection}");
     ```

3. **Проверь Rigidbody настройки:**
   - Не должно быть Freeze X/Z Position
   - Только Freeze Rotation X/Z

### Если камера не следует:

1. **Проверь CameraFollower логи:**
   - Добавь временно в Consumer:
     ```csharp
     Debug.Log($"Camera pos: {_camera.transform.position}");
     ```

2. **Проверь offset в GameConfig:**
   - Может быть слишком большой или маленький

---

## 📞 Если ничего не помогает

**Собери информацию и проверь:**

1. Скопируй **ВСЕ логи** из Console
2. Проверь **каждый компонент** по чеклисту выше
3. Перекомпилируй проект: **Ctrl+R** или **Cmd+R**
4. Перезагрузи Unity: Close → Open project снова
5. Удали Library папку, открой заново (ядерный вариант)

