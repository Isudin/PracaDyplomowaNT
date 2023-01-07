using System;
using System.Linq;
using Soneta.Business;
using Soneta.Config;
using Soneta.Towary;
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

        public object GetListDefaultDeliveryService()
        {
            View view = TowaryModule.GetInstance(Session).Towary.CreateView();
            view.Condition = new FieldCondition.Equal("Typ", TypTowaru.Usługa);
            return view.ToList<Towar>();
        }

        private Towar _defaultDeliveryService = null;
        public Towar DefaultDeliveryService
        {
            get
            {
                if (_defaultDeliveryService != null)
                    return _defaultDeliveryService;

                Guid guid = GetVal("DefaultDeliveryService", Guid.Empty);
                View view = TowaryModule.GetInstance(Session).Towary.CreateView();
                view.Condition = new FieldCondition.Equal("Guid", guid);

                return _defaultDeliveryService = view.Any() ? (Towar)view.First() : null;
            }
            set
            {
                SetVal("DefaultDeliveryService", value, AttributeType._guid);
                _defaultDeliveryService = value;
            }
        }

        public object GetListParcelTemplateTypeFeature() => GetListFromTable("Towary", FeatureTypeNumber.String);
        [ControlEdit(ControlEditKind.ComboBox)]
        public string ParcelTemplateTypeFeature
        {
            get => GetVal("ParcelTemplateType", string.Empty);
            set => SetVal("ParcelTemplateType", value, AttributeType._string);
        }
    }
}