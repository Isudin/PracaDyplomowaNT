using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Soneta.Business;
using Soneta.Business.UI;
using Soneta.CRM;
using Soneta.Handel;
using Soneta.Magazyny;
using Soneta.Towary;
using Soneta.Types;

[assembly: Worker(typeof(PracaDyplomowaNT.OrderImport.OrderImport), typeof(DokHandlowe))]
namespace PracaDyplomowaNT.OrderImport
{
    public class OrderImport
    {
        #region Rows And Columns

        private const int StartingRow = 1;
        private const int ItemIdColumn = 0;
        private const int QuantityColumn = 1;
        private const int PriceColumn = 2;
        private const int CodColumn = 3;
        private const int FirstNameColumn = 4;
        private const int LastNameColumn = 5;
        private const int StreetColumn = 6;
        private const int HouseNumberColumn = 7;
        private const int ApartmentNumberColumn = 8;
        private const int CityColumn = 9;
        private const int PostalCodeColumn = 10;
        private const int EmailColumn = 11;
        private const int PhoneColumn = 12;
        private const int TargetPointColumn = 13;

        #endregion

        private int _createdDocuments = 0;

        private class OrdersByContractor
        {
            public OrdersByContractor(Kontrahent contractor, List<Order> orders, string targetPoint, bool cod)
            {
                Contractor = contractor;
                Orders = orders;
                TargetPoint = targetPoint;
                Cod = cod;
            }

            public Kontrahent Contractor { get; set; }
            public List<Order> Orders { get; set; }
            public string TargetPoint { get; set; }
            public bool Cod { get; set; }
        }

        [Context] public CsvParams Parameters { get; set; }
        [Context] public Session Session { get; set; }
        [Context] public Magazyn Warehouse { get; set; }
        public Config Config { get; set; }

        [Action("PD_NT/Zaimportuj zamówienia CSV", Mode = ActionMode.SingleSession | ActionMode.Progress)]
        public object ImportOrders()
        {
            string[] content = ReadCsvFromFile(Parameters.FilePath);
            if (!content.Any())
                throw new Exception("Nie udało się odczytać danych ze wskazanego pliku!");

            Config = new Config() { Session = Session };
            List<OrdersByContractor> ordersByContractor = GetDeserializedOrdersByContractor(content);
            CreateDocuments(ordersByContractor);

            if (_createdDocuments > 0)
                return new MessageBoxInformation("Import zamówień", $"Pomyślnie zaimportowano {_createdDocuments} zamówień.");

            return new MessageBoxInformation("Import zamówieńa", $"Nie zaimportowano żadnego zamówienia.");
        }

        private string[] ReadCsvFromFile(string uri)
        {
            var responseStream = new StreamReader(uri);
            string content = responseStream.ReadToEnd();
            return content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private List<OrdersByContractor> GetDeserializedOrdersByContractor(string[] content)
        {
            var ordersByContractor = new List<OrdersByContractor>();
            for (int row = StartingRow; row < content.Length; row++)
            {
                Trace.WriteLine($"Odczytywanie danych z pliku", "Progress");
                Trace.WriteLine(100 * row / content.Length, "Progress");

                Order order = DeserializeOrder(content[row]);
                Kontrahent contractor = GetContractor(order);
                AssignOrderByContractor(ordersByContractor, order, contractor);
            }

            return ordersByContractor;
        }

        private Order DeserializeOrder(string serializedOrder)
        {
            string[] cells = serializedOrder.Split(',');
            var order = new Order();
            order.ItemId = cells[ItemIdColumn];
            order.Quantity = new Quantity(double.Parse(cells[QuantityColumn], CultureInfo.InvariantCulture));
            order.Price = new DoubleCy(double.Parse(cells[PriceColumn], CultureInfo.InvariantCulture));
            order.Cod = int.Parse(cells[CodColumn]) == 1;
            order.FirstName = cells[FirstNameColumn];
            order.LastName = cells[LastNameColumn];
            order.Street = cells[StreetColumn];
            order.HouseNumber = cells[HouseNumberColumn];
            order.ApartmentNumber = cells[ApartmentNumberColumn];
            order.City = cells[CityColumn];
            order.PostalCode = cells[PostalCodeColumn];
            order.Email = cells[EmailColumn];
            order.PhoneNumber = cells[PhoneColumn];
            order.TargetPoint = cells[TargetPointColumn];
            return order;
        }

        private Kontrahent GetContractor(Order order)
        {
            Kontrahent contractor = null;
            using (Session session = Session.Login.CreateSession(false, false))
            {
                using (ITransaction transaction = session.Logout(true))
                {
                    var crm = CRMModule.GetInstance(session);
                    View contractors = crm.Kontrahenci.CreateView();
                    contractors.Condition = new FieldCondition.Equal("Nazwa", $"{order.FirstName} {order.LastName}");
                    contractor = contractors.Any() ? (Kontrahent)contractors.First() : CreateContractor(order, crm);
                    UpdateContractor(order, contractor);

                    transaction.CommitUI();
                }

                session.Save();
            }

            return contractor;
        }

        private Kontrahent CreateContractor(Order order, CRMModule crmModule)
        {
            var contractor = new Kontrahent();
            crmModule.Kontrahenci.AddRow(contractor);
            contractor.Nazwa = $"{order.FirstName} {order.LastName}";
            contractor.EMAIL = order.Email;
            contractor.Kontakt.TelefonKomorkowy = order.PhoneNumber;
            contractor.Adres.KodKraju = "PL";
            contractor.Adres.Miejscowosc = order.City;
            contractor.Adres.Ulica = order.Street;
            contractor.Adres.NrDomu = order.HouseNumber;
            contractor.Adres.NrLokalu = order.ApartmentNumber;
            contractor.Adres.KodPocztowyS = order.PostalCode;

            return contractor;
        }

        private void UpdateContractor(Order order, Kontrahent contractor)
        {
            if (!string.IsNullOrWhiteSpace(order.Email) && contractor.EMAIL != order.Email)
                contractor.EMAIL = order.Email;

            if (!string.IsNullOrWhiteSpace(order.PhoneNumber) && contractor.Kontakt.TelefonKomorkowy != order.PhoneNumber)
                contractor.Kontakt.TelefonKomorkowy = order.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(order.City) && contractor.Adres.Miejscowosc != order.City)
                contractor.Adres.Miejscowosc = order.City;

            if (!string.IsNullOrWhiteSpace(order.Street) && contractor.Adres.Ulica != order.Street)
                contractor.Adres.Ulica = order.Street;

            if (!string.IsNullOrWhiteSpace(order.HouseNumber) && contractor.Adres.NrDomu != order.HouseNumber)
                contractor.Adres.NrDomu = order.HouseNumber;

            if (!string.IsNullOrWhiteSpace(order.ApartmentNumber) && contractor.Adres.NrLokalu != order.ApartmentNumber)
                contractor.Adres.NrLokalu = order.ApartmentNumber;

            if (!string.IsNullOrWhiteSpace(order.PostalCode) && contractor.Adres.KodPocztowyS != order.PostalCode)
                contractor.Adres.KodPocztowyS = order.PostalCode;
        }

