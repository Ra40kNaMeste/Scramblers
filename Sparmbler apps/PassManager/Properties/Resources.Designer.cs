﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PassManager.Properties {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PassManager.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Keys.
        /// </summary>
        internal static string KeysTabName {
            get {
                return ResourceManager.GetString("KeysTabName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Name.
        /// </summary>
        internal static string NameProperty {
            get {
                return ResourceManager.GetString("NameProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Pass.
        /// </summary>
        internal static string PassPatchsBarName {
            get {
                return ResourceManager.GetString("PassPatchsBarName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Path.
        /// </summary>
        internal static string PathProperty {
            get {
                return ResourceManager.GetString("PathProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Safe file.
        /// </summary>
        internal static string SaveFileTitle {
            get {
                return ResourceManager.GetString("SaveFileTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на pass.ps.
        /// </summary>
        internal static string SavePassDefaultFolder {
            get {
                return ResourceManager.GetString("SavePassDefaultFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на password.ps.
        /// </summary>
        internal static string SavePassDefaultName {
            get {
                return ResourceManager.GetString("SavePassDefaultName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на pass.mps.
        /// </summary>
        internal static string SavePathDefaultName {
            get {
                return ResourceManager.GetString("SavePathDefaultName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Select file.
        /// </summary>
        internal static string SelectFileTitle {
            get {
                return ResourceManager.GetString("SelectFileTitle", resourceCulture);
            }
        }
    }
}
