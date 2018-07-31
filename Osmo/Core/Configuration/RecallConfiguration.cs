namespace Osmo.Core.Configuration
{
    class RecallConfiguration : ConfigurationFile
    {
        #region Singleton implementation
        private static RecallConfiguration instance;
        public static RecallConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new RecallConfiguration();

                return instance;
            }
        }
        #endregion

        private double mVolume;
        private bool mIsMuted;

        public string LastSkinPathEditor { get; set; }

        public string LastSkinPathMixerLeft { get; set; }

        public string LastSkinPathMixerRight { get; set; }

        public double Volume
        {
            get => mVolume;
            set
            {
                mVolume = value;
            }
        }

        public bool IsMuted
        {
            get => mIsMuted;
            set
            {
                mIsMuted = value;
            }
        }

        private RecallConfiguration() : base("recall")
        {
            Load();
        }

        public void Save()
        {
            Content = new string[]
            {
                "Volume:" + Volume,
                "IsMuted:" + IsMuted,
                "LastSkinPathEditor:" + LastSkinPathEditor,
                "LastSkinPathMixerLeft:" + LastSkinPathMixerLeft,
                "LastSkinPathMixerRight:" + LastSkinPathMixerRight
            };

            base.Save(Content);
            
        }

        public void Load()
        {
            Content = base.LoadFile(this);
            LoadDefaults();

            if (Content != null)
            {
                foreach (string str in Content)
                {
                    string[] property = GetPropertyPair(str);
                    switch (property[0])
                    {
                        case "Volume":
                            mVolume = Parser.TryParse(property[1], .8);
                            break;

                        case "IsMuted":
                            mIsMuted = Parser.TryParse(property[1], false);
                            break;

                        case "LastSkinPathEditor":
                            LastSkinPathEditor = property[1];
                            break;

                        case "LastSkinPathMixerLeft":
                            LastSkinPathMixerLeft = property[1];
                            break;

                        case "LastSkinPathMixerRight":
                            LastSkinPathMixerRight = property[1];
                            break;
                    }
                }
            }
        }

        private void LoadDefaults()
        {
            mVolume = .8;
        }
    }
}
