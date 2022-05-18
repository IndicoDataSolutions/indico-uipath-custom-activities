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

            builder.AddCustomAttributes(typeof(IndicoScope), new DesignerAttribute(typeof(IndicoScopeDesigner)));
            builder.AddCustomAttributes(typeof(IndicoScope), new HelpKeywordAttribute(""));

            //builder.AddCustomAttributes(typeof(ListDatasets), new DesignerAttribute(typeof(ListDatasetsDesigner)));
            //builder.AddCustomAttributes(typeof(ListDatasets), new HelpKeywordAttribute(""));

            //builder.AddCustomAttributes(typeof(DocumentExtraction), new DesignerAttribute(typeof(DocumentExtractionDesigner)));
            //builder.AddCustomAttributes(typeof(DocumentExtraction), new HelpKeywordAttribute(""));

            //builder.AddCustomAttributes(typeof(ListWorkflows), new DesignerAttribute(typeof(ListWorkflowsDesigner)));
            //builder.AddCustomAttributes(typeof(ListWorkflows), new HelpKeywordAttribute(""));

            //builder.AddCustomAttributes(typeof(WorkflowSubmission), new DesignerAttribute(typeof(WorkflowSubmissionDesigner)));
            //builder.AddCustomAttributes(typeof(WorkflowSubmission), new HelpKeywordAttribute(""));

            //builder.AddCustomAttributes(typeof(SubmissionResult), new DesignerAttribute(typeof(SubmissionResultDesigner)));
            //builder.AddCustomAttributes(typeof(SubmissionResult), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(ListSubmissions), new DesignerAttribute(typeof(ListSubmissionsDesigner)));
            builder.AddCustomAttributes(typeof(ListSubmissions), new HelpKeywordAttribute(""));

            //builder.AddCustomAttributes(typeof(SubmitReview), new DesignerAttribute(typeof(SubmitReviewDesigner)));
            //builder.AddCustomAttributes(typeof(SubmitReview), new HelpKeywordAttribute(""));
            //builder.AddCustomAttributes(typeof(ListWithTimeout), categoryAttribute);
            //builder.AddCustomAttributes(typeof(ListWithTimeout), new DesignerAttribute(typeof(ListWithTimeoutDesigner)));
            //builder.AddCustomAttributes(typeof(ListWithTimeout), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
