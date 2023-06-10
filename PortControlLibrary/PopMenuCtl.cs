using System.Runtime.CompilerServices;
using cPopMenu6;
using stdole;

namespace PortControlLibrary;

public partial class PopMenuCtl : UserControl, PopMenu
{
    public PopMenu vbaControl = new();

    public PopMenuCtl()
    {
        InitializeComponent();
    }

    public int ShowPopupMenu(ref object objTo, object vKeyParent, float x, float y, CSPShowPopupMenuConstants eOptions = CSPShowPopupMenuConstants.TPM_LEFTALIGN) => vbaControl.ShowPopupMenu(ref objTo, vKeyParent, x, y, eOptions);
    public void set_BackgroundPicture(ref StdPicture value) => vbaControl.set_BackgroundPicture(ref value);
    public void set_BackgrdounPicture(ref StdPicture value) => vbaControl.set_BackgrdounPicture(ref value);
    public StdPicture get_BackgroundPicture() => vbaControl.get_BackgroundPicture();
    public void ClearBackgroundPicture() => vbaControl.ClearBackgroundPicture();
    void _PopMenu.set_Font(ref StdFont value) => vbaControl.set_Font(ref value);
    StdFont _PopMenu.get_Font() => vbaControl.get_Font();
    public void GetVersion(ref int lMajor, ref int lMinor, ref int lRevision) => vbaControl.GetVersion(ref lMajor, ref lMinor, ref lRevision);
    public void SystemMenuRemoveItem(int lPosition) => vbaControl.SystemMenuRemoveItem(lPosition);
    public int SystemMenuAppendItem(string sCaption) => vbaControl.SystemMenuAppendItem(sCaption);
    public void SystemMenuRestore() => vbaControl.SystemMenuRestore();
    public void set_ImageList(ref object value) => vbaControl.set_ImageList(ref value);
    public void GetHierarchyForIndexPosition(object vKey, ref Array lHierarchy) => vbaControl.GetHierarchyForIndexPosition(vKey, ref lHierarchy);
    public int ClearSubMenusOfItem(object vKey) => vbaControl.ClearSubMenusOfItem(vKey);
    public void RemoveItem(object vKey) => vbaControl.RemoveItem(vKey);
    public int AddItem(string sCaption, string sKey = "", string sHelptext = "", int lItemData = 0, int lParentIndex = 0, int lIconIndex = -1, bool bChecked = false, bool bEnabled = true) => vbaControl.AddItem(sCaption, sKey, sHelptext, lItemData, lParentIndex, lIconIndex, bChecked, bEnabled);
    public int ReplaceItem(object vKey, object sCaption, object sHelptext, object lItemData, object lIconIndex, object bChecked, object bEnabled) => vbaControl.ReplaceItem(vKey, sCaption, sHelptext, lItemData, lIconIndex, bChecked, bEnabled);
    public int InsertItem(string sCaption, object vKeyBefore, string sKey = "", string sHelptext = "", int lItemData = 0, int lIconIndex = -1, bool bChecked = false, bool bEnabled = true) => vbaControl.InsertItem(sCaption, vKeyBefore, sKey, sHelptext, lItemData, lIconIndex, bChecked, bEnabled);
    public void EnsureMenuSeparators(int hMenu) => vbaControl.EnsureMenuSeparators(hMenu);
    public void Clear() => vbaControl.Clear();
    public void SubClassMenu([IDispatchConstant] object oForm, bool bLeaveTopLevelMenus = false) => vbaControl.SubClassMenu(oForm, bLeaveTopLevelMenus);
    public void CheckForNewItems() => vbaControl.CheckForNewItems();
    public void UnsubclassMenu() => vbaControl.UnsubclassMenu();

    public CSPHighlightStyleConstants HighlightStyle { get => vbaControl.HighlightStyle; set => vbaControl.HighlightStyle = value; }
    public bool HighlightCheckedItems { get => vbaControl.HighlightCheckedItems; set => vbaControl.HighlightCheckedItems = value; }

    public bool get_MenuExists(object vKey) => vbaControl.MenuExists[vKey];
    public int get_MenuIndex(object vKey) => vbaControl.MenuIndex[vKey];
    public string get_MenuKey(int lIndex) => vbaControl.MenuKey[lIndex];
    public void set_MenuKey(int lIndex, string value) => vbaControl.MenuKey[lIndex] = value;
    public string get_MenuTag(object vKey) => vbaControl.MenuTag[vKey];
    public void set_MenuTag(object vKey, string value) => vbaControl.MenuTag[vKey] = value;
    public bool get_MenuDefault(object vKey) => vbaControl.MenuDefault[vKey];
    public void set_MenuDefault(object vKey, bool value) => vbaControl.MenuDefault[vKey] = value;

    public int TickIconIndex { get => vbaControl.TickIconIndex; set => vbaControl.TickIconIndex = value; }

    public string get_SystemMenuCaption(int lPosition) => vbaControl.SystemMenuCaption[lPosition];

    public int SystemMenuCount => vbaControl.SystemMenuCount;

