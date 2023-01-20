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
            get => GetValue("TargetPointFeature", string.Empty);
            set => SetValue("TargetPointFeature", value, AttributeType._string);
        }

        public object GetListCodFeature() => GetListFromTable("DokHandlowe", FeatureTypeNumber.Bool);
        [ControlEdit(ControlEditKind.ComboBox)]
        public string CodFeature
        {
            get => GetValue("CodFeature", string.Empty);
            set => SetValue("CodFeature", value, AttributeType._string);
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

                Guid guid = GetValue("DefaultDeliveryService", Guid.Empty);
                View view = TowaryModule.GetInstance(Session).Towary.CreateView();
                view.Condition = new FieldCondition.Equal("Guid", guid);

                return _defaultDeliveryService = view.Any() ? (Towar)view.First() : null;
            }
            set
            {
                SetValue("DefaultDeliveryService", value, AttributeType._guid);
                _defaultDeliveryService = value;
            }
        }

        public string ApiUsername
        {
            get => GetValue("ApiUsername", string.Empty);
            set => SetValue("ApiUsername", value, AttributeType._string);
        }

        public string ApiToken
        {
            get => GetValueNoLimitChars("ApiToken");
            set => SetValueNoLimitChars("ApiToken", value, AttributeType._string);
        }

        public object GetListParcelTemplateTypeFeature() => GetListFromTable("Towary", FeatureTypeNumber.String);
        [ControlEdit(ControlEditKind.ComboBox)]
        public string ParcelTemplateTypeFeature
        {
            get => GetValue("ParcelTemplateType", string.Empty);
            set => SetValue("ParcelTemplateType", value, AttributeType._string);
        }

        public object GetListParcelTrackingNumberFeature() => GetListFromTable("DokHandlowe", FeatureTypeNumber.String);
        [ControlEdit(ControlEditKind.ComboBox)]
        public string ParcelTrackingNumberFeature
        {
            get => GetValue("ParcelTrackingNumberFeature", string.Empty);
            set => SetValue("ParcelTrackingNumberFeature", value, AttributeType._string);
        }

        public object GetListParcelIdFeature() => GetListFromTable("DokHandlowe", FeatureTypeNumber.Int);
        [ControlEdit(ControlEditKind.ComboBox)]
        public string ParcelIdFeature
        {
            get => GetValue("ParcelIdFeature", string.Empty);
            set => SetValue("ParcelIdFeature", value, AttributeType._string);
        }

        public object GetListParcelStatusFeature() => GetListFromTable("DokHandlowe", FeatureTypeNumber.String);
        [ControlEdit(ControlEditKind.ComboBox)]
        public string ParcelStatusFeature
        {
            get => GetValue("ParcelStatusFeature", string.Empty);
            set => SetValue("ParcelStatusFeature", value, AttributeType._string);
        }
    }
}