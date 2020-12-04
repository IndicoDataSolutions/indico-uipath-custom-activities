using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Activities.Statements;
using System.ComponentModel;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.IndicoScope_DisplayName))]
    [LocalizedDescription(nameof(Resources.IndicoScope_Description))]
    public class IndicoScope : ContinuableAsyncNativeActivity
    {
        #region Properties

        [Browsable(false)]
        public ActivityAction<IObjectContainer> Body { get; set; }

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.IndicoScope_Host_DisplayName))]
        [LocalizedDescription(nameof(Resources.IndicoScope_Host_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Host { get; set; }

        [LocalizedDisplayName(nameof(Resources.IndicoScope_Token_DisplayName))]
        [LocalizedDescription(nameof(Resources.IndicoScope_Token_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Token { get; set; }

        // A tag used to identify the scope in the activity context
        internal static string ParentContainerPropertyTag => "ScopeActivity";

        // Object Container: Add strongly-typed objects here and they will be available in the scope's child activities.
        private readonly IObjectContainer _objectContainer;

        #endregion


        #region Constructors

        public IndicoScope(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;

            Body = new ActivityAction<IObjectContainer>
            {
                Argument = new DelegateInArgument<IObjectContainer> (ParentContainerPropertyTag),
                Handler = new Sequence { DisplayName = Resources.Do }
            };
        }

        public IndicoScope() : this(new ObjectContainer())
        {

        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            if (Host == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Host)));
            if (Token == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Token)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<NativeActivityContext>> ExecuteAsync(NativeActivityContext  context, CancellationToken cancellationToken)
        {
            // Inputs
            string host = Host.Get(context);
            string token = Token.Get(context);
            Application application = new Application(token, host);
            _objectContainer.Add(application);

            return (ctx) => {
                // Schedule child activities
                if (Body != null)
				    ctx.ScheduleAction<IObjectContainer>(Body, _objectContainer, OnCompleted, OnFaulted);

                // Outputs
            };
        }

        #endregion


        #region Events

        private void OnFaulted(NativeActivityFaultContext faultContext, System.Exception propagatedException, ActivityInstance propagatedFrom)
        {
            faultContext.CancelChildren();
            Cleanup();
        }

        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            Cleanup();
        }

        #endregion


        #region Helpers
        
        private void Cleanup()
        {
            var disposableObjects = _objectContainer.Where(o => o is IDisposable);
            foreach (var obj in disposableObjects)
            {
                if (obj is IDisposable dispObject)
                    dispObject.Dispose();
            }
            _objectContainer.Clear();
        }

        #endregion
    }
}

