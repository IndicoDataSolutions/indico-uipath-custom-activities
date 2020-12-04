using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Indico.Entity;
using Indico.Request;
using Indico.Jobs;
using Indico.Mutation;
using Indico.Query;
using Indico.RPAActivities.Entity;

namespace Indico.RPAActivities
{
    public class Application
    {
        #region Properties

        IndicoClient _client;

        #endregion

        #region Constructors

        public Application(string token, string host)
        {
            IndicoConfig config = new IndicoConfig(host: host, apiToken: token);
            _client = new IndicoClient(config);
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

            GraphQLRequest request = _client.GraphQLRequest(query, "GetDatasets");
            JObject result = await request.Call();
            JArray datasets = (JArray) result.GetValue("datasets");
            return datasets.ToObject<List<Dataset>>();
        }

        public async Task<List<Workflow>> ListWorkflows(int datasetId)
        {
            ListWorkflows listWorkflows = new ListWorkflows(_client)
            {
                DatasetIds = {datasetId}
            };
            List<Workflow> workflows = await listWorkflows.Exec();
            return workflows;
        }

        public async Task<ModelGroup> GetModelGroup(int mgId)
        {
            ModelGroup mg = await _client.ModelGroupQuery(mgId).Exec();
            return mg;
        }

        public async Task<Document> ExtractDocument(string document, string configType = "standard")
        {
            JObject extractConfig = new JObject()
            {
                {"preset_config", configType}
            };

            DocumentExtraction ocr = _client.DocumentExtraction(extractConfig);
            Job job = await ocr.Exec(document);
            JObject result = await job.Result();
            string resUrl = (string) result.GetValue("url");
            Storage.Blob blob = await _client.RetrieveBlob(resUrl).Exec();
            JObject obj = blob.AsJSONObject();

            var doc = new Document();
            doc.Text = (string) obj.GetValue("text");

            return doc;
        }


        public async Task<List<Dictionary<string, double>>> Classify(List<string> values, int modelGroup)
        {
            ModelGroup mg = await _client.ModelGroupQuery(modelGroup).Exec();
            var status = await _client.ModelGroupLoad(mg).Exec();
            Job job = await _client.ModelGroupPredict(mg).Data(values).Exec();
            JArray jobResult = await job.Results();
            return jobResult.ToObject<List<Dictionary<string, double>>>();
        }

        public async Task<List<List<Extraction>>> Extract(List<string> values, int modelGroup)
        {
            ModelGroup mg = await _client.ModelGroupQuery(modelGroup).Exec();
            string status = await _client.ModelGroupLoad(mg).Exec();
            Job job = await _client.ModelGroupPredict(mg).Data(values).Exec();
            JArray jobResult = await job.Results();
            return jobResult.ToObject<List<List<Extraction>>>();
        }
    }
}