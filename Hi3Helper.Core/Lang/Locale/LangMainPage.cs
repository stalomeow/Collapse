﻿namespace Hi3Helper
{
    public partial class Locale
    {
        #region MainPage
        public partial class LocalizationParams
        {
            public LangMainPage _MainPage { get; set; } = LangFallback?._MainPage;
            public class LangMainPage
            {
                public string PageTitle { get; set; } = LangFallback?._MainPage.PageTitle;
                public string RegionChangeConfirm { get; set; } = LangFallback?._MainPage.RegionChangeConfirm;
                public string RegionChangeConfirmBtn { get; set; } = LangFallback?._MainPage.RegionChangeConfirmBtn;
                public string RegionChangeWarnTitle { get; set; } = LangFallback?._MainPage.RegionChangeWarnTitle;
                public string RegionChangeWarnExper1 { get; set; } = LangFallback?._MainPage.RegionChangeWarnExper1;
                public string RegionLoadingTitle { get; set; } = LangFallback?._MainPage.RegionLoadingTitle;
                public string RegionLoadingAPITitle1 { get; set; } = LangFallback?._MainPage.RegionLoadingAPITitle1;
                public string RegionLoadingAPITitle2 { get; set; } = LangFallback?._MainPage.RegionLoadingAPITitle2;
                public string RegionLoadingSubtitleTimeOut { get; set; } = LangFallback?._MainPage.RegionLoadingSubtitleTimeOut;
                public string RegionLoadingSubtitleTooLong { get; set; } = LangFallback?._MainPage.RegionLoadingSubtitleTooLong;
                public string NotifNeverAsk { get; set; } = LangFallback?._MainPage.NotifNeverAsk;
                public string RegionComboBoxPlaceholder { get; set; } = LangFallback?._MainPage.RegionComboBoxPlaceholder;
            }
        }
        #endregion
    }
}
