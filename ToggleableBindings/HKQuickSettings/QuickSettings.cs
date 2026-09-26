#nullable enable
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using GlobalEnums;
using Modding;
using Modding.Patches;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ToggleableBindings.Extensions;
using ToggleableBindings.JsonNet;
using ToggleableBindings.Utility;
using UnityEngine;

namespace ToggleableBindings.HKQuickSettings
{
    internal class QuickSettings
    {
        public event Action? Initialized;

        public event Action? Unloaded;

        public event Action? GlobalSettingsSaved;

        public event Action? GlobalSettingsLoaded;

        public event Action<int>? SaveSettingsSaved;

        public event Action<int>? SaveSettingsLoaded;

        private const string SettingsGlobalFileName = "Settings.Global.json";
        private const string SettingsSaveFileName = "Settings.Save{0}.json";

        private static readonly string _baseDataPath = Application.persistentDataPath + '/';
        private static readonly Dictionary<string, string> _modSettingsPathMap = new();

        private static readonly JsonSerializerSettings _serializerSettings = new()
        {
            Formatting = Formatting.Indented,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Populate,
            ReferenceLoopHandling = ReferenceLoopHandling.Error,
            TypeNameHandling = TypeNameHandling.Auto,
            ConstructorHandling = ConstructorHandling.Default,
            ContractResolver = ShouldSerializeContractResolver.Instance,
            SerializationBinder = JsonSerializationBinder.Instance,
            Error = LogAndIgnoreSerializationBindingErrors
        };

        private static JsonSerializer Serializer { get; } = JsonSerializer.Create(_serializerSettings);

        private Assembly? _owningAssembly;
        private string? _modName;
        private readonly List<QuickSettingInfo> _globalSettings = new();
        private readonly List<QuickSettingInfo> _saveSettings = new();

        private string SettingsDirectory => _baseDataPath + ModName + "." + nameof(QuickSettings) + '/';

        protected string ModName
        {
            get => _modName ?? throw new InvalidOperationException($"This {nameof(QuickSettings)} object wasn't initialized properly.");
            private set => _modName = value;
        }

        public int? CurrentSaveSlot { get; internal set; }

        public QuickSettings()
        {
            LogDebug("Attempting initialization via parameterless ctor...");

            _owningAssembly = Assembly.GetCallingAssembly();
            var modType = _owningAssembly
                .GetTypes()
                .Where(t => typeof(Mod).IsAssignableFrom(t))
                .FirstOrDefault();

            Initialize(modType?.Name);
        }

        public QuickSettings(Mod mod) : this(mod.GetType()) { }

        public QuickSettings(string modName)
        {
            _owningAssembly = Assembly.GetCallingAssembly();
            Initialize(modName);
        }

        public QuickSettings(Type modType)
        {
            if (!modType.IsAssignableTo(typeof(Mod)))
                throw new ArgumentException("Cannot initialize a QuickSettings instance with a type that does not derive from Mod.");

            Initialize(modType.Name);
        }

        [MemberNotNull(nameof(ModName), nameof(_owningAssembly))]
        protected void Initialize(string? modName)
        {
            if (modName == null)
                throw new ArgumentNullException(nameof(modName), $"Couldn't find the name of the mod to use. Try using '{nameof(QuickSettings)}(Type)'.");

            ModName = modName;

            _owningAssembly ??= Assembly.GetCallingAssembly();
            RegisterDefinedSettings();
            LoadGlobalSettings();
            AddHooks();

            if (GameManager.instance && GameManager.instance.gameState is GameState.PLAYING or GameState.PAUSED)
            {
                CurrentSaveSlot = GameManager.instance.profileID;
                LoadSaveSettings();
            }

            Initialized?.Invoke();
        }

        public void Unload()
        {
            RemoveHooks();
            SaveAllSettings();
            Unloaded?.Invoke();
        }

        public void AddSetting(MemberInfo member, string? settingName = null, bool isPerSave = false)
        {
            QuickSettingInfo settingInfo = new(member, settingName, isPerSave);
            var settings = GetSettingsList(isPerSave);
            settings.Add(settingInfo);
        }

