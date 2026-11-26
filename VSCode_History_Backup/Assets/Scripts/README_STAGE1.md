# 🎮 GAMEJAM - STAGE 1 IMPLEMENTATION

## 📚 Документация по управлению и конфигурации

Если **не работает управление**, эта документация поможет исправить проблему.

---

## 📖 ДОКУМЕНТЫ (Выбери свою ситуацию):

### 1. **Управление не работает? → ЧИТАЙ ПЕРВЫМ**
📄 **[STAGE1_STEP_BY_STEP.md](./STAGE1_STEP_BY_STEP.md)**
- Пошаговая инструкция с 5 шагами
- Точные настройки каждого компонента
- Ожидаемые результаты после каждого шага
- **Начни отсюда, если что-то сломано**

---

### 2. **Визуальная справка по конфигурации**
🖼️ **[STAGE1_VISUAL_REFERENCE.md](./STAGE1_VISUAL_REFERENCE.md)**
- Как должна выглядеть Hierarchy
- Скриншоты Inspector для каждого объекта
- Примеры правильных настроек
- **Используй это для сравнения с твоей сценой**

---

### 3. **Диагностика и частые ошибки**
🔍 **[STAGE1_TROUBLESHOOTING.md](./STAGE1_TROUBLESHOOTING.md)**
- Проблема #1, #2, #3, #4 с решениями
- Что делать, если логи показывают ошибку
- Распространённые проблемы и как их исправить

---

### 4. **Быстрый чеклист**
✅ **[STAGE1_QUICK_CHECKLIST.md](./STAGE1_QUICK_CHECKLIST.md)**
- Быстрый список того, что нужно проверить
- Ожидаемые консольные логи
- Одностраничная справка (распечатай!)

---

### 5. **Изначальная инструкция по настройке**
📋 **[STAGE1_SETUP_INSTRUCTIONS.md](./STAGE1_SETUP_INSTRUCTIONS.md)**
- Основная инструкция по настройке сцены
- Шаги для физики, камеры, земли
- (Первоначальный план перед реализацией)

---

## 🚀 БЫСТРЫЙ СТАРТ

### Если ты только начинаешь:
1. Читай **STAGE1_STEP_BY_STEP.md** (пошаговая инструкция)
2. Смотри **STAGE1_VISUAL_REFERENCE.md** (для сравнения)
3. Проверь **STAGE1_QUICK_CHECKLIST.md** (финальная проверка)

### Если управление сломалось:
1. Нажми Play → посмотри **Console** (Window → General → Console)
2. Читай логи ошибок
3. Смотри **STAGE1_TROUBLESHOOTING.md** (найди свою проблему)
4. Выполни шаги из STAGE1_STEP_BY_STEP.md

### Если логи говорят ✓ но ничего не работает:
1. Запусти диагностику:
   - Создай пустой GameObject
   - Add Component → Stage1Diagnostic
   - Нажми Play → посмотри отчёт в Console
2. Сравни с **STAGE1_VISUAL_REFERENCE.md**

---

## 📝 КОДЫ И СКРИПТЫ

### Основные скрипты (в папках):

#### `/Core/`
- `GameConfig.cs` - Константы (скорость, ускорение, камера)

#### `/Input/`
- `PlayerInputState.cs` - Модель данных ввода
- `PlayerInputHandler.cs` - Чтение Input System (ITickable)

#### `/Player/`
- `PlayerModel.cs` - Ссылка на Rigidbody + velocity
- `PlayerMovement.cs` - Логика движения (velocity-based, ITickable)

#### `/Camera/`
- `CameraFollower.cs` - Следование камеры (ITickable)

#### `/Managers/`
- `GameManager.cs` - Инициализация игры

#### `/Installers/`
- `GameInstaller.cs` - Zenject DI привязки (главный монтаж)

#### `/Debug/`
- `Stage1Diagnostic.cs` - Диагностический скрипт (помощник при ошибках)

---

## 🎮 УПРАВЛЕНИЕ (ПОСЛЕ НАСТРОЙКИ)

```
WASD / Arrow Keys / Gamepad Left Stick → Движение
Shift / Gamepad LB/RB → Рывок (2x скорость, cooldown 0.5s)
```

---

## ✅ ПРОВЕРКА ЗАВЕРШЕНИЯ

**Stage 1 завершен если:**

- [x] Rabbit двигается с WASD
- [x] Shift даёт рывок (2x скорость)
- [x] Камера следует с фиксированного угла
- [x] Rabbit упирается в препятствия (не проходит сквозь)
- [x] Console показывает только ✓ логи (без ошибок)
- [x] Нет NullReferenceException или других ошибок

---

## 🔗 АРХИТЕКТУРА

```
INPUT SYSTEM
    ↓
PlayerInputHandler (ITickable) → читает Input System каждый фрейм
    ↓
PlayerInputState (модель) → хранит Move, IsDashing
    ↓
PlayerMovement (ITickable) → применяет velocity к Rigidbody
    ↓
PHYSICS → Rabbit движется!

╔════════════════════════════════════════════╗
║ ZENJECT DI АВТОМАТИЧЕСКИ ПРИВЯЗЫВАЕТ:      ║
║ Input → Movement → Physics                 ║
║ Camera → Follow Player                     ║
║ Все системы готовы и синхронизированы      ║
╚════════════════════════════════════════════╝
```

---

## 📊 ДИАГНОСТИКА ЧЕРЕЗ СКРИПТ

**Если логи показывают ошибку, используй Stage1Diagnostic:**

1. Создай пустой GameObject: `Right-click → Create Empty`
2. Назови его `Diagnostic`
3. Add Component → Stage1Diagnostic
4. Нажми Play
5. Console покажет полный отчёт что работает/не работает

---

## 💡 ЕСЛИ ЗАСТРЯЛ

**Порядок действий:**

1. **Посмотри Console** (Window → General → Console)
2. **Скопируй первую ошибку** (красное сообщение)
3. **Найди эту ошибку в STAGE1_TROUBLESHOOTING.md**
4. **Выполни шаги решения**
5. **Нажми Play снова**

---

## 🎯 ПЕРЕЙДИ НА STAGE 2

Когда Stage 1 работает идеально:
- [ ] Добавить прыжок (Space)
- [ ] Добавить анимации
- [ ] Добавить звуки
- [ ] Улучшить камеру

---

## 📞 КОНТАКТ

Если не поможет ничего:
- Проверь **все** документы выше
- Запусти Stage1Diagnostic
- Убедись что сохранил сцену (Ctrl+S)
- Перезагрузи Unity (File → Close Project → Open Project)

---

**Stage 1 - Пошли! 🚀**

