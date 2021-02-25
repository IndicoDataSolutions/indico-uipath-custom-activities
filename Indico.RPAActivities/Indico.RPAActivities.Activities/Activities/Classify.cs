using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Activities;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.Classify_DisplayName))]
    [LocalizedDescription(nameof(Resources.Classify_Description))]
    public class Classify : IndicoActivityBase<
        (List<string> Text, int ModelGroup),
        (List<string> TopResults, List<Dictionary<string, double>> Results)
    >
    {
        [LocalizedDisplayName(nameof(Resources.Classify_Text_DisplayName))]
        [LocalizedDescription(nameof(Resources.Classify_Text_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<List<string>> Text { get; set; }

        [LocalizedDisplayName(nameof(Resources.Classify_ModelGroup_DisplayName))]
        [LocalizedDescription(nameof(Resources.Classify_ModelGroup_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> ModelGroup { get; set; }

        [LocalizedDisplayName(nameof(Resources.Classify_TopResults_DisplayName))]
        [LocalizedDescription(nameof(Resources.Classify_TopResults_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<List<String>> TopResults { get; set; }

        [LocalizedDisplayName(nameof(Resources.Classify_Results_DisplayName))]
        [LocalizedDescription(nameof(Resources.Classify_Results_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<List<Dictionary<string, double>>> Results { get; set; }


        public Classify()
        {
            Constraints.Add(ActivityConstraints.HasParentType<Classify, IndicoScope>(string.Format(Resources.ValidationScope_Error, Resources.IndicoScope_DisplayName)));
        }


        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (Text == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Text)));
            if (ModelGroup == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ModelGroup)));

            base.CacheMetadata(metadata);
        }

        protected override (List<string> Text, int ModelGroup) GetInputs(AsyncCodeActivityContext ctx) => (Text.Get(ctx), ModelGroup.Get(ctx));

        protected override async Task<(List<string> TopResults, List<Dictionary<string, double>> Results)> ExecuteAsync((List<string> Text, int ModelGroup) input, CancellationToken cancellationToken)
        {
            var classifyResults = await Application.Classify(input.Text, input.ModelGroup, cancellationToken);

            return (GetTopResults(classifyResults).ToList(), classifyResults);
        }

        private IEnumerable<string> GetTopResults(List<Dictionary<string, double>> classifyResults)
        {
            foreach (var resultSet in classifyResults)
            {
                var topClass = new KeyValuePair<string, double>();
                foreach (var result in resultSet)
                {
                    if (result.Value > topClass.Value)
                    {
                        topClass = result;
                    }
                }

                yield return topClass.Key;
            }
        }

        protected override void SetOutputs(AsyncCodeActivityContext ctx, (List<string> TopResults, List<Dictionary<string, double>> Results) output)
        {
            TopResults.Set(ctx, TopResults);
            Results.Set(ctx, Results);
        }
    }
}
