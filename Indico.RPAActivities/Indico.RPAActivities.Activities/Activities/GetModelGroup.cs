using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Activities;
using Indico.RPAActivities.Activities.Properties;
using IndicoV2.Models.Models;
using UiPath.Shared.Activities.Localization;

namespace Indico.RPAActivities.Activities
{
    [LocalizedCategory(nameof(Resources.PreparationCategory))]
    [LocalizedDisplayName(nameof(Resources.GetModelGroup_DisplayName))]
    [LocalizedDescription(nameof(Resources.GetModelGroup_Description))]
    public class GetModelGroup : IndicoActivityBase<int, IModelGroup>
    {
        [LocalizedDisplayName(nameof(Resources.GetModelGroup_ModelGroupID_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetModelGroup_ModelGroupID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<int> ModelGroupID { get; set; }

        [LocalizedDisplayName(nameof(Resources.GetModelGroup_ModelGroupData_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetModelGroup_ModelGroupData_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<IModelGroup> ModelGroupData { get; set; }
        
        protected override int GetInputs(AsyncCodeActivityContext ctx) => ModelGroupID.Get(ctx);

        protected override async Task<IModelGroup> ExecuteAsync(int modelGroupId, CancellationToken cancellationToken) =>
            await Application.GetModelGroup(modelGroupId, cancellationToken);

        protected override void SetOutputs(AsyncCodeActivityContext ctx, IModelGroup output) => ModelGroupData.Set(ctx, output);
    }
}

