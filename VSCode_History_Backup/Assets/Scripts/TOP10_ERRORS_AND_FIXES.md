# 🔴 TOP 10 ОШИБОК И РЕШЕНИЯ

Если ты видишь одну из этих ошибок в Console → сразу переходи к решению.

---

## ❌ ОШИБКА #1
```
✗ [GameInstaller] No Animator found in scene! 
Make sure rabbit has Animator component.
```

### Что это значит:
Кролик (rabbit) не имеет компонента Animator.

### Как исправить:
1. В Hierarchy найди rabbit
2. Выбери его
3. Inspector → Add Component → **Animator**
4. Сохрани (Ctrl+S) → Play

---

## ❌ ОШИБКА #2
```
✗ [GameInstaller] Rabbit has no Rigidbody!
```

### Что это значит:
На кролике нет физики (Rigidbody).

### Как исправить:
1. Выбери rabbit в Hierarchy
2. Inspector → Add Component → Physics → **Rigidbody**
3. **ВАЖНЫЕ НАСТРОЙКИ:**
   ```
   Mass: 1
   Drag: 0.3
   Use Gravity: ✓ (галочка должна быть!)
   Constraints:
     ✓ Freeze Rotation X
     ✓ Freeze Rotation Z
   ```
4. Сохрани → Play

---

## ❌ ОШИБКА #3
```
✗ [GameInstaller] Main camera not found!
```

### Что это значит:
Камера не имеет правильного Tag.

### Как исправить:
1. В Hierarchy найди **Main Camera**
2. Выбери его
3. Вверху в Inspector жми **Tag** dropdown
4. Выбери **MainCamera** (важно: с большой M и C!)
5. Если нет такого tag → Create Tag → добавь "MainCamera"
6. Сохрани → Play

---

## ❌ ОШИБКА #4
```
[PlayerInputHandler] PlayerInput component not found in scene!
```

### Что это значит:
В сцене нет объекта с компонентом PlayerInput.

### Как исправить:
1. Right-click в Hierarchy → Create Empty
2. Назови его **PlayerInput**
3. Выбери этот новый объект
4. Inspector → Add Component → Player Input
5. В компоненте "Player Input" найди поле **Actions**
6. Нажми на dropdown и выбери **InputSystem_Actions**
7. Сохрани → Play

---

## ❌ ОШИБКА #5
```
ArgumentException: There is no InputAction 'Move'
```

### Что это значит:
Input System Actions не сгенерировали C# код.

### Как исправить:
1. В папке Assets найди файл **InputSystem_Actions.inputactions**
2. **Right-click → Generate C# Class**
3. Жди пока Unity перезагружается (~30 сек)
4. Дождись появления нового файла **InputSystem_Actions.cs**
5. Выбери компонент PlayerInput (в сцене)
6. Поле Actions → выбери **InputSystem_Actions** из dropdown
7. Play

---

## ❌ ОШИБКА #6
```
NullReferenceException: Object reference not set to an instance of an object
```

### Что это значит:
Какой-то компонент не может найти нужный объект (неполная конфигурация).

### Как исправить:
1. **Запусти Stage1Diagnostic:**
   - Create Empty → Add Component → Stage1Diagnostic
   - Play → посмотри отчёт в Console
