using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PracaDyplomowaNT.Shipx;
using PracaDyplomowaNT.Shipx.Model;
using PracaDyplomowaNT.Shipx.Requests;
using Soneta.Business;
using Soneta.Business.UI;
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
        public MessageBoxInformation Send()
        {
            VerifyDocument();
            Config = new Config() { Session = Document.Session };

            var shipment = new Shipment();
            shipment.SendingMethod = "dispatch_order";
            string targetPoint = Document.Features.GetString(Config.TargetPointFeature);
            shipment.Service = string.IsNullOrWhiteSpace(targetPoint) ? "inpost_courier_standard" : "inpost_locker_standard";
            shipment.CustomAttributes = new CustomAttributes() { TargetPoint = targetPoint };
            shipment.Receiver = GetReceiver(shipment.Service);
            shipment.Parcels = GetParcels();
            if (Document.Features.GetBool(Config.CodFeature))
            {
                if (shipment.Service == "inpost_locker_standard")
                    throw new Exception("Przesyłki paczkomatowe nie obsługują dodatkowych usług.");

                shipment.Cod = new Cod()
                {
                    Amount = (double)Document.BruttoCy.Value,
                    Currency = Document.BruttoCy.Symbol
                };
            }
            AddServices(shipment);

            shipment = PostShipment(shipment);
            for (int i = 0; i < 10; i++)
            {

                System.Threading.Thread.Sleep(2000);
                shipment = GetShipment(shipment.Id);
                if (!string.IsNullOrWhiteSpace(shipment.TrackingNumber)) break;
            }

            SaveShipmentData(shipment.TrackingNumber, shipment.Id, shipment.Status);
            return string.IsNullOrWhiteSpace(shipment.TrackingNumber)
                ? new MessageBoxInformation("Napotkano błąd", $"Serwer nie zwrócił numeru przesyłki. Sprawdź czy posiadasz środki na koncie InPost.")
                : new MessageBoxInformation("Sukces", $"Pomyślnie wysłano przesyłkę. Numer przesyłki: {shipment.TrackingNumber}");
        }

        private void AddServices(Shipment shipment)
        {
            var additionalServices = new List<string>();
            if (Parameters.EmailNotification)
                additionalServices.Add("email");

            if (Parameters.SmsNotification)
                additionalServices.Add("sms");

            if (Parameters.SaturdayDelivery)
                additionalServices.Add("saturday");

            if (additionalServices.Count > 0)
                if (shipment.Service == "inpost_locker_standard")
                    throw new Exception("Przesyłki paczkomatowe nie obsługują dodatkowych usług.");
                else
                    shipment.AdditionalServices = additionalServices;
        }

        private void VerifyDocument()
        {
            if (Document.Definicja.Kategoria != KategoriaHandlowa.Sprzedaż || !Document.Zatwierdzony)
                throw new Exception("Paczkę można wysłać tylko dla zatwierdzonych faktur!");
        }

        private Receiver GetReceiver(string service)
        {
            var receiver = new Receiver()
            {
                Name = Document.Kontrahent.Nazwa,
                Email = Document.Kontrahent.EMAIL,
                Phone = Document.Kontrahent.Kontakt.TelefonKomorkowy,
            };

            if (service == "inpost_courier_standard")
            {
                receiver.FirstName = Document.Kontrahent.Nazwa.Split(' ')[0];
                receiver.LastName = Document.Kontrahent.Nazwa.Split(' ')[1];
                receiver.CompanyName = Document.Kontrahent.Nazwa;
                receiver.Address = new Address
                {
                    CountryCode = Document.Kontrahent.KodKraju,
                    PostCode = Document.Kontrahent.Adres.KodPocztowyS,
                    City = Document.Kontrahent.Adres.Miejscowosc,
                    Street = Document.Kontrahent.Adres.Ulica,
                    BuildingNumber = $"{Document.Kontrahent.Adres.NrDomu} {Document.Kontrahent.Adres.NrLokalu}"
                };
            }

            return receiver;
        }

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
                    switch (position.Towar.Features.GetString(Config.ParcelTemplateTypeFeature))
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

        private Shipment PostShipment(Shipment shipment)
        {
            string url = $@"{apiUrl}organizations/{Config.ApiUsername}/shipments/";
            string json = JsonConvert.SerializeObject(shipment);
            RequestsBase request = Requests.Requests.PreparePostRequest(url, Config);
            request.Json = json;
            object response = request.ApiRequest();

            if (response.ToString() == "")
                throw new Exception("Otrzymano pustą odpowiedź z serwera podczas pobierania statusu.");

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            return JsonConvert.DeserializeObject<Shipment>(response.ToString(), settings);
        }

        private Shipment GetShipment(int id)
        {
            string URL = $@"{apiUrl}shipments/{id}";
            RequestsBase request = Requests.Requests.PrepareGetRequest(URL, Config);
            object odpowiedz = request.ApiRequest();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            return JsonConvert.DeserializeObject<Shipment>(odpowiedz.ToString(), settings);
        }

        private void SaveShipmentData(string trackingNumer, int id, string status)
        {
            using (Session session = Config.Session.Login.CreateSession(false, false))
            {
                using (ITransaction transaction = session.Logout(true))
                {
                    DokumentHandlowy document = session.Get(Document);
                    document.Features[Config.ParcelTrackingNumberFeature] = trackingNumer;
                    document.Features[Config.ParcelIdFeature] = id;
                    document.Features[Config.ParcelStatusFeature] = status;

                    transaction.CommitUI();
                }
                session.Save();
            }
        }
    }
}
