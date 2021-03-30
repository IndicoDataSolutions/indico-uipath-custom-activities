using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using Indico.RPAActivities.Activities.Design.Designers;
using Indico.RPAActivities.Activities.Design.Properties;

namespace Indico.RPAActivities.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var rootCategoryAttribute = new CategoryAttribute($"{Resources.RootCategory}");
            var workflowCategoryAttribute = new CategoryAttribute($"{Resources.WorkflowCategory}");
            var ocrCategoryAttribute = new CategoryAttribute($"{Resources.OCRCategory}");
            var submissionCategoryAttribute = new CategoryAttribute($"{Resources.SubmissionCategory}");
            var preparationCategoryAttribute = new CategoryAttribute($"{Resources.PreparationCategory}");

            builder.AddCustomAttributes(typeof(IndicoScope), rootCategoryAttribute);
            builder.AddCustomAttributes(typeof(IndicoScope), new DesignerAttribute(typeof(IndicoScopeDesigner)));
            builder.AddCustomAttributes(typeof(IndicoScope), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(ListDatasets), preparationCategoryAttribute);
            builder.AddCustomAttributes(typeof(ListDatasets), new DesignerAttribute(typeof(ListDatasetsDesigner)));
            builder.AddCustomAttributes(typeof(ListDatasets), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetModelGroup), preparationCategoryAttribute);
            builder.AddCustomAttributes(typeof(GetModelGroup), new DesignerAttribute(typeof(GetModelGroupDesigner)));
            builder.AddCustomAttributes(typeof(GetModelGroup), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(DocumentExtraction), ocrCategoryAttribute);
            builder.AddCustomAttributes(typeof(DocumentExtraction), new DesignerAttribute(typeof(DocumentExtractionDesigner)));
            builder.AddCustomAttributes(typeof(DocumentExtraction), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(Classify), preparationCategoryAttribute);
            builder.AddCustomAttributes(typeof(Classify), new DesignerAttribute(typeof(ClassifyDesigner)));
            builder.AddCustomAttributes(typeof(Classify), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(ListWorkflows), workflowCategoryAttribute);
            builder.AddCustomAttributes(typeof(ListWorkflows), new DesignerAttribute(typeof(ListWorkflowsDesigner)));
            builder.AddCustomAttributes(typeof(ListWorkflows), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(WorkflowSubmission), workflowCategoryAttribute);
            builder.AddCustomAttributes(typeof(WorkflowSubmission), new DesignerAttribute(typeof(WorkflowSubmissionDesigner)));
            builder.AddCustomAttributes(typeof(WorkflowSubmission), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(SubmissionResult), submissionCategoryAttribute);
            builder.AddCustomAttributes(typeof(SubmissionResult), new DesignerAttribute(typeof(SubmissionResultDesigner)));
            builder.AddCustomAttributes(typeof(SubmissionResult), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(ListSubmissions), submissionCategoryAttribute);
            builder.AddCustomAttributes(typeof(ListSubmissions), new DesignerAttribute(typeof(ListSubmissionsDesigner)));
            builder.AddCustomAttributes(typeof(ListSubmissions), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(SubmitReview), submissionCategoryAttribute);
            builder.AddCustomAttributes(typeof(SubmitReview), new DesignerAttribute(typeof(SubmitReviewDesigner)));
            builder.AddCustomAttributes(typeof(SubmitReview), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
