using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Activities;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities.Localization;
using IndicoV2.Workflows.Models;

namespace Indico.RPAActivities.Activities
{
    [LocalizedCategory(nameof(Resources.WorkflowCategory))]
    [LocalizedDisplayName(nameof(Resources.ListWorkflows_DisplayName))]
    [LocalizedDescription(nameof(Resources.ListWorkflows_Description))]
    public class ListWorkflows : IndicoActivityBase<int, List<IWorkflow>>
    {
        [LocalizedDisplayName(nameof(Resources.ListWorkflows_DatasetID_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListWorkflows_DatasetID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<int> DatasetID { get; set; }

        [LocalizedDisplayName(nameof(Resources.ListWorkflows_Workflows_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListWorkflows_Workflows_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<List<IWorkflow>> Workflows { get; set; }

        protected override int GetInputs(AsyncCodeActivityContext ctx) => DatasetID.Get(ctx);

        protected override async Task<List<IWorkflow>> ExecuteAsync(int dataSetId, CancellationToken cancellationToken) =>
            (await Application.ListWorkflows(dataSetId, cancellationToken)).ToList();

        protected override void SetOutputs(AsyncCodeActivityContext ctx, List<IWorkflow> output) => Workflows.Set(ctx, output);
    }
}
