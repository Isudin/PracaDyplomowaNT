using System.Text;
using Soneta.Business;
using Soneta.CRM;

namespace PracaDyplomowaNT.Triggers.Verifiers
{
    public class ContractorVerifier : RowVerifier, ISessionable
    {
        public ContractorVerifier(IRow row) : base(row) { }

        public static void AddContractorVerifier(IRow row) => row.Session.Verifiers.Add(new ContractorVerifier(row));

        public Kontrahent Contractor => (Kontrahent)Row;

        public Session Session => Contractor.Session;

        public override string Description
        {
            get
            {
                var builder = new StringBuilder();
                builder.AppendLine("Nie można zatwierdzić kontrahenta! Wymagane dane:");
                if (!HasEmail)
                    builder.AppendLine("- adres email");

                if (!HasPhoneNumber)
                    builder.AppendLine("- numer telefonu");

                return builder.ToString();
            }
        }

        protected override bool IsValid() => HasEmail && HasPhoneNumber;

        private bool HasEmail => !string.IsNullOrWhiteSpace(Contractor.EMAIL);
        private bool HasPhoneNumber => !string.IsNullOrWhiteSpace(Contractor.Kontakt.TelefonKomorkowy);
    }
}

