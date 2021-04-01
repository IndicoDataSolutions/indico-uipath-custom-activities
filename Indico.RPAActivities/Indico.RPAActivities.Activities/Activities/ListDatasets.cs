using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Indico.RPAActivities.Activities.Activities;
using Indico.RPAActivities.Activities.Properties;
using Indico.UiPath.Shared.Activities.Localization;
using IndicoV2.DataSets.Models;

namespace Indico.RPAActivities.Activities
{
    [LocalizedCategory(nameof(Resources.PreparationCategory))]
    [LocalizedDisplayName(nameof(Resources.ListDatasets_DisplayName))]
    [LocalizedDescription(nameof(Resources.ListDatasets_Description))]
    public class ListDatasets : IndicoActivityBase<bool, List<IDataSetFull>>
    {
        [LocalizedDisplayName(nameof(Resources.ListDatasets_Datasets_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListDatasets_Datasets_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<List<IDataSetFull>> Datasets { get; set; }
        
        protected override bool GetInputs(AsyncCodeActivityContext ctx) => true; // no input, using dummy bool

        protected override async Task<List<IDataSetFull>> ExecuteAsync(bool input, CancellationToken cancellationToken) =>
            (await Application.ListDatasets(cancellationToken)).ToList();

        protected override void SetOutputs(AsyncCodeActivityContext ctx, List<IDataSetFull> output) =>
            Datasets.Set(ctx, output);
    }
}

