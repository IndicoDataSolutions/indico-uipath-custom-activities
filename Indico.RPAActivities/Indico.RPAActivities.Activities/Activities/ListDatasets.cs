using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Indico.RPAActivities.Activities.Activities;
using Indico.RPAActivities.Activities.Properties;
using Indico.RPAActivities.Entity;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.ListDatasets_DisplayName))]
    [LocalizedDescription(nameof(Resources.ListDatasets_Description))]
    public class ListDatasets : IndicoActivityBase<bool, List<Dataset>>
    {
        [LocalizedDisplayName(nameof(Resources.ListDatasets_Datasets_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListDatasets_Datasets_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<List<Dataset>> Datasets { get; set; }


        public ListDatasets()
        {
            Constraints.Add(ActivityConstraints.HasParentType<ListDatasets, IndicoScope>(string.Format(Resources.ValidationScope_Error, Resources.IndicoScope_DisplayName)));
        }
        
        protected override bool GetInputs(AsyncCodeActivityContext ctx) => true; // no input, using dummy bool

        protected override Task<List<Dataset>> ExecuteAsync(bool input, CancellationToken cancellationToken) =>
            Application.ListDatasets(cancellationToken);

        protected override void SetOutputs(AsyncCodeActivityContext ctx, List<Dataset> output) =>
            Datasets.Set(ctx, output);
    }
}

