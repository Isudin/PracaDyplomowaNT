using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PracaDyplomowaNT;
using PracaDyplomowaNT.Shipx.Model;
using Soneta.Business;
using Soneta.Handel;

[assembly: Worker(typeof(TestingClass), typeof(DokHandlowe))]
namespace PracaDyplomowaNT
{
    public class TestingClass
    {

        [Action("Test")]
        public void Test()
        {
            string json = "\"{\\\"href\\\":\\\"https://sandbox-api-shipx-pl.easypack24.net/v1/shipments/12039906\\\",\\\"id\\\":12039906,\\\"status\\\":\\\"created\\\",\\\"tracking_number\\\":null,\\\"service\\\":\\\"inpost_locker_standard\\\",\\\"reference\\\":null,\\\"is_return\\\":false,\\\"application_id\\\":3232,\\\"created_by_id\\\":null,\\\"external_customer_id\\\":null,\\\"sending_method\\\":null,\\\"end_of_week_collection\\\":false,\\\"comments\\\":null,\\\"mpk\\\":null,\\\"additional_services\\\":[],\\\"custom_attributes\\\":{\\\"target_point\\\":\\\"BBI01HO\\\"},\\\"cod\\\":{\\\"amount\\\":null,\\\"currency\\\":null},\\\"insurance\\\":{\\\"amount\\\":null,\\\"currency\\\":null},\\\"sender\\\":{\\\"id\\\":24083109,\\\"name\\\":null,\\\"company_name\\\":\\\"X DEFT SPÓŁKA Z OGRANICZONĄ ODPOWIEDZIALNOŚCIĄ SPÓŁKA KOMANDYTOWA\\\",\\\"first_name\\\":\\\"Natan\\\",\\\"last_name\\\":\\\"Trojak\\\",\\\"email\\\":\\\"natantrojak@wp.pl\\\",\\\"phone\\\":\\\"512290211\\\",\\\"address\\\":{\\\"id\\\":22876389,\\\"street\\\":\\\"Gorkiego M.\\\",\\\"building_number\\\":\\\"10\\\",\\\"line1\\\":null,\\\"line2\\\":null,\\\"city\\\":\\\"Bielsko-Biała\\\",\\\"post_code\\\":\\\"43-300\\\",\\\"country_code\\\":\\\"PL\\\"}},\\\"receiver\\\":{\\\"id\\\":24083108,\\\"name\\\":\\\"00001\\\",\\\"company_name\\\":null,\\\"first_name\\\":null,\\\"last_name\\\":null,\\\"email\\\":\\\"natantrojak@wp.pl\\\",\\\"phone\\\":\\\"123123123\\\",\\\"address\\\":null},\\\"selected_offer\\\":null,\\\"offers\\\":[],\\\"transactions\\\":[],\\\"parcels\\\":[{\\\"id\\\":12509068,\\\"identify_number\\\":\\\"BUT_NAR_43__1\\\",\\\"tracking_number\\\":null,\\\"is_non_standard\\\":false,\\\"template\\\":\\\"medium\\\",\\\"dimensions\\\":{\\\"length\\\":380.0,\\\"width\\\":640.0,\\\"height\\\":190.0,\\\"unit\\\":\\\"mm\\\"},\\\"weight\\\":{\\\"amount\\\":25.0,\\\"unit\\\":\\\"kg\\\"}}],\\\"created_at\\\":\\\"2023-01-08T18:57:59.619+01:00\\\",\\\"updated_at\\\":\\\"2023-01-08T18:57:59.619+01:00\\\"}\"";
            JsonConvert.DeserializeObject<Shipment>(json);

        }
    }
}
