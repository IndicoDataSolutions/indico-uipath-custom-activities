using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Activities;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities.Localization;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.DocumentExtraction_DisplayName))]
    [LocalizedDescription(nameof(Resources.DocumentExtraction_Description))]
    public class DocumentExtraction : IndicoActivityBase<(string ConfigType, string Document), string>
    {
        [LocalizedDisplayName(nameof(Resources.DocumentExtraction_ConfigType_DisplayName))]
        [LocalizedDescription(nameof(Resources.DocumentExtraction_ConfigType_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ConfigType { get; set; } = "standard";

        [LocalizedDisplayName(nameof(Resources.DocumentExtraction_Document_DisplayName))]
        [LocalizedDescription(nameof(Resources.DocumentExtraction_Document_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> Document { get; set; }

        [LocalizedDisplayName(nameof(Resources.DocumentExtraction_Results_DisplayName))]
        [LocalizedDescription(nameof(Resources.DocumentExtraction_Results_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> Results { get; set; }
        
        protected override (string ConfigType, string Document) GetInputs(AsyncCodeActivityContext ctx) =>
            (ConfigType.Get(ctx), Document.Get(ctx));

        protected override async Task<string> ExecuteAsync((string ConfigType, string Document) input, CancellationToken cancellationToken)
            => await Application.ExtractDocument(input.Document, input.ConfigType, cancellationToken);

        protected override void SetOutputs(AsyncCodeActivityContext ctx, string output) => Results.Set(ctx, output);
    }
}

