using System;
using System.Collections.Generic;
using System.Linq;
using PracaDyplomowaNT.Shipx;
using PracaDyplomowaNT.Shipx.Model;
using Soneta.Business;
using Soneta.Handel;

[assembly: Worker(typeof(SendShipment), typeof(DokumentHandlowy))]
namespace PracaDyplomowaNT.Shipx
{
    public class SendShipment
    {
        public const string apiUrl = "https://sandbox-api-shipx-pl.easypack24.net/v1/";

        [Context] public DokumentHandlowy Document { get; set; }
        [Context] public ShipmentParameters Parameters { get; set; }
        public Config Config { get; set; }

        [Action("PD_NT/Wyślij przesyłkę", Mode = ActionMode.SingleSession | ActionMode.Progress)]
        public void Send()
        {
            VerifyDocument();

            var shipment = new Shipment();
            shipment.SendingMethod = "dispatch_order";
            //shipment.SendingMethod = "inpost_locker_standard";
            shipment.Receiver = GetReceiver();
            //shipment.Sender = 
            shipment.Parcels = GetParcels();
            if (Document.Features.GetBool(Config.CodFeature))
                shipment.Cod = new Cod()
                {
                    Amount = (double)Document.BruttoCy.Value,
                    Currency = Document.BruttoCy.Symbol
                };

        }

        private void VerifyDocument()
        {
            if (Document.Definicja.Kategoria != KategoriaHandlowa.Sprzedaż || !Document.Zatwierdzony)
                throw new Exception("Paczkę można wysłać tylko dla zatwierdzonych faktur!");
        }

        private Receiver GetReceiver() => new Receiver()
        {
            Name = Document.Kontrahent.Kod,
            Email = Document.Kontrahent.EMAIL,
            Phone = Document.Kontrahent.Adres.Telefon
        };

        private List<Parcel> GetParcels()
        {
            var parcels = new List<Parcel>();
            foreach (PozycjaDokHandlowego position in Document.Pozycje.Where(x => x.Towar.Typ != Soneta.Towary.TypTowaru.Usługa))
            {
                int packages = 1;
                if (position.Ilosc.Symbol == "szt")
                    packages = (int)position.Ilosc.Value;
                for (int i = 0; i < packages; i++)
                {
                    string template = string.Empty;
                    switch (Config.ParcelTemplateTypeFeature)
                    {
                        case "A":
                            template = "small";
                            break;
                        case "B":
                            template = "medium";
                            break;
                        case "C":
                            template = "large";
                            break;
                    }

                    parcels.Add(new Parcel()
                    {
                        Id = $"{position.Towar.Kod}__{i + 1}",
                        Template = template
                    });
                }

            }

            return parcels;
        }

        private void PrepareRequest()
        {

        }
    }
}
