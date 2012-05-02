/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 反射辅助类
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// BindingFlags
        /// </summary>
        private static readonly BindingFlags _BindingFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase;

        #region SpiderSetting
        /// <summary>
        /// 从给定的程序集中加载第一个SpiderSetting的派生类型或其本身,并实例化派生类型,失败时返回NULL
        /// </summary>
        /// <param name="assemblyFile">程序集文件名</param>
        /// <returns>SpiderSetting</returns>
        public static SpiderSetting LoadSpiderSetting(string assemblyFile, params object[] args)
        {
            return LoadSpiderSetting(assemblyFile, "", args);
        }

        /// <summary>
        /// 从给定的程序集中加载与参数fullTypeName一致的类型,并实例化,失败时返回NULL
        /// </summary>
        /// <param name="assemblyFile">程序集文件名</param>
        /// <param name="fullTypeName">完整类型名,为空或NULL时,自动寻找SpiderSetting的派生类型</param>
        /// <returns>SpiderSetting</returns>
        public static SpiderSetting LoadSpiderSetting(string assemblyFile, string fullTypeName, params object[] args)
        {
            if (!File.Exists(assemblyFile))
            {
                return null;
            }

            Assembly assembly = GetAssembly(assemblyFile);
            return LoadSpiderSetting(assembly, fullTypeName, args);
        }

        /// <summary>
        /// 从给定字节数组加载第一个SpiderSetting的派生类型或其本身,并实例化,失败时返回NULL
        /// </summary>
        /// <param name="data">字节数组</param>
        /// <returns>SpiderSetting</returns>
        public static SpiderSetting LoadSpiderSetting(byte[] data, params object[] args)
        {
            Assembly assembly = GetAssembly(data);
            return LoadSpiderSetting(assembly, null, args);
        }

        /// <summary>
        /// 从给定字节数组加载与参数fullTypeName一致的类型,并实例化,失败时返回NULL
        /// </summary>
        /// <param name="data">字节数组</param>
        /// <param name="fullTypeName">完整类型名,为空或NULL时,自动寻找SpiderSetting的派生类型</param>
        /// <returns>SpiderSetting</returns>
        public static SpiderSetting LoadSpiderSetting(byte[] data, string fullTypeName, params object[] args)
        {
            Assembly assembly = GetAssembly(data);
            return LoadSpiderSetting(assembly, fullTypeName, args);
        }

        /// <summary>
        /// 从给定的程序集加载与参数fullTypeName一致的类型,并实例化,失败时返回NULL
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="fullTypeName">完整类型名,为空或NULL时,自动寻找SpiderSetting的派生类型</param>
        /// <returns>SpiderSetting</returns>
        public static SpiderSetting LoadSpiderSetting(Assembly assembly, string fullTypeName, params object[] args)
        {
            Type[] types = LoadTypes(assembly, fullTypeName, typeof(SpiderSetting));
            if (types.Length < 1)
            {
                return null;
            }
            object obj = _typeToInstance(types[0], args);
            if (null == obj)
            {
                return null;
            }
            return obj as SpiderSetting;
        }

        #endregion

        #region UrlExtractor
        /// <summary>
        /// 从程序集加载UrlExtractor类型数组,失败时返回NULL
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <returns>UrlExtractor[]</returns>
        public static UrlExtractor[] LoadUrlExtractors(Assembly assembly, params object[] args)
        {
            Type[] types = LoadTypes(assembly, null, typeof(UrlExtractor));
            List<UrlExtractor> list = new List<UrlExtractor>();
            if (types.Length < 1)
            {
                return null;
            }
            for (int i = 0; i < types.Length; i++)
            {
                UrlExtractor item = _typeToInstance(types[0], args) as UrlExtractor;
                if (null != item)
                {
                    list.Add(item);
                }
            }

            UrlExtractor[] array = new UrlExtractor[list.Count];
            list.CopyTo(array);
            return array;
        }

        /// <summary>
        /// 从程序集加载与给定参数fullTypeName一致的类型,失败时返回NULL
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="fullTypeName"></param>
        /// <returns></returns>
        public static UrlExtractor LoadUrlExtractor(Assembly assembly, string fullTypeName, params object[] args)
        {
            if (null == assembly || null == fullTypeName || "" == fullTypeName)
            {
                return null;
            }

            Type[] types = LoadTypes(assembly, fullTypeName, typeof(UrlExtractor));
            if (types.Length < 1)
            {
                return null;
            }

            return _typeToInstance(types[0], args) as UrlExtractor;
        }
        #endregion

        #region ContentHandler

        public static IContentHandler[] LoadContentHandlers(Assembly assembly, params object[] args)
        {
            Type[] types = LoadTypes(assembly, null, typeof(IContentHandler));
            List<IContentHandler> list = new List<IContentHandler>();
            if (types.Length < 1)
            {
                return null;
            }
            for (int i = 0; i < types.Length; i++)
            {
                IContentHandler item = _typeToInstance(types[0], args) as IContentHandler;
                if (null != item)
                {
                    list.Add(item);
                }
            }

            IContentHandler[] array = new IContentHandler[list.Count];
            list.CopyTo(array);
            return array;
        }

        public static IContentHandler LoadContentHandler(Assembly assembly, string fullTypeName, params object[] args)
        {
            if (null == assembly || null == fullTypeName || "" == fullTypeName)
            {
                return null;
            }

            Type[] types = LoadTypes(assembly, fullTypeName, typeof(IContentHandler));
            if (types.Length < 1)
            {
                return null;
            }

            return _typeToInstance(types[0], args) as IContentHandler;
        }
        #endregion

        #region UrlManager
        public static UrlManager LoadUrlManager(Assembly assembly, string fullTypeName, params object[] args)
        {
            if (null == assembly || null == fullTypeName || "" == fullTypeName)
            {
                return null;
            }

            Type[] types = LoadTypes(assembly, fullTypeName, typeof(UrlManager));
            if (types.Length < 1)
            {
                return null;
            }

            return _typeToInstance(types[0], args) as UrlManager;
        }
        #endregion

        #region Private
        private static Assembly GetAssembly(string file)
        {
            return Assembly.LoadFile(file);
        }

        private static Assembly GetAssembly(byte[] rawData)
        {
            return Assembly.Load(rawData);
        }

        private static Type[] LoadTypes(Assembly assembly, string fullTypeName)
        {
            return LoadTypes(assembly, fullTypeName, null);
        }

        private static Type[] LoadTypes(Assembly assembly, Type baseType)
        {
            return LoadTypes(assembly, null, baseType);
        }

        /// <summary>
        /// 从指定程序集加载指定名称或基类型的类型数组,失败时返回空类型数组,即:Type[0]
        /// </summary>
        /// <param name="assembly">指定程序集</param>
        /// <param name="fullTypeName">指定类型的完全限定名</param>
        /// <param name="baseType">基类型</param>
        /// <returns>Type[]</returns>
        private static Type[] LoadTypes(Assembly assembly, string fullTypeName, Type baseType)
        {
            if (null == assembly || (null == baseType && String.IsNullOrEmpty(fullTypeName)))
            {
                return new Type[0];
            }

            //如果指定类型名,则返回第一个符合的类型
            if (!String.IsNullOrEmpty(fullTypeName))
            {
                Type t = assembly.GetType(fullTypeName, false, true);
                if (null == t)
                {
                    return new Type[0];
                }
                return new Type[] { t };
            }

            Type[] allTypes = assembly.GetExportedTypes();
            List<Type> list = new List<Type>();
            foreach (Type t in allTypes)
            {
                if (null != baseType && (t == baseType || t.IsSubclassOf(baseType)))
                {
                    list.Add(t);
                }
            }

            Type[] types = new Type[list.Count];
            if (list.Count > 0)
            {
                list.CopyTo(types, 0);
            }

            return types;
        }

        #endregion

        #region converters

        private static object _typeToInstance(Type type, params object[] args)
        {
            return type.Assembly.CreateInstance(type.FullName, true, _BindingFlags, null, args, null, null);
        }

        private static IContentHandler _ContentHandlerConverter(object obj)
        {
            return obj as IContentHandler;
        }

        private static UrlExtractor _UrlExtractorConverter(object obj)
        {
            return obj as UrlExtractor;
        }

        private static SpiderSetting _SpiderSettingConverter(object obj)
        {
            return obj as SpiderSetting;
        }
        #endregion
    }
}