        public bool RemoveSetting(string settingName, bool isPerSave)
        {
            return RemoveSetting(si => si.Name == settingName, isPerSave);
        }

        public bool RemoveSetting(MemberInfo member, bool isPerSave)
        {
            return RemoveSetting(si => si.MemberInfo == member, isPerSave);
        }

        public bool RemoveSetting(Func<QuickSettingInfo, bool> predicate, bool isPerSave)
        {
            var settings = GetSettingsList(isPerSave);
            int removeAt = settings.FindIndex(si => predicate(si));
            if (removeAt is -1)
                return false;

            settings.RemoveAt(removeAt);
            return true;
        }

        private void RegisterDefinedSettings()
        {
            const BindingFlags memberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            _globalSettings.Clear();
            _saveSettings.Clear();
            foreach (var type in _owningAssembly!.GetTypes())
            {
                foreach (var member in type.GetMembers(memberFlags))
                {
                    var attr = member.GetCustomAttribute<QuickSettingAttribute>(false);
                    if (attr == null)
                        continue;

                    AddSetting(member, attr.SettingName, attr.IsPerSave);

                    LogDebug($"Registered setting '{attr.SettingName}'.");
                }
            }
        }

        private List<QuickSettingInfo> GetSettingsList(bool isPerSave)
        {
            return !isPerSave ? _globalSettings : _saveSettings;
        }

        public string GetGlobalSettingsPath()
        {
            return SettingsDirectory + SettingsGlobalFileName;
        }

        public string? GetSaveSettingsPath(int? saveSlotID = null)
        {
            if (saveSlotID == null && CurrentSaveSlot == null)
                return null;

            return SettingsDirectory + string.Format(SettingsSaveFileName, saveSlotID ?? CurrentSaveSlot);
        }

        public void SaveAllSettings()
        {
            SaveGlobalSettings();
            if (CurrentSaveSlot != null)
                SaveSaveSettings();
        }

        public void SaveGlobalSettings()
        {
            LogDebug("Saving global settings...");

            EnsureSettingsDirectory();
            var filePath = GetGlobalSettingsPath();

            try
            {
                SerializeAndSave(filePath, _globalSettings);
                GlobalSettingsSaved?.Invoke();

                LogDebug("Global settings saved!");
            }
            catch (Exception) when (LogExc("Failed to save settings."))
            { }
            catch (IOException ex)
            {
                LogError("There was a problem writing the settings file: " + ex.Message);
                LogDebug(ex);
            }
            catch (JsonSerializationException ex)
            {
                LogError("Failed to serialize the data to the settings file: " + ex.Message);
                LogDebug(ex);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                LogDebug(ex);
            }
        }

        private void LoadGlobalSettings()
        {
            LogDebug("Loading global settings...");

            var filePath = GetGlobalSettingsPath();
            if (!File.Exists(filePath))
                return;

            try
            {
                LoadAndDeserialize(filePath, _globalSettings);
                GlobalSettingsLoaded?.Invoke();

                LogDebug("Global settings loaded!");
            }
            catch (Exception) when (LogExc("Failed to load settings."))
            { }
            catch (FileNotFoundException ex)
            {
                LogError("Tried to read from a settings file that doesn't exist.");
                LogDebug(ex);
            }
            catch (IOException ex)
            {
                LogError("There was a problem reading the settings file: " + ex.Message);
                LogDebug(ex);
            }
            catch (JsonSerializationException ex)
            {
                LogError("Failed to deserialize the data within the settings file: " + ex.Message);
                LogDebug(ex);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                LogDebug(ex);
            }
        }

