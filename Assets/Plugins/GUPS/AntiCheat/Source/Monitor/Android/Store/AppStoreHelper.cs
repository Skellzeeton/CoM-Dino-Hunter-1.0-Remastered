// System
using System;

namespace GUPS.AntiCheat.Monitor.Android
{
    /// <summary>
    /// Helper class for interacting with Android app stores.
    /// </summary>
    public static class AppStoreHelper
    {
        /// <summary>
        /// Gets the AppStore enum value for a specified package name.
        /// </summary>
        /// <param name="_Package">The package name associated with an AppStore.</param>
        /// <returns>The corresponding AppStore enum value.</returns>
        public static EAppStore GetStore(String _Package)
        {
            // Most stores have a unique package name, verify first.
            switch (_Package)
            {
                // API <= 22 com.android.packageinstaller
                case "com.android.packageinstaller":
                // API >= 23 com.google.android.packageinstaller
                case "com.google.android.packageinstaller":
                    return EAppStore.AndroidPackageInstaller;

                case "com.amazon.venezia":
                    return EAppStore.AmazonAppstore;

                case "cm.aptoide.pt":
                    return EAppStore.Aptoide;

                case "com.farsitel.bazaar":
                    return EAppStore.CafeBazaar;

                case "org.fdroid.fdroid":
                    return EAppStore.FDroid;

                case "com.android.vending":
                    return EAppStore.GooglePlayStore;

                case "com.huawei.appmarket":
                    return EAppStore.HuaweiAppGallery;

                case "ir.mservices.market":
                    return EAppStore.Myket;

                case "com.oppo.market":
                    return EAppStore.OppoAppMarket;

                case "com.sec.android.app.samsungapps":
                    return EAppStore.SamsungGalaxyStore;

                case "com.taptap":
                    return EAppStore.TapTap;

                case "com.bbk.appstore":
                    return EAppStore.VivoAppStore;

                case "com.xiaomi.market":
                    return EAppStore.XiaomiMiGetApps;

                case "com.xda.labs.play":
                    return EAppStore.XDALabs;
            }

            // Some may have multiple package names, verify them (could be com.oculus.twilight, com.oculus.store, etc.).
            if (_Package.StartsWith("com.oculus."))
            {
                return EAppStore.MetaHorizonStore;
            }

            // If the store is not recognized, return Unknown.
            return EAppStore.Unknown;
        }
    }
}
