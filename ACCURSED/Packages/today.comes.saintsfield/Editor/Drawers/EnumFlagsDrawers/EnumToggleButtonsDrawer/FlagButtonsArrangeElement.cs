#if UNITY_2021_3_OR_NEWER
using SaintsField.Editor.UIToolkitElements.ValueButtons;

namespace SaintsField.Editor.Drawers.EnumFlagsDrawers.EnumToggleButtonsDrawer
{
    public class FlagButtonsArrangeElement: AbsValueButtonsArrangeElement<FlagButton>
    {
        private readonly bool _isULong;

        public FlagButtonsArrangeElement(AbsValueButtonsCalcElement valueButtonsCalcElement, bool isULong = false)
            : base(valueButtonsCalcElement, MakeRow(isULong))
        {
            _isULong = isULong;
        }

        protected override AbsValueButtonsRow<FlagButton> MakeValueButtonsRow()
        {
            return MakeRow(_isULong);
        }

        private static AbsValueButtonsRow<FlagButton> MakeRow(bool isULong)
        {
            return new FlagButtonsRow(isULong);
        }
    }
}
#endif