        public void SaveSaveSettings()
        {
            if (CurrentSaveSlot == null)
                throw new InvalidOperationException("Can't save save-specific settings when a save isn't loaded.");

            LogDebug($"Saving settings for save slot {CurrentSaveSlot}...");

            EnsureSettingsDirectory();

            string filePath = GetSaveSettingsPath()!;

            try
            {
                SerializeAndSave(filePath, _saveSettings);

                SaveSettingsSaved?.Invoke(CurrentSaveSlot.GetValueOrDefault());
                LogDebug("Save settings saved!");
            }
            catch (Exception) when (LogExc("Failed to save settings."))
            { }
            catch (IOException ex)
            {
                LogError("There was a problem writing the settings file: " + ex.Message);
                LogDebug(ex);
            }
            catch (JsonSerializationException ex)
            {
                LogError("Failed to serialize the data to the settings file: " + ex.Message);
                LogDebug(ex);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                LogDebug(ex);
            }
        }

        private void LoadSaveSettings()
        {
            if (CurrentSaveSlot == null)
                throw new InvalidOperationException("Can't load save-specific settings when a save isn't loaded.");

            LogDebug($"Loading settings for save slot {CurrentSaveSlot}...");

            var filePath = GetSaveSettingsPath()!;
            if (!File.Exists(filePath))
                return;

            try
            {
                LoadAndDeserialize(filePath, _saveSettings);
                SaveSettingsLoaded?.Invoke(CurrentSaveSlot.GetValueOrDefault());
                LogDebug("Save settings loaded!");
            }
            catch (Exception) when (LogExc("Failed to load settings."))
            { }
            catch (FileNotFoundException ex)
            {
                LogError("Tried to read from a settings file that doesn't exist: " + filePath);
                LogDebug(ex);
            }
            catch (IOException ex)
            {
                LogError("There was a problem reading the settings file: " + ex.Message);
                LogDebug(ex);
            }
            catch (JsonSerializationException ex)
            {
                LogError("Failed to deserialize the data within the settings file: " + ex.Message);
                LogDebug(ex);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                LogDebug(ex);
            }
        }

        private void EnsureSettingsDirectory()
        {
            Directory.CreateDirectory(SettingsDirectory);
        }