2. Найди в отчёте все ✗ (красные крестики)
3. По каждому ✗ выполни исправление из списка выше (ошибки #1-#5)

---

## ❌ ОШИБКА #7
```
Rabbit doesn't move / WASD doesn't work
```

### Что это значит:
Управление настроено, но движение не работает. Обычно это:
- Rigidbody имеет неправильные настройки
- Input не читается
- PlayerMovement скрипт не запустился

### Как исправить:
1. **Проверь Rigidbody rabbit:**
   - Use Gravity должна быть ✓
   - Freeze Position НЕ должна иметь галочку на X или Z
   - Только Freeze Rotation X и Z!

2. **Проверь PlayerInput:**
   - Должен быть в сцене (не удалён)
   - Actions должны быть = InputSystem_Actions

3. **Проверь консольные логи:**
   ```
   ✓ [GameInstaller] Input system bound
   ✓ [GameInstaller] Found rabbit: Rabbit
   ✓ [GameInstaller] Player movement bound
   ✓ [GameInstaller] Camera follower bound
   ```
   - Если видишь все галочки → проблема в физике (пункт 1)

---

## ❌ ОШИБКА #8
```
Rabbit falls through the floor / проваливается сквозь пол
```

### Что это значит:
Ground (земля) не имеет коллайдера или Rigidbody настроен неправильно.

### Как исправить:
1. **Проверь Ground:**
   - Выбери Ground (Plane) в Hierarchy
   - Должен быть компонент **Box Collider**
   - Если нет → Add Component → Physics → Box Collider

2. **Проверь Rigidbody rabbit:**
   - Use Gravity: **✓** (ДОЛЖНА быть галочка!)
   - Freeze Position Y: **☐** (НЕ должна быть галочка!)

3. Сохрани → Play

---

## ❌ ОШИБКА #9
```
Camera doesn't follow / камера не следует
```

### Что это значит:
CameraFollower скрипт не может найти rabbit или камеру.

### Как исправить:
1. **Проверь Main Camera Tag:**
   - Main Camera → Inspector → Tag = MainCamera

2. **Проверь, что rabbit имеет Animator:**
   - CameraFollower ищет Animator на scene
   - rabbit должен иметь Animator компонент

3. **Проверь логи:**
   ```
   ✓ [GameInstaller] Camera follower bound
   ```
   - Если видишь эту галочку → камера должна следить
   - Если видишь ✗ → значит Camera не найдена (вернись к ошибке #3)

---

## ❌ ОШИБКА #10
```
Scene Context doesn't exist / Zenject не инициализирует
```

### Что это значит:
Нет объекта Scene Context для Zenject DI.

### Как исправить:
1. Right-click в Hierarchy
2. → 3D Object → **Scene Context**
3. Выбери Scene Context
4. Inspector → Add Component → **GameInstaller**
5. Сохрани → Play

---

## 🔍 ПРОВЕРКА: ВСЁ ЛИ ИСПРАВЛЕНО?

После исправления каждой ошибки:

1. **Сохрани сцену** (Ctrl+S)
2. **Нажми Play**
3. **Смотри Console**
4. Если вновь видишь ошибку → повтори исправление

**Если все ошибки исправлены:**
- Логи должны показывать только ✓ (зелёные галочки)
- Нет красных ошибок
- WASD движет rabbit
- Shift даёт рывок

---

## 🆘 ЕСЛИ НИЧЕГО НЕ ПОМОГАЕТ

**Попробуй в этом порядке:**

1. **Перекомпилируй:**
   ```
   Assets → Reimport All
   ```
   Жди 30 сек

2. **Перезагрузи скрипты:**
   ```
   Ctrl+R (Windows/Linux)
   Cmd+R (Mac)
   ```

3. **Перезагрузи Unity:**
   ```
   File → Close Project
   Re-open project
   ```

4. **Ядерный вариант (если ничего не помогает):**
   - Закрой Unity
   - Удали папку `Library/` из проекта
   - Открой Unity снова
   - Жди пока перезагрузится (~5 минут)

---

## 📋 ДИАГНОСТИЧЕСКИЙ СКРИПТ

**Если не знаешь в чём проблема:**

1. Create Empty GameObject
2. Add Component → **Stage1Diagnostic**
3. Play
4. Смотри Console
5. Найди в отчёте все ✗ (красные крестики)
6. По каждому ✗ выполни исправление из списка выше

---

## ✅ УСПЕШНАЯ КОНФИГУРАЦИЯ

Когда ВСЁ работает, консоль показывает:

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

**И управление работает:**
- WASD → movement
- Shift → dash
- Камера следит
- Никаких красных ошибок

---

**УДАЧИ! 🚀**

