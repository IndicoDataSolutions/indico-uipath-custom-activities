//using System.Activities;
//using System.Threading;
//using System.Threading.Tasks;
//using Indico.RPAActivities.Activities.Activities;
//using Indico.RPAActivities.Activities.Properties;
//using UiPath.Shared.Activities.Localization;
//using IndicoV2.Ocr.Models;

//namespace Indico.RPAActivities.Activities
//{
//    [LocalizedCategory(nameof(Resources.OCRCategory))]
//    [LocalizedDisplayName(nameof(Resources.DocumentExtraction_DisplayName))]
//    [LocalizedDescription(nameof(Resources.DocumentExtraction_Description))]
//    public class DocumentExtraction : IndicoActivityBase<(DocumentExtractionPreset Preset, string Document), string>
//    {
//        [LocalizedDisplayName(nameof(Resources.DocumentExtraction_ConfigType_DisplayName))]
//        [LocalizedDescription(nameof(Resources.DocumentExtraction_ConfigType_Description))]
//        [LocalizedCategory(nameof(Resources.Input_Category))]
//        public InArgument<DocumentExtractionPreset> Preset { get; set; } = DocumentExtractionPreset.Standard;

//        [LocalizedDisplayName(nameof(Resources.DocumentExtraction_Document_DisplayName))]
//        [LocalizedDescription(nameof(Resources.DocumentExtraction_Document_Description))]
//        [LocalizedCategory(nameof(Resources.Input_Category))]
//        [RequiredArgument]
//        public InArgument<string> Document { get; set; }

//        [LocalizedDisplayName(nameof(Resources.DocumentExtraction_Results_DisplayName))]
//        [LocalizedDescription(nameof(Resources.DocumentExtraction_Results_Description))]
//        [LocalizedCategory(nameof(Resources.Output_Category))]
//        public OutArgument<string> Results { get; set; }
        
//        protected override (DocumentExtractionPreset Preset, string Document) GetInputs(AsyncCodeActivityContext ctx) =>
//            (Preset.Get(ctx), Document.Get(ctx));

//        protected override async Task<string> ExecuteAsync((DocumentExtractionPreset Preset, string Document) input, CancellationToken cancellationToken)
//            => await Application.ExtractDocument(input.Document, input.Preset, cancellationToken);

//        protected override void SetOutputs(AsyncCodeActivityContext ctx, string output) => Results.Set(ctx, output);
//    }
//}

