using Gma.System.MouseKeyHook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OLEDSaver
{
    class HotKey
    {
        [JsonProperty("key")]
        public string KeyString;
        [JsonProperty("modifiers")]
        public string ModifiersString;
        [JsonProperty("brightness"), DefaultValue(null)]
        public float? Brightness;
        [JsonProperty("reload"), DefaultValue(false)]
        public bool Reload;

        [JsonIgnore]
        public Keys Key;
        [JsonIgnore]
        public Keys Modifiers;

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            Key = ConvertToKeys(KeyString);
            Modifiers = ConvertToKeys(ModifiersString);
        }

        private Keys ConvertToKeys(string keysString)
        {
            Keys keys = Keys.None;

            foreach (var keyString in keysString.Split(','))
            {
                if (Enum.TryParse(keyString.Trim(), out Keys key))
                {
                    keys |= key;
                }
            }

            return keys;
        }
    }

    class HotKeyTools
    {
        [JsonProperty("hotkeys")]
        public HotKey[] HotKeys;

        //[JsonIgnore]
        private IKeyboardMouseEvents _GlobalKeyHook;
        private bool _KeyHooksLoaded;

        public delegate void BrightnessChangedDelegate(float brightness);
        public event BrightnessChangedDelegate BrightnessChanged;

        public delegate void ReloadDelegate();
        public event ReloadDelegate Reload;

        public static HotKeyTools? LoadFromFile(string fileName)
        {
            if (fileName == null || !File.Exists(fileName))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<HotKeyTools>(File.ReadAllText(fileName));
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            _GlobalKeyHook = Hook.GlobalEvents();
            _GlobalKeyHook.KeyDown += GlobalHookKeyDown;
        }
        private void GlobalHookKeyDown(object sender, KeyEventArgs e)
        {
            foreach (var key in HotKeys)
            {
                if (e.KeyCode == key.Key && e.Modifiers == key.Modifiers)
                {
                    if (key.Brightness != null && BrightnessChanged != null)
                    {
                        BrightnessChanged(key.Brightness.Value);
                        e.Handled = true;
                        return;
                    }
                    else if (key.Reload && Reload != null)
                    {
                        Reload();
                        e.Handled = true;
                        return;
                    }
                }
            }
        }
    }
}
