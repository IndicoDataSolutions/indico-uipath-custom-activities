using System.Activities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Activities;
using Indico.RPAActivities.Activities.Properties;
using Indico.RPAActivities.Entity;
using UiPath.Shared.Activities.Localization;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.ExtractValues_DisplayName))]
    [LocalizedDescription(nameof(Resources.ExtractValues_Description))]
    public class ExtractValues : IndicoActivityBase<(List<string> Text, int ModelGroup), List<List<Extraction>>>
    {
        [LocalizedDisplayName(nameof(Resources.ExtractValues_Text_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExtractValues_Text_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<List<string>> Text { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExtractValues_ModelGroup_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExtractValues_ModelGroup_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> ModelGroup { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExtractValues_Results_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExtractValues_Results_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<List<List<Extraction>>> Results { get; set; }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (Text == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Text)));
            if (ModelGroup == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ModelGroup)));

            base.CacheMetadata(metadata);
        }

        protected override (List<string> Text, int ModelGroup) GetInputs(AsyncCodeActivityContext ctx) => (Text.Get(ctx), ModelGroup.Get(ctx));

        protected override Task<List<List<Extraction>>> ExecuteAsync((List<string> Text, int ModelGroup) input,
            CancellationToken cancellationToken)
            => Application.Extract(input.Text, input.ModelGroup, cancellationToken);

        protected override void SetOutputs(AsyncCodeActivityContext ctx, List<List<Extraction>> output) =>
            Results.Set(ctx, output);
    }
}
