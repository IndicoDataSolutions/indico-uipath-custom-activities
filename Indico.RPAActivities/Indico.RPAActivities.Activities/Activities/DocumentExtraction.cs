using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Indico.RPAActivities.Activities.Properties;
using Indico.RPAActivities.Models;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPath.Shared.Activities.Utilities;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.DocumentExtraction_DisplayName))]
    [LocalizedDescription(nameof(Resources.DocumentExtraction_Description))]
    public class DocumentExtraction : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.Timeout_DisplayName))]
        [LocalizedDescription(nameof(Resources.Timeout_Description))]
        public InArgument<int> TimeoutMS { get; set; } = 60000;

        [LocalizedDisplayName(nameof(Resources.DocumentExtraction_ConfigType_DisplayName))]
        [LocalizedDescription(nameof(Resources.DocumentExtraction_ConfigType_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ConfigType { get; set; }

        [LocalizedDisplayName(nameof(Resources.DocumentExtraction_Document_DisplayName))]
        [LocalizedDescription(nameof(Resources.DocumentExtraction_Document_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Document { get; set; }

        [LocalizedDisplayName(nameof(Resources.DocumentExtraction_Results_DisplayName))]
        [LocalizedDescription(nameof(Resources.DocumentExtraction_Results_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<Document> Results { get; set; }

        #endregion


        #region Constructors

        public DocumentExtraction()
        {
            Constraints.Add(ActivityConstraints.HasParentType<DocumentExtraction, IndicoScope>(string.Format(Resources.ValidationScope_Error, Resources.IndicoScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (ConfigType == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ConfigType)));
            if (Document == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Document)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var timeout = TimeoutMS.Get(context);

            // Set a timeout on the execution
            var task = ExecuteWithTimeout(context, cancellationToken);
            if (await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)) != task) throw new TimeoutException(Resources.Timeout_Error);

            // Outputs
            return (ctx) => {
                Results.Set(ctx, task.Result);
            };
        }

        private async Task<Document> ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
        {

            var objectContainer = context.GetFromContext<IObjectContainer>(IndicoScope.ParentContainerPropertyTag);
            var application = objectContainer.Get<Application>();

            var document = Document.Get(context);
            var config = ConfigType.Get(context);
            var extractedDocument = await application.ExtractDocument(document, config);
            return extractedDocument;
        }

        #endregion
    }
}

