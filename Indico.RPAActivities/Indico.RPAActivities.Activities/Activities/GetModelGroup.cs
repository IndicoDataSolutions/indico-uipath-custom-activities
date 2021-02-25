using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Indico.Entity;
using Indico.RPAActivities.Activities.Activities;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.GetModelGroup_DisplayName))]
    [LocalizedDescription(nameof(Resources.GetModelGroup_Description))]
    public class GetModelGroup : IndicoActivityBase<int, ModelGroup>
    {
        [LocalizedDisplayName(nameof(Resources.GetModelGroup_ModelGroupID_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetModelGroup_ModelGroupID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> ModelGroupID { get; set; }

        [LocalizedDisplayName(nameof(Resources.GetModelGroup_ModelGroupData_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetModelGroup_ModelGroupData_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<ModelGroup> ModelGroupData { get; set; }


        public GetModelGroup()
        {
            Constraints.Add(ActivityConstraints.HasParentType<GetModelGroup, IndicoScope>(string.Format(Resources.ValidationScope_Error, Resources.IndicoScope_DisplayName)));
        }


        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (ModelGroupID == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ModelGroupID)));

            base.CacheMetadata(metadata);
        }

        protected override int GetInputs(AsyncCodeActivityContext ctx) => ModelGroupID.Get(ctx);

        protected override Task<ModelGroup> ExecuteAsync(int modelGroupId, CancellationToken cancellationToken) =>
            Application.GetModelGroup(modelGroupId, cancellationToken);

        protected override void SetOutputs(AsyncCodeActivityContext ctx, ModelGroup output) => ModelGroupData.Set(ctx, output);
    }
}

