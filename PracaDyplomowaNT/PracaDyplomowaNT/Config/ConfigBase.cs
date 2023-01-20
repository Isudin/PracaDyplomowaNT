using System.Collections;
using System.Text;
using Soneta.Business;
using Soneta.Business.Db;
using Soneta.Config;

namespace PracaDyplomowaNT
{
    public abstract class ConfigBase : ISessionable
    {
        private string MainKey => "PracaDyplomowaMain";
        private string SubKey => "PracaDyplomowaSub";

        private CfgNode Root { get; set; }

        private Session session;

        [Context]
        public Session Session
        {
            get => session;
            set
            {
                session = value;
                Root = new CfgManager(session).Root;
            }
        }

        protected void SetValue<T>(string name, T value, AttributeType type)
        {
            using (ITransaction t = Root.Session.Logout(true))
            {
                CfgNode node1 = Root.FindSubNode(MainKey, false);
                if (node1 == null)
                    node1 = Root.AddNode(MainKey, CfgNodeType.Node);

                CfgNode node2 = node1.FindSubNode(SubKey, false);
                if (node2 == null)
                    node2 = node1.AddNode(SubKey, CfgNodeType.Leaf);

                CfgAttribute attr = node2.FindAttribute(name, false);
                if (attr == null)
                    attr = node2.AddAttribute(name, type, value);
                else
                    attr.Value = value;

                t.CommitUI();
            }
        }

        protected T GetValue<T>(string name, T def)
        {
            if (Root == null)
                return def;

            CfgNode node1 = Root.FindSubNode(MainKey, false);
            if (node1 == null)
                return def;

            CfgNode node2 = node1.FindSubNode(SubKey, false);
            if (node2 == null)
                return def;

            CfgAttribute attr = node2.FindAttribute(name, false);
            if (attr == null)
                return def;

            return (T)attr.Value;
        }

        public ArrayList GetListFromTable(string tableName, FeatureTypeNumber featureType)
        {
            var res = new ArrayList { "" };
            View featuresDefinitions = BusinessModule.GetInstance(Session).FeatureDefs.CreateView();
            featuresDefinitions.Condition &= new FieldCondition.Equal("TableName", tableName);
            foreach (var row in featuresDefinitions)
            {
                var o = (FeatureDefinition)row;
                if (o.TypeNumber == featureType)
                    res.Add(o.Name);
            }

            return res;
        }

        protected string GetValueNoLimitChars(string name, string def = "", int propertiesToReset = 10)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < propertiesToReset; i++)
                sb.Append(GetValue($"{name}Part{i}", def));

            return sb.ToString();
        }

        protected void SetValueNoLimitChars(string name, string value, AttributeType attrType = AttributeType._null, int propertiesToReset = 10)
        {
            var startSubstring = 0;
            var length = value.Length;
            for (int i = 0; i < propertiesToReset; i++)
            {
                if (length < 256)
                {
                    if (i == 0)
                        SetValue($"{name}Part{i}", value, attrType);
                    else
                        SetValue($"{name}Part{i}", "", attrType);
                }
                else
                {
                    if (startSubstring + 255 < length)
                        SetValue($"{name}Part{i}", value.Substring(startSubstring, 255), attrType);
                    else if (length - startSubstring > 0)
                        SetValue($"{name}Part{i}", value.Substring(startSubstring, length - startSubstring), attrType);
                    else
                        SetValue($"{name}Part{i}", "", attrType);

                    startSubstring += 255;
                }
            }
        }
    }
}