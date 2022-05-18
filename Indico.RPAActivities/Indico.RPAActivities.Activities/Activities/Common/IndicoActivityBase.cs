using System;
using System.Activities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Properties;
using IndicoV2.Submissions.Models;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPath.Shared.Activities.Utilities;

namespace Indico.RPAActivities.Activities.Activities
{
    public abstract class IndicoActivityBase<TInput, TOutput> : ContinuableAsyncCodeActivity
    {
        protected Application Application { get; private set; }


        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.Timeout_DisplayName))]
        [LocalizedDescription(nameof(Resources.Timeout_Description))]
        public InArgument<int> TimeoutMS { get; set; } = 60000;

        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        protected IndicoActivityBase()
        {
            Constraints.Add(ActivityConstraints.HasParentType<IndicoActivityBase<TInput, TOutput>, IndicoScope>(string.Format(Resources.ValidationScope_Error, Resources.IndicoScope_DisplayName)));

        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var objectContainer = context.GetFromContext<IObjectContainer>(IndicoScope.ParentContainerPropertyTag);
            Application = objectContainer.Get<Application>();

            return base.BeginExecute(context, callback, state);

        }

        protected abstract void Results(AsyncCodeActivityContext context, IEnumerable<TOutput> result);

        protected abstract Task<TOutput> ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default);
    }
}
