# 🔴 КРИТИЧНОЕ: Видишь только [GameManager] лог

## ПРОБЛЕМА

```
[GameManager] Game initialized
```

**Это значит Zenject НЕ инициализирует!**

---

## РЕШЕНИЕ (2 МИНУТЫ)

### Шаг 1: Создай Scene Context

```
Hierarchy:
Right-click → 3D Object → Scene Context
```

**Это объект для Zenject. БЕЗ НЕГО Zenject НЕ РАБОТАЕТ!**

---

### Шаг 2: Добавь GameInstaller

```
Выбери Scene Context
Inspector → Add Component → Game Installer
```

---

### Шаг 3: Создай PlayerInput

```
Hierarchy:
Right-click → Create Empty → назови PlayerInput
Add Component → Player Input
```

В компоненте "Player Input":
```
Actions: InputSystem_Actions (выбери из dropdown)
```

---

### Шаг 4: Сохрани и Play

```
Ctrl+S (сохрани сцену)
Нажми Play
Смотри Console
```

---

## ЕСЛИ ВСЕ ЛОГИ ПОЯВИЛИСЬ

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
[GameManager] Game initialized
```

**ОТЛИЧНО! Теперь тестируй WASD → Rabbit должен двигаться!**

---

## ЕСЛИ ВСЕГДА ТОЛЬКО GAMEMANAGER

**Значит Scene Context или GameInstaller не настроены.**

1. Проверь Hierarchy:
   - ✓ Scene Context существует
   - ✓ На Scene Context есть компонент "Game Installer"

2. Проверь PlayerInput:
   - ✓ PlayerInput объект в сцене
   - ✓ Есть компонент "Player Input"
   - ✓ Actions = InputSystem_Actions

3. Перекомпилируй:
   ```
   Assets → Reimport All
   ```

4. Нажми Play снова

---

**ВСЁ! Этого достаточно для 90% проблем. 🚀**

