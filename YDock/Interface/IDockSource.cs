using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace YDock.Interface
{
    public interface IDockSource
    {
        /// <summary>
        /// 布局管理器，包含控制Dock布局的一些操作，例如Show，Hide之类
        /// </summary>
        IDockControl DockControl { get; set; }

        /// <summary>
        /// 显示用的标题
        /// </summary>
        string Header { get; }

        /// <summary>
        /// 显示用的图标
        /// </summary>
        ImageSource Icon { get; }
    }

    public interface IDockDocSource : IDockSource
    {
        /// <summary>
        /// 源文件是否修改
        /// </summary>
        bool IsModified { get; set; }

        /// <summary>
        /// 源文件的完整路径
        /// </summary>
        string FullFileName { get; }

        /// <summary>
        /// 源文件的名称
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// 源文件的保存接口
        /// </summary>
        void Save();

        /// <summary>
        /// 重新加载源文件
        /// </summary>
        void ReLoad();

        /// <summary>
        /// 提供一个操作，在关闭选项卡前会询问是否允许关闭
        /// </summary>
        /// <returns></returns>
        bool AllowClose();
    }
}