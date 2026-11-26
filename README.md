# Bad Bunny Day

Восстановленный Unity-проект игры Bad Bunny Day (Chungus Bunny Day).

## Что внутри

- `Assets/`, `Packages/`, `ProjectSettings/` - полный Unity-проект (Unity `6000.6.4f1`), восстановленный из финального Windows-билда `project.zip` от 22.12.2025 (сцены `MainMenu.unity` и `Level1.unity`, настройки `SettingsManager`/`SettingsMenu`, `PauseManager`, шрифт `Faithful.ttf`, спрайты, модели, анимации, звуки, шейдеры и плагины).
- `Assets/Scripts/Assembly-CSharp/` - 18 из 21 C#-скриптов заменены оригинальными исходниками с комментариями из VS Code Local History (включая `SettingsManager`, `SettingsMenu`, `LoadSceneManager`, `ThirdPersonCamera` от 17.12 и `PlayerController` от 19.12); 3 оставшихся файла (`Farmer.cs` с обновлением под массив `_shootIndicators`, `PauseManager.cs`, `GameControls.cs`) оставлены из чистой декомпиляции AssetRipper.
- `VSCode_History_Backup/` - архив всех 107 файлов из VS Code Local History (включая `Farmer.cs` от 18.12 и черновик рефакторинга `ARCHITECTURE.md`). Папка вынесена за пределы `Assets/`, чтобы не мешать сборке в Unity.

## Открытие в Unity

1. Открыть папку проекта в Unity Hub (версия `6000.6.4f1`, проверено в batchmode без ошибок компиляции).
2. Основные сцены: `Assets/Scenes/MainMenu.unity` и `Assets/Scenes/Level1.unity`.
