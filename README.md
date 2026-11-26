# 🐰 Bad Bunny Day

![Unity](https://img.shields.io/badge/Unity-6_(6000.2%2B)-black?style=flat&logo=unity)
![C#](https://img.shields.io/badge/Language-C%23-blue)
![Event](https://img.shields.io/badge/HSE_Game_Jam-KAPKAN-purple)
![Status](https://img.shields.io/badge/Status-Completed-success)

![Cover](Screenshots/main-banner.png)

> 🏆 **Game Jam Project**
>
> Проект разработан командой **KAPKAN** в рамках геймджема **ВШЭ II** (декабрь 2025).
>
> **Команда:**
> *   **Кувико Даниил (fuheshka)** — Team Lead, Game Design, Gameplay & Systems Programming
> *   **Шевченко Никита** — Level Design
> *   **Каруна Вадим (somacruz799)** — Art, UI/UX

**Bad Bunny Day** — это динамичная 3D-аркада от третьего лица на **Unity 6**. Вы играете за наглого зайца, которому предстоит съесть как можно больше овощей на фермерском огороде, вовремя донести добычу до сундука и не попасть под пулю фермера.

---

## 📸 Скриншоты
| Под прицелом фермера | На ферме |
|:---:|:---:|
| ![Gameplay](Screenshots/gameplay-1.png) | ![Bunny](Screenshots/gameplay-2.png) |

---

## 🎮 Геймплей и Особенности

*   **Жадность (Risk vs. Reward):** Съеденные овощи не сразу идут в зачет. Чем больше заяц ест, тем сильнее он толстеет, замедляется и становится более легкой мишенью.
*   **Банкинг очков:** Накопленные очки нужно донести до сундука (`BankZone`), чтобы сбросить лишний вес и зафиксировать счет.
*   **Жировой щит:** Накопленная масса поглощает одно попадание фермера, но сжигает часть набранных очков и дает кратковременное ускорение.
*   **Уворот:** Используйте укрытия и рывок для уклонения от выстрелов фермера.
*   **Визуал и Звук:** Яркий Low-Poly стиль, пиксельный интерфейс и процедурная анимация «желе» (Squash & Stretch).

---

## 🕹 Управление

| Действие | Клавиша |
| :--- | :--- |
| Передвижение | `WASD` / `Стрелки` |
| Обзор камеры | `Мышь` |
| Прыжок | `Space` |
| Рывок (уворот) | `Left Shift` |
| Есть овощи / Сдать очки в сундук | `E` *(удержание)* |
| Пауза / Настройки | `Esc` |

---

## 🛠 Техническая часть

*   **Движок:** Unity 6 (`6000.2+` / проверено на `6000.6.4f1`), Universal Render Pipeline (URP), Input System.
*   **Зависимости:** `Zenject` (Extenject DI), `UniTask`.
*   **Скрипты (`Assets/Scripts/Assembly-CSharp/`):**
    *   `PlayerController`, `ProceduralAnimator` — управление от третьего лица, прыжок (*Coyote Time*, *Jump Buffer*, *Variable Jump Height*), рывок, система веса/щита и процедурная анимация.
    *   `Farmer` — патрулирование, реакция на шум, прицеливание и стрельба.
    *   `EdibleObject`, `FoodSpawner`, `BankZone`, `ScoreManager`, `LevelTimer` — спавн еды, набор массы, сдача очков и таймер.
    *   `Waypoint`, `HealthUI`, `StaminaUI`, `ScoreUI`, `TimerUI`, `SettingsManager`, `SettingsMenu`, `PauseManager` — 3D-маркеры, HUD, пауза и настройки.

---

## 🚀 Запуск в Unity

1. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/Fuheshka/Bad-Bunny-Day.git
   ```
2. Откройте папку проекта в **Unity Hub** (Unity `6000.6.4f1` или `6000.2+`).
3. Откройте сцену `Assets/Scenes/MainMenu.unity` (или `Assets/Scenes/Level1.unity`) и нажмите **Play**.

<details>
<summary>Структура репозитория</summary>

- `Assets/` — сцены, скрипты, префабы, модели, анимации, звуки, шейдеры и плагины.
- `Packages/` и `ProjectSettings/` — конфигурация проекта Unity.
- `Screenshots/` — баннер и скриншоты для README.
- `VSCode_History_Backup/` — архив исходников из истории разработки (вне `Assets/`, не участвует в сборке Unity).

</details>