        private static void LogAndIgnoreSerializationBindingErrors(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
        {
            if (args.CurrentObject == args.ErrorContext.OriginalObject && AnyBinderExceptions(args.ErrorContext.Error))
            {
                ToggleableBindings.Instance.LogError(args.ErrorContext.Error.Message);
                ToggleableBindings.Instance.Log("Ignore the above error if you recently uninstalled or disabled a mod.");
                args.ErrorContext.Handled = true;
            }

            static bool AnyBinderExceptions(Exception ex)
            {
                return InnerExceptionsAndSelf(ex).Any(e => e is JsonSerializationBinderException);
            }

            static IEnumerable<Exception> InnerExceptionsAndSelf(Exception ex)
            {
                while (ex != null)
                {
                    yield return ex;
                    ex = ex.InnerException;
                }
            }
        }

        private void AddHooks()
        {
            On.GameManager.OnApplicationQuit += GameManager_OnApplicationQuit;
            On.GameManager.SetState += GameManager_SetState;
            On.GameManager.LoadGame += GameManager_LoadGame;
            On.GameManager.SaveGame_int_Action1 += GameManager_SaveGame;
        }

        private void RemoveHooks()
        {
            On.GameManager.OnApplicationQuit -= GameManager_OnApplicationQuit;
            On.GameManager.SetState -= GameManager_SetState;
            On.GameManager.LoadGame -= GameManager_LoadGame;
            On.GameManager.SaveGame_int_Action1 -= GameManager_SaveGame;
        }

        private void GameManager_OnApplicationQuit(On.GameManager.orig_OnApplicationQuit orig, GameManager self)
        {
            SaveGlobalSettings();
            orig(self);
        }

        private void GameManager_SetState(On.GameManager.orig_SetState orig, GameManager self, GameState newState)
        {
            orig(self, newState);
            if (newState == GlobalEnums.GameState.MAIN_MENU)
                CurrentSaveSlot = null;
        }

        private void GameManager_LoadGame(On.GameManager.orig_LoadGame orig, GameManager self, int saveSlot, Action<bool> callback)
        {
            orig(self, saveSlot, callback);
            CurrentSaveSlot = saveSlot;

            CoroutineBuilder.New
                .WithYield(new WaitWhile(() => !HeroController.instance), null)
                .WithAction(LoadSaveSettings)
                .Start();
        }

        private void GameManager_SaveGame(On.GameManager.orig_SaveGame_int_Action1 orig, GameManager self, int saveSlot, Action<bool> callback)
        {
            CurrentSaveSlot = saveSlot;
            SaveSaveSettings();
            orig(self, saveSlot, callback);
        }

        private void SerializeAndSave(string filePath, in List<QuickSettingInfo> settings)
        {
            var settingsData = new List<QuickSettingData>();
            foreach (var setting in settings)
            {
                const BindingFlags methodFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                var declaringType = setting.MemberInfo.DeclaringType;

                var onSerializing = declaringType.GetMethod("OnSerializing", methodFlags);
                onSerializing?.Invoke(null, null);

                var settingData = new QuickSettingData
                {
                    SettingName = setting.Key,
                    SettingValue = setting.MemberInfo.GetMemberValue(null)
                };
                settingsData.Add(settingData);

                var onSerialized = declaringType.GetMethod("OnSerialized", methodFlags);
                onSerialized?.Invoke(null, null);
            }

            string serialized = JsonConvert.SerializeObject(settingsData, _serializerSettings);
            File.WriteAllText(filePath, serialized);
        }

        private void LoadAndDeserialize(string filePath, in List<QuickSettingInfo> settings)
        {
            const BindingFlags methodFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            string json = File.ReadAllText(filePath);
            var jArray = JArray.Parse(json);

            var settingsDict = settings.ToDictionary(s => s.Key);
            foreach (var setting in jArray.Children<JObject>())
            {
                var settingName = setting.Value<string>(nameof(QuickSettingData.SettingName));
                if (string.IsNullOrEmpty(settingName))
                {
                    ToggleableBindings.Instance.LogError("Encountered setting with an empty name, skipping.");
                    continue;
                }

                string settingKey = settingName!;
                if (settingsDict.TryGetValue(settingKey, out var settingInfo))
                {
                    var declaringType = settingInfo.MemberInfo.DeclaringType;
                    if (declaringType == null)
                    {
                        ToggleableBindings.Instance.LogError("Couldn't find declaring type for setting: " + settingKey);
                        continue;
                    }

                    var onDeserializing = declaringType.GetMethod("OnDeserializing", methodFlags);
                    onDeserializing?.Invoke(null, null);

                    Type memberType = settingInfo.MemberInfo.GetUnderlyingType();
                    var valueToken = setting.GetValue(nameof(QuickSettingData.SettingValue));
                    if (valueToken == null)
                    {
                        ToggleableBindings.Instance.LogError("Couldn't find value token for setting: " + settingKey);
                        continue;
                    }

                    var memberValue = valueToken.ToObject(memberType, Serializer);
                    settingInfo.MemberInfo.SetMemberValue(null, memberValue);

                    var onDeserialized = declaringType.GetMethod("OnDeserialized", methodFlags);
                    onDeserialized?.Invoke(null, null);
                }
                else
                    ToggleableBindings.Instance.LogError("Couldn't find a setting with the specified key: " + settingKey);
            }
        }

        private void Log(object? message = null, Modding.LogLevel logLevel = Modding.LogLevel.Info)
        {
            string prefix = nameof(QuickSettings);
            if (_modName != null)
                prefix += $" ({_modName})";

            string full = $"[{prefix}] - {message}";
            Modding.Logger.Log(full, logLevel);
        }

        private void LogDebug(object? message) => Log(message, Modding.LogLevel.Debug);

        private void LogWarn(object? message) => Log(message, Modding.LogLevel.Warn);

        private void LogError(object? message) => Log(message, Modding.LogLevel.Error);

        private bool LogExc(object? message)
        {
            Log(message, Modding.LogLevel.Error);
            return false;
        }
    }
}
