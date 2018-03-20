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

        internal void AttachTo()
        {
            if (_parent is AnchorSideGroupControl)
            {
                var _group = (_parent as AnchorSideGroupControl).Model as LayoutGroup;
                var _children = _relativeObj.Children.ToList();
                _children.Reverse();
                _relativeObj.Dispose();
                foreach (var child in _children)
                    _group.Attach(child, _index);
            }
            if (_parent is LayoutGroupPanel)
            {
                var panel = _parent as LayoutGroupPanel;
                if (_relativeObj.View == null)
                    new AnchorSideGroupControl(_relativeObj);
                var ctrl = _relativeObj.View as AnchorSideGroupControl;
                if (ctrl.TryDeatchFromParent(false))
                    panel.AttachChild(_relativeObj.View, _mode, _index);
                Dispose();
            }
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