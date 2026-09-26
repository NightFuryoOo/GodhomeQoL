#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using ToggleableBindings.Debugging;
using ToggleableBindings.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ToggleableBindings
{
    internal sealed class FakePrefab
    {
        private static readonly GameObject _prefabContainer;

        private const string NamePrefix = "[Prefab] ";
        private readonly GameObject _prefab;
        private readonly string _prefabName;

        public string Name => _prefabName[NamePrefix.Length..];

        public GameObject UnsafeGameObject => _prefab;

        static FakePrefab()
        {
            _prefabContainer = new GameObject("[[Prefabs]]");
            _prefabContainer.SetActive(false);
            Object.DontDestroyOnLoad(_prefabContainer);
        }

        internal static void Unload()
        {
            Object.DestroyImmediate(_prefabContainer, true);
        }

        public FakePrefab(GameObject original, string? prefabName = null, bool setActive = false)
        {
            if (original == null)
                throw new ArgumentNullException(nameof(original));

            _prefabName = NamePrefix + (prefabName ?? original.name);
            _prefab = Object.Instantiate(original, _prefabContainer.transform);
            _prefab.name = _prefabName;
            if (setActive)
                _prefab.SetActive(true);
        }

        public GameObject Instantiate(Transform? parent = null, bool instantiateInWorldSpace = true)
        {
            var output = Object.Instantiate(_prefab, parent, instantiateInWorldSpace);
            output.name = _prefabName[NamePrefix.Length..];

            return output;
        }

        public static FakePrefab Create(string? name, Action<GameObject>? initializer)
        {
            var tempGO = ObjectFactory.Create("TemporaryPrefabCopy", _prefabContainer);

            initializer?.Invoke(tempGO);

            var output = new FakePrefab(tempGO, name);
            Object.DestroyImmediate(tempGO, true);
            return output;
        }

        public static FakePrefab CreateCopy(FakePrefab original, string? name, Action<GameObject>? initializer)
        {
            if (original == null)
                throw new ArgumentNullException(nameof(original));

            var tempGO = original.Instantiate(_prefabContainer.transform);
            tempGO.name = "TemporaryPrefabCopy";

            initializer?.Invoke(tempGO);

            var output = new FakePrefab(tempGO, name);
            Object.DestroyImmediate(tempGO, true);
            return output;
        }
    }
}