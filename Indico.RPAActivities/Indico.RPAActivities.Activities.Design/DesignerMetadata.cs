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

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(IndicoScope), categoryAttribute);
            builder.AddCustomAttributes(typeof(IndicoScope), new DesignerAttribute(typeof(IndicoScopeDesigner)));
            builder.AddCustomAttributes(typeof(IndicoScope), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(ListDatasets), categoryAttribute);
            builder.AddCustomAttributes(typeof(ListDatasets), new DesignerAttribute(typeof(ListDatasetsDesigner)));
            builder.AddCustomAttributes(typeof(ListDatasets), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetModelGroup), categoryAttribute);
            builder.AddCustomAttributes(typeof(GetModelGroup), new DesignerAttribute(typeof(GetModelGroupDesigner)));
            builder.AddCustomAttributes(typeof(GetModelGroup), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(DocumentExtraction), categoryAttribute);
            builder.AddCustomAttributes(typeof(DocumentExtraction), new DesignerAttribute(typeof(DocumentExtractionDesigner)));
            builder.AddCustomAttributes(typeof(DocumentExtraction), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(Classify), categoryAttribute);
            builder.AddCustomAttributes(typeof(Classify), new DesignerAttribute(typeof(ClassifyDesigner)));
            builder.AddCustomAttributes(typeof(Classify), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(ExtractValues), categoryAttribute);
            builder.AddCustomAttributes(typeof(ExtractValues), new DesignerAttribute(typeof(ExtractValuesDesigner)));
            builder.AddCustomAttributes(typeof(ExtractValues), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(ListWorkflows), categoryAttribute);
            builder.AddCustomAttributes(typeof(ListWorkflows), new DesignerAttribute(typeof(ListWorkflowsDesigner)));
            builder.AddCustomAttributes(typeof(ListWorkflows), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(WorkflowSubmission), categoryAttribute);
            builder.AddCustomAttributes(typeof(WorkflowSubmission), new DesignerAttribute(typeof(WorkflowSubmissionDesigner)));
            builder.AddCustomAttributes(typeof(WorkflowSubmission), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(SubmissionResult), categoryAttribute);
            builder.AddCustomAttributes(typeof(SubmissionResult), new DesignerAttribute(typeof(SubmissionResultDesigner)));
            builder.AddCustomAttributes(typeof(SubmissionResult), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
