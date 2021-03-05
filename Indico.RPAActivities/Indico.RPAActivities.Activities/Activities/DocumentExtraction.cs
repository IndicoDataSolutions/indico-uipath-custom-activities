using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Activities;
using Indico.RPAActivities.Activities.Properties;
using Indico.RPAActivities.Entity;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.DocumentExtraction_DisplayName))]
    [LocalizedDescription(nameof(Resources.DocumentExtraction_Description))]
    public class DocumentExtraction : IndicoActivityBase<(string ConfigType, string Document), Document>
    {
        [LocalizedDisplayName(nameof(Resources.DocumentExtraction_ConfigType_DisplayName))]
        [LocalizedDescription(nameof(Resources.DocumentExtraction_ConfigType_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ConfigType { get; set; } = "standard";

        [LocalizedDisplayName(nameof(Resources.DocumentExtraction_Document_DisplayName))]
        [LocalizedDescription(nameof(Resources.DocumentExtraction_Document_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Document { get; set; }

        [LocalizedDisplayName(nameof(Resources.DocumentExtraction_Results_DisplayName))]
        [LocalizedDescription(nameof(Resources.DocumentExtraction_Results_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<Document> Results { get; set; }


        public DocumentExtraction()
        {
            Constraints.Add(ActivityConstraints.HasParentType<DocumentExtraction, IndicoScope>(string.Format(Resources.ValidationScope_Error, Resources.IndicoScope_DisplayName)));
        }


        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (ConfigType == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ConfigType)));
            if (Document == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Document)));

            base.CacheMetadata(metadata);
        }
        
        protected override (string ConfigType, string Document) GetInputs(AsyncCodeActivityContext ctx) =>
            (ConfigType.Get(ctx), Document.Get(ctx));

        protected override async Task<Document> ExecuteAsync((string ConfigType, string Document) input, CancellationToken cancellationToken)
            => await Application.ExtractDocument(input.Document, input.ConfigType, cancellationToken);

        protected override void SetOutputs(AsyncCodeActivityContext ctx, Document output) => Results.Set(ctx, output);
    }
}

