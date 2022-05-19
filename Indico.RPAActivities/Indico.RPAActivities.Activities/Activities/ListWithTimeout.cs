//using System;
//using System.Activities;
//using System.Threading;
//using System.Threading.Tasks;
//using Indico.RPAActivities.Activities.Properties;
//using Indico.UiPath.Shared.Activities;
//using Indico.UiPath.Shared.Activities.Localization;

//namespace Indico.RPAActivities.Activities
//{
//    [LocalizedDisplayName(nameof(Resources.ListWithTimeout_DisplayName))]
//    [LocalizedDescription(nameof(Resources.ListWithTimeout_Description))]
//    public class ListWithTimeout : ContinuableAsyncCodeActivity
//    {
//        #region Properties

//        /// <summary>
//        /// If set, continue executing the remaining activities even if the current activity has failed.
//        /// </summary>
//        [LocalizedCategory(nameof(Resources.Common_Category))]
//        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
//        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
//        public override InArgument<bool> ContinueOnError { get; set; }

//        [LocalizedCategory(nameof(Resources.Common_Category))]
//        [LocalizedDisplayName(nameof(Resources.Timeout_DisplayName))]
//        [LocalizedDescription(nameof(Resources.Timeout_Description))]
//        public InArgument<int> TimeoutMS { get; set; } = 60000;

//        [LocalizedDisplayName(nameof(Resources.ListWithTimeout_One_DisplayName))]
//        [LocalizedDescription(nameof(Resources.ListWithTimeout_One_Description))]
//        [LocalizedCategory(nameof(Resources.Input_Category))]
//        public InArgument<string> One { get; set; }

//        #endregion


//        #region Constructors

//        public ListWithTimeout()
//        {
//        }

//        #endregion


//        #region Protected Methods

//        //protected override void CacheMetadata(CodeActivityMetadata metadata)
//        //{

//        //    base.CacheMetadata(metadata);
//        //}

//        //protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
//        //{
//        //    // Inputs
//        //    var timeout = TimeoutMS.Get(context);
//        //    var one = One.Get(context);

//        //    // Set a timeout on the execution
//        //    var task = ExecuteWithTimeout(context, cancellationToken);
//        //    if (await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)) != task) throw new TimeoutException(Resources.Timeout_Error);

//        //    // Outputs
//        //    return (ctx) => {
//        //    };
//        //}

//        //private async Task ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
//        //{
//        //    ///////////////////////////
//        //    // Add execution logic HERE
//        //    ///////////////////////////
//        //}

//        #endregion
//    }
//}

