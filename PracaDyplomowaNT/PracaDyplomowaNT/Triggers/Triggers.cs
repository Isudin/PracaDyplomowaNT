using Soneta.Business;
using Soneta.CRM;
using Soneta.Handel;

[assembly: ProgramInitializer(typeof(PracaDyplomowaNT.Triggers.Initializer))]
namespace PracaDyplomowaNT.Triggers
{
    public class Initializer : IProgramInitializer
    {
        void IProgramInitializer.Initialize() => new Triggers();
    }

    public class Triggers
    {
        static Triggers()
        {
            CRMModule.KontrahentSchema.AddOnAdded(VerifyContractor);
            HandelModule.DokumentHandlowySchema.AddOnAdded(VerifyDocument);
        }

        private static void VerifyContractor(CRMModule.KontrahentRow row)
        {
            if (row.State != RowState.Detached && row.State != RowState.Deleted)
                Verifiers.ContractorVerifier.AddContractorVerifier(row);
        }

        private static void VerifyDocument(HandelModule.DokumentHandlowyRow row)
        {
            if (row.State != RowState.Detached && row.State != RowState.Deleted)
                Verifiers.DocumentVerifier.AddDocumentVerifier(row);
        }
    }
}
