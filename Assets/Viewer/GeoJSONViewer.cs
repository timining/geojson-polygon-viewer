using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using GeoJSON;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GeoJSONViewer
{
    public class GeoJSONViewer : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _pathInput;

        [SerializeField]
        private Button _browseButton;

        [SerializeField]
        private TMP_Text _statusText;

        [SerializeField]
        private OrbitCamera _orbitCamera;

        private GameObject _currentRoot;
        private const float LineThickness = 2f;

        private void Awake()
        {
            EventSystem es = FindAnyObjectByType<EventSystem>();
            if (es == null) return;

            if (es.GetComponent<StandaloneInputModule>() == null) return;

            Type inputSystemModule = Type.GetType(
                "UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemModule == null) return;

            if (es.GetComponent(inputSystemModule) == null)
            {
                DestroyImmediate(es.GetComponent<StandaloneInputModule>());
                es.gameObject.AddComponent(inputSystemModule);
            }
        }

        private void Start()
        {
            _browseButton.onClick.AddListener(Browse);
            SetStatus("Selecciona un archivo GeoJSON.");
        }

        private void Browse()
        {
            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-NoProfile -Command \"Add-Type -AssemblyName System.Windows.Forms; $d = New-Object System.Windows.Forms.OpenFileDialog; $d.Filter = 'GeoJSON|*.geojson;*.json|Todos|*.*'; if ($d.ShowDialog() -eq 'OK') { Write-Output $d.FileName }\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string path = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(path))
            {
                _pathInput.text = path;
                Load();
            }
        }

        private void Load()
        {
            string path = _pathInput.text.Trim();

            if (!File.Exists(path))
            {
                SetStatus($"Archivo no encontrado: {path}");
                return;
            }

            if (_currentRoot != null)
            {
                Destroy(_currentRoot);
                _currentRoot = null;
            }

            SetStatus("Cargando...");

            string json = File.ReadAllText(path);
            GeoJSONObject geoJSON = GeoJSONObject.Parse(json);

            if (geoJSON == null)
            {
                SetStatus("Error: no se pudo parsear el GeoJSON.");
                return;
            }

            if (!(geoJSON is FeatureCollection fc))
            {
                SetStatus($"Tipo no soportado: {geoJSON.GetType().Name}. Solo FeatureCollection.");
                return;
            }

            _currentRoot = GeoJSONHelper.ProcessFeatureCollection(fc, LineThickness, Color.white, Color.white);

            SetStatus($"Cargado: {fc.features.Count} features.");
            StartCoroutine(FitNextFrame());
        }

        private IEnumerator FitNextFrame()
        {
            yield return null;
            Bounds bounds = OrbitCamera.CalculateBounds(_currentRoot);
            _orbitCamera.FitToView(bounds);
        }

        private void SetStatus(string message)
        {
            if (_statusText != null)
            {
                _statusText.text = message;
            }
        }
    }
}
