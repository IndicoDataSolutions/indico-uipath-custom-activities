using System;
using System.IO;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Properties;
using Indico.RPAActivities.Activities.Activities;
using Indico.UiPath.Shared.Activities.Localization;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.RetrieveStorageUrl_DisplayName))]
    [LocalizedDescription(nameof(Resources.RetrieveStorageUrl_Description))]
    public class RetrieveStorageUrl : IndicoActivityBase<string, Stream>
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.RetrieveStorageUrl_StorageUrl_DisplayName))]
        [LocalizedDescription(nameof(Resources.RetrieveStorageUrl_StorageUrl_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> StorageUrl { get; set; }

        [LocalizedDisplayName(nameof(Resources.RetrieveStorageUrl_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.RetrieveStorageUrl_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<Stream> Result { get; set; }

        #endregion


        #region Protected Methods

        protected override string GetInputs(AsyncCodeActivityContext ctx) => (StorageUrl.Get(ctx));
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (StorageUrl == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(StorageUrl)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Stream> ExecuteWithTimeout(string storageUrl, CancellationToken cancellationToken) =>
            await Application.RetrieveStorageUrl(storageUrl, cancellationToken);

        protected override void SetResults(AsyncCodeActivityContext ctx, Stream output) =>
            Result.Set(ctx, output);

        #endregion
    }
}

