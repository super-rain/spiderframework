/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Souex.Spider.Framework.Core
{
    /// <summary>
    /// 蜘蛛配置信息的提供程序接口,实现配置信息的读取和存储
    /// </summary>
    public interface ISpiderSettingProvider
    {
        void ReadSetting(SpiderSetting setting);
        void WriteSetting(SpiderSetting setting);
    }
}
