# 🚀 НЕМЕДЛЕННО НАЧНИ С ЭТОГО

**У тебя не работает управление. Выполни это ПРЯМО СЕЙЧАС:**

---

## ШАГ 1: Запусти диагностику (2 минуты)

1. **Открой SampleScene.unity в Unity**

2. **Создай пустой диагностический объект:**
   - Right-click в Hierarchy
   - Create Empty
   - Назови его `DiagHelper`

3. **Добавь диагностический компонент:**
   - Выбери DiagHelper
   - Add Component → Stage1Diagnostic

4. **Нажми Play**

5. **Открой Console:**
   - Window → General → Console (или Ctrl+Shift+C)

6. **Смотри отчёт:**
   - Там будут все проверки
   - Ищи красные ✗ (проблемы)
   - Ищи зелёные ✓ (работает)

---

## ШАГ 2: По каждому ✗ следуй исправлению ниже

### Если видишь:
```
✗ NO ANIMATOR FOUND!
```
**Решение:**
1. В Hierarchy найди Rabbit (модель зайца)
2. Выбери его
3. Add Component → Animator

---

### Если видишь:
```
✗ NO RIGIDBODY!
```
**Решение:**
1. Выбери Rabbit
2. Add Component → Physics → Rigidbody
3. Настройки Rigidbody:
   - Mass: `1`
   - Drag (Linear Damping): `0.3`
   - Use Gravity: `✓` (галочка должна быть!)
   - Freeze Rotation X: `✓`
   - Freeze Rotation Z: `✓`
4. Нажми Play

---

### Если видишь:
```
✗ NO CAPSULE COLLIDER!
```
**Решение:**
1. Выбери Rabbit
2. Add Component → Physics → Capsule Collider
3. Настройки:
   - Center: `(0, 1, 0)`
   - Radius: `0.5`
   - Height: `2`
   - Direction: `Y-Axis`
4. Нажми Play

---

### Если видишь:
```
✗ NO MAIN CAMERA! Check that camera has 'MainCamera' tag.
```
**Решение:**
1. В Hierarchy найди Main Camera
2. Вверху в Inspector жми **Tag** dropdown
3. Выбери **MainCamera** (или создай новый, если его нет)
4. Нажми Play

---

### Если видишь:
```
✗ NO PLAYER INPUT!
```
**Решение:**
1. Right-click в Hierarchy → Create Empty
2. Назови `PlayerInput`
3. Add Component → Player Input
4. В компоненте Player Input:
   - Actions: выбери из dropdown `InputSystem_Actions`
5. Нажми Play

---

### Если видишь:
```
ArgumentException: There is no InputAction 'Move'
```
**Решение:**
1. В папке Assets найди `InputSystem_Actions.inputactions` файл
2. Right-click на него → **Generate C# Class**
3. Жди пока Unity перезагружается (30 сек)
4. Снова выбери PlayerInput компонент
5. В поле Actions выбери `InputSystem_Actions`
6. Нажми Play

---

### Если видишь:
```
⚠ Skipping (no Animator found)
```
**Это проблема!**
- Решение: выполни исправление для "NO ANIMATOR FOUND"

---

## ШАГ 3: После исправления каждой ошибки

1. **Сохрани сцену:** Ctrl+S
2. **Нажми Play**
3. **Смотри Console**
4. Если новых ошибок нет → переходи к следующей

---

## ШАГ 4: Когда диагностика показывает ВСЕ ✓

**Логи должны быть:**
```
[1] CHECKING FOR ANIMATOR (Rabbit)...
   ✓ Found Animator on: Rabbit

[2] CHECKING FOR RIGIDBODY...
   ✓ Rigidbody found!
     Mass: 1, Drag: 0.3
     Use Gravity: True

[3] CHECKING FOR CAPSULE COLLIDER...
   ✓ Capsule Collider found!
     Center: (0, 1, 0), Radius: 0.5, Height: 2

[4] CHECKING FOR MAIN CAMERA...
   ✓ Main Camera found!
     Tag: MainCamera

[5] CHECKING FOR PLAYER INPUT COMPONENT...
   ✓ PlayerInput found on: PlayerInput

[6] CHECKING INPUT ACTIONS...
   ✓ 'Move' action exists
   ✓ 'Sprint' action exists

[7] CHECKING FOR GROUND PLANE...
   ✓ Ground plane found: Ground

[8] CHECKING FOR ZENJECT INSTALLER...
   ✓ GameInstaller found!
```

**Если видишь ВСЕ галочки ✓:**
- Нажми Play ещё раз (без диагностики)
- Попробуй WASD → Rabbit должен двигаться!

---

## ШАГ 5: Если диагностика ✓ но управление не работает

**Это значит проблема в другом месте. Проверь:**

1. **Нажми Play** (не диагностику, а обычный Play)

2. **Смотри Console на ДРУГИЕ красные ошибки:**
   - Скопируй первую красную ошибку (не из диагностики)

3. **Смотри в документе:** `/STAGE1_TROUBLESHOOTING.md`
   - Найди свою ошибку там

---

## ШАГ 6: Финальный тест

**Когда диагностика показала ВСЕ ✓:**

1. **Удали DiagHelper объект** (он для диагностики)
2. **Нажми Play**
3. **Тестируй управление:**
   - Нажми **W** → Rabbit движется вперёд?
   - Нажми **A** → влево?
   - Нажми **S** → назад?
   - Нажми **D** → вправо?
   - Держи **Shift+W** → быстрее?
   - Отпусти WASD → останавливается?

**Если ВСЕ работает → ЭТАП 1 ЗАВЕРШЁН! 🎉**

---

## 🆘 ЕСЛИ ДИАГНОСТИКА НЕ ПОМОГЛА

**Сделай следующее:**

1. **Перекомпилируй:**
   - Assets → Reimport All
   - Жди 30 секунд

2. **Проверь версию Input System:**
   - Window → TextMesh Pro → Import
   - Потом: Ctrl+R (перекомпилировать)

3. **Перезагрузи Unity:**
   - File → Close Scene
   - File → Open Recent → SampleScene
   - Или полностью закрой и открой Unity

4. **Очисти кэш (если ничего выше не помогло):**
   - Закрой Unity
   - Удали папку `Library/` из проекта
   - Открой Unity снова

---

## 📞 ЕСЛИ НУЖНА ПОЛНАЯ СПРАВКА

Когда диагностика помогла, прочитай подробные гайды:

- 📄 **STAGE1_STEP_BY_STEP.md** - пошаговая инструкция
- 🖼️ **STAGE1_VISUAL_REFERENCE.md** - как должно выглядеть
- 📖 **README_STAGE1.md** - общий обзор документов

---

**НАЧНИ СЕЙЧАС! 🚀 Выполни ШАГ 1 и запусти диагностику.**

