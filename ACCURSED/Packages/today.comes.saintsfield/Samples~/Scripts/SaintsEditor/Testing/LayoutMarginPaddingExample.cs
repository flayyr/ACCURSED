using SaintsField.Playa;

namespace SaintsField.Samples.Scripts.SaintsEditor.Testing
{
    public class LayoutMarginPaddingExample : SaintsMonoBehaviour
    {
        public string top;  // no padding
        [LayoutStart("Indent", paddingLeft: 18)]  // Unity's default padding value is 18
        public int m1;
        [LayoutStart("./Indent", paddingLeft: 18)]  // subgroup will inherent the last padding
        public int m2;
        [LayoutStart("./Indent", paddingLeft: 18)]
        public int m3;

        [LayoutEnd]
        public int resetIndent;

        [Separator(10)]

        [LayoutStart("Top<icon=LensFlare Icon/>Box", ELayout.FoldoutBox, paddingLeft: -4)]  // get rid of the default padding
        public int i1;
        public int i2;

        [LayoutStart("./Continue", paddingLeft: 12)]
        public int nestedIndent;

        [LayoutStart("./Continue", paddingLeft: 18)]
        public int nestedIndent2;

        [LayoutStart("GhostGroup", marginTop: 10, marginBottom: 10)]
        public int g1;
        [LayoutStart("./GhostGroup")]
        public int g2;
        [LayoutStart("./GhostGroup")]
        public int g3;

    }
}
