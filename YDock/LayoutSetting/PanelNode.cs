using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using YDock.Enum;
using YDock.Interface;

namespace YDock.LayoutSetting
{
    public class PanelNode : ILayoutNode
    {
        public PanelNode(PanelNode parent)
        {
            _parent = parent;
            _children = new LinkedList<ILayoutNode>();
        }

        public bool IsDocument { get { return _isDocument; } }
        private bool _isDocument;

        public DockSide Side { get { return _side; } }
        private DockSide _side;

        public Direction Direction { get { return _direction; } }
        private Direction _direction;

        public LayoutNodeType Type { get { return LayoutNodeType.Panel; } }

        public ILayoutNode Parent { get { return _parent; } }
        private PanelNode _parent;

        public IEnumerable<ILayoutNode> Children { get { return _children; } }
        private LinkedList<ILayoutNode> _children;

        public void ApplyLayout(DockManager dockManager, bool isFloat = false)
        {
            if (_side == DockSide.None)
            {
                if (_isDocument)
                {
                    var node = _children.First;
                    var relative = default(IDockControl);
                    while (node != null)
                    {
                        relative = (node.Value as GroupNode).TryApplyLayoutAsDocument(dockManager, isFloat);
                        node = node.Next;
                        if (relative != null)
                            break;
                    }

                    while (node != null)
                    {
                        var _relative = (node.Value as GroupNode).TryApplyLayoutAsDocument(dockManager, relative);
                        node = node.Next;
                        if (_relative != null)
                            relative = _relative;
                    }
                }
                else
                {
                    var panelNode = _children.First(n => n.Type == LayoutNodeType.Panel && (n as PanelNode).Side == DockSide.None) as PanelNode;
                    panelNode.ApplyLayout(dockManager);
                    var node = _children.Find(panelNode);
                    if (node.Previous != null)
                    {
                        var cur = node.Previous;
                        while (cur != null)
                        {
                            if (cur.Value.Type == LayoutNodeType.Panel)
                                (cur.Value as PanelNode).ApplyLayout(dockManager);
                            else (cur.Value as GroupNode).ApplyLayout(dockManager);
                            cur = cur.Previous;
                        }
                    }
                    if (node.Next != null)
                    {
                        var cur = node.Next;
                        while (cur != null)
                        {
                            if (cur.Value.Type == LayoutNodeType.Panel)
                                (cur.Value as PanelNode).ApplyLayout(dockManager);
                            else (cur.Value as GroupNode).ApplyLayout(dockManager);
                            cur = cur.Next;
                        }
                    }
                }
            }
            else _TryCompleteLayout(dockManager, null, isFloat);
        }

        private void _TryCompleteLayout(DockManager dockManager, IDockControl relative, bool isFloat = false)
        {
            var dic = new Dictionary<IDockControl, PanelNode>();
            var children = relative == null ? _children : _children.Skip(1);

            if (relative != null && _children.First.Value.Type == LayoutNodeType.Panel)
                dic.Add(relative, _children.First.Value as PanelNode);

            foreach (var item in children)
            {
                if (relative == null)
                {
                    if (item.Type == LayoutNodeType.Group)
                        relative = (item as GroupNode).ApplyLayout(dockManager, isFloat);
                    else
                    {
                        relative = (item as PanelNode).TryGetFirstLevelElement(dockManager, null, Direction.None, isFloat);
                        dic.Add(relative, item as PanelNode);
                    }
                }
                else
                {
                    if (item.Type == LayoutNodeType.Group)
                        relative = (item as GroupNode).ApplyLayout(dockManager, relative, _direction);
                    else
                    {
                        relative = (item as PanelNode).TryGetFirstLevelElement(dockManager, relative, _direction);
                        dic.Add(relative, item as PanelNode);
                    }
                }
            }

            foreach (var pair in dic)
                pair.Value._TryCompleteLayout(dockManager, pair.Key, isFloat);
        }

        public IDockControl TryGetFirstLevelElement(DockManager dockManager, IDockControl target = null, Direction dir = Direction.None, bool isFloat = false)
        {
            if (_children.First.Value.Type == LayoutNodeType.Panel)
                return (_children.First.Value as PanelNode).TryGetFirstLevelElement(dockManager, target, dir, isFloat);
            else if (target != null)
                return (_children.First.Value as GroupNode).ApplyLayout(dockManager, target, dir);
            else return (_children.First.Value as GroupNode).ApplyLayout(dockManager, isFloat);
        }

        public void Load(XElement ele)
        {
            _isDocument = bool.Parse(ele.Attribute("IsDocument").Value);
            _side = (DockSide)System.Enum.Parse(typeof(DockSide), ele.Attribute("Side").Value);
            _direction = (Direction)System.Enum.Parse(typeof(Direction), ele.Attribute("Direction").Value);
            foreach (var item in ele.Elements())
            {
                var node = default(ILayoutNode);
                if (item.Name == "Panel")
                    node = new PanelNode(this);
                else node = new GroupNode(this);
                node.Load(item);
                _children.AddLast(node);
            }
        }

        public void Dispose()
        {
            foreach (var child in _children)
                child.Dispose();
            _children.Clear();

            _parent = null;
        }
    }
}