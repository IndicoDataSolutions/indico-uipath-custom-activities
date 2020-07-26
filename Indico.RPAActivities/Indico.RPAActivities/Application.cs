using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indico;
using Indico.Request;
using Newtonsoft.Json.Linq;
using Indico.RPAActivities.Models;
using Newtonsoft.Json;
using AutoMapper;
using Indico.Jobs;

namespace Indico.RPAActivities
{
    public class Application
    {
        #region Properties
        protected IndicoClient Client;
        #endregion

        #region Constructors
        public Application() { }
        private Mapper mapper;

        public Application(string token, string host)
        {
            IndicoConfig config = new IndicoConfig(host: host, apiToken: token);
            Client = new IndicoClient(config);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Entity.ModelGroup, Models.ModelGroup>();
                cfg.CreateMap<Entity.Model, Models.Model>();
            });
            mapper = new Mapper(mapperConfig);
        }
        #endregion

        public async Task<List<Dataset>> ListDatasets()
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
                JObject result = await request.Call();
                JArray datasets = (JArray)result.GetValue("datasets");
                return datasets.ToObject<List<Dataset>>();
            } catch(AggregateException sae)
            {
                Console.WriteLine("Call failed " + sae.ToString());
                throw new IndicoActivityException();
            }
        }


        public async Task<Models.ModelGroup> GetModelGroup(int mgId)
        {
            try
            {
                Entity.ModelGroup mg = await Client.ModelGroupQuery(mgId).Exec();
                return mapper.Map<Models.ModelGroup>(mg);
            }
            catch(AggregateException sae)
            {
                Console.WriteLine("Call failed " + sae.ToString());
                throw new IndicoActivityException();
            }
        }

        public async Task<Document> ExtractDocument(string document, string configType = "standard")
        {
            try
            {
                JObject extractConfig = new JObject()
                {
                    { "preset_config", configType }
                };

                Mutation.DocumentExtraction ocr = Client.DocumentExtraction(extractConfig);
                Jobs.Job job = await ocr.Exec(document);
                JObject result = await job.Result();
                string resUrl = (string)result.GetValue("url");
                Storage.Blob blob = await Client.RetrieveBlob(resUrl).Exec();
                JObject obj = blob.AsJSONObject();

                var doc = new Document();
                doc.Text = (string)obj.GetValue("text");

                return doc;
            }
            catch(AggregateException sae)
            {
                Console.WriteLine("Call failed " + sae.ToString());
                throw new IndicoActivityException();
            }
        }


        public async Task<List<Dictionary<string, double>>> Classify(List<string> values, int modelGroup)
        {
            try
            {
                Entity.ModelGroup mg = await Client.ModelGroupQuery(modelGroup).Exec();
                var status = await Client.ModelGroupLoad(mg).Exec();
                Job job = await Client.ModelGroupPredict(mg).Data(values).Exec();
                JArray jobResult = await job.Results();
                return jobResult.ToObject<List<Dictionary<string, double>>>();

            }
            catch (AggregateException sae)
            {
                Console.WriteLine("Call failed " + sae.ToString());
                throw new IndicoActivityException();
            }
        }

        public async Task<List<List<Extraction>>> Extract(List<string> values, int modelGroup)
        {
            try
            {
                Entity.ModelGroup mg = await Client.ModelGroupQuery(modelGroup).Exec();
                var status = await Client.ModelGroupLoad(mg).Exec();
                Job job = await Client.ModelGroupPredict(mg).Data(values).Exec();
                JArray jobResult = await job.Results();
                return jobResult.ToObject<List<List<Extraction>>>();


            }
            catch (AggregateException sae)
            {
                Console.WriteLine("Call failed " + sae.ToString());
                throw new IndicoActivityException();
            }
        }
    }
}
