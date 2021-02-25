using System.Activities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Indico.Entity;
using Indico.RPAActivities.Activities.Activities;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.ListWorkflows_DisplayName))]
    [LocalizedDescription(nameof(Resources.ListWorkflows_Description))]
    public class ListWorkflows : IndicoActivityBase<int, List<Workflow>>
    {
        [LocalizedDisplayName(nameof(Resources.ListWorkflows_DatasetID_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListWorkflows_DatasetID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> DatasetID { get; set; }

        [LocalizedDisplayName(nameof(Resources.ListWorkflows_Workflows_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListWorkflows_Workflows_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<object> Workflows { get; set; }


        public ListWorkflows()
        {
            Constraints.Add(ActivityConstraints.HasParentType<ListWorkflows, IndicoScope>(string.Format(Resources.ValidationScope_Error, Resources.IndicoScope_DisplayName)));
        }


        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (DatasetID == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(DatasetID)));

            base.CacheMetadata(metadata);
        }

        protected override int GetInputs(AsyncCodeActivityContext ctx) => DatasetID.Get(ctx);

        protected override Task<List<Workflow>> ExecuteAsync(int dataSetId, CancellationToken cancellationToken) =>
            Application.ListWorkflows(dataSetId, cancellationToken);

        protected override void SetOutputs(AsyncCodeActivityContext ctx, List<Workflow> output) => Workflows.Set(ctx, output);
    }
}