        private void AssignOrderByContractor(List<OrdersByContractor> assignedOrders, Order importedOrder, Kontrahent contractor)
        {
            OrdersByContractor ordersByContractor = assignedOrders.FirstOrDefault(x => x.Contractor.Guid == contractor.Guid && x.TargetPoint == importedOrder.TargetPoint && x.Cod == importedOrder.Cod);
            if (ordersByContractor != null)
                ordersByContractor.Orders.Add(importedOrder);
            else
            {
                var importedOrders = new List<Order>
                {
                    importedOrder
                };
                assignedOrders.Add(new OrdersByContractor(contractor, importedOrders, importedOrder.TargetPoint,
                    importedOrder.Cod));
            }
        }

        private void CreateDocuments(List<OrdersByContractor> assignedOrders)
        {
            int errors = 0;
            for (int i = 0; i < assignedOrders.Count; i++)
            {
                OrdersByContractor ordersByContractor = assignedOrders[i];
                Trace.WriteLine($"Odczytywanie danych z pliku", "Progress");
                Trace.WriteLine(100 * i / assignedOrders.Count, "Progress");

                try
                {
                    CreateDocument(ordersByContractor);
                }
                catch (Exception ex)
                {
                    errors++;
                }
            }

            _createdDocuments = assignedOrders.Count - errors;
        }

        private void CreateDocument(OrdersByContractor ordersByContractor)
        {
            using (Session session = ordersByContractor.Contractor.Session.Login.CreateSession(false, false))
            {
                using (ITransaction transaction = session.Logout(true))
                {
                    var handelModule = HandelModule.GetInstance(session);
                    var document = new DokumentHandlowy();
                    handelModule.DokHandlowe.AddRow(document);
                    document.Definicja = Parameters.OrderDefinition;
                    document.Magazyn = Warehouse;
                    document.Kontrahent = ordersByContractor.Contractor;
                    document.Features[Config.TargetPointFeature] = ordersByContractor.TargetPoint;
                    document.Features[Config.CodFeature] = ordersByContractor.Cod;

                    foreach (Order importedOrder in ordersByContractor.Orders)
                        CreatePosition(handelModule, document, importedOrder);

                    AddDefaultService(handelModule, document);

                    transaction.CommitUI();
                }

                session.Save();
            }
        }

        private void CreatePosition(HandelModule handelModule, DokumentHandlowy document, Order order)
            => CreatePosition(handelModule, document, GetWareById(order.ItemId), order.Price, order.Quantity);

        private void AddDefaultService(HandelModule handelModule, DokumentHandlowy document)
        {
            Towar ware = Config.DefaultDeliveryService;
            CreatePosition(handelModule, document, ware, ware.Ceny["Detaliczna"].Brutto, new Quantity(1));
        }

        private void CreatePosition(HandelModule handelModule, DokumentHandlowy document, Towar ware, DoubleCy price, Quantity quantity)
        {
            var position = new PozycjaDokHandlowego(document);
            handelModule.PozycjeDokHan.AddRow(position);
            position.Towar = ware;
            position.Cena = price;
            position.Ilosc = quantity;
        }

        private Towar GetWareById(string id) => TowaryModule.GetInstance(Session).Towary.WgKodu[id];
    }
}