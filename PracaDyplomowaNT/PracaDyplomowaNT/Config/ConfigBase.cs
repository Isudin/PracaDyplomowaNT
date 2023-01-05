using System.Collections;
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

        protected void SetVal<T>(string name, T value, AttributeType type)
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

        protected T GetVal<T>(string name, T def)
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
    }
}