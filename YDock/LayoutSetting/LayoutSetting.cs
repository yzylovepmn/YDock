using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YDock.LayoutSetting
{
    public class LayoutSetting
    {
        public LayoutSetting(string name, XElement layout)
        {
            _name = name;
            Layout = layout;
        }

        public string Name { get { return _name; } }
        private string _name;

        internal XElement Layout
        {
            get { return _layout; }
            set
            {
                if (_layout != value)
                {
                    _layout = value;
                    var name_attr = _layout.Attribute("Name");
                    if (name_attr == null)
                        _layout.SetAttributeValue("Name", _name);
                    else name_attr.Value = _name;
                }
            }
        }
        private XElement _layout;

        public void Save(XElement parent)
        {
            parent.Add(_layout);
        }

        public void Load(XElement layout)
        {
            Layout = layout;
        }
    }
}