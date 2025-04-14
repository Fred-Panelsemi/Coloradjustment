using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Markup;

namespace PanelSemi_Coloradjustment.Helper
{
    public static class CultureHelper
    {
        private static PropertyInfo mRes;

        private static MethodInfo mFindRes;

        private static bool mInit = false;

        private static DispatcherObject mTar = null;

        private static ResourceDictionary mLastRes;

        private static readonly Dictionary<CultureInfo, ResourceDictionary> mSupCult = new Dictionary<CultureInfo, ResourceDictionary>();

        private static CultureInfo mCurCult = Thread.CurrentThread.CurrentCulture;

        public static bool IsInitialized => mInit;

        public static CultureInfo CurrentCulture => mCurCult;

        public static void Initial(Assembly asm, CultureInfo defaultCult = null, DispatcherObject target = null)
        {
            if (mInit)
            {
                return;
            }

            if (target == null)
            {
                mTar = Application.Current;
            }
            else
            {
                mTar = target;
            }

            Type type = mTar.GetType();
            mRes = type.GetProperty("Resources");
            mFindRes = type.GetMethod("TryFindResource");
            string[] manifestResourceNames = asm.GetManifestResourceNames();
            Regex reg = new Regex("(?<=\\.)\\w{2}-\\w{2,}");
            foreach (string item in manifestResourceNames.Where((string r) => reg.IsMatch(r)))
            {
                using (Stream stream = asm.GetManifestResourceStream(item))
                {
                    ResourceDictionary value = XamlReader.Load(stream) as ResourceDictionary;
                    Match match = reg.Match(item);
                    if (match.Success)
                    {
                        mSupCult.Add(CultureInfo.GetCultureInfo(match.Value), value);
                    }
                    else
                    {
                        mSupCult.Add(new CultureInfo(""), value);
                    }
                }
            }

            mInit = true;
            if (defaultCult != null && mSupCult.ContainsKey(defaultCult))
            {
                ChangeCulture(defaultCult);
            }
        }

        public static void Initial(IList<Uri> files, CultureInfo defaultCult = null, DispatcherObject target = null)
        {
            if (mInit)
            {
                return;
            }

            if (target == null)
            {
                mTar = Application.Current;
            }
            else
            {
                mTar = target;
            }

            Type type = mTar.GetType();
            mRes = type.GetProperty("Resources");
            mFindRes = type.GetMethod("TryFindResource");
            foreach (Uri file in files)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.ToString());
                ResourceDictionary value = Application.LoadComponent(file) as ResourceDictionary;
                CultureInfo key = new CultureInfo(fileNameWithoutExtension);
                mSupCult.Add(key, value);
            }

            mInit = true;
            if (defaultCult != null && mSupCult.ContainsKey(defaultCult))
            {
                ChangeCulture(defaultCult);
            }
        }

        public static void ChangeCulture(CultureInfo culture)
        {
            if (!mSupCult.TryGetValue(culture, out var rd))
            {
                return;
            }

            if (mLastRes != null)
            {
                mTar.TryInvoke((DispatcherObject d) => (mRes.GetValue(d) as ResourceDictionary).MergedDictionaries.Remove(mLastRes));
            }

            mTar.TryInvoke(delegate (DispatcherObject d)
            {
                (mRes.GetValue(d) as ResourceDictionary).MergedDictionaries.Add(rd);
            });
            mLastRes = rd;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            mCurCult = culture;
        }

        public static TObj GetResource<TObj>(object name)
        {
            if (mLastRes == null)
            {
                return mTar.TryInvoke((DispatcherObject d) => (TObj)mFindRes.Invoke(d, new object[1] { name }));
            }

            return (TObj)mLastRes[name];
        }
    }
}
