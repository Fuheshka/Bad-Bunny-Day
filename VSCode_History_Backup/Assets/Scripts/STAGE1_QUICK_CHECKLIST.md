# ✅ STAGE 1 - БЫСТРЫЙ ЧЕКЛИСТ

Распечатай этот файл или открой его рядом с Unity!

---

## 📋 ЧЕКЛИСТ КОНФИГУРАЦИИ СЦЕНЫ

### 1. RABBIT ПЕРСОНАЖ

```
Rabbit в Hierarchy:
  [ ] Имеет компонент Animator
  [ ] Имеет компонент Rigidbody
      [ ] Mass: 1
      [ ] Drag: 0.3
      [ ] Use Gravity: ✓
      [ ] Freeze Rotation X: ✓
      [ ] Freeze Rotation Z: ✓
  [ ] Имеет компонент Capsule Collider
      [ ] Center: (0, 1, 0)
      [ ] Radius: 0.5
      [ ] Height: 2
```

### 2. КАМЕРА

```
Main Camera:
  [ ] Существует в сцене
  [ ] Tag установлен на "MainCamera"
  [ ] Position примерно: (0, 2, -3)
```

### 3. GROUND (ЗЕМЛЯ)

```
Ground/Plane:
  [ ] Создан (3D Object → Plane)
  [ ] Position: (0, 0, 0)
  [ ] Scale: (10, 1, 10)
  [ ] Имеет BoxCollider компонент
```

### 4. INPUT SYSTEM

```
PlayerInput объект в сцене:
  [ ] Существует
  [ ] Имеет компонент PlayerInput
  [ ] Actions указан на InputSystem_Actions
  
InputSystem_Actions файл:
  [ ] Существует (Assets/InputSystem_Actions.inputactions)
  [ ] C# код был сгенерирован (InputSystem_Actions.cs)
  [ ] Содержит Action "Move" (Vector2)
  [ ] Содержит Action "Sprint" (Button)
```

### 5. ZENJECT

```
Scene Context:
  [ ] Создан в сцене (3D Object → Scene Context)
  [ ] Имеет компонент GameInstaller
  
GameInstaller:
  [ ] Скрипт прикреплен к Scene Context
  [ ] Скрипт скомпилирован (нет красных иконок)
```

### 6. ПРЕПЯТСТВИЯ (ОПЦИОНАЛЬНО)

```
Obstacles:
  [ ] Создано 3-5 кубов
  [ ] Они разбросаны по сцене
  [ ] Каждый имеет BoxCollider
```

---

## 🎮 КОНСОЛЬНЫЕ ЛОГИ (ОЖИДАЕМЫЕ)

**При нажатии Play в Console должны быть:**

```
════════════════════════════════════════
[GameInstaller] Starting bindings...
════════════════════════════════════════
✓ [GameInstaller] Input system bound
✓ [GameInstaller] Found rabbit: [имя_зайца]
✓ [GameInstaller] Player movement bound
✓ [GameInstaller] Camera follower bound
════════════════════════════════════════
[GameInstaller] Bindings complete!
════════════════════════════════════════
[PlayerInputHandler] Found PlayerInput component
```

**Если видишь это → ВСЁ РАБОТАЕТ ✓**

---

## 🎮 ТЕСТ УПРАВЛЕНИЯ

```
Нажми Play, потом:

[ ] Нажми W → Rabbit движется вперёд
[ ] Нажми A → Rabbit движется влево
[ ] Нажми S → Rabbit движется назад
[ ] Нажми D → Rabbit движется вправо
[ ] Держи Shift + WASD → скорость х2 (рывок)
[ ] Отпусти WASD → Rabbit останавливается
[ ] Камера следует сзади-слева под углом
[ ] Rabbit не проваливается сквозь Ground
[ ] Rabbit упирается в Obstacles
```

**Если ВСЕ галочки → Этап 1 ЗАВЕРШЁН! 🎉**

---

## 🔴 ТИПИЧНЫЕ ОШИБКИ

| Проблема | Чек | Решение |
|----------|-----|---------|
| Rabbit не двигается | Логи console | Добавь Rigidbody или PlayerInput |
| Rabbit падает сквозь пол | Ground имеет BoxCollider | Добавь BoxCollider к Ground |
| Камера не следует | Main Camera tag | Установи Tag = "MainCamera" |
| Input не работает | PlayerInput в сцене | Создай объект с PlayerInput компонентом |
| Ошибка "No InputAction 'Move'" | InputSystem_Actions.cs | Регенерируй: Right-click → Generate C# |
| Zenject не инициализирует | Scene Context | Создай Scene Context (3D → Scene Context) |

---

## 📱 БЫСТРЫЕ КОМАНДЫ

| Действие | Сочетание |
|----------|-----------|
| Play/Stop | Spacebar |
| Новый Plane | Right-click Hierarchy → 3D Object → Plane |
| Новый Cube | Right-click Hierarchy → 3D Object → Cube |
| Добавить компонент | Add Component в Inspector |
| Найти в сцене | Ctrl+H (Find in Scene) |
| Перекомпилировать | Ctrl+R (или Cmd+R на Mac) |

---

## 🆘 ЕСЛИ НИЧЕГО НЕ РАБОТАЕТ

1. **Открой Console** (Window → General → Console)
2. **Скопируй ВСЕ красные ошибки**
3. **Запусти диагностику:**
   - Создай пустой объект → Add Component → Stage1Diagnostic
   - Нажми Play → посмотри отчёт в Console
4. **Пройди Шаг по Шагу:**
   - Открой STAGE1_STEP_BY_STEP.md (подробная инструкция)
   - Выполни все 5 шагов последовательно

---

## 📌 ВАЖНО

- **Сохраняй часто** (Ctrl+S)
- **Проверяй Console после каждого изменения**
- **Если добавил компонент → нажми Play**
- **Если ошибка → читай сообщение об ошибке в Console**

---

## ✨ КОД ДО ЭТОГО

**Если всё работает, переходи на Stage 2:**
- [ ] Добавить прыжок (Space)
- [ ] Добавить анимацию бега
- [ ] Добавить звуки шагов
- [ ] Улучшить камеру (follow более гладко)