    public string get_Caption(object vKey) => vbaControl.Caption[vKey];
    public void set_Caption(object vKey, string value) => vbaControl.Caption[vKey] = value;
    public bool get_Enabled(object vKey) => vbaControl.Enabled[vKey];
    public void set_Enabled(object vKey, bool value) => vbaControl.Enabled[vKey] = value;
    public bool get_Checked(object vKey) => vbaControl.Checked[vKey];
    public void set_Checked(object vKey, bool value) => vbaControl.Checked[vKey] = value;
    public string get_HelpText(object vKey) => vbaControl.HelpText[vKey];
    public void set_HelpText(object vKey, string value) => vbaControl.HelpText[vKey] = value;
    public int get_ItemIcon(object vKey) => vbaControl.ItemIcon[vKey];
    public void set_ItemIcon(object vKey, int value) => vbaControl.ItemIcon[vKey] = value;
    public int get_ItemData(object vKey) => vbaControl.ItemData[vKey];
    public void set_ItemData(object vKey, int value) => vbaControl.ItemData[vKey] = value;
    public int get_hPopupMenu(object vKey) => vbaControl.hPopupMenu[vKey];
    public int get_PositionInMenu(object vKey) => vbaControl.PositionInMenu[vKey];
    public int get_NextSibling(object vKey) => vbaControl.NextSibling[vKey];
    public int get_SiblingCount(object vKey) => vbaControl.SiblingCount[vKey];
    public dynamic get_HasChildren(object vKey) => vbaControl.HasChildren[vKey];
    public int get_FirstChild(object vKey) => vbaControl.FirstChild[vKey];
    public int get_Parent(object vKey) => vbaControl.Parent[vKey];
    public int get_UltimateParent(object vKey) => vbaControl.UltimateParent[vKey];
    public int get_IndexForMenuHierarchyParamArray(ref object[] vHierarchy) => vbaControl.IndexForMenuHierarchyParamArray[ref vHierarchy];
    public int get_IndexForMenuHierarchy(ref Array lHierarchy) => vbaControl.IndexForMenuHierarchy[ref lHierarchy];
    public string get_HierarchyPath(object vKey, int lStartLevel, string sSeparator) => vbaControl.HierarchyPath[vKey, lStartLevel, sSeparator];
    public int get_IDForIndex(object vKey) => vbaControl.IDForIndex[vKey];

    public short Count => vbaControl.Count;

    public int MenuItemsPerScreen => vbaControl.MenuItemsPerScreen;

    public uint ActiveMenuForeColor { get => vbaControl.ActiveMenuForeColor; set => vbaControl.ActiveMenuForeColor = value; }
    public uint InActiveMenuForeColor { get => vbaControl.InActiveMenuForeColor; set => vbaControl.InActiveMenuForeColor = value; }
    public uint MenuBackgroundColor { get => vbaControl.MenuBackgroundColor; set => vbaControl.MenuBackgroundColor = value; }
    public bool OfficeXpStyle { get => vbaControl.OfficeXpStyle; set => vbaControl.OfficeXpStyle = value; }

    public event __PopMenu_ClickEventHandler PopMenuClick
    {
        add
        {
            vbaControl.Click += value;
        }

        remove
        {
            vbaControl.Click -= value;
        }
    }
    event __PopMenu_ClickEventHandler __PopMenu_Event.Click
    {
        add
        {
            vbaControl.Click += value;
        }

        remove
        {
            vbaControl.Click -= value;
        }
    }

    public event __PopMenu_SystemMenuClickEventHandler SystemMenuClick
    {
        add
        {
            vbaControl.SystemMenuClick += value;
        }

        remove
        {
            vbaControl.SystemMenuClick -= value;
        }
    }

    public event __PopMenu_ItemHighlightEventHandler ItemHighlight
    {
        add
        {
            vbaControl.ItemHighlight += value;
        }

        remove
        {
            vbaControl.ItemHighlight -= value;
        }
    }

    public event __PopMenu_SystemMenuItemHighlightEventHandler SystemMenuItemHighlight
    {
        add
        {
            vbaControl.SystemMenuItemHighlight += value;
        }

        remove
        {
            vbaControl.SystemMenuItemHighlight -= value;
        }
    }

    public event __PopMenu_MenuExitEventHandler MenuExit
    {
        add
        {
            vbaControl.MenuExit += value;
        }

        remove
        {
            vbaControl.MenuExit -= value;
        }
    }

    public event __PopMenu_InitPopupMenuEventHandler InitPopupMenu
    {
        add
        {
            vbaControl.InitPopupMenu += value;
        }

        remove
        {
            vbaControl.InitPopupMenu -= value;
        }
    }

    public event __PopMenu_WinIniChangeEventHandler WinIniChange
    {
        add
        {
            vbaControl.WinIniChange += value;
        }

        remove
        {
            vbaControl.WinIniChange -= value;
        }
    }

    public event __PopMenu_NewMDIMenuEventHandler NewMDIMenu
    {
        add
        {
            vbaControl.NewMDIMenu += value;
        }

        remove
        {
            vbaControl.NewMDIMenu -= value;
        }
    }

    public event __PopMenu_RequestNewMenuDetailsEventHandler RequestNewMenuDetails
    {
        add
        {
            vbaControl.RequestNewMenuDetails += value;
        }

        remove
        {
            vbaControl.RequestNewMenuDetails -= value;
        }
    }
}
