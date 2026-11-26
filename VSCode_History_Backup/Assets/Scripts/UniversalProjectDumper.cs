using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class UniversalProjectDumper : EditorWindow
{
    // --- НАСТРОЙКИ ---
    
    // Имя выходного файла
    private const string OUTPUT_FILE_NAME = "Project_Code_Dump.txt";

    // Список папок, которые мы ИГНОРИРУЕМ (плагины, ассеты, библиотеки)
    // Скрипт будет пропускать файлы, если в их пути встречается любое из этих слов.
    private static readonly List<string> IgnoredPathKeywords = new List<string>()
    {
        // Системные и стандартные
        "/Plugins/",
        "/Editor/",         // Обычно редакторские скрипты не нужны для контекста геймплея
        "/Resources/",      // Часто там лежат ассеты, а не код, но если код есть - можно убрать эту строку
        "/StreamingAssets/",
        
        // Популярные плагины (добавляй сюда свои)
        "TextMesh Pro",
        "Cinemachine",
        "Odin Inspector",
        "Newtonsoft",       // Json.NET
        "DemiLib",          // DOTween
        "DOTween",
        "LeanTween",
        "ProBuilder",
        "Polybrush",
        "PostProcessing",
        
        // Архитектура и Утилиты (то, что ты просил убрать)
        "Zenject",
        "Extenject",
        "UniTask",
        "UniRx",
        "VContainer",
        "NaughtyAttributes",
        
        // Сеть и Сервисы
        "Mirror",
        "Photon",
        "FishNet",
        "GoogleMobileAds",
        "FacebookSDK",
        
        // Тесты
        "/Tests/",
        "TestRunner"
    };

    // Расширения файлов, которые мы хотим собрать
    private static readonly string[] AllowedExtensions = { ".cs", ".shader", ".hlsl", ".cginc" };

    // -----------------

    [MenuItem("Tools/Dump Project Code (Smart Filter)")]
    public static void DumpScripts()
    {
        string projectRoot = Path.GetDirectoryName(Application.dataPath); // Папка с проектом (над Assets)
        string outputPath = Path.Combine(projectRoot, OUTPUT_FILE_NAME);
        
        StringBuilder sb = new StringBuilder();
        
        sb.AppendLine($"--- UNIVERSAL PROJECT DUMP ---");
        sb.AppendLine($"Project: {Application.productName}");
        sb.AppendLine($"Date: {System.DateTime.Now}");
        sb.AppendLine($"Unity Version: {Application.unityVersion}");
        sb.AppendLine("--------------------------------------------------\n");

        // 1. Ищем все файлы в Assets
        string[] allFiles = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        
        int processedCount = 0;
        int ignoredCount = 0;

        try
        {
            float progress = 0f;
            for (int i = 0; i < allFiles.Length; i++)
            {
                string filePath = allFiles[i];
                
                // Отображение прогресс бара (для больших проектов)
                if (i % 50 == 0)
                {
                    progress = (float)i / allFiles.Length;
                    if (EditorUtility.DisplayCancelableProgressBar("Dumping Scripts", $"Processing: {Path.GetFileName(filePath)}", progress))
                    {
                        Debug.LogWarning("Dump cancelled by user.");
                        break;
                    }
                }

                // --- ФИЛЬТРАЦИЯ ---

                // 1. Проверка расширения
                string ext = Path.GetExtension(filePath).ToLower();
                if (!AllowedExtensions.Contains(ext)) continue;

                // 2. Нормализация пути для проверки
                string normalizedPath = filePath.Replace("\\", "/");
                string relativePath = "Assets" + normalizedPath.Substring(Application.dataPath.Length);

                // 3. Игнорируем сам этот скрипт дампера
                if (normalizedPath.Contains("UniversalProjectDumper.cs")) continue;

                // 4. Проверка на черный список папок
                bool isIgnored = false;
                foreach (var keyword in IgnoredPathKeywords)
                {
                    // Проверяем, содержит ли путь запрещенное слово
                    // Используем IgnoreCase для надежности
                    if (relativePath.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        isIgnored = true;
                        break;
                    }
                }

                if (isIgnored)
                {
                    ignoredCount++;
                    continue;
                }

                // --- ЗАПИСЬ ---
                
                sb.AppendLine($"--- FILE: {relativePath} ---");
                try 
                {
                    sb.AppendLine(File.ReadAllText(filePath));
                }
                catch (System.Exception ex)
                {
                    sb.AppendLine($"[ERROR READING FILE: {ex.Message}]");
                }
                sb.AppendLine($"--- END FILE ---\n");
                
                processedCount++;
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        // Сохранение
        File.WriteAllText(outputPath, sb.ToString());
        
        // Открытие файла
        System.Diagnostics.Process.Start(outputPath);
        
        Debug.Log($"✅ <b>DUMP COMPLETE!</b>\n" +
                  $"Saved to: {outputPath}\n" +
                  $"Included Files: {processedCount}\n" +
                  $"Ignored Files: {ignoredCount} (Plugins/ThirdParty)");
    }
}