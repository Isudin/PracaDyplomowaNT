using Soneta.Business;
using Soneta.Config;
using Soneta.Types;

[assembly: Worker(typeof(PracaDyplomowaNT.Config))]
namespace PracaDyplomowaNT
{
    public class Config : ConfigBase
    {
        Date d = Date.Today;
        public object GetListTargetPointFeature() => GetListFromTable("DokHandlowe", FeatureTypeNumber.String);
        [ControlEdit(ControlEditKind.ComboBox)]
        public string TargetPointFeature
        {
            get => GetVal("TargetPointFeature", string.Empty);
            set => SetVal("TargetPointFeature", value, AttributeType._string);
        }

        public object GetListCodFeature() => GetListFromTable("DokHandlowe", FeatureTypeNumber.Bool);
        [ControlEdit(ControlEditKind.ComboBox)]
        public string CodFeature
        {
            get => GetVal("CodFeature", string.Empty);
            set => SetVal("CodFeature", value, AttributeType._string);
        }
    }
}