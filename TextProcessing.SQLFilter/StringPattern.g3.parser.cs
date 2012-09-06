namespace Vosen.SQLFilter
{
    partial class StringPatternParser
    {
        partial void CreateTreeAdaptor(ref Antlr.Runtime.Tree.ITreeAdaptor adaptor)
        {
            adaptor = new StringTreeAdaptor();
        }
    }
}
