using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDock.Enum;
using YDock.Interface;
using YDock.View;

namespace YDock.Model
{
    internal class AttachObject : IDisposable
    {
        internal AttachObject(LayoutGroup relativeObj, INotifyDisposable parent, int index, AttachMode mode = AttachMode.None)
        {
            _relativeObj = relativeObj;
            _parent = parent;
            _index = index;
            _mode = mode;
            _parent.Disposed += OnDisposed;
        }

        private LayoutGroup _relativeObj;

        private INotifyDisposable _parent;
        internal INotifyDisposable Parent
        {
            get { return _parent; }
        }

        private int _index = -1;
        internal int Index { get { return _index; } }

        private AttachMode _mode;

        internal bool AttachTo()
        {
            if (_parent is BaseGroupControl)
            {
                if (_mode == AttachMode.None)
                {
                    var _group = (_parent as BaseGroupControl).Model as ILayoutGroup;
                    var _children = _relativeObj.Children.ToList();
                    _children.Reverse();
                    _relativeObj.Dispose();
                    foreach (var child in _children)
                        _group.Attach(child, Math.Min(_index, _group.Children.Count() - 1));
                }
                else
                {
                    var targetctrl = _parent as AnchorSideGroupControl;
                    if (targetctrl.DockViewParent != null)
                    {
                        if (_relativeObj.View == null)
                            _relativeObj.View = new AnchorSideGroupControl(_relativeObj);
                        var ctrl = _relativeObj.View as AnchorSideGroupControl;
                        if (ctrl.TryDeatchFromParent(false))
                        {
                            if (targetctrl.DockViewParent == null) return false;
                            switch (_mode)
                            {
                                case AttachMode.Left:
                                    targetctrl.AttachTo(targetctrl.DockViewParent as LayoutGroupPanel, ctrl, DropMode.Left);
                                    break;
                                case AttachMode.Top:
                                    targetctrl.AttachTo(targetctrl.DockViewParent as LayoutGroupPanel, ctrl, DropMode.Top);
                                    break;
                                case AttachMode.Right:
                                    targetctrl.AttachTo(targetctrl.DockViewParent as LayoutGroupPanel, ctrl, DropMode.Right);
                                    break;
                                case AttachMode.Bottom:
                                    targetctrl.AttachTo(targetctrl.DockViewParent as LayoutGroupPanel, ctrl, DropMode.Bottom);
                                    break;
                            }
                        }
                        else return false;
                    }
                    else return false;
                    Dispose();
                }
            }
            if (_parent is LayoutGroupPanel)
            {
                if (_relativeObj.View == null)
                    _relativeObj.View = new AnchorSideGroupControl(_relativeObj);
                var ctrl = _relativeObj.View as AnchorSideGroupControl;
                if (_mode == AttachMode.None)
                {
                    var panel = _parent as LayoutGroupPanel;
                    if (ctrl.TryDeatchFromParent(false))
                        panel.AttachChild(_relativeObj.View, _mode, Math.Min(_index, panel.Children.Count - 1));
                    else return false;
                }
                else
                {
                    var panel = (_parent as LayoutGroupPanel).DockViewParent as LayoutGroupPanel;
                    if (panel != null)
                    {
                        if (ctrl.TryDeatchFromParent(false))
                            panel.AttachChild(_relativeObj.View, _mode, Math.Min(_index, panel.Children.Count - 1));
                        else return false;
                    }
                    else return false;
                }
                Dispose();
            }
            return true;
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_parent != null)
                _parent.Disposed -= OnDisposed;
            if (_relativeObj != null)
                _relativeObj.AttachObj = null;
            _relativeObj = null;
            _parent = null;
        }
    }
}