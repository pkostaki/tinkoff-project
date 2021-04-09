using Windows.UI.Xaml.Controls;

namespace TTApplication.Mvvm.View.Common.List
{
    /// <summary>
    /// Realize <see cref="PanelItemPresentationAbstract"/> for item to have ability to update item's content according it's position at list, to catch clicks i.e.
    /// </summary>
    public abstract class PanelItemPresentationAbstract:UserControl
    {
        /// <summary>
        /// Calls on visible items at screen. Override to update visual state according <param name="progress"></param>
        /// </summary>
        /// <param name="progress">value from interval [-1, 1]. Accordingly: -1 for left tile, center has 0, right tile has 1</param>
        public abstract void ProcessVisibleState(double progress);
        /// <summary>
        /// Calls on hidden items at screen. Override to update visual state according, i.e. for prepare deferred content
        /// </summary>
        /// <param name="progress">value that bigger then 1 </param>
        public abstract void ProcessHiddenState(double progress);
        /// <summary>
        /// Calls when tile place at the center of the list(screen)
        /// </summary>
        public abstract void NotifyPlacedCenter();
        /// <summary>
        /// Calls when item was selected (mouse/touch devices)
        /// </summary>
        public abstract void NotifySelected();
        
    }
}