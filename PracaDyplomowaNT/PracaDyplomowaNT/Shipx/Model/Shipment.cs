using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PracaDyplomowaNT.Shipx.Model
{
    public class Shipment
    {
        [JsonProperty("href")] public string Href { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("parcels")] public List<Parcel> Parcels { get; set; }
        [JsonProperty("custom_attributes")] public CustomAttributes CustomAttributes { get; set; }
        [JsonProperty("sender")] public Sender Sender { get; set; }
        [JsonProperty("receiver")] public Receiver Receiver { get; set; }
        [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }
        [JsonProperty("cod")] public Cod Cod { get; set; }
        [JsonProperty("insurance")] public Insurance Insurance { get; set; }
        [JsonProperty("additional_services")] public List<string> AdditionalServices { get; set; }
        [JsonProperty("reference")] public string Reference { get; set; }
        [JsonProperty("is_return")] public bool IsReturn { get; set; }
        [JsonProperty("tracking_number")] public string TrackingNumber { get; set; }
        [JsonProperty("created_by_id")] public int CreatedById { get; set; }
        [JsonProperty("offers")] public List<Offer> Offers { get; set; }
        [JsonProperty("selected_offer")] public Offer SelectedOffer { get; set; }
        // [JsonProperty("transactions")] public List<Transaction> Transactions { get; set; }
        [JsonProperty("sending_method")] public string SendingMethod { get; set; }
        [JsonProperty("external_customer_id")] public string ExternalCustomerId { get; set; }
    }

    public class Address
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("street")] public string Street { get; set; }
        [JsonProperty("building_number")] public string BuildingNumber { get; set; }
        [JsonProperty("city")] public string City { get; set; }
        [JsonProperty("post_code")] public string PostCode { get; set; }
        [JsonProperty("country_code")] public string CountryCode { get; set; }
    }

    public class Carrier
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
    }

    public class Cod
    {
        [JsonProperty("amount")] public double Amount { get; set; }
        [JsonProperty("currency")] public string Currency { get; set; }
    }

    public class CustomAttributes
    {
        [JsonProperty("target_point")] public string TargetPoint { get; set; }
        [JsonProperty("dropoff_point")] public string DropoffPoint { get; set; }
        [JsonProperty("sending_method")] public string SendingMethod { get; set; }
        [JsonProperty("dispatch_order_id")] public int DispatchOrderId { get; set; }
    }

    public class Insurance
    {
        [JsonProperty("amount")] public int Amount { get; set; }
        [JsonProperty("currency")] public string Currency { get; set; }
    }

    public class Offer
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("carrier")] public Carrier Carrier { get; set; }
        [JsonProperty("service")] public Service Service { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("expires_at")] public DateTime ExpiresAt { get; set; }
        [JsonProperty("rate")] public double Rate { get; set; }
        [JsonProperty("currency")] public string Currency { get; set; }

        [JsonProperty("unavailability_reasons")]
        public string UnavailabilityReasons { get; set; }
    }

    public class Receiver
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("company_name")] public string CompanyName { get; set; }
        [JsonProperty("first_name")] public string FirstName { get; set; }
        [JsonProperty("last_name")] public string LastName { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("phone")] public string Phone { get; set; }
        [JsonProperty("address")] public Address Address { get; set; }
    }

    public class Sender
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("company_name")] public string CompanyName { get; set; }
        [JsonProperty("first_name")] public string FirstName { get; set; }
        [JsonProperty("last_name")] public string LastName { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("phone")] public string Phone { get; set; }
        [JsonProperty("address")] public Address Address { get; set; }
    }

    public class Service
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
    }
}