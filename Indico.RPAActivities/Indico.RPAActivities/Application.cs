using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indico;
using Indico.Request;
using Indico.Entity;
using Newtonsoft.Json.Linq;
using Indico.RPAActivities.Models;
using Newtonsoft.Json;

namespace Indico.RPAActivities
{
    public class Application
    {
        #region Properties
        protected IndicoClient Client;
        #endregion

        #region Constructors
        public Application() { }

        public Application(string token, string host)
        {
            IndicoConfig config = new IndicoConfig(host: host, apiToken: token);
            Client = new IndicoClient(config);
        }
        #endregion

        public List<Dataset> ListDatasets()
        {
            string query = @"
              query GetDatasets {
                datasets {
                  id
                  name
                  status
                  rowCount
                  numModelGroups
                  modelGroups {
                    id
                  }
                }
              }
            ";

            try
            {
                GraphQLRequest request = Client.GraphQLRequest(query, "GetDatasets");
                JObject response = request.Call();
                return response.ToObject<List<Dataset>>();

            } catch(AggregateException sae)
            {
                Console.WriteLine("Call failed " + sae.ToString());
                throw new IndicoActivityException();
            }
        }


        public Indico.Entity.ModelGroup GetModelGroup(int mgId)
        {
            try
            {
                //TODO: Map this to a local model so it's easier to deal with in UIPath
                Indico.Entity.ModelGroup mg = Client.ModelGroupQuery(mgId).Exec();
                return mg;
            }
            catch(AggregateException sae)
            {
                Console.WriteLine("Call failed " + sae.ToString());
                throw new IndicoActivityException();
            }
        }
    }
}
