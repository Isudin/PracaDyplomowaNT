using Soneta.Business;
using Soneta.Business.UI;
using Soneta.Handel;
using Soneta.Types;

namespace PracaDyplomowaNT.OrderImport
{
    public class CsvParams : ContextBase
    {
        public CsvParams(Context cx) : base(cx)
        {
        }
        
        public string FilePath { get; set; }

        public object GetListOrderDefinition() => HandelModule.GetInstance(Session).DefDokHandlowych.WgKategorii[KategoriaHandlowa.ZamówienieOdbiorcy];
        public DefDokHandlowego OrderDefinition { get; set; }
    }
}