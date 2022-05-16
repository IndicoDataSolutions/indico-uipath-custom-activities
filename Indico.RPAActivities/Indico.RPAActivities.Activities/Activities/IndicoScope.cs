using System;
using System.Activities;
using System.Activities.Statements;
using System.ComponentModel;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Indico.RPAActivities.Activities
{
    [LocalizedCategory(nameof(Resources.RootCategory))]
    [LocalizedDisplayName(nameof(Resources.IndicoScope_DisplayName))]
    [LocalizedDescription(nameof(Resources.IndicoScope_Description))]
    public class IndicoScope : NativeActivity
    {
        [Browsable(false)]
        public ActivityAction<IObjectContainer> Body { get; set; }

        [LocalizedDisplayName(nameof(Resources.IndicoScope_Host_DisplayName))]
        [LocalizedDescription(nameof(Resources.IndicoScope_Host_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> Host { get; set; } = "https://app.indico.io";

        [LocalizedDisplayName(nameof(Resources.IndicoScope_Token_DisplayName))]
        [LocalizedDescription(nameof(Resources.IndicoScope_Token_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> Token { get; set; }

        // A tag used to identify the scope in the activity context
        internal static string ParentContainerPropertyTag => "ScopeActivity";

        // Object Container: Add strongly-typed objects here and they will be available in the scope's child activities.
        private readonly IObjectContainer _objectContainer;


        public IndicoScope()
        {
            _objectContainer = new ObjectContainer();

            Body = new ActivityAction<IObjectContainer>
            {
                Argument = new DelegateInArgument<IObjectContainer> (ParentContainerPropertyTag),
                Handler = new Sequence { DisplayName = Resources.Do }
            };
        }

        protected override void Execute(NativeActivityContext context)
        {
            // Inputs
            string host = Host.Get(context);
            string token = Token.Get(context);
            Application application = new Application(token, host);
            _objectContainer.Add(application);

            if (Body != null)
            {
                context.ScheduleAction<IObjectContainer>(Body, _objectContainer, OnCompleted, OnFaulted);
            }
        }

        private void OnFaulted(NativeActivityFaultContext faultContext, System.Exception propagatedException, ActivityInstance propagatedFrom)
        {
            faultContext.CancelChildren();
            Cleanup();
        }

        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            Cleanup();
        }

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
    }
}

