using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using YDock.Enum;

namespace YDock.Interface
{
    public interface IDockManager : IDockView
    {
        ImageSource DockImageSource { get; set; }
        string DockTitle { get; set; }
        IDockControl ActiveControl { get; }

        event EventHandler ActiveDockChanged;
        IDockControl SelectedDocument { get; }
        IEnumerable<IDockControl> DockControls { get; }

        void HideAll();
        void UpdateTitleAll();
        void RegisterDocument(IDockSource content, bool canSelect = false, double desiredWidth = Constants.DockDefaultWidthLength, double desiredHeight = Constants.DockDefaultHeightLength);
        void RegisterDock(IDockSource content, DockSide side = DockSide.Left, bool canSelect = false, double desiredWidth = Constants.DockDefaultWidthLength, double desiredHeight = Constants.DockDefaultHeightLength);
        void RegisterFloat(IDockSource content, DockSide side = DockSide.Left, double desiredWidth = Constants.DockDefaultWidthLength, double desiredHeight = Constants.DockDefaultHeightLength);
        void AttachTo(IDockControl source, IDockControl target, AttachMode mode, double ratio = 1);
    }
}