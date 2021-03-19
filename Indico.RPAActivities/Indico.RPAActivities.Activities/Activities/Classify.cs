using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Activities;
using Indico.RPAActivities.Activities.Properties;
using IndicoV2.Models.Models;
using UiPath.Shared.Activities.Localization;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.Classify_DisplayName))]
    [LocalizedDescription(nameof(Resources.Classify_Description))]
    public class Classify : IndicoActivityBase<
        (List<string> Text, int ModelGroup),
        (List<string> TopResults, IPredictionJobResult Results)
    >
    {
        [LocalizedDisplayName(nameof(Resources.Classify_Text_DisplayName))]
        [LocalizedDescription(nameof(Resources.Classify_Text_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<List<string>> Text { get; set; }

        [LocalizedDisplayName(nameof(Resources.Classify_ModelGroup_DisplayName))]
        [LocalizedDescription(nameof(Resources.Classify_ModelGroup_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<int> ModelGroup { get; set; }

        [LocalizedDisplayName(nameof(Resources.Classify_TopResults_DisplayName))]
        [LocalizedDescription(nameof(Resources.Classify_TopResults_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<List<string>> TopResults { get; set; }

        [LocalizedDisplayName(nameof(Resources.Classify_Results_DisplayName))]
        [LocalizedDescription(nameof(Resources.Classify_Results_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<IPredictionJobResult> Results { get; set; }
        

        protected override (List<string> Text, int ModelGroup) GetInputs(AsyncCodeActivityContext ctx) => (Text.Get(ctx), ModelGroup.Get(ctx));

        protected override async Task<(List<string> TopResults, IPredictionJobResult Results)> ExecuteAsync((List<string> Text, int ModelGroup) input, CancellationToken cancellationToken)
        {
            var classifyResults = await Application.Classify(input.Text, input.ModelGroup, cancellationToken);

            return (GetTopResults(classifyResults).ToList(), classifyResults);
        }

        private IEnumerable<string> GetTopResults(IPredictionJobResult classifyResults)
        {
            foreach (var classifyResult in classifyResults)
            foreach (var prediction in classifyResult)
            {
                var topClass = new KeyValuePair<string, double>();
                foreach (var result in prediction.Confidence)
                {
                    if (result.Value > topClass.Value)
                    {
                        topClass = result;
                    }
                }

                yield return topClass.Key;
            }
        }

        protected override void SetOutputs(AsyncCodeActivityContext ctx, (List<string> TopResults, IPredictionJobResult Results) output)
        {
            TopResults.Set(ctx, output.TopResults);
            Results.Set(ctx, output.Results);
        }
    }
}
