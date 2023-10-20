using Newtonsoft.Json;
using System.ComponentModel;
using WindowsGrep.Common;

namespace WindowsGrep.Configuration
{
    public class ConfigurationManager
    {
        #region Fields..
        private const string CONFIG_FILENAME = "config.json";

        private string _applicationDataDirectory;
        #endregion Fields..

        #region Properties..
        public Dictionary<ConfigItem, object> ConfigItemCollection { get; set; }

        private static ConfigurationManager _instance;
        public static ConfigurationManager Instance
        {
            get => _instance ?? (_instance = new ConfigurationManager());
        }
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        public void Initialize()
        {
            string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _applicationDataDirectory = Path.Combine(appDataDirectory, AppDomain.CurrentDomain.FriendlyName);

            if (!Directory.Exists(_applicationDataDirectory))
                Directory.CreateDirectory(_applicationDataDirectory);

            LoadConfiguration();
        }

        public void LoadDefaultConfiguration()
        {
            ConfigItemCollection = new Dictionary<ConfigItem, object>();

            var configItemCollection = EnumUtils.GetValues<ConfigItem>().ToList();
            configItemCollection.ForEach(configItem => ConfigItemCollection[configItem] = configItem.GetCustomAttribute<DefaultValueAttribute>()?.Value);
        }

        private void LoadConfiguration()
        {
            try
            {
                string configFilepath = Path.Combine(_applicationDataDirectory, CONFIG_FILENAME);

                // Load existing
                if (File.Exists(configFilepath))
                {
                    string jsonRaw = File.ReadAllText(configFilepath);
                    ConfigItemCollection = JsonConvert.DeserializeObject<Dictionary<ConfigItem, object>>(jsonRaw);

                    // Backfill defaults for new/missing settings
                    EnumUtils.GetValues<ConfigItem>()?.ToList().ForEach(x =>
                    {
                        if (!ConfigItemCollection.ContainsKey(x))
                            ConfigItemCollection[x] = x.GetCustomAttribute<DefaultValueAttribute>().Value;
                    });
                }

                // Load and save defaults
                else
                    LoadDefaultConfiguration();

                SaveConfiguration();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not load configuration - {ex.Message}");
            }
        }

        public void SaveConfiguration()
        {
            try
            {
                string configFilepath = Path.Combine(_applicationDataDirectory, CONFIG_FILENAME);
                string jsonRaw = JsonConvert.SerializeObject(ConfigItemCollection, Formatting.Indented);

                File.WriteAllText(configFilepath, jsonRaw);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not save configuration - {ex.Message}");
            }
        }
        #endregion Methods..
    }
}