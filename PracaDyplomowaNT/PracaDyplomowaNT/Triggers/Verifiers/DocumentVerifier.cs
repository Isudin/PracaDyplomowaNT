using System.Text;
using Soneta.Business;
using Soneta.Core;
using Soneta.Handel;

namespace PracaDyplomowaNT.Triggers.Verifiers
{
    public class DocumentVerifier : RowVerifier, ISessionable
    {
        public DocumentVerifier(IRow row) : base(row) { }

        public static void AddDocumentVerifier(IRow row) => row.Session.Verifiers.Add(new DocumentVerifier(row));

        public DokumentHandlowy Document => (DokumentHandlowy)Row;

        public Session Session => Document.Session;

        public override string Description => "Wymagany jest pełny adres kontrahenta bądź numer paczkomatu!";

        protected override bool IsValid() => Document.Kategoria != KategoriaHandlowa.Sprzedaż || HasWholeAddress || HasTargetPoint;

        private bool HasWholeAddress
        {
            get
            {
                Adres address = Document.Kontrahent.Adres;
                return string.IsNullOrWhiteSpace(address.KodKraju)
                    || string.IsNullOrWhiteSpace(address.KodPocztowyS)
                    || string.IsNullOrWhiteSpace(address.Miejscowosc)
                    || string.IsNullOrWhiteSpace(address.Ulica)
                    || string.IsNullOrWhiteSpace(address.NrDomu)
                    || string.IsNullOrWhiteSpace(address.NrLokalu)
                    ? false
                    : true;
            }
        }
        private bool HasTargetPoint
        {
            get
            {
                var config = new Config() { Session = Document.Session };
                return string.IsNullOrWhiteSpace(Document.Features.GetString(config.TargetPointFeature)) ? false : true;
            }
        }
    }
}
