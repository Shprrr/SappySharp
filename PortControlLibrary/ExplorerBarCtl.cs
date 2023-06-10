using vbalExplorerBarLib6;

namespace PortControlLibrary;

public partial class ExplorerBarCtl : UserControl, vbalExplorerBarCtl
{
    public vbalExplorerBarCtl vbaControl = new();

    public ExplorerBarCtl()
    {
        InitializeComponent();
    }

    public void set_ImageList(ref object value) => vbaControl.set_ImageList(ref value);
    public void set_BarTitleImageList(ref object value) => vbaControl.set_BarTitleImageList(ref value);

    public bool ShowFocusRect { get => vbaControl.ShowFocusRect; set => vbaControl.ShowFocusRect = value; }
    public uint BackColorStart { get => vbaControl.BackColorStart; set => vbaControl.BackColorStart = value; }
    public uint BackColorEnd { get => vbaControl.BackColorEnd; set => vbaControl.BackColorEnd = value; }
    public bool UseExplorerTransitionStyle { get => vbaControl.UseExplorerTransitionStyle; set => vbaControl.UseExplorerTransitionStyle = value; }
    public bool UseExplorerStyle { get => vbaControl.UseExplorerStyle; set => vbaControl.UseExplorerStyle = value; }
    public EExplorerBarStyles Style { get => vbaControl.Style; set => vbaControl.Style = value; }

    public uint get_DefaultPanelColor(bool bIsSpecial) => vbaControl.DefaultPanelColor[bIsSpecial];

    public bool Redraw { get => vbaControl.Redraw; set => vbaControl.Redraw = value; }

    public cExplorerBars Bars => vbaControl.Bars;

    public event __vbalExplorerBarCtl_BarRightClickEventHandler BarRightClick
    {
        add
        {
            vbaControl.BarRightClick += value;
        }

        remove
        {
            vbaControl.BarRightClick -= value;
        }
    }

    public event __vbalExplorerBarCtl_BarClickEventHandler BarClick
    {
        add
        {
            vbaControl.BarClick += value;
        }

        remove
        {
            vbaControl.BarClick -= value;
        }
    }

    public event __vbalExplorerBarCtl_ItemRightClickEventHandler ItemRightClick
    {
        add
        {
            vbaControl.ItemRightClick += value;
        }

        remove
        {
            vbaControl.ItemRightClick -= value;
        }
    }

    public event __vbalExplorerBarCtl_ItemClickEventHandler ItemClick
    {
        add
        {
            vbaControl.ItemClick += value;
        }

        remove
        {
            vbaControl.ItemClick -= value;
        }
    }

    public event __vbalExplorerBarCtl_HighlightEventHandler Highlight
    {
        add
        {
            vbaControl.Highlight += value;
        }

        remove
        {
            vbaControl.Highlight -= value;
        }
    }

    public event __vbalExplorerBarCtl_SettingChangeEventHandler SettingChange
    {
        add
        {
            vbaControl.SettingChange += value;
        }

        remove
        {
            vbaControl.SettingChange -= value;
        }
    }
}
