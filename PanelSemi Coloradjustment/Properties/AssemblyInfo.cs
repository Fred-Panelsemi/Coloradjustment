using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// 組件的一般資訊是由下列的屬性集控制。
// 變更這些屬性的值即可修改組件的相關
// 資訊。
[assembly: AssemblyTitle("PanelSemi Coloradjustment")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("PanelSemi Coloradjustment")]
[assembly: AssemblyCopyright("Copyright ©  2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 將 ComVisible 設為 false 可對 COM 元件隱藏
// 組件中的類型。若必須從 COM 存取此組件中的類型，
// 的類型，請在該類型上將 ComVisible 屬性設定為 true。
[assembly: ComVisible(false)]

//若要開始建置可當地語系化的應用程式，請在
//.csproj 檔案中的 <UICulture>CultureYouAreCodingWith</UICulture>
//在 <PropertyGroup> 中。例如，如果原始程式檔使用美式英文， 
//請將 <UICulture> 設為 en-US。然後取消註解下列
//NeutralResourceLanguage 屬性。在下一行中更新 "en-US"，
//以符合專案檔中的 UICulture 設定。

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //主題特定資源字典的位置
                                     //(在頁面中找不到時使用，
                                     // 或應用程式資源字典中找不到資源時)
    ResourceDictionaryLocation.SourceAssembly //泛型資源字典的位置
                                              //(在頁面中找不到時使用，
                                              // 或是應用程式或任何主題特定資源字典中找不到資源時)
)]


// 組件的版本資訊由下列四個值所組成:
//
//      主要版本
//      次要版本
//      組建編號
//      修訂
// v1.0.0.0 Beta
//-----------------------------------
// v1.0.0.0 【正式釋出版】
//-----------------------------------
// v1.0.0.0 【不變更版本】
//          【修正】 儲存色差後 不再該ID離開色差調節模式後 FPGA Reg 會寫到調整色差前的值 
//-----------------------------------
// v1.0.0.1 【修正】 變換色域時或刷成調整前的顏色資訊
//          【新增】 預設數值按鈕 方便使用者切換數值
//          【新增】 ID ON OFF 功能
//          【新增】 MCU Reset 功能 (將屏幕關機功能)
//          【修正】 很多屏幕連接時左側ID顯示區無法容下這麼多ID
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
