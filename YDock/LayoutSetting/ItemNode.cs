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
    public class ItemNode : ILayoutNode
    {
        public ItemNode(GroupNode parent)
        {
            _parent = parent;
        }

        public int ID { get { return _id; } }
        private int _id;

        public LayoutNodeType Type { get { return LayoutNodeType.Item; } }

        public ILayoutNode Parent { get { return _parent; } }
        private GroupNode _parent;

        public IEnumerable<ILayoutNode> Children { get { yield break; } }

        public void Load(XElement ele)
        {
            _id = int.Parse(ele.Value);
        }

        public void Dispose()
        {
            _parent = null;
        }
    }
}